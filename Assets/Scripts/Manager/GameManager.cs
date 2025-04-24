using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Audio Request 达到某个分数的时候的音效, 玩家被击中的音效 ,添加盾被击碎的音效和动画
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameState currentGameState;
    public GameState GameStat => currentGameState;
    [SerializeField] private PlayerState currentPlayerState;
    public PlayerState PlayerState => currentPlayerState;
    [SerializeField] private GameEvent currentgameEvent;
    public GameEvent GameEvent => currentgameEvent;

    // Example about how to use
    // OnGameStateChange += MethodWithParameter
    public static event Action<GameState> OnGameStateChange;
    public static event Action<PlayerState> OnPlayerStateChange;
    public static event Action<GameEvent> OnGameEventSent;

    [Header("Player Information")]
    public int CurrentPlayerHealth = 3;
    [SerializeField] int MaxPlayerHealth = 3;

    public int remainingSmallBallNum = 10;
    [SerializeField] int MaxSmallBallNum = 10;

    public bool ProtectShell;

    [Header("Game Information")]
    public float Score;
    public int destroyPlanetNum;
    public int numOfSmallBallShooted;
    public int MaxReachComboNum; //达到的最大连击数
    public bool isResurrection;
    public bool isAllBallUsed;


    public void GameInitialsation()
    {
        CurrentPlayerHealth = MaxPlayerHealth;
        remainingSmallBallNum = MaxSmallBallNum;
        destroyPlanetNum = 0;
        numOfSmallBallShooted = 0;
        MaxReachComboNum = 0;
        Score = 0;
        ProtectShell = false;
        isResurrection = false;
        UpdatePlayerState(PlayerState.Idel);
        SendGameEvent(GameEvent.Null);
    }

    #region GameState

    public void UpdateGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        switch (newGameState)
        {
            case GameState.GameStart:
                HandleGameStart();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            case GameState.GamePause:
                HandleGamePause();
                break;
        }
        OnGameStateChange?.Invoke(newGameState);
    }

    private void HandleGameOver()
    {
        Debug.Log("GameOver");
    }

    private void HandleGamePause()
    {

    }

    private void HandleGameStart()
    {
        GameInitialsation();
    }

    #endregion

    #region PlayerState
    public void UpdatePlayerState(PlayerState newPlayerState)
    {
        currentPlayerState = newPlayerState;
        switch (newPlayerState)
        {
            case PlayerState.ShootingABall:
                handlePlayerShootingABall();
                break;
            case PlayerState.GetHit:
                handlePlayerGetHit();
                break;
            case PlayerState.TakingDamage:
                handlePlayerTakingDamage();
                break;
            case PlayerState.Dead:
                handlePlayerDead();
                break;

        }
        OnPlayerStateChange?.Invoke(newPlayerState);
    }

    private void handlePlayerShootingABall()
    {
        if (remainingSmallBallNum > 0)
        {
            remainingSmallBallNum--;
            numOfSmallBallShooted++;
        }
        if (remainingSmallBallNum <= 0)
        {
            SendGameEvent(GameEvent.AllBallUsed);
        }
    }

    private void handlePlayerDead()
    {
        if (isResurrection)//代表复活道具
        {
            SendGameEvent(GameEvent.ResurrectionUsed);
        }
        else
        {
            UpdateGameState(GameState.GameOver);
        }
    }

    private void handlePlayerTakingDamage()
    {
        CurrentPlayerHealth -= 1;
        if (CurrentPlayerHealth <= 0)
        {
            UpdatePlayerState(PlayerState.Dead);
        }
    }

    private void handlePlayerGetHit()
    {
        if (ProtectShell)
        {
            ProtectShell = false;
            // 添加盾被击碎的音效和动画
            SendGameEvent(GameEvent.ProtectShellBreak);
        }
        else
        {
            UpdatePlayerState(PlayerState.TakingDamage);
        }
    }

    #endregion

    #region GameEvent

    public void SendGameEvent(GameEvent newGameEvent)
    {
        currentgameEvent = newGameEvent;
        switch (newGameEvent)
        {
            case GameEvent.ThreeComboHit:
                HandleThreeComboHit();
                break;
            case GameEvent.TenComboHit:
                HandleTenComboHit();
                break;
            case GameEvent.RewardABall:
                handleRewardABall();
                break;
            case GameEvent.ResurrectionUsed:
                HandleResurrectionUsed();
                break;
            case GameEvent.AllBallUsed:
                HandleAllBallUsed();
                break;
        }
        OnGameEventSent?.Invoke(newGameEvent);
    }

    private void HandleAllBallUsed()
    {
        isAllBallUsed = true;
        if (PoolManager.Instance.SmallBallPool.ActiveCount == 0 && isAllBallUsed)
        {
            UpdateGameState(GameState.GameOver);
        }
    }

    private void HandleResurrectionUsed()
    {
        isResurrection = false;
        CurrentPlayerHealth = MaxPlayerHealth; //以最大血量复活
    }

    private void handleRewardABall()
    {
        addSmallBallNum(1);
        isAllBallUsed = false;
    }

    private void HandleTenComboHit()
    {

    }

    private void HandleThreeComboHit()
    {

    }
    #endregion

    public void addScore(int ScoreNum, float ScoreMultiplier)
    {
        Score += ScoreNum * ScoreMultiplier;
    }

    public void addSmallBallNum(int Num)
    {
        int newNum = remainingSmallBallNum + Num;
        if (newNum >= MaxSmallBallNum)
        {
            remainingSmallBallNum = MaxSmallBallNum;
            //用于动画的逻辑层, 如果已经满了要有个提示
            SendGameEvent(GameEvent.SmallBallIsFull);
        }
        else
        {
            remainingSmallBallNum = newNum;
        }

    }

    public void AddDestroyPlanetNum()
    {
        destroyPlanetNum++;
    }

    public void AddNumOfSmallBallShooted()
    {
        numOfSmallBallShooted++;
    }

    public void CompareMaxComboNum(int comboNum)
    {
        if (comboNum >= MaxReachComboNum)
        {
            MaxReachComboNum = comboNum;
        }
    }

    public void GetPlayerProtectShell()
    {
        ProtectShell = true;
    }

}
//We need add more State or Event in future.
//Events for GameState
public enum GameState
{
    GameStart,
    GameOver,
    GamePause,

}

//Events for PlayerState, some Player behavior 
public enum PlayerState//通常被击中请调用GetHit, 除非真伤调用TakingDamage
{
    Idel,
    Aiming,
    ShootingABall,
    GetHit, // 被击中可能不需要动效, 只需要TakingDamage时和ProtectShellBreak有动效就可以了
    TakingDamage,
    Dead,

}

// Events for other GameEvent not include above. 
public enum GameEvent
{
    Null,
    ThreeComboHit,
    TenComboHit,// if a ball comboNum reach 10
    AllBallUsed,
    RewardABall,
    GetProtectShell,
    ProtectShellBreak,
    SmallBallIsFull,
    GetResurrection,
    ResurrectionUsed // 复活使用了

}

public enum CheatingMode
{
    InfiniteHealth, // 无限生命
    InfiniteSmallBallNumber, //无限小球数
    InfiniteSmallBallHitShellNumber,// 无限小球碰壁次数

}