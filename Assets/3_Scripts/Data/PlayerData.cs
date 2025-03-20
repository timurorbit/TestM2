using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data
{
    public class PlayerData
    {
        public PlayerProgress playerProgress;
        public PlayerUISettings playerUISettings;
        private readonly SaveManager saveManager;

        public PlayerData(SaveManager saveManager)
        {
            this.saveManager = saveManager;
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
            playerProgress = saveManager.LoadProgress();
        }

        public void SaveProgress()
        {
            saveManager.SaveProgress(playerProgress);
        }

        public void ResetProgress()
        {
            playerProgress = saveManager.ResetProgress();
        }

        public void LoadUISettings()
        {
            playerUISettings = saveManager.LoadSettings();
        }

        public void saveUISettings()
        {
            saveManager.SaveSettings(playerUISettings);
        }

        public void ResetUISettings()
        {
            playerUISettings = saveManager.ResetSettings();
        }
        
    }
}