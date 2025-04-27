using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBallPool : BasePool<SmallBall>
{
    public List<SmallBall> smallBalls;
    public int visibleActiveBallNum;

    void Start()
    {
        GameManager.OnGameStateChange += onGameStateChange;
        if (ActiveCount <= 0 && GameManager.Instance.isAllBallUsed)
        {
            GameManager.Instance.UpdateGameState(GameState.GameOver);
        }
    }

    void Update()
    {
        visibleActiveBallNum = ActiveCount;
    }

    protected override SmallBall OnCreatePoolItem()
    {
        var smallBall = base.OnCreatePoolItem();
        smallBalls.Add(smallBall);
        return smallBall;
    }

    protected override void OnReleasePoolItem(SmallBall obj)
    {
        base.OnReleasePoolItem(obj);
        if (ActiveCount <= 1 && GameManager.Instance.isAllBallUsed)
        {
            GameManager.Instance.UpdateGameState(GameState.GameOver);
        }
    }

    public void ReleaseAllSmallBall()
    {
        foreach (var smallBall in smallBalls)
        {
            if (smallBall.gameObject.activeSelf)
            {
                smallBall.ReleaseItself();
            }

        }
    }

    private void onGameStateChange(GameState gameState)
    {
        if (gameState == GameState.GameOver)
        {
            ReleaseAllSmallBall();
        }
    }


}
