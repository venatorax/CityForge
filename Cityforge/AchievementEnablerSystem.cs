using Colossal.PSI.Common;
using Colossal.Serialization.Entities;
using Game;
using Game.Achievements;
using Game.City;
using Game.SceneFlow;
using Unity.Entities;
using UnityEngine.Scripting;

namespace CityForge
{

    [UpdateAfter(typeof(AchievementTriggerSystem))]
    public partial class AchievementEnablerSystem : GameSystemBase
    {
        private CityConfigurationSystem m_CityConfigurationSystem;
        private bool _wasDisabled;

        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            m_CityConfigurationSystem = World.GetOrCreateSystemManaged<CityConfigurationSystem>();
            _wasDisabled = false;
            Mod.log.Info("AchievementEnablerSystem created — achievements will stay active.");
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (mode != GameMode.Game) return;

            var setting = Mod.Setting;
            if (setting != null && !setting.EnableAchievements) return;

            m_CityConfigurationSystem.usedMods.Clear();
            Mod.log.Info("AchievementEnablerSystem: cleared usedMods — achievement tab will show as available.");
        }

        [Preserve]
        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            if (setting != null && !setting.EnableAchievements) return;

            if (GameManager.instance == null) return;
            if (GameManager.instance.state == GameManager.State.Loading) return;

            var platform = PlatformManager.instance;
            if (platform == null) return;

            if (!platform.achievementsEnabled)
            {
                platform.achievementsEnabled = true;

                if (!_wasDisabled)
                {
                    Mod.log.Info("AchievementEnablerSystem: re-enabled achievementsEnabled flag.");
                    _wasDisabled = true;
                }
            }
            else
            {
                _wasDisabled = false;
            }
        }

        [Preserve]
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
