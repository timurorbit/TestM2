using System;
using System.Threading.Tasks;
using _3_Scripts.Data.Services;

namespace _3_Scripts.Data
{
    public class SaveManager : IDisposable
    {
        
        private readonly ISaveSystem<PlayerData> _savePlayerDataSystem;
        private readonly PlayerData _playerData;

        public SaveManager(ISaveSystem<PlayerData> saveProgressSystem, PlayerData playerData)
        {
            _savePlayerDataSystem = saveProgressSystem;
            _playerData = playerData;
            _playerData.DataChanged += savePlayerData;
        }

        private void savePlayerData()
        {
            _savePlayerDataSystem.Save(_playerData);
        }

        public void Dispose()
        {
            _playerData.DataChanged -= savePlayerData;
        }

        public Task ResetPlayerData()
        {
            _playerData.ResetToDefaults();
            return _savePlayerDataSystem.Save(_playerData);
        }
    }
}