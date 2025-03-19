using System;
using System.Collections;
using System.Collections.Generic;
using _3_Scripts.Data;
using _3_Scripts.Data.Structure;
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
    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 1.0f / Time.timeScale;
        isOpen = false;
        animator.SetBool("isOpen", isOpen);
        playerUISettings = PlayerStats.Instance.playerUISettings;
        colorblindToggle.SetEnabled(playerUISettings.CurrentColorList == 1);
        vibrationToggle.SetEnabled(playerUISettings.VibrationEnabled);
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
        }
    }

    public void OnVibrationClick(bool value)
    {
        if (playerUISettings.VibrationEnabled != value) {
            playerUISettings.VibrationEnabled = value;
            if (value)
                Handheld.Vibrate();
        }
    }

    private void OnDisable()
    {
       PlayerStats.Instance.saveUISettings();
    }
}
