using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    



    public void UpdateGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        OnGameStateChange?.Invoke(newGameState);
    }


    public void UpdatePlayerState(PlayerState newPlayerState)
    {
        currentPlayerState = newPlayerState;
        OnPlayerStateChange?.Invoke(newPlayerState);
    }

    public void SendGameEvent(GameEvent newGameEvent)
    {
        currentgameEvent = newGameEvent;
        OnGameEventSent?.Invoke(newGameEvent);
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
    TakingDamage,
    Dead,
    
}

// Events for other GameEvent not include above. 
public enum GameEvent 
{
    Null,
    TenComboHit,// if a ball comboNum reach 10
}