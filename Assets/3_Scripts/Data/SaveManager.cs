using _3_Scripts.Data.Services;
using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data
{
    public class SaveManager
    {
        
        private readonly ISaveSystem<PlayerProgress> saveProgressSystem;
        private readonly ISaveSystem<PlayerUISettings> saveSettingsSystem;

        public SaveManager(ISaveSystem<PlayerProgress> saveProgressSystem, ISaveSystem<PlayerUISettings> saveSettingsSystem)
        {
            this.saveProgressSystem = saveProgressSystem;
            this.saveSettingsSystem = saveSettingsSystem;
        }

        public void SaveSettings(PlayerUISettings settings)
        {
           saveSettingsSystem.Save(settings); 
        }

        public void SaveProgress(PlayerProgress progress)
        {
            saveProgressSystem.Save(progress);
        }

        public PlayerProgress LoadProgress()
        {
            return saveProgressSystem.Load();
        }

        public PlayerUISettings LoadSettings()
        {
            return saveSettingsSystem.Load();
        }

        public PlayerProgress ResetProgress()
        {
           return saveProgressSystem.Reset();
        } 

        public PlayerUISettings ResetSettings()
        {
           return saveSettingsSystem.Reset();
        }
    }
}