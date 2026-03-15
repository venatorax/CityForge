using System;
using Colossal.UI.Binding;
using Game.UI;

namespace CityForge
{
    public partial class CityForgeUISystem : UISystemBase
    {
        public static CityForgeUISystem Instance { get; private set; }
        private const string MOD = "cheatmod";

        private bool _pendingSave = false;
        private float _saveDelaySeconds = 1.0f;
        private float _saveTimer = 0f;

        private ValueBinding<bool> _panelVisible;
        private ValueBinding<bool> _showButton;
        private ValueBinding<bool> _infiniteMoney;
        private ValueBinding<bool> _keepMilestones;
        private ValueBinding<int> _targetMilestone;
        private ValueBinding<bool> _overrideRes;
        private ValueBinding<int> _resLow;
        private ValueBinding<int> _resMed;
        private ValueBinding<int> _resHigh;
        private ValueBinding<bool> _forceResBuild;
        private ValueBinding<bool> _overrideCom;
        private ValueBinding<int> _comValue;
        private ValueBinding<bool> _forceComBuild;
        private ValueBinding<bool> _overrideInd;
        private ValueBinding<int> _indValue;
        private ValueBinding<bool> _forceIndBuild;
        private ValueBinding<bool> _overrideOff;
        private ValueBinding<int> _offValue;
        private ValueBinding<bool> _forceOffBuild;
        private ValueBinding<bool> _instantConstruction;
        private ValueBinding<int> _constructionSpeedIndex;
        private ValueBinding<bool> _overrideMoveIn;
        private ValueBinding<int> _moveInMultiplier;
        private ValueBinding<bool> _overrideTourists;
        private ValueBinding<int> _touristMultiplier;
        private ValueBinding<bool> _maxHappiness;
        private ValueBinding<bool> _richCitizens;
        private ValueBinding<bool> _maxEducation;
        private ValueBinding<bool> _maxCompanyEfficiency;
        private ValueBinding<bool> _resetOnNewMap;
        private ValueBinding<bool> _buildingLevelAvailable;
        private ValueBinding<bool> _keepStorageFull;
        private ValueBinding<string> _panelBgColor;

        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            var s = Mod.Setting;

            AddBinding(_panelVisible = new ValueBinding<bool>(MOD, "PanelVisible", false));
            AddBinding(_showButton = new ValueBinding<bool>(MOD, "ShowButton", s.ShowToolbarButton));
            AddBinding(_infiniteMoney = new ValueBinding<bool>(MOD, "InfiniteMoney", s.InfiniteMoney));
            AddBinding(_keepMilestones = new ValueBinding<bool>(MOD, "KeepMilestones", s.KeepMilestonesUnlocked));
            AddBinding(_targetMilestone = new ValueBinding<int>(MOD, "TargetMilestone", s.TargetMilestone));
            AddBinding(_overrideRes = new ValueBinding<bool>(MOD, "OverrideRes", s.OverrideResidentialDemand));
            AddBinding(_resLow = new ValueBinding<int>(MOD, "ResLow", s.ResidentialDemandLow));
            AddBinding(_resMed = new ValueBinding<int>(MOD, "ResMed", s.ResidentialDemandMedium));
            AddBinding(_resHigh = new ValueBinding<int>(MOD, "ResHigh", s.ResidentialDemandHigh));
            AddBinding(_forceResBuild = new ValueBinding<bool>(MOD, "ForceResBuild", s.ForceResidentialBuild));
            AddBinding(_overrideCom = new ValueBinding<bool>(MOD, "OverrideCom", s.OverrideCommercialDemand));
            AddBinding(_comValue = new ValueBinding<int>(MOD, "ComValue", s.CommercialDemandValue));
            AddBinding(_forceComBuild = new ValueBinding<bool>(MOD, "ForceComBuild", s.ForceCommercialBuild));
            AddBinding(_overrideInd = new ValueBinding<bool>(MOD, "OverrideInd", s.OverrideIndustrialDemand));
            AddBinding(_indValue = new ValueBinding<int>(MOD, "IndValue", s.IndustrialDemandValue));
            AddBinding(_forceIndBuild = new ValueBinding<bool>(MOD, "ForceIndBuild", s.ForceIndustrialBuild));
            AddBinding(_overrideOff = new ValueBinding<bool>(MOD, "OverrideOff", s.OverrideOfficeDemand));
            AddBinding(_offValue = new ValueBinding<int>(MOD, "OffValue", s.OfficeDemandValue));
            AddBinding(_forceOffBuild = new ValueBinding<bool>(MOD, "ForceOffBuild", s.ForceOfficeBuild));
            AddBinding(_instantConstruction = new ValueBinding<bool>(MOD, "InstantConstruction", s.InstantConstruction));
            AddBinding(_constructionSpeedIndex = new ValueBinding<int>(MOD, "ConstructionSpeedIndex", s.ConstructionSpeedIndex));
            AddBinding(_overrideMoveIn = new ValueBinding<bool>(MOD, "OverrideMoveIn", s.OverrideMoveIn));
            AddBinding(_moveInMultiplier = new ValueBinding<int>(MOD, "MoveInMultiplier", s.MoveInMultiplier));
            AddBinding(_overrideTourists = new ValueBinding<bool>(MOD, "OverrideTourists", s.OverrideTourists));
            AddBinding(_touristMultiplier = new ValueBinding<int>(MOD, "TouristMultiplier", s.TouristMultiplier));
            AddBinding(_maxHappiness = new ValueBinding<bool>(MOD, "MaxHappiness", s.MaxHappiness));
            AddBinding(_richCitizens = new ValueBinding<bool>(MOD, "RichCitizens", s.RichCitizens));
            AddBinding(_maxEducation = new ValueBinding<bool>(MOD, "MaxEducation", s.MaxEducation));
            AddBinding(_maxCompanyEfficiency = new ValueBinding<bool>(MOD, "MaxCompanyEfficiency", s.MaxCompanyEfficiency));
            AddBinding(_resetOnNewMap = new ValueBinding<bool>(MOD, "ResetOnNewMap", s.ResetOnNewMap));
            AddBinding(_buildingLevelAvailable = new ValueBinding<bool>(MOD, "BuildingLevelAvailable", false));
            AddBinding(_keepStorageFull = new ValueBinding<bool>(MOD, "KeepStorageFull", false));
            AddBinding(_panelBgColor = new ValueBinding<string>(MOD, "PanelBgColor", s.PanelBgColor ?? "#121418"));

            AddBinding(new TriggerBinding(MOD, "TogglePanel", () => _panelVisible.Update(!_panelVisible.value)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetPanelVisible", v => _panelVisible.Update(v)));

            AddBinding(new TriggerBinding<int>(MOD, "AddMoney", amount =>
            {
                Mod.log.Info($"AddMoney trigger: {amount:+#;-#;0}");
                CityForgeSystem.Instance?.RequestAddMoney(amount);
            }));

            AddBinding(new TriggerBinding<bool>(MOD, "SetInfiniteMoney",
                v => UpdateAndSave(_infiniteMoney, v, val => Mod.Setting.InfiniteMoney = val)));

            AddBinding(new TriggerBinding(MOD, "UnlockMilestones", () =>
                CityForgeSystem.Instance?.RequestUnlockAllMilestones()));

            AddBinding(new TriggerBinding(MOD, "AdvanceToTarget", () =>
                CityForgeSystem.Instance?.RequestUnlockMilestonesTo(_targetMilestone.value)));

            AddBinding(new TriggerBinding<int>(MOD, "SetTargetMilestone",
                v => UpdateAndSave(_targetMilestone, v, val => Mod.Setting.TargetMilestone = val)));

            AddBinding(new TriggerBinding<bool>(MOD, "SetKeepMilestones", v =>
            {
                _keepMilestones.Update(v);
                Mod.Setting.KeepMilestonesUnlocked = v;
                if (v) CityForgeSystem.Instance?.RequestUnlockAllMilestones();
                ScheduleSave();
            }));

            AddBinding(new TriggerBinding(MOD, "AddDevPoints", () =>
                CityForgeSystem.Instance?.RequestAddDevTreePoints()));

            AddBinding(new TriggerBinding<bool>(MOD, "SetOverrideRes", v => UpdateAndSave(_overrideRes, v, val => Mod.Setting.OverrideResidentialDemand = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetResLow", v => UpdateAndSave(_resLow, v, val => Mod.Setting.ResidentialDemandLow = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetResMed", v => UpdateAndSave(_resMed, v, val => Mod.Setting.ResidentialDemandMedium = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetResHigh", v => UpdateAndSave(_resHigh, v, val => Mod.Setting.ResidentialDemandHigh = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetForceResBuild", v => UpdateAndSave(_forceResBuild, v, val => Mod.Setting.ForceResidentialBuild = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetOverrideCom", v => UpdateAndSave(_overrideCom, v, val => Mod.Setting.OverrideCommercialDemand = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetComValue", v => UpdateAndSave(_comValue, v, val => Mod.Setting.CommercialDemandValue = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetForceComBuild", v => UpdateAndSave(_forceComBuild, v, val => Mod.Setting.ForceCommercialBuild = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetOverrideInd", v => UpdateAndSave(_overrideInd, v, val => Mod.Setting.OverrideIndustrialDemand = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetIndValue", v => UpdateAndSave(_indValue, v, val => Mod.Setting.IndustrialDemandValue = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetForceIndBuild", v => UpdateAndSave(_forceIndBuild, v, val => Mod.Setting.ForceIndustrialBuild = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetOverrideOff", v => UpdateAndSave(_overrideOff, v, val => Mod.Setting.OverrideOfficeDemand = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetOffValue", v => UpdateAndSave(_offValue, v, val => Mod.Setting.OfficeDemandValue = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetForceOffBuild", v => UpdateAndSave(_forceOffBuild, v, val => Mod.Setting.ForceOfficeBuild = val)));

            AddBinding(new TriggerBinding<bool>(MOD, "SetInstantConstruction",
                v => UpdateAndSave(_instantConstruction, v, val => Mod.Setting.InstantConstruction = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetConstructionSpeedIndex", v =>
            {
                int clamped = Math.Max(0, Math.Min(v, 3));
                UpdateAndSave(_constructionSpeedIndex, clamped, val => Mod.Setting.ConstructionSpeedIndex = val);
            }));

            AddBinding(new TriggerBinding<bool>(MOD, "SetOverrideMoveIn",
                v => UpdateAndSave(_overrideMoveIn, v, val => Mod.Setting.OverrideMoveIn = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetMoveInMultiplier", v =>
            {
                int clamped = Math.Max(1, Math.Min(v, 10));
                UpdateAndSave(_moveInMultiplier, clamped, val => Mod.Setting.MoveInMultiplier = val);
            }));
            AddBinding(new TriggerBinding<bool>(MOD, "SetOverrideTourists",
                v => UpdateAndSave(_overrideTourists, v, val => Mod.Setting.OverrideTourists = val)));
            AddBinding(new TriggerBinding<int>(MOD, "SetTouristMultiplier", v =>
            {
                int clamped = Math.Max(1, Math.Min(v, 10));
                UpdateAndSave(_touristMultiplier, clamped, val => Mod.Setting.TouristMultiplier = val);
            }));

            AddBinding(new TriggerBinding<bool>(MOD, "SetMaxHappiness",
                v => UpdateAndSave(_maxHappiness, v, val => Mod.Setting.MaxHappiness = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetRichCitizens",
                v => UpdateAndSave(_richCitizens, v, val => Mod.Setting.RichCitizens = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetMaxEducation",
                v => UpdateAndSave(_maxEducation, v, val => Mod.Setting.MaxEducation = val)));

            AddBinding(new TriggerBinding(MOD, "UpgradeAllBuildings", () =>
                CityForgeSystem.Instance?.RequestUpgradeAllBuildings()));
            AddBinding(new TriggerBinding(MOD, "FillStorage", () =>
                CityForgeSystem.Instance?.RequestFillStorage()));
            AddBinding(new TriggerBinding<bool>(MOD, "SetKeepStorageFull", v =>
            {
                _keepStorageFull.Update(v);
                CityForgeSystem.Instance?.SetKeepStorageFull(v);
            }));
            AddBinding(new TriggerBinding<bool>(MOD, "SetMaxCompanyEfficiency",
                v => UpdateAndSave(_maxCompanyEfficiency, v, val => Mod.Setting.MaxCompanyEfficiency = val)));
            AddBinding(new TriggerBinding<bool>(MOD, "SetResetOnNewMap",
                v => UpdateAndSave(_resetOnNewMap, v, val => Mod.Setting.ResetOnNewMap = val)));
            AddBinding(new TriggerBinding<string>(MOD, "SetPanelBgColor",
                v => UpdateAndSave(_panelBgColor, v, val => Mod.Setting.PanelBgColor = val)));
        }


        private void UpdateAndSave<T>(ValueBinding<T> binding, T value, Action<T> setter)
        {
            binding.Update(value);
            setter(value);
            ScheduleSave();
        }


        private void ScheduleSave()
        {
            _pendingSave = true;
            _saveTimer = _saveDelaySeconds;
        }

        public void FlushSave()
        {
            if (!_pendingSave) return;
            _pendingSave = false;
            _saveTimer = 0f;
            Mod.SafeApplyAndSave(Mod.Setting, "FlushSave");
        }

        public void RefreshAllBindings()
        {
            var s = Mod.Setting;
            if (s == null) return;

            _infiniteMoney.Update(s.InfiniteMoney);
            _keepMilestones.Update(s.KeepMilestonesUnlocked);
            _overrideRes.Update(s.OverrideResidentialDemand);
            _resLow.Update(s.ResidentialDemandLow);
            _resMed.Update(s.ResidentialDemandMedium);
            _resHigh.Update(s.ResidentialDemandHigh);
            _forceResBuild.Update(s.ForceResidentialBuild);
            _overrideCom.Update(s.OverrideCommercialDemand);
            _comValue.Update(s.CommercialDemandValue);
            _forceComBuild.Update(s.ForceCommercialBuild);
            _overrideInd.Update(s.OverrideIndustrialDemand);
            _indValue.Update(s.IndustrialDemandValue);
            _forceIndBuild.Update(s.ForceIndustrialBuild);
            _overrideOff.Update(s.OverrideOfficeDemand);
            _offValue.Update(s.OfficeDemandValue);
            _forceOffBuild.Update(s.ForceOfficeBuild);
            _instantConstruction.Update(s.InstantConstruction);
            _constructionSpeedIndex.Update(s.ConstructionSpeedIndex);
            _overrideMoveIn.Update(s.OverrideMoveIn);
            _moveInMultiplier.Update(s.MoveInMultiplier);
            _overrideTourists.Update(s.OverrideTourists);
            _touristMultiplier.Update(s.TouristMultiplier);
            _maxHappiness.Update(s.MaxHappiness);
            _richCitizens.Update(s.RichCitizens);
            _maxEducation.Update(s.MaxEducation);
            _maxCompanyEfficiency.Update(s.MaxCompanyEfficiency);
            _keepStorageFull.Update(false);
        }

        protected override void OnUpdate()
        {

            if (_pendingSave)
            {
                _saveTimer -= UnityEngine.Time.unscaledDeltaTime;
                if (_saveTimer <= 0f)
                {
                    _pendingSave = false;
                    Mod.SafeApplyAndSave(Mod.Setting, "OnUpdate");
                }
            }
            bool show = Mod.Setting?.ShowToolbarButton ?? true;
            if (_showButton.value != show) _showButton.Update(show);

            bool bla = CityForgeSystem.Instance?.BuildingLevelFeatureAvailable ?? false;
            if (_buildingLevelAvailable.value != bla) _buildingLevelAvailable.Update(bla);
        }
    }
}