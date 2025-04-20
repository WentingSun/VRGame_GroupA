using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject GameOverMenu;

    [SerializeField] private TMPro.TextMeshProUGUI currentScoreText; // 暂停菜单显示当前分数
    [SerializeField] private TMPro.TextMeshProUGUI totalScoreText; // 游戏结束菜单显示总分数

    private void Start()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameState gameState)
    {
        Debug.Log($"GameState changed to: {gameState}"); // 添加日志
        HideAllMenus();

        switch (gameState)
        {
            case GameState.GameStart:
                ShowStartMenu();
                break;

            case GameState.GamePause:
                ShowPauseMenu();
                break;

            case GameState.GameOver:
                ShowGameOverMenu();
                break;

            case GameState.GameResume:
                ResumeGame();
                break;
        }
    }

    private void HideAllMenus()
    {
        StartMenu.SetActive(false);
        PauseMenu.SetActive(false);
        GameOverMenu.SetActive(false);
    }

    private void ShowStartMenu()
    {
        StartMenu.SetActive(true);
    }

    private void ShowPauseMenu()
    {
        PauseMenu.SetActive(true);
        UpdateCurrentScore(); // 更新当前分数
        EnableCollisionBypass(true); // 启用穿模逻辑
    }

    private void ShowGameOverMenu()
    {
        GameOverMenu.SetActive(true);
        UpdateTotalScore(); // 更新总分数
    }

    private void ResumeGame()
    {
        EnableCollisionBypass(false); // 禁用穿模逻辑
    }

    private void UpdateCurrentScore()
    {
        if (GameManager.Instance != null)
        {
            int currentScore = GameManager.Instance.GetCurrentScore();
            currentScoreText.text = $"Current Score: {currentScore}";
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null. Cannot update current score.");
        }
    }

    private void UpdateTotalScore()
    {
        if (GameManager.Instance != null)
        {
            int totalScore = GameManager.Instance.GetTotalScore();
            totalScoreText.text = $"Total Score: {totalScore}";
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null. Cannot update total score.");
        }
    }

    private void EnableCollisionBypass(bool enable)
    {
        int ballLayer = LayerMask.NameToLayer("Ball");
        int staticSceneLayer = LayerMask.NameToLayer("StaticScene");

        if (ballLayer == -1 || staticSceneLayer == -1)
        {
            Debug.LogError("Layer names 'Ball' or 'StaticScene' are not defined in the project settings.");
            return;
        }

        Physics.IgnoreLayerCollision(ballLayer, staticSceneLayer, enable);
    }

    // 按钮事件
    public void OnStartGameButton()
    {
        GameManager.Instance.UpdateGameState(GameState.GamePlay);
    }

    public void OnTutorialButton()
    {
        Debug.Log("Tutorial started.");
    }

    public void OnExitGameButton()
    {
        Application.Quit();
    }

    public void OnResumeGameButton()
    {
        GameManager.Instance.UpdateGameState(GameState.GameResume);
    }

    public void OnRestartGameButton()
    {
        GameManager.Instance.GameInitialsation();
        GameManager.Instance.UpdateGameState(GameState.GamePlay);
    }

    public void OnReturnToMainMenuButton()
    {
        GameManager.Instance.UpdateGameState(GameState.GameStart);
    }
}