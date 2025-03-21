using System;
using System.Threading.Tasks;
using _3_Scripts.Data;
using _3_Scripts.Data.Services;
using _3_Scripts.Data.Structure;
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
            try
            { 
                DontDestroyOnLoad(this);
                if (SceneManager.GetActiveScene().name != Bootstrap)
                {
                    SceneManager.LoadScene(Bootstrap); 
                }
                await InitializeGame();
                SceneManager.LoadScene(GameLevel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task InitializeGame()
        {
            ISaveSystem<PlayerProgress> saveProgressSystem = new SaveProgressSystem();
            ISaveSystem<PlayerUISettings> saveSettingsSystem = new SaveSettingsSystem();

            SaveManager saveManager = new SaveManager(saveProgressSystem, saveSettingsSystem);

            PlayerProgress progress = await saveProgressSystem.Load();
            PlayerUISettings playerUISettings = await saveSettingsSystem.Load();

            
            PlayerData playerData = new PlayerData(saveManager, progress, playerUISettings);

            ServiceLocator.RegisterPlayerData(playerData);
        }
    }
}