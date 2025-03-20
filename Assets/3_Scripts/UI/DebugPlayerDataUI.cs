using _3_Scripts.Data;
using _3_Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPlayerDataUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CurrentLevelText;
    [SerializeField] private TextMeshProUGUI PreviousHighScoreText;
    [SerializeField] private TextMeshProUGUI CurrentColorListText;
    [SerializeField] private TextMeshProUGUI vibrationEnabledText;
    
    private PlayerData playerData;

    private void Start()
    {
        playerData = ServiceLocator.GetPlayerData();
        playerData.onProgressUpdate += UpdateProgressDebugUI;
        playerData.onSettingsUpdate += UpdateSettingsDebugUI;
        UpdateUI();
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
        CurrentLevelText.text = $"CurrentLevel: {playerData.playerProgress.CurrentLevel}";
        PreviousHighScoreText.text = $"HighScore: {playerData.playerProgress.PreviousHighScore}";
    }

    private void UpdateSettingsDebugUI()
    {
        if (playerData == null)
        {
            return;
        }
        CurrentColorListText.text = $"Color List: {playerData.playerUISettings.CurrentColorList}";
        vibrationEnabledText.text = $"Vibration Enabled: {playerData.playerUISettings.VibrationEnabled}";
    }

    private void OnDisable()
    {
        if (playerData == null)
        {
            return;
        }
        playerData.onProgressUpdate -= UpdateProgressDebugUI;
        playerData.onSettingsUpdate -= UpdateSettingsDebugUI;
    }

    public void ResetPlayerData()
    {
        Debug.Log("Reset Player Data");
        playerData.ResetAll();
    }
}
