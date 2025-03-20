using _3_Scripts.Data;

namespace _3_Scripts.Utils
{
    public static class ServiceLocator
    {
        private static PlayerData _playerData;
    
        public static void RegisterPlayerData(PlayerData data)
        {
            _playerData = data;
        }

        public static PlayerData GetPlayerData()
        {
            return _playerData;
        }
    }
}