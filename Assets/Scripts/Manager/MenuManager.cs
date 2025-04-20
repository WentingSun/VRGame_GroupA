using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    GameObject Startmenu;
    GameObject GameOverMenu;
    GameObject PauseMenu;

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
            PauseMenu.SetActive(true);
        }
        else if (gameState == GameState.GameStart)
        {

        }
    }


}
