using System;
using _3_Scripts.Data;
using _3_Scripts.Data.Structure;
using _3_Scripts.Infrastructure;
using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [SerializeField]
    CustomToggle colorblindToggle;
    [SerializeField]
    CustomToggle vibrationToggle;

    Animator animator;
    bool isOpen;
    
    private PlayerData playerData;
    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 1.0f / Time.timeScale;
        isOpen = false;
    }

    private void Start()
    {
        InitializePlayerData();
        animator.SetBool("isOpen", isOpen);
        colorblindToggle.SetEnabled(playerData.CurrentColorList == 1);
        vibrationToggle.SetEnabled(playerData.VibrationEnabled);
    }

    private void InitializePlayerData()
    {
        playerData = ServiceLocator.Instance.GetService<PlayerData>();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }

    public void OnColorblindClick(bool value)
    {
        if (playerData.CurrentColorList == 1 != value) {
            playerData.CurrentColorList = value ? 1 : 0;
            TileColorManager.Instance.SetColorList(playerData.CurrentColorList);
        }
    }

    public void OnVibrationClick(bool value)
    {
        if (playerData.VibrationEnabled != value) {
            playerData.VibrationEnabled = value;
            if (value)
                Handheld.Vibrate();
        }
    }
}
