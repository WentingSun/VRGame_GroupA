using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Audio Request 达到某个分数的时候的音效
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

    [Header("Game Information")]
    public float Score;
    public int destoryPlanetNum;
    public int numOfSmallBallShooted;
    public int MaxReachComboNum; //达到的最大连击数


    public void GameInitialsation()
    {
        CurrentPlayerHealth = MaxPlayerHealth;
        remainingSmallBallNum = MaxSmallBallNum;
        destoryPlanetNum = 0;
        numOfSmallBallShooted = 0;
        MaxReachComboNum = 0;
        Score = 0;
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
        OnPlayerStateChange?.Invoke(newPlayerState);
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
        }
        OnGameEventSent?.Invoke(newGameEvent);
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
            //
        }
        else
        {
            remainingSmallBallNum = newNum;
        }

    }

    public void AddDestoryPlanetNum()
    {
        destoryPlanetNum++;
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
public enum PlayerState
{
    Idel,
    Aiming,
    ShootingABall,
    TakingDamage,
    Dead,

}

// Events for other GameEvent not include above. 
public enum GameEvent
{
    Null,
    ThreeComboHit,
    TenComboHit,// if a ball comboNum reach 10
    AllBallUsed
}