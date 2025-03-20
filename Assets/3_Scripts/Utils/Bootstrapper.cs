using System;
using _3_Scripts.Data;
using _3_Scripts.Data.Services;
using _3_Scripts.Data.Structure;
using UnityEngine;

namespace _3_Scripts.Utils
{
    public class Bootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            ISaveSystem<PlayerProgress> saveProgressSystem = new SaveProgressSystem();
            ISaveSystem<PlayerUISettings> saveSettingsSystem = new SaveSettingsSystem();

            SaveManager saveManager = new SaveManager(saveProgressSystem, saveSettingsSystem);

            PlayerProgress progress = saveProgressSystem.Load();
            PlayerUISettings uiSettings = saveSettingsSystem.Load();

            
            PlayerData playerData = new PlayerData(saveManager, progress, uiSettings);

            ServiceLocator.RegisterPlayerData(playerData);
        }
    }
}