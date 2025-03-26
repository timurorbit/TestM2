using System;
using _3_Scripts.Data;
using _3_Scripts.Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPlayerDataUI : MonoBehaviour, IDisposable
{
    [SerializeField] private TextMeshProUGUI CurrentLevelText;
    [SerializeField] private TextMeshProUGUI PreviousHighScoreText;
    [SerializeField] private TextMeshProUGUI CurrentColorListText;
    [SerializeField] private TextMeshProUGUI vibrationEnabledText;
    
    private PlayerData playerData;
    public SaveManager saveManager;

    private void Start()
    {
        playerData = ServiceLocator.Instance.GetService<PlayerData>();
        saveManager = ServiceLocator.Instance.GetService<SaveManager>();
        UpdateUI();
        playerData.DataChanged += UpdateUI;
    }

    private void UpdateUI()
    {
        UpdateProgressDebugUI();
        UpdateSettingsDebugUI();
    }

    private void UpdateProgressDebugUI()
    {
        if (playerData == null)
        {
            return;
        }
        CurrentLevelText.text = $"CurrentLevel: {playerData.CurrentLevel}";
        PreviousHighScoreText.text = $"HighScore: {playerData.PreviousHighScore}";
    }

    private void UpdateSettingsDebugUI()
    {
        if (playerData == null)
        {
            return;
        }
        CurrentColorListText.text = $"Color List: {playerData.CurrentColorList}";
        vibrationEnabledText.text = $"Vibration Enabled: {playerData.VibrationEnabled}";
    }

    public async void ResetPlayerData()
    {
        await saveManager.ResetPlayerData();
        Debug.Log("Game data reset successfully");
    }

    public void Dispose()
    {
        playerData.DataChanged -= UpdateUI;
    }
}
