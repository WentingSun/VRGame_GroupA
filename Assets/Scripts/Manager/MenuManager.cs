using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{

    void Start()
    {
        GameManager.OnGameStateChange += onGameStateChange;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.OnGameStateChange -= onGameStateChange;
    }

    private void onGameStateChange(GameState gameState)
    {
        if (gameState == GameState.GamePause)
        {

        }
        else if (gameState == GameState.GameStart)
        {

        }
    }


}
