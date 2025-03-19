using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data
{
    public class PlayerStats : Singleton<PlayerStats>
    {
        public PlayerProgress playerProgress;
        public PlayerUISettings playerUISettings;
        private SaveManager saveManager;

        private void Awake()
        {
            saveManager = new SaveManager();
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