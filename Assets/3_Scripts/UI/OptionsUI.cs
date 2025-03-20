using _3_Scripts.Data;
using _3_Scripts.Data.Structure;
using _3_Scripts.Utils;
using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [SerializeField]
    CustomToggle colorblindToggle;
    [SerializeField]
    CustomToggle vibrationToggle;

    Animator animator;
    bool isOpen;

    private PlayerUISettings playerUISettings;
    private PlayerData playerData;
    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 1.0f / Time.timeScale;
        isOpen = false;
        animator.SetBool("isOpen", isOpen);
        InitializePlayerData();
        colorblindToggle.SetEnabled(playerUISettings.CurrentColorList == 1);
        vibrationToggle.SetEnabled(playerUISettings.VibrationEnabled);
    }

    private void InitializePlayerData()
    {
        playerData = ServiceLocator.GetPlayerData();
        playerUISettings = playerData.playerUISettings;
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }

    public void OnColorblindClick(bool value)
    {
        if (playerUISettings.CurrentColorList == 1 != value) {
            playerUISettings.CurrentColorList = value ? 1 : 0;
            TileColorManager.Instance.SetColorList(playerUISettings.CurrentColorList);
            playerData.saveUISettings();
        }
    }

    public void OnVibrationClick(bool value)
    {
        if (playerUISettings.VibrationEnabled != value) {
            playerUISettings.VibrationEnabled = value;
            playerData.saveUISettings();
            if (value)
                Handheld.Vibrate();
        }
    }
}
