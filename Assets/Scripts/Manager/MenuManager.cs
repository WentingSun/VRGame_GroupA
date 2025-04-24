using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private GameObject StartMenu; // 初始菜单
    [SerializeField] private GameObject PauseMenu; // 暂停菜单
    [SerializeField] private GameObject GameOverMenu; // 游戏结束菜单

    [SerializeField] private TMPro.TextMeshProUGUI currentScoreText; // 暂停菜单显示当前分数
    [SerializeField] private TMPro.TextMeshProUGUI totalScoreText; // 游戏结束菜单显示总分数

    private void Start()
    {
        GameManager.OnGameStateChange += OnGameStateChange;

        // 手动调用以初始化菜单
        OnGameStateChange(GameManager.Instance.GameStat);
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

    private void PositionMenuInFrontOfCamera(GameObject menu)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 forward = cam.transform.forward;
        Vector3 position = cam.transform.position + forward * 2f; // 2米前方

        menu.transform.position = position;

        // 始终面向摄像机
        Vector3 lookAtPos = cam.transform.position;
        lookAtPos.y = menu.transform.position.y; // 可选：保持Y轴水平
        menu.transform.LookAt(lookAtPos);
        menu.transform.Rotate(0, 180, 0); // 因为LookAt背面朝向玩家，所以旋转180度
    }

    private void HideAllMenus()
    {
        StartMenu.SetActive(false);
        PauseMenu.SetActive(false);
        GameOverMenu.SetActive(false);
    }

    private void ShowStartMenu()
    {
        PositionMenuInFrontOfCamera(StartMenu);
        StartMenu.SetActive(true);
    }

    private void ShowPauseMenu()
    {
        PositionMenuInFrontOfCamera(PauseMenu);
        PauseMenu.SetActive(true);
        UpdateCurrentScore(); // 更新当前分数
        EnableCollisionBypass(true); // 启用穿模逻辑
    }

    private void ShowGameOverMenu()
    {
        PositionMenuInFrontOfCamera(GameOverMenu);
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
        //TODO : Implement tutorial logic here
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