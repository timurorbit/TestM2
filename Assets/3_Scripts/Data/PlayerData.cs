using System;
using System.Threading.Tasks;
using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data
{
    public class PlayerData
    {
        private readonly SaveManager saveManager;
        public readonly PlayerProgress playerProgress;
        public readonly PlayerUISettings playerUISettings;

        public Action onProgressUpdate;
        public Action onSettingsUpdate;

        public PlayerData(SaveManager saveManager, PlayerProgress playerProgress, PlayerUISettings playerUISettings)
        {
            this.saveManager = saveManager;
            this.playerProgress = playerProgress;
            this.playerUISettings = playerUISettings;
        }

        public async Task SaveAll()
        {
            await Task.WhenAll(SaveProgress(), SaveUISettings());
        }

        public async Task LoadAll()
        {
            var progress = LoadProgress();
            var settings = LoadUISettings();
            await Task.WhenAll(progress, settings);
        }

        public async Task ResetAll()
        {
            await Task.WhenAll(ResetProgress(), ResetUISettings());
        }

        public async Task SaveUISettings()
        {
            await saveManager.SaveSettings(playerUISettings);
            onSettingsUpdate?.Invoke();
        }

        public async Task LoadProgress()
        {
            UpdateCurrentProgress(await saveManager.LoadProgress());
            onProgressUpdate?.Invoke();
        }

        public async Task LoadUISettings()
        {
            UpdateCurrentSettings(await saveManager.LoadSettings());
            onSettingsUpdate?.Invoke();
        }

        public async Task SaveProgress()
        {
            await saveManager.SaveProgress(playerProgress);
            onProgressUpdate?.Invoke();
        }

        public async Task ResetProgress()
        {
            UpdateCurrentProgress(await saveManager.ResetProgress());
            onProgressUpdate?.Invoke();
        }

        public async Task ResetUISettings()
        {
            UpdateCurrentSettings(await saveManager.ResetUISettings());
            onSettingsUpdate?.Invoke();
        }

        private void UpdateCurrentSettings(PlayerUISettings saved)
        {
            if (saved == null)
            {
                return;
            }

            playerUISettings.CurrentColorList = saved.CurrentColorList;
            playerUISettings.VibrationEnabled = saved.VibrationEnabled;
        }

        private void UpdateCurrentProgress(PlayerProgress savedProgress)
        {
            if (savedProgress == null)
            {
                return;
            }

            playerProgress.CurrentLevel = savedProgress.CurrentLevel;
            playerProgress.PreviousHighScore = savedProgress.PreviousHighScore;
        }
    }
}