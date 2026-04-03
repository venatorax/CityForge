using Game;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Economy;
using Game.Objects;
using Game.Prefabs;
using Game.PSI;
using Game.Simulation;
using Game.Zones;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CityForge
{
    public partial class CityForgeSystem : GameSystemBase
    {
        public static CityForgeSystem Instance { get; private set; }
        public bool BuildingLevelFeatureAvailable => _buildingLevelType != null && !(_buildingLevelType.FullName ?? "").StartsWith("Game.Prefabs");
        public int CurrentPopulation => (_citizenQuery != null && !_citizenQuery.IsEmpty) ? _citizenQuery.CalculateEntityCount() : 0;

        private EntityQuery _playerMoneyQuery;
        private EntityQuery _underConstructionQuery;
        private EntityQuery _citizenQuery;
        private EntityQuery _householdQuery;
        private EntityQuery _householdWithResourcesQuery;
        private EntityQuery _storageQuery;
        private EntityQuery _attractivenessQuery;
        private EntityQuery _demandParamQuery;
        private EntityQuery _placedBuildingQuery;
        private EntityQuery _prefabBuildingQuery;
        private EntityQuery _efficiencyQuery;

        private bool _pendingFillStorage = false;
        private bool _keepStorageFull = false;
        private bool _demandParamQueryReady = false;
        private bool _buildingLevelSetupDone = false;
        private bool _efficiencySetupDone = false;

        private Type _buildingLevelType;

        private MilestoneSystem _milestoneSystem;
        private ResidentialDemandSystem _resDemandSystem;
        private CommercialDemandSystem _comDemandSystem;
        private IndustrialDemandSystem _indDemandSystem;
        private DevTreeSystem _devTreeSystem;
        private ZoneSpawnSystem _zoneSpawnSystem;
        private bool _lastFastSpawn;

        private Action<ResidentialDemandSystem, int3> _setResLast;
        private FieldInfo _resBuildingDemandNV;
        private System.Reflection.PropertyInfo _nativeValueProp;
        private Action<CommercialDemandSystem, int> _setComLast;
        private Action<IndustrialDemandSystem, int> _setIndLast;
        private Action<IndustrialDemandSystem, int> _setOffLast;
        private FieldInfo _indBuildingDemandsField;
        private FieldInfo _comBuildingDemandsField;

        private bool _lastUnlimitedRes, _lastUnlimitedCom, _lastUnlimitedInd, _lastUnlimitedOff;

        private long _pendingMoneyTotal = 0;
        private bool _pendingUnlockMilestones = false;
        private int _pendingDevPointsAmount = 0;
        private bool _pendingUnlockAllDevTree = false;
        private bool _pendingUpgradeBuildings = false;
        private int _pendingTargetMilestone = -1;
        private bool _pendingUnlockMapTiles = false;

        private int _buildingTargetLevel = 5;

        private int _postUpgradeBoostFrames = 0;
        private const int POST_UPGRADE_BOOST_DURATION = 300;
        private static readonly int3 _fullDemand = new int3(100, 100, 100);

        private struct UpgradePair { public Entity Inst; public Entity TargetPrefab; }
        private System.Collections.Generic.List<UpgradePair> _upgradeBatchList
            = new System.Collections.Generic.List<UpgradePair>(2048);
        private int _upgradeBatchIndex = 0;
        private const int UPGRADE_BATCH_SIZE = 50;
        private const int UPGRADE_FRAME_INTERVAL = 3;
        private int _upgradeFrameCounter = 0;

        private int _lastKnownAchievedMilestone = -1;
        private bool _lastInfiniteMoney;
        private uint _frameCount;

        private const int MAX_MILESTONE = 20;
        private static readonly byte[] SpeedBonus = { 0, 0, 1, 3 };

        private float[] _attracOriginals;
        private FieldInfo[] _attracAllFloatFields;
        private bool _attracOriginalsCaptured = false;
        private bool _attracBoostWasActive = false;

        private float[] _demandOriginals;
        private FieldInfo[] _demandAllFloatFields;
        private bool _demandOriginalsCaptured = false;
        private bool _demandBoostWasActive = false;

        private ComponentTypeHandle<Citizen> _citizenTypeHandle;
        private ComponentTypeHandle<UnderConstruction> _underConstructionTypeHandle;
        private BufferTypeHandle<Efficiency> _efficiencyBufferHandle;
        private BufferTypeHandle<Resources> _resourcesBufferHandle;

        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;

            var gm = Game.SceneFlow.GameManager.instance;
            if (gm != null)
                gm.onGameLoadingComplete += OnGameLoadingComplete;

            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

            _playerMoneyQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerMoney>());
            _underConstructionQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<UnderConstruction>());
            _citizenQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Citizen>());
            _householdQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Household>());
            _householdWithResourcesQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Household>(),
                ComponentType.ReadWrite<Resources>()
            );
            _storageQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<Resources>(),
                ComponentType.Exclude<Household>()
            );

            _milestoneSystem = World.GetOrCreateSystemManaged<MilestoneSystem>();
            _resDemandSystem = World.GetOrCreateSystemManaged<ResidentialDemandSystem>();
            _comDemandSystem = World.GetOrCreateSystemManaged<CommercialDemandSystem>();
            _indDemandSystem = World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
            _devTreeSystem = World.GetOrCreateSystemManaged<DevTreeSystem>();
            _zoneSpawnSystem = World.GetOrCreateSystemManaged<ZoneSpawnSystem>();

            _setResLast = BuildSetter<ResidentialDemandSystem, int3>("m_LastBuildingDemand", flags);
            _resBuildingDemandNV = typeof(ResidentialDemandSystem).GetField("m_BuildingDemand", flags);
            if (_resBuildingDemandNV != null) _nativeValueProp = _resBuildingDemandNV.FieldType.GetProperty("value");
            _setComLast = BuildSetter<CommercialDemandSystem, int>("m_LastBuildingDemand", flags);
            _setIndLast = BuildSetterMulti<IndustrialDemandSystem, int>(flags,
                "m_LastIndustrialBuildingDemand", "m_LastBuildingDemand");
            _setOffLast = BuildSetterMulti<IndustrialDemandSystem, int>(flags,
                "m_LastOfficeBuildingDemand", "m_OfficeLastBuildingDemand");

            _indBuildingDemandsField = typeof(IndustrialDemandSystem).GetField("m_IndustrialBuildingDemands", flags);

            _comBuildingDemandsField = typeof(CommercialDemandSystem).GetField("m_BuildingDemands", flags);
            if (_comBuildingDemandsField == null)
            {
                _comBuildingDemandsField = typeof(CommercialDemandSystem)
                    .GetFields(flags)
                    .FirstOrDefault(f => !f.FieldType.IsGenericType && f.Name.Contains("Demand") && f.Name.Contains("Building"));
                Mod.log.Info($"ComBuildingDemandsField fallback: {_comBuildingDemandsField?.Name ?? "not found"}");
            }

            _citizenTypeHandle = GetComponentTypeHandle<Citizen>(false);
            _underConstructionTypeHandle = GetComponentTypeHandle<UnderConstruction>(false);
            _efficiencyBufferHandle = GetBufferTypeHandle<Efficiency>(false);
            _resourcesBufferHandle = GetBufferTypeHandle<Resources>(false);

            SetupAttractivenessQuery();
            SetupDemandParamQuery();
        }

        private void SetupAttractivenessQuery()
        {
            try { _attractivenessQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<AttractivenessParameterData>()); }
            catch (Exception e) { Mod.log.Warn($"SetupAttractivenessQuery: {e.Message}"); }
        }

        private void SetupDemandParamQuery()
        {
            try
            {
                _demandParamQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<DemandParameterData>());
                _demandParamQueryReady = true;
            }
            catch (Exception e) { Mod.log.Warn($"SetupDemandParamQuery: {e.Message}"); }
        }

        private void EnsureBuildingLevelSetup()
        {
            if (_buildingLevelSetupDone) return;
            _buildingLevelSetupDone = true;
            try
            {
                _placedBuildingQuery = EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<BuildingCondition>(),
                    ComponentType.ReadOnly<PrefabRef>()
                );
                _prefabBuildingQuery = EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<SpawnableBuildingData>(),
                    ComponentType.ReadOnly<BuildingData>(),
                    ComponentType.ReadOnly<BuildingPropertyData>()
                );
                _buildingLevelType = typeof(BuildingCondition);
            }
            catch (Exception e) { Mod.log.Warn($"EnsureBuildingLevelSetup: {e.Message}"); }
        }

        private void EnsureEfficiencySetup()
        {
            if (_efficiencySetupDone) return;
            _efficiencySetupDone = true;
            try { _efficiencyQuery = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Efficiency>()); }
            catch (Exception e) { Mod.log.Warn($"EnsureEfficiencySetup: {e.Message}"); }
        }

        private static Action<TTarget, TValue> BuildSetter<TTarget, TValue>(string fieldName, BindingFlags flags)
        {
            var fi = typeof(TTarget).GetField(fieldName, flags);
            return fi == null ? null : CompileFieldSetter<TTarget, TValue>(fi);
        }

        private static Action<TTarget, TValue> BuildSetterMulti<TTarget, TValue>(BindingFlags flags, params string[] names)
        {
            foreach (var name in names)
            {
                var fi = typeof(TTarget).GetField(name, flags);
                if (fi != null) return CompileFieldSetter<TTarget, TValue>(fi);
            }
            return null;
        }

        private static Action<TTarget, TValue> CompileFieldSetter<TTarget, TValue>(FieldInfo fi)
        {
            try
            {
                var t = Expression.Parameter(typeof(TTarget), "t");
                var v = Expression.Parameter(typeof(TValue), "v");
                return Expression.Lambda<Action<TTarget, TValue>>(
                    Expression.Assign(Expression.Field(t, fi), v), t, v).Compile();
            }
            catch (Exception e) { Mod.log.Warn($"CompileFieldSetter({fi.Name}): {e.Message}"); return null; }
        }

        private void SetResNativeValue(int3 val)
        {
            if (_resBuildingDemandNV == null || _nativeValueProp == null || _resDemandSystem == null) return;
            try
            {
                var nv = _resBuildingDemandNV.GetValue(_resDemandSystem);
                if (nv != null) _nativeValueProp.SetValue(nv, val);
            }
            catch (Exception e) { Mod.log.Warn($"SetResNativeValue: {e.Message}"); }
        }

        public void RequestFillStorage() => _pendingFillStorage = true;
        public void SetKeepStorageFull(bool v) { _keepStorageFull = v; if (v) _pendingFillStorage = true; }
        public void RequestAddMoney(int amount) => Interlocked.Add(ref _pendingMoneyTotal, amount);
        public void RequestUnlockAllMilestones() => _pendingUnlockMilestones = true;
        public void RequestAddDevTreePoints(int amount) => Interlocked.Add(ref _pendingDevPointsAmount, Math.Max(0, amount));
        public void RequestUnlockAllDevTree() => _pendingUnlockAllDevTree = true;
        public void RequestUpgradeAllBuildings() => _pendingUpgradeBuildings = true;
        public void RequestSetBuildingTargetLevel(int level) => _buildingTargetLevel = Math.Max(1, Math.Min(5, level));
        public void RequestUnlockAllMapTiles() => _pendingUnlockMapTiles = true;
        public void RequestUnlockMilestonesTo(int target) =>
            _pendingTargetMilestone = Math.Max(1, Math.Min(target, MAX_MILESTONE));

        protected override void OnUpdate()
        {
            var s = Mod.Setting;
            if (s == null) return;
            _frameCount++;

            _citizenTypeHandle.Update(this);
            _underConstructionTypeHandle.Update(this);
            _efficiencyBufferHandle.Update(this);
            _resourcesBufferHandle.Update(this);

            long total = Interlocked.Exchange(ref _pendingMoneyTotal, 0);
            if (total != 0) AddMoney((int)Math.Max(Math.Min(total, int.MaxValue), int.MinValue));

            if (s.InfiniteMoney != _lastInfiniteMoney) { _lastInfiniteMoney = s.InfiniteMoney; ApplyInfiniteMoney(s.InfiniteMoney); }
            if (s.InfiniteMoney && _frameCount % 3000u == 0 && !_playerMoneyQuery.IsEmpty)
            {
                var pm = _playerMoneyQuery.GetSingleton<PlayerMoney>();
                if (pm.money < 10_000_000) AddMoney(10_000_000 - pm.money);
            }

            ApplyDemand(s);

            if (_postUpgradeBoostFrames > 0)
            {
                _postUpgradeBoostFrames--;
                if (!s.OverrideResidentialDemand) { _setResLast?.Invoke(_resDemandSystem, _fullDemand); SetResNativeValue(_fullDemand); }
                if (!s.OverrideCommercialDemand) _setComLast?.Invoke(_comDemandSystem, 100);
                if (!s.OverrideIndustrialDemand) _setIndLast?.Invoke(_indDemandSystem, 100);
                if (!s.OverrideOfficeDemand) _setOffLast?.Invoke(_indDemandSystem, 100);
            }

            if (_pendingUnlockMilestones) { _pendingUnlockMilestones = false; ApplyMilestoneTo(MAX_MILESTONE); }
            if (_pendingTargetMilestone >= 0) { int t = _pendingTargetMilestone; _pendingTargetMilestone = -1; ApplyMilestoneTo(t); }
            if (s.KeepMilestonesUnlocked && _frameCount % 300u == 0 && _milestoneSystem != null)
                if (_milestoneSystem.nextMilestone - 1 < _lastKnownAchievedMilestone) ApplyMilestoneTo(MAX_MILESTONE);

            { int pts = Interlocked.Exchange(ref _pendingDevPointsAmount, 0); if (pts > 0 && _devTreeSystem != null) _devTreeSystem.points += pts; }
            if (_pendingUnlockAllDevTree) { _pendingUnlockAllDevTree = false; if (_devTreeSystem != null) _devTreeSystem.points = 99999; }

            bool doInstant = s.InstantConstruction;
            int speedIdx = Math.Max(0, Math.Min(s.ConstructionSpeedIndex, SpeedBonus.Length - 1));
            byte bonus = SpeedBonus[speedIdx];
            if ((doInstant && _frameCount % 3u == 0) || (!doInstant && bonus > 0))
                if (!_underConstructionQuery.IsEmpty) ApplyConstructionProgress(doInstant, bonus);

            if (s.MaxHappiness && !_citizenQuery.IsEmpty) ApplyMaxHappiness();

            if (s.RichCitizens && _frameCount % 300u == 0 && !_householdWithResourcesQuery.IsEmpty)
                ApplyRichCitizens();

            if (s.MaxEducation && !_citizenQuery.IsEmpty) ApplyMaxEducation();
            if (s.OverrideEducation && _frameCount % 300u == 0 && !_citizenQuery.IsEmpty)
                ApplyEducationDistribution(s.EduLevel0, s.EduLevel1, s.EduLevel2, s.EduLevel3, s.EduLevel4);

            if (_frameCount % 600u == 0) ApplyAttractivenessBoost(s);
            if (_demandParamQueryReady && _frameCount % 600u == 0) ApplyDemandParamBoost(s);

            if (s.MaxCompanyEfficiency)
            {
                EnsureEfficiencySetup();
                if (_efficiencyQuery != null && !_efficiencyQuery.IsEmpty) ApplyMaxEfficiency();
            }

            if (_frameCount == 10u) EnsureBuildingLevelSetup();

            if (_pendingUpgradeBuildings)
            {
                _pendingUpgradeBuildings = false;
                EnsureBuildingLevelSetup();
                CollectUpgradeBatch();
                _postUpgradeBoostFrames = POST_UPGRADE_BOOST_DURATION;
                _upgradeFrameCounter = 0;
                Mod.log.Info($"UpgradeAll (target L{_buildingTargetLevel}): {_upgradeBatchList.Count} buildings queued.");
            }

            if (_upgradeBatchIndex < _upgradeBatchList.Count)
            {
                _upgradeFrameCounter++;
                if (_upgradeFrameCounter >= UPGRADE_FRAME_INTERVAL)
                {
                    _upgradeFrameCounter = 0;
                    FlushUpgradeBatch(UPGRADE_BATCH_SIZE);
                }
            }

            if (_pendingUnlockMapTiles) { _pendingUnlockMapTiles = false; Dependency.Complete(); UnlockAllMapTiles(); }

            if (_pendingFillStorage) { _pendingFillStorage = false; FillAllStorage(); }
            if (_keepStorageFull && _frameCount % 60 == 0) FillAllStorage();
        }

        [BurstCompile]
        private struct MaxHappinessJob : IJobChunk
        {
            public ComponentTypeHandle<Citizen> CitizenHandle;
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                                bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var citizens = chunk.GetNativeArray(ref CitizenHandle);
                for (int i = 0; i < citizens.Length; i++)
                {
                    var c = citizens[i];
                    c.m_WellBeing = 200;
                    c.m_Health = 200;
                    citizens[i] = c;
                }
            }
        }

        private void ApplyMaxHappiness()
        {
            Dependency = new MaxHappinessJob { CitizenHandle = _citizenTypeHandle }
                .ScheduleParallel(_citizenQuery, Dependency);
        }

        [BurstCompile]
        private struct ConstructionProgressJob : IJobChunk
        {
            public ComponentTypeHandle<UnderConstruction> Handle;
            public bool Instant;
            public byte Bonus;
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                                bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var arr = chunk.GetNativeArray(ref Handle);
                for (int i = 0; i < arr.Length; i++)
                {
                    var c = arr[i];
                    if (Instant) c.m_Progress = byte.MaxValue;
                    else { int nv = c.m_Progress + Bonus; c.m_Progress = nv >= 255 ? (byte)255 : (byte)nv; }
                    arr[i] = c;
                }
            }
        }

        private void ApplyConstructionProgress(bool instant, byte bonus)
        {
            Dependency = new ConstructionProgressJob { Handle = _underConstructionTypeHandle, Instant = instant, Bonus = bonus }
                .ScheduleParallel(_underConstructionQuery, Dependency);
        }

        [BurstCompile]
        private struct RichCitizensJob : IJobChunk
        {
            public BufferTypeHandle<Resources> ResourcesHandle;
            public int TargetMoney;
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                                bool useEnabledMask, in v128 chunkEnabledMask)
            {
                const Resource moneyRes = Resource.Money;
                var accessor = chunk.GetBufferAccessor(ref ResourcesHandle);
                for (int i = 0; i < accessor.Length; i++)
                {
                    var buf = accessor[i];
                    for (int j = 0; j < buf.Length; j++)
                    {
                        var entry = buf[j];
                        if (entry.m_Resource == moneyRes)
                        {
                            if (entry.m_Amount < TargetMoney) { entry.m_Amount = TargetMoney; buf[j] = entry; }
                            break;
                        }
                    }
                }
            }
        }

        private void ApplyRichCitizens()
        {
            Dependency = new RichCitizensJob { ResourcesHandle = _resourcesBufferHandle, TargetMoney = 500_000 }
                .ScheduleParallel(_householdWithResourcesQuery, Dependency);
        }

        [BurstCompile]
        private struct MaxEducationJob : IJobChunk
        {
            public ComponentTypeHandle<Citizen> CitizenHandle;
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                                bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var citizens = chunk.GetNativeArray(ref CitizenHandle);
                for (int i = 0; i < citizens.Length; i++)
                {
                    var c = citizens[i];
                    c.SetEducationLevel(4);
                    citizens[i] = c;
                }
            }
        }

        private void ApplyMaxEducation()
        {
            Dependency = new MaxEducationJob { CitizenHandle = _citizenTypeHandle }
                .ScheduleParallel(_citizenQuery, Dependency);
        }

        [BurstCompile]
        private struct EducationDistributionJob : IJobChunk
        {
            public ComponentTypeHandle<Citizen> CitizenHandle;
            public ushort T0, T1, T2, T3;
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                                bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var citizens = chunk.GetNativeArray(ref CitizenHandle);
                for (int i = 0; i < citizens.Length; i++)
                {
                    var c = citizens[i];
                    ushort r = c.m_PseudoRandom;
                    int level = r < T0 ? 0 : r < T1 ? 1 : r < T2 ? 2 : r < T3 ? 3 : 4;
                    c.SetEducationLevel(level);
                    citizens[i] = c;
                }
            }
        }

        private void ApplyEducationDistribution(int p0, int p1, int p2, int p3, int p4)
        {
            int total = p0 + p1 + p2 + p3 + p4;
            if (total <= 0) return;
            ushort t0 = (ushort)((long)p0 * 65535 / total);
            ushort t1 = (ushort)((long)(p0 + p1) * 65535 / total);
            ushort t2 = (ushort)((long)(p0 + p1 + p2) * 65535 / total);
            ushort t3 = (ushort)((long)(p0 + p1 + p2 + p3) * 65535 / total);
            Dependency = new EducationDistributionJob
            {
                CitizenHandle = _citizenTypeHandle,
                T0 = t0,
                T1 = t1,
                T2 = t2,
                T3 = t3
            }.ScheduleParallel(_citizenQuery, Dependency);
        }

        private void CollectUpgradeBatch()
        {
            if (_buildingLevelType == null || _placedBuildingQuery == null || _prefabBuildingQuery == null)
            { Mod.log.Warn("BuildingLevel setup not ready."); return; }

            _upgradeBatchList.Clear();
            _upgradeBatchIndex = 0;

            try
            {
                var prefabEntities = _prefabBuildingQuery.ToEntityArray(Allocator.Temp);
                var spawnableDatas = _prefabBuildingQuery.ToComponentDataArray<SpawnableBuildingData>(Allocator.Temp);
                var buildingDatas = _prefabBuildingQuery.ToComponentDataArray<BuildingData>(Allocator.Temp);
                var propertyDatas = _prefabBuildingQuery.ToComponentDataArray<BuildingPropertyData>(Allocator.Temp);

                var l5Map = new System.Collections.Generic.Dictionary<long, Entity>();
                for (int i = 0; i < prefabEntities.Length; i++)
                {
                    if (spawnableDatas[i].m_Level != _buildingTargetLevel) continue;
                    long key = BuildL5Key(
                        spawnableDatas[i].m_ZonePrefab.Index,
                        buildingDatas[i].m_LotSize.x,
                        buildingDatas[i].m_LotSize.y,
                        (int)(buildingDatas[i].m_Flags & (Game.Prefabs.BuildingFlags.LeftAccess | Game.Prefabs.BuildingFlags.RightAccess)));
                    if (!l5Map.ContainsKey(key)) l5Map[key] = prefabEntities[i];
                }
                prefabEntities.Dispose();
                spawnableDatas.Dispose();
                buildingDatas.Dispose();
                propertyDatas.Dispose();

                var placed = _placedBuildingQuery.ToEntityArray(Allocator.Temp);
                var prefabRefs = _placedBuildingQuery.ToComponentDataArray<PrefabRef>(Allocator.Temp);

                for (int i = 0; i < placed.Length; i++)
                {
                    Entity inst = placed[i];
                    Entity prefab = prefabRefs[i].m_Prefab;

                    if (!EntityManager.HasComponent<SpawnableBuildingData>(prefab)) continue;
                    if (EntityManager.HasComponent<UnderConstruction>(inst)) continue;
                    if (EntityManager.HasComponent<Abandoned>(inst)) continue;

                    var spawnable = EntityManager.GetComponentData<SpawnableBuildingData>(prefab);
                    if (spawnable.m_Level >= _buildingTargetLevel) continue;

                    var bdata = EntityManager.GetComponentData<BuildingData>(prefab);
                    long key = BuildL5Key(
                        spawnable.m_ZonePrefab.Index,
                        bdata.m_LotSize.x,
                        bdata.m_LotSize.y,
                        (int)(bdata.m_Flags & (Game.Prefabs.BuildingFlags.LeftAccess | Game.Prefabs.BuildingFlags.RightAccess)));

                    if (!l5Map.TryGetValue(key, out Entity l5Prefab)) continue;
                    _upgradeBatchList.Add(new UpgradePair { Inst = inst, TargetPrefab = l5Prefab });
                }
                placed.Dispose();
                prefabRefs.Dispose();
            }
            catch (Exception e) { Mod.log.Warn($"CollectUpgradeBatch: {e.Message}"); }
        }

        private void FlushUpgradeBatch(int maxCount)
        {
            int applied = 0;
            while (_upgradeBatchIndex < _upgradeBatchList.Count && applied < maxCount)
            {
                var pair = _upgradeBatchList[_upgradeBatchIndex];
                _upgradeBatchIndex++;

                if (!EntityManager.Exists(pair.Inst)) continue;
                if (EntityManager.HasComponent<UnderConstruction>(pair.Inst)) continue;
                if (EntityManager.HasComponent<Abandoned>(pair.Inst)) continue;

                try
                {
                    EntityManager.AddComponentData(pair.Inst, new UnderConstruction
                    {
                        m_NewPrefab = pair.TargetPrefab,
                        m_Progress = byte.MaxValue
                    });
                    applied++;
                }
                catch (Exception e) { Mod.log.Warn($"FlushUpgradeBatch {pair.Inst}: {e.Message}"); }
            }

            if (applied > 0)
                Mod.log.Info($"UpgradeAll: {applied} this frame, {_upgradeBatchList.Count - _upgradeBatchIndex} remaining.");
        }

        // ── Map Tile Unlock ─────────────────────────────────────────────────────
        // dnSpy confirmed (Game.dll → Game.Areas):
        //   MapTile : IComponentData, IEmptySerializable  — marker on every map tile entity
        //   No "Locked" component exists in Game.Areas.
        //
        // Strategy: All map tile entities carry MapTile.
        // Unpurchased tiles lack an Owner component (Game.Common.Owner).
        // To "buy" a tile: add Owner { m_Owner = cityEntity } to it.
        // MapTileSystem then picks up the change on next tick.

        private void UnlockAllMapTiles()
        {
            try
            {
                // City entity — the owner we assign tiles to
                var citySystem = World.GetOrCreateSystemManaged<CitySystem>();
                Entity cityEntity = citySystem?.City ?? Entity.Null;
                if (cityEntity == Entity.Null)
                {
                    Mod.log.Warn("UnlockAllMapTiles: city entity not found");
                    return;
                }

                // All map tile entities that do NOT yet have an Owner
                var query = EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<MapTile>(),
                    ComponentType.Exclude<Owner>()
                );
                var tiles = query.ToEntityArray(Allocator.Temp);
                int count = tiles.Length;

                for (int i = 0; i < tiles.Length; i++)
                {
                    try { EntityManager.AddComponentData(tiles[i], new Owner { m_Owner = cityEntity }); }
                    catch { }
                }

                tiles.Dispose();
                query.Dispose();
                Mod.log.Info($"UnlockAllMapTiles: {count} tiles unlocked (Owner set to city).");
            }
            catch (Exception e) { Mod.log.Warn($"UnlockAllMapTiles: {e.Message}"); }
        }

        private static long BuildL5Key(int zonePrefabIndex, int lotX, int lotY, int accessFlags)
            => ((long)zonePrefabIndex << 24) | ((long)(lotX & 0xFF) << 16) | ((long)(lotY & 0xFF) << 8) | (long)(accessFlags & 0xFF);

        [BurstCompile]
        private struct SetMaxEfficiencyJob : IJobChunk
        {
            public BufferTypeHandle<Efficiency> EfficiencyBufferHandle;
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                                bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var buffers = chunk.GetBufferAccessor(ref EfficiencyBufferHandle);
                for (int i = 0; i < buffers.Length; i++)
                {
                    var buf = buffers[i];
                    for (int j = 0; j < buf.Length; j++) { var e = buf[j]; e.m_Efficiency = 1.0f; buf[j] = e; }
                }
            }
        }

        private void ApplyMaxEfficiency()
        {
            Dependency = new SetMaxEfficiencyJob { EfficiencyBufferHandle = _efficiencyBufferHandle }
                .ScheduleParallel(_efficiencyQuery, Dependency);
        }

        private void SetCommercialBuildingDemandsArray(int value)
        {
            if (_comBuildingDemandsField == null || _comDemandSystem == null) return;
            try
            {
                var arr = (NativeArray<int>)_comBuildingDemandsField.GetValue(_comDemandSystem);
                if (!arr.IsCreated) return;
                for (int i = 0; i < arr.Length; i++) arr[i] = value;
            }
            catch (Exception e) { Mod.log.Warn($"SetCommercialBuildingDemandsArray: {e.Message}"); }
        }

        private void SetIndustrialBuildingDemandsArray(int value, bool fillAll)
        {
            if (_indBuildingDemandsField == null || _indDemandSystem == null) return;
            try
            {
                var arr = (NativeArray<int>)_indBuildingDemandsField.GetValue(_indDemandSystem);
                if (!arr.IsCreated) return;
                for (int i = 0; i < arr.Length; i++) arr[i] = value;
            }
            catch (Exception e) { Mod.log.Warn($"SetIndustrialBuildingDemandsArray: {e.Message}"); }
        }

        private void ApplyDemand(Setting s)
        {
            if (s.ForceResidentialBuild)
            {
                var full = new int3(100, 100, 100);
                _setResLast?.Invoke(_resDemandSystem, full);
                SetResNativeValue(full);
                if (!_lastUnlimitedRes) { _lastUnlimitedRes = true; _resDemandSystem?.SetUnlimitedDemand(true); }
                else _resDemandSystem?.SetUnlimitedDemand(true);
            }
            else if (s.OverrideResidentialDemand)
            {
                bool wu = s.ResidentialDemandLow == 100 && s.ResidentialDemandMedium == 100 && s.ResidentialDemandHigh == 100;
                var vals = new int3(s.ResidentialDemandLow, s.ResidentialDemandMedium, s.ResidentialDemandHigh);
                _lastUnlimitedRes = wu;
                _resDemandSystem?.SetUnlimitedDemand(wu);
                _setResLast?.Invoke(_resDemandSystem, vals);
                SetResNativeValue(vals);
            }
            else if (_lastUnlimitedRes) { _lastUnlimitedRes = false; _resDemandSystem?.SetUnlimitedDemand(false); }

            if (s.ForceCommercialBuild)
            {
                _setComLast?.Invoke(_comDemandSystem, 100);
                SetCommercialBuildingDemandsArray(100);
                if (!_lastUnlimitedCom) { _lastUnlimitedCom = true; _comDemandSystem?.SetUnlimitedDemand(true); }
                else _comDemandSystem?.SetUnlimitedDemand(true);
            }
            else if (s.OverrideCommercialDemand)
            {
                bool wu = s.CommercialDemandValue == 100;
                if (wu != _lastUnlimitedCom) { _lastUnlimitedCom = wu; _comDemandSystem?.SetUnlimitedDemand(wu); }
                _setComLast?.Invoke(_comDemandSystem, s.CommercialDemandValue);
                SetCommercialBuildingDemandsArray(s.CommercialDemandValue);
            }
            else if (_lastUnlimitedCom) { _lastUnlimitedCom = false; _comDemandSystem?.SetUnlimitedDemand(false); }

            if (s.ForceIndustrialBuild)
            {
                _setIndLast?.Invoke(_indDemandSystem, 100);
                SetIndustrialBuildingDemandsArray(100, false);
                if (!_lastUnlimitedInd) { _lastUnlimitedInd = true; _indDemandSystem?.SetUnlimitedDemand(true); }
                else _indDemandSystem?.SetUnlimitedDemand(true);
            }
            else if (s.OverrideIndustrialDemand)
            {
                int indVal = s.IndustrialDemandValue;
                bool wantUnlimInd = indVal == 100;
                _setIndLast?.Invoke(_indDemandSystem, indVal);
                SetIndustrialBuildingDemandsArray(indVal, false);
                if (wantUnlimInd != _lastUnlimitedInd) { _lastUnlimitedInd = wantUnlimInd; _indDemandSystem?.SetUnlimitedDemand(wantUnlimInd); }
                else if (wantUnlimInd) _indDemandSystem?.SetUnlimitedDemand(true);
            }
            else if (_lastUnlimitedInd) { _lastUnlimitedInd = false; _indDemandSystem?.SetUnlimitedDemand(false); }

            if (s.ForceOfficeBuild)
            {
                _setOffLast?.Invoke(_indDemandSystem, 100);
                SetIndustrialBuildingDemandsArray(100, true);
                if (!_lastUnlimitedInd)
                {
                    if (!_lastUnlimitedOff) { _lastUnlimitedOff = true; _indDemandSystem?.SetUnlimitedDemand(true); }
                    else _indDemandSystem?.SetUnlimitedDemand(true);
                }
                else { _lastUnlimitedOff = true; }
            }
            else if (s.OverrideOfficeDemand)
            {
                int offVal = s.OfficeDemandValue;
                bool wantUnlimOff = offVal == 100;
                _setOffLast?.Invoke(_indDemandSystem, offVal);
                SetIndustrialBuildingDemandsArray(offVal, true);
                if (!_lastUnlimitedInd)
                {
                    if (wantUnlimOff != _lastUnlimitedOff) { _lastUnlimitedOff = wantUnlimOff; _indDemandSystem?.SetUnlimitedDemand(wantUnlimOff); }
                    else if (wantUnlimOff) _indDemandSystem?.SetUnlimitedDemand(true);
                }
                else { _lastUnlimitedOff = offVal == 100; }
            }
            else if (_lastUnlimitedOff) { _lastUnlimitedOff = false; if (!_lastUnlimitedInd) _indDemandSystem?.SetUnlimitedDemand(false); }


            bool wantFastSpawn = s.ForceResidentialBuild || s.ForceCommercialBuild
                              || s.ForceIndustrialBuild || s.ForceOfficeBuild;
            if (wantFastSpawn != _lastFastSpawn)
            {
                _lastFastSpawn = wantFastSpawn;
                if (_zoneSpawnSystem != null) _zoneSpawnSystem.debugFastSpawn = wantFastSpawn;
            }
        }

        private void ApplyAttractivenessBoost(Setting s)
        {
            try
            {
                if (_attractivenessQuery == null || _attractivenessQuery.IsEmpty) return;
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                bool isActive = s.OverrideTourists && s.TouristMultiplier > 1;

                if (!isActive)
                {
                    if (_attracBoostWasActive && _attracOriginalsCaptured)
                    {
                        var p = _attractivenessQuery.GetSingleton<AttractivenessParameterData>();
                        object b = p;
                        for (int i = 0; i < _attracAllFloatFields.Length; i++) _attracAllFloatFields[i].SetValue(b, _attracOriginals[i]);
                        _attractivenessQuery.SetSingleton((AttractivenessParameterData)b);
                    }
                    _attracBoostWasActive = false;
                    return;
                }

                if (!_attracOriginalsCaptured)
                {
                    _attracAllFloatFields = typeof(AttractivenessParameterData).GetFields(flags).Where(f => f.FieldType == typeof(float)).ToArray();
                    _attracOriginals = new float[_attracAllFloatFields.Length];
                    var data = _attractivenessQuery.GetSingleton<AttractivenessParameterData>();
                    object box = data;
                    for (int i = 0; i < _attracAllFloatFields.Length; i++) _attracOriginals[i] = (float)_attracAllFloatFields[i].GetValue(box);
                    _attracOriginalsCaptured = true;
                    return;
                }

                float mult = (float)s.TouristMultiplier;
                var param = _attractivenessQuery.GetSingleton<AttractivenessParameterData>();
                object boxed = param;
                for (int i = 0; i < _attracAllFloatFields.Length; i++)
                {
                    float orig = _attracOriginals[i];
                    _attracAllFloatFields[i].SetValue(boxed, orig > 0f ? orig * mult : orig);
                }
                _attractivenessQuery.SetSingleton((AttractivenessParameterData)boxed);
                _attracBoostWasActive = true;
            }
            catch (Exception e) { Mod.log.Warn($"ApplyAttractivenessBoost: {e.Message}"); }
        }

        private void ApplyDemandParamBoost(Setting s)
        {
            if (_demandParamQuery == null || _demandParamQuery.IsEmpty) return;

            bool overrideMoveIn = s.OverrideMoveIn && s.MoveInMultiplier > 1;
            bool overrideTourists = s.OverrideTourists && s.TouristMultiplier > 1;
            bool anyActive = overrideMoveIn || overrideTourists;

            if (!anyActive)
            {
                if (_demandBoostWasActive && _demandOriginalsCaptured) RestoreDemandParamOriginals();
                _demandBoostWasActive = false;
                return;
            }

            try
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                if (!_demandOriginalsCaptured)
                {
                    _demandAllFloatFields = typeof(DemandParameterData).GetFields(flags).Where(f => f.FieldType == typeof(float)).ToArray();
                    _demandOriginals = new float[_demandAllFloatFields.Length];
                    var singleton = _demandParamQuery.GetSingleton<DemandParameterData>();
                    object box = singleton;
                    for (int i = 0; i < _demandAllFloatFields.Length; i++) _demandOriginals[i] = (float)_demandAllFloatFields[i].GetValue(box);
                    _demandOriginalsCaptured = true;
                    return;
                }

                var param = _demandParamQuery.GetSingleton<DemandParameterData>();
                object paramBoxed = param;
                float mim = overrideMoveIn ? (float)s.MoveInMultiplier : 1f;
                float tm = overrideTourists ? (float)s.TouristMultiplier : 1f;

                for (int i = 0; i < _demandAllFloatFields.Length; i++)
                {
                    float orig = _demandOriginals[i];
                    string name = _demandAllFloatFields[i].Name;
                    float mult = 1f;
                    if (name.Contains("HouseholdSpawn") || name.Contains("SpawnSpeed") || name.Contains("MoveIn") || name.Contains("TeenSpawn"))
                        mult = mim;
                    else if (name.Contains("Hotel") || name.Contains("Tourist") || name.Contains("Visitor"))
                        mult = tm;
                    _demandAllFloatFields[i].SetValue(paramBoxed, orig > 0f ? orig * mult : orig);
                }
                _demandParamQuery.SetSingleton((DemandParameterData)paramBoxed);
                _demandBoostWasActive = true;
            }
            catch (Exception e) { Mod.log.Warn($"ApplyDemandParamBoost: {e.Message}"); }
        }

        private void RestoreDemandParamOriginals()
        {
            if (_demandAllFloatFields == null || _demandOriginals == null) return;
            try
            {
                var param = _demandParamQuery.GetSingleton<DemandParameterData>();
                object boxed = param;
                for (int i = 0; i < _demandAllFloatFields.Length; i++) _demandAllFloatFields[i].SetValue(boxed, _demandOriginals[i]);
                _demandParamQuery.SetSingleton((DemandParameterData)boxed);
            }
            catch (Exception e) { Mod.log.Warn($"RestoreDemandParamOriginals: {e.Message}"); }
        }

        private EntityArchetype _unlockEventArchetype;
        private EntityQuery _milestoneLevelGroup;
        private EntityQuery _milestoneDataGroup;
        private bool _milestoneAccessSetup = false;

        private void EnsureMilestoneAccess()
        {
            if (_milestoneAccessSetup || _milestoneSystem == null) return;
            _milestoneAccessSetup = true;
            try
            {
                var bf = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                var mlg = typeof(MilestoneSystem).GetField("m_MilestoneLevelGroup", bf)?.GetValue(_milestoneSystem);
                _milestoneLevelGroup = mlg != null ? (EntityQuery)mlg : default;
                var mdg = typeof(MilestoneSystem).GetField("m_MilestoneGroup", bf)?.GetValue(_milestoneSystem);
                _milestoneDataGroup = mdg != null ? (EntityQuery)mdg : default;
                var uea = typeof(MilestoneSystem).GetField("m_UnlockEventArchetype", bf)?.GetValue(_milestoneSystem);
                _unlockEventArchetype = uea != null ? (EntityArchetype)uea : default;
            }
            catch (Exception e) { Mod.log.Warn($"EnsureMilestoneAccess: {e.Message}"); }
        }

        private void ApplyMilestoneTo(int target)
        {
            if (_milestoneSystem == null) return;
            EnsureMilestoneAccess();
            try
            {
                int achieved = _milestoneSystem.nextMilestone - 1;
                if (achieved >= target) return;

                var milestoneEntities = _milestoneDataGroup.ToEntityArray(Allocator.TempJob);
                var milestoneDataArr = _milestoneDataGroup.ToComponentDataArray<MilestoneData>(Allocator.TempJob);
                var levelSingleton = _milestoneLevelGroup.GetSingleton<MilestoneLevel>();
                var cityEntity = _milestoneSystem.World.GetOrCreateSystemManaged<CitySystem>().City;
                var money = EntityManager.GetComponentData<PlayerMoney>(cityEntity);
                var credit = EntityManager.GetComponentData<Creditworthiness>(cityEntity);

                try
                {
                    for (int i = levelSingleton.m_AchievedMilestone; i < milestoneDataArr.Length; i++)
                    {
                        if (milestoneDataArr[i].m_Index > target) break;
                        Entity ue = EntityManager.CreateEntity(_unlockEventArchetype);
                        EntityManager.SetComponentData(ue, new Unlock(milestoneEntities[i]));
                        levelSingleton.m_AchievedMilestone = math.max(levelSingleton.m_AchievedMilestone, milestoneDataArr[i].m_Index);
                        money.Add(milestoneDataArr[i].m_Reward);
                        credit.m_Amount += milestoneDataArr[i].m_LoanLimit;
                    }
                }
                finally { milestoneEntities.Dispose(); milestoneDataArr.Dispose(); }

                _milestoneLevelGroup.SetSingleton(levelSingleton);
                EntityManager.SetComponentData(cityEntity, money);
                EntityManager.SetComponentData(cityEntity, credit);
                _lastKnownAchievedMilestone = levelSingleton.m_AchievedMilestone;
                Mod.log.Info($"Milestones advanced to {levelSingleton.m_AchievedMilestone}.");
            }
            catch (Exception e) { Mod.log.Warn($"ApplyMilestoneTo({target}): {e.Message}"); }
        }

        private void AddMoney(int amount)
        {
            if (_playerMoneyQuery.IsEmpty) return;
            var pm = _playerMoneyQuery.GetSingleton<PlayerMoney>();
            pm.Add(amount);
            _playerMoneyQuery.SetSingleton(pm);
            Mod.log.Info($"AddMoney: {amount:+#;-#;0}");
        }

        private void ApplyInfiniteMoney(bool enabled)
        {
            if (_playerMoneyQuery.IsEmpty) return;
            var pm = _playerMoneyQuery.GetSingleton<PlayerMoney>();
            pm.m_Unlimited = enabled;
            _playerMoneyQuery.SetSingleton(pm);
            if (enabled && pm.money < 10_000_000) AddMoney(10_000_000 - pm.money);
        }

        [BurstCompile]
        private struct FillStorageJob : IJobChunk
        {
            public BufferTypeHandle<Resources> ResourcesHandle;
            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                                bool useEnabledMask, in v128 chunkEnabledMask)
            {
                var buffers = chunk.GetBufferAccessor(ref ResourcesHandle);
                for (int i = 0; i < buffers.Length; i++)
                {
                    var buf = buffers[i];
                    for (int j = 0; j < buf.Length; j++)
                    {
                        var r = buf[j];
                        if (r.m_Resource != Resource.NoResource && r.m_Resource != Resource.Money)
                        { r.m_Amount = 1_000_000; buf[j] = r; }
                    }
                }
            }
        }

        private void FillAllStorage()
        {
            if (_storageQuery.IsEmpty) return;
            _resourcesBufferHandle.Update(this);
            Dependency = new FillStorageJob { ResourcesHandle = _resourcesBufferHandle }
                .ScheduleParallel(_storageQuery, Dependency);
        }

        private MethodInfo _setComponentDataMethod;
        private MethodInfo _getComponentDataMethod;

        private object GetComponentDataReflected(Entity entity, Type componentType)
        {
            try
            {
                if (_getComponentDataMethod == null)
                    _getComponentDataMethod = typeof(EntityManager)
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(m => m.Name == "GetComponentData" && m.IsGenericMethod && m.GetParameters().Length == 1);
                return _getComponentDataMethod?.MakeGenericMethod(componentType).Invoke(EntityManager, new object[] { entity });
            }
            catch { return null; }
        }

        private void SetComponentDataReflected(Entity entity, Type componentType, object data)
        {
            try
            {
                if (_setComponentDataMethod == null)
                    _setComponentDataMethod = typeof(EntityManager)
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(m => m.Name == "SetComponentData" && m.IsGenericMethod && m.GetParameters().Length == 2);
                _setComponentDataMethod?.MakeGenericMethod(componentType).Invoke(EntityManager, new[] { entity, data });
            }
            catch { }
        }

        private void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            if (mode != GameMode.Game) return;
            var s = Mod.Setting;
            if (s == null || !s.ResetOnNewMap) return;

            s.ResetCheats();
            s.ApplyAndSave();

            _lastInfiniteMoney = false;
            _lastUnlimitedRes = false; _lastUnlimitedCom = false;
            _lastUnlimitedInd = false; _lastUnlimitedOff = false;
            _lastFastSpawn = false;
            _keepStorageFull = false;
            _postUpgradeBoostFrames = 0;

            _resDemandSystem?.SetUnlimitedDemand(false);
            _comDemandSystem?.SetUnlimitedDemand(false);
            _indDemandSystem?.SetUnlimitedDemand(false);
            if (_zoneSpawnSystem != null) _zoneSpawnSystem.debugFastSpawn = false;

            CityForgeUISystem.Instance?.RefreshAllBindings();
            Mod.log.Info("onGameLoadingComplete - cheats reset.");
        }

        protected override void OnDestroy()
        {
            var gm = Game.SceneFlow.GameManager.instance;
            if (gm != null)
                gm.onGameLoadingComplete -= OnGameLoadingComplete;

            _resDemandSystem?.SetUnlimitedDemand(false);
            _comDemandSystem?.SetUnlimitedDemand(false);
            _indDemandSystem?.SetUnlimitedDemand(false);
            if (_zoneSpawnSystem != null) _zoneSpawnSystem.debugFastSpawn = false;

            if (_demandOriginalsCaptured && _demandParamQueryReady && _demandParamQuery != null && !_demandParamQuery.IsEmpty)
            {
                try
                {
                    var p = _demandParamQuery.GetSingleton<DemandParameterData>();
                    object b = p;
                    for (int i = 0; i < _demandAllFloatFields.Length; i++) _demandAllFloatFields[i].SetValue(b, _demandOriginals[i]);
                    _demandParamQuery.SetSingleton((DemandParameterData)b);
                }
                catch { }
            }

            if (_attracOriginalsCaptured && _attractivenessQuery != null && !_attractivenessQuery.IsEmpty)
            {
                try
                {
                    var p = _attractivenessQuery.GetSingleton<AttractivenessParameterData>();
                    object b = p;
                    for (int i = 0; i < _attracAllFloatFields.Length; i++) _attracAllFloatFields[i].SetValue(b, _attracOriginals[i]);
                    _attractivenessQuery.SetSingleton((AttractivenessParameterData)b);
                }
                catch { }
            }

            if (!_playerMoneyQuery.IsEmpty)
            {
                var pm = _playerMoneyQuery.GetSingleton<PlayerMoney>();
                if (pm.m_Unlimited) { pm.m_Unlimited = false; _playerMoneyQuery.SetSingleton(pm); }
            }

            _upgradeBatchList.Clear();
            _upgradeBatchIndex = 0;
            Instance = null;
            base.OnDestroy();
        }
    }
}