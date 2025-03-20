using System;
using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data
{
    public class PlayerData
    {
        public readonly PlayerProgress playerProgress;
        public readonly PlayerUISettings playerUISettings;
        public Action onProgressUpdate;
        public Action onSettingsUpdate;
        private readonly SaveManager saveManager;

        public PlayerData(SaveManager saveManager, PlayerProgress playerProgress, PlayerUISettings playerUISettings)
        {
            this.saveManager = saveManager;
            this.playerProgress = playerProgress;
            this.playerUISettings = playerUISettings;
        }

        public void SaveAll()
        {
            SaveProgress();
            saveUISettings();
        }

        public void LoadAll()
        {
            LoadProgress();
            LoadUISettings();
        }

        public void ResetAll()
        {
            ResetProgress();
            ResetUISettings();
        }

        public void LoadProgress()
        {
            UpdateCurrentProgress(saveManager.LoadProgress());
            onProgressUpdate?.Invoke();
        }

        public void SaveProgress()
        {
            saveManager.SaveProgress(playerProgress);
            onProgressUpdate?.Invoke();
        }

        public void ResetProgress()
        {
            UpdateCurrentProgress(saveManager.ResetProgress());
            onProgressUpdate?.Invoke();
        }

        public void LoadUISettings()
        {
            UpdateCurrentSettings(saveManager.LoadSettings());
            onSettingsUpdate?.Invoke();
        }

        public void saveUISettings()
        {
            saveManager.SaveSettings(playerUISettings);
            onSettingsUpdate?.Invoke();
        }

        public void ResetUISettings()
        {
            UpdateCurrentSettings(saveManager.ResetUISettings());
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