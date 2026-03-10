using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using System;
using System.Threading;

namespace CityForge
{
    public class Mod : IMod
    {
        public const string MOD_NAME = "CityForge";
        public const string MOD_VERSION = "1.0.8";

        public static ILog log = LogManager.GetLogger($"{nameof(CityForge)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public static Setting Setting { get; private set; }

        private Setting _setting;
        private GameManager.EventGamePreload _onGameLoadingComplete;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info($"{MOD_NAME} v{MOD_VERSION} loading...");

            try
            {
                _setting = new Setting(this);
                _setting.RegisterInOptionsUI();
                Setting = _setting;

                GameManager.instance.localizationManager.AddSource("en-US", new Setting.LocaleEN(_setting));
                GameManager.instance.localizationManager.AddSource("de-DE", new Setting.LocaleDE(_setting));
                GameManager.instance.localizationManager.AddSource("fr-FR", new Setting.LocaleFR(_setting));
                GameManager.instance.localizationManager.AddSource("es-ES", new Setting.LocaleES(_setting));
                GameManager.instance.localizationManager.AddSource("it-IT", new Setting.LocaleIT(_setting));
                GameManager.instance.localizationManager.AddSource("ja-JP", new Setting.LocaleJA(_setting));
                GameManager.instance.localizationManager.AddSource("ko-KR", new Setting.LocaleKO(_setting));
                GameManager.instance.localizationManager.AddSource("pl-PL", new Setting.LocalePL(_setting));
                GameManager.instance.localizationManager.AddSource("pt-BR", new Setting.LocalePT(_setting));
                GameManager.instance.localizationManager.AddSource("ru-RU", new Setting.LocaleRU(_setting));
                GameManager.instance.localizationManager.AddSource("zh-HANS", new Setting.LocaleZHHans(_setting));
                GameManager.instance.localizationManager.AddSource("zh-HANT", new Setting.LocaleZHHant(_setting));

                AssetDatabase.global.LoadSettings(nameof(CityForge), _setting, new Setting(this));

                _onGameLoadingComplete = (purpose, mode) => OnGameLoadingComplete(mode);
                GameManager.instance.onGameLoadingComplete += _onGameLoadingComplete;

                updateSystem.UpdateAt<CityForgeSystem>(SystemUpdatePhase.GameSimulation);
                updateSystem.UpdateAt<CityForgeUISystem>(SystemUpdatePhase.UIUpdate);
                updateSystem.UpdateAt<AchievementEnablerSystem>(SystemUpdatePhase.GameSimulation);

                log.Info($"{MOD_NAME} v{MOD_VERSION} loaded.");
            }
            catch (Exception e)
            {
                log.Error($"{MOD_NAME} failed to load: {e}");
                throw;
            }
        }

        private void OnGameLoadingComplete(GameMode mode)
        {
            if (mode != GameMode.Game) return;
            var s = Setting;
            if (s == null || !s.ResetOnNewMap) return;

            s.ResetCheats();


            SafeApplyAndSave(s, "onGameLoadingComplete");

            CityForgeUISystem.Instance?.RefreshAllBindings();
            log.Info("onGameLoadingComplete - cheats reset.");
        }

        internal static void SafeApplyAndSave(Setting s, string context = "", int maxRetries = 4)
        {
            if (s == null) return;
            int delayMs = 150;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    s.ApplyAndSave();
                    return; 
                }
                catch (System.IO.IOException ioEx) when (attempt < maxRetries)
                {
                    log.Warn($"CityForge: ApplyAndSave attempt {attempt} failed ({context}): {ioEx.Message} — retrying in {delayMs}ms");
                    Thread.Sleep(delayMs);
                    delayMs *= 2; 
                }
                catch (Exception ex)
                {
                    log.Error($"CityForge: ApplyAndSave failed ({context}): {ex}");
                    return;
                }
            }

            try { s.ApplyAndSave(); }
            catch (Exception ex) { log.Error($"CityForge: ApplyAndSave final attempt failed ({context}): {ex}"); }
        }

        public void OnDispose()
        {
            log.Info($"{MOD_NAME} disposing...");

            if (_onGameLoadingComplete != null && GameManager.instance != null)
            {
                GameManager.instance.onGameLoadingComplete -= _onGameLoadingComplete;
                _onGameLoadingComplete = null;
            }

            if (_setting != null)
            {
                _setting.UnregisterInOptionsUI();
                _setting = null;
                Setting = null;
            }
        }
    }
}