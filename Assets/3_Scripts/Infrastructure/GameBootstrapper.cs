using System.Threading.Tasks;
using _3_Scripts.Data;
using _3_Scripts.Data.Services;
using _3_Scripts.Infrastructure.Logging;
using UnityEditor;
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
            ILogReporter logReporter = new ConditionalLogReporter();
            ServiceLocator.Instance.RegisterService(logReporter);
            
            ISaveSystem<PlayerData> saveProgressSystem = new JsonSavePlayerDataSystem(200, logReporter);
            PlayerData playerData = await saveProgressSystem.Load();

            SaveManager saveManager = new SaveManager(saveProgressSystem, playerData);
            
            ServiceLocator.Instance.RegisterService(saveManager);
            ServiceLocator.Instance.RegisterService(playerData);
        }
    }
}