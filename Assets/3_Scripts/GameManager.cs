using System.Collections;
using System.Collections.Generic;
using _3_Scripts.Data;
using _3_Scripts.Data.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum GameState
{
    Intro = 0,
    Playing = 1,
    Win = 2,
    WaitingLose = 3,
    Lose = 4,
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    Tower tower;
    [SerializeField]
    PercentCounter percentCounter;
    [SerializeField]
    BallShooter ballShooter;
    [SerializeField]
    TextMeshProUGUI ballCountText;
    [SerializeField]
    ComboUI comboUI;
    [SerializeField]
    Animation oneBallRemaining;
    [SerializeField]
    AnimationCurve percentRequiredPerLevel;
    [SerializeField]
    AnimationCurve floorsPerLevel;
    [SerializeField]
    AnimationCurve ballToTileRatioPerLevel;
    [SerializeField]
    AnimationCurve colorCountPerLevel;
    [SerializeField]
    AnimationCurve specialTileChancePerLevel;

    [SerializeField]
    ParticleSystem tileDestroyFx;
    [SerializeField]
    ParticleSystem tileExplosionFx;

    Animator animator;

    float minPercent = 0;
    int tileCount;
    int destroyedTileCount;
    int ballCount;
    GameState gameState = GameState.Intro;

    private PlayerProgress playerProgress;
    private PlayerUISettings playerUISettings;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        animator.speed = 1.0f / Time.timeScale;
        FxPool.Instance.EnsureQuantity(tileExplosionFx, 3);
        FxPool.Instance.EnsureQuantity(tileDestroyFx, 30);
        PlayerStats.Instance.LoadAll();
    }

    private void Start()
    {
        playerProgress = PlayerStats.Instance.playerProgress;
        playerUISettings = PlayerStats.Instance.playerUISettings;
        TileColorManager.Instance.SetColorList(playerUISettings.CurrentColorList);
        TileColorManager.Instance.SetMaxColors(Mathf.FloorToInt(colorCountPerLevel.Evaluate(playerProgress.CurrentLevel)), true);
        SetupTower();

        minPercent = percentRequiredPerLevel.Evaluate(playerProgress.CurrentLevel);
        tileCount = tower.FloorCount * tower.TileCountPerFloor;
        ballCount = Mathf.FloorToInt(ballToTileRatioPerLevel.Evaluate(playerProgress.CurrentLevel) * tileCount);
        ballCountText.text = ballCount.ToString("N0");
        ballShooter.OnBallShot += OnBallShot;

        SetPercentCounterValues();
    }

    private void SetupTower()
    {
        tower.FloorCount = Mathf.FloorToInt(floorsPerLevel.Evaluate(playerProgress.CurrentLevel));
        tower.SpecialTileChance = specialTileChancePerLevel.Evaluate(playerProgress.CurrentLevel);
        tower.OnTileDestroyedCallback += OnTileDestroyed;
        tower.BuildTower();
    }

    private void SetPercentCounterValues()
    {
        percentCounter.SetColor(TileColorManager.Instance.GetColor(Mathf.FloorToInt(Random.value * TileColorManager.Instance.ColorCount)));
        percentCounter.SetLevel(playerProgress.CurrentLevel);
        percentCounter.SetValue(playerProgress.PreviousHighScore);
        percentCounter.SetShadowValue(playerProgress.PreviousHighScore);
        percentCounter.SetValueSmooth(0f);
    }

    void OnBallShot()
    {
        ballCount--;
        ballCountText.text = ballCount.ToString("N0");
        if (ballCount == 1) {
            oneBallRemaining.Play();
        }
        else if (ballCount == 0)
        {
            CountScore();
            SetGameState(GameState.WaitingLose);
        }
    }

    private void CountScore()
    {
        var highScore = ((float)destroyedTileCount / tileCount) / minPercent;
        if (highScore > playerProgress.PreviousHighScore)
        {
            playerProgress.PreviousHighScore = highScore;
            PlayerStats.Instance.SaveProgress();
        }
    }

    void SetGameState(GameState state)
    {
        gameState = state;
        animator.SetInteger("GameState", (int)state);
    }

    public void OnTileDestroyed(TowerTile tile)
    {
        if (gameState == GameState.Playing || gameState == GameState.WaitingLose) {
            comboUI.CountCombo(tile.transform.position);
            destroyedTileCount++;
            float p = (float)destroyedTileCount / tileCount;
            percentCounter.SetValueSmooth(p / minPercent);
            if (p >= minPercent) {
                CameraShakeManager.Instance.StopAll(true);
                CameraShakeManager.Instance.enabled = false;
                playerProgress.CurrentLevel++;
                playerProgress.PreviousHighScore = 0;
                PlayerStats.Instance.SaveProgress();
                SetGameState(GameState.Win);
                if (playerUISettings.VibrationEnabled)
                    Handheld.Vibrate();
            }
        }
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
        tower.StartGame();
    }

}
