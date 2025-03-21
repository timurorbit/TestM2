using System.Threading.Tasks;
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

        public async Task SaveSettings(PlayerUISettings settings)
        {
           await saveSettingsSystem.Save(settings); 
        }

        public async Task SaveProgress(PlayerProgress progress)
        {
           await saveProgressSystem.Save(progress);
        }

        public async Task<PlayerProgress> LoadProgress()
        {
            return await saveProgressSystem.Load();
        }

        public async Task<PlayerUISettings> LoadSettings()
        {
            return await saveSettingsSystem.Load();
        }

        public async Task<PlayerProgress> ResetProgress()
        {
           return await saveProgressSystem.Reset();
        } 

        public async Task<PlayerUISettings> ResetUISettings()
        {
           return await saveSettingsSystem.Reset();
        }
    }
}