using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private VRPointer vrPointer; // 手势控制器
    [SerializeField] private GameObject StartMenu; // 初始菜单
    [SerializeField] private GameObject PauseMenu; // 暂停菜单
    [SerializeField] private GameObject GameOverMenu; // 游戏结束菜单

    [SerializeField] private TMPro.TextMeshProUGUI currentScoreText; // 暂停菜单显示当前分数
    [SerializeField] private TMPro.TextMeshProUGUI totalScoreText; // 游戏结束菜单显示总分数

    [SerializeField] private TMPro.TextMeshProUGUI smallBallShootedText; // 显示小球射击数量
    [SerializeField] private TMPro.TextMeshProUGUI videoHintText; // 视频提示文字
    [SerializeField] private List<string> videoHints = new List<string>
    {
        "motherfucker",
        "caonimade",
        "woaisinile 111"
    };
    private void Start()
    {
        GameManager.OnGameStateChange += OnGameStateChange;

        // 手动调用以初始化菜单
        OnGameStateChange(GameManager.Instance.GameStat);

        // 确保视频屏幕在游戏开始时隐藏
        if (videoScreen != null)
        {
            videoScreen.SetActive(false);
        }
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
    public void OnPauseGameButton()
    {
        Debug.Log("Game paused.");
        GameManager.Instance.UpdateGameState(GameState.GamePause);
    }

    private void PositionMenuInFrontOfCamera(GameObject menu)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 forward = cam.transform.forward;
        Vector3 position = cam.transform.position + forward * 1f; // 1米前方

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
        Time.timeScale = 0f; // 暂停游戏
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

        // 更新总分数
        UpdateTotalScore();

        // 更新小球射击数量
        if (GameManager.Instance != null)
        {
            int smallBallShooted = GameManager.Instance.numOfSmallBallShooted;
            smallBallShootedText.text = $"Balls Shot: {smallBallShooted}";
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null. Cannot update small ball count.");
        }
    }

    private void ResumeGame()
    {
        EnableCollisionBypass(false); // 禁用穿模逻辑
        Time.timeScale = 1f;
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
    [SerializeField] private VideoPlayer videoPlayer; // 视频播放器
    [SerializeField] private GameObject videoScreen; // 视频显示的屏幕（例如 RawImage 或 3D 对象）
    [SerializeField] private List<VideoClip> tutorialVideos; // 按顺序播放的视频列表

    private int currentVideoIndex = 0;

    public void OnTutorialButton()
    {
        if (tutorialVideos == null || tutorialVideos.Count == 0)
        {
            Debug.LogError("No tutorial videos assigned.");
            return;
        }

        Debug.Log("Tutorial started.");
        videoScreen.SetActive(true); // 显示视频屏幕
        currentVideoIndex = 0; // 从第一个视频开始
        PlayNextVideo();
    }

    private void PlayNextVideo()
    {
        if (currentVideoIndex >= tutorialVideos.Count)
        {
            Debug.Log("All tutorial videos finished.");
            videoScreen.SetActive(false); // 隐藏视频屏幕
            videoHintText.text = ""; // 清空提示文字

            // 重新启用手势
            if (vrPointer != null)
            {
                vrPointer.enabled = true;
            }

            return;
        }

        // 禁用手势
        if (vrPointer != null)
        {
            vrPointer.enabled = false;
        }

        // 设置当前视频
        videoPlayer.clip = tutorialVideos[currentVideoIndex];
        Debug.Log($"Setting video clip: {tutorialVideos[currentVideoIndex].name}");
        videoPlayer.Play();
        Debug.Log($"Playing video: {tutorialVideos[currentVideoIndex].name}");

        // 更新提示文字
        if (currentVideoIndex < videoHints.Count)
        {
            videoHintText.text = videoHints[currentVideoIndex];
        }
        else
        {
            videoHintText.text = "";
        }

        // 确保移除之前的事件监听器，避免重复调用
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(UnityEngine.Video.VideoPlayer vp)
    {
        Debug.Log("Video finished playing.");
        videoPlayer.loopPointReached -= OnVideoFinished; // 移除事件监听
        currentVideoIndex++; // 播放下一个视频

        if (currentVideoIndex < tutorialVideos.Count)
        {
            PlayNextVideo(); // 播放下一个视频
        }
        else
        {
            Debug.Log("All tutorial videos finished.");
            videoScreen.SetActive(false); // 隐藏视频屏幕
            videoHintText.text = ""; // 清空提示文字

            // 重新启用手势
            if (vrPointer != null)
            {
                vrPointer.enabled = true;
            }
        }
    }

    // 按钮事件
    public void OnStartGameButton()
    {
        Time.timeScale = 1f; // 确保游戏时间正常流动
        GameManager.Instance.UpdateGameState(GameState.GamePlay);
    }


    public void OnExitGameButton()
    {
        Debug.Log("Exiting game...");

    /* #if UNITY_EDITOR
        // 如果在 Unity 编辑器中，退出播放模式
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // 如果是构建后的应用程序，退出游戏
        Application.Quit();
    #endif */
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