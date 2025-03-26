using System.Threading.Tasks;
using _3_Scripts.Data;
using _3_Scripts.Data.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _3_Scripts.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour
    {
        private const string Bootstrap = "Bootstrap";
        private const string GameLevel = "GameScene";

        private async void Awake()
        {
            DontDestroyOnLoad(this);
            if (SceneManager.GetActiveScene().name != Bootstrap)
            {
                SceneManager.LoadScene(Bootstrap);
            }

            await InitializeGame();
            SceneManager.LoadScene(GameLevel);
        }

        private async Task InitializeGame()
        {
            ISaveSystem<PlayerData> saveProgressSystem = new JsonSavePlayerDataSystem();
            PlayerData playerData = await saveProgressSystem.Load();

            SaveManager saveManager = new SaveManager(saveProgressSystem, playerData);
            
            ServiceLocator.Instance.RegisterService(saveManager);
            ServiceLocator.Instance.RegisterService(playerData);
        }
    }
}