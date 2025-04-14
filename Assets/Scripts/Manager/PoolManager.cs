using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{

    public SmallBallPool SmallBallPool;
    public PlanetPool defaultPlanetPool;




    public void testSmallBallPoolGet()
    {
        Debug.Log("test button");
        var ball = SmallBallPool.Get();
        
        ball.transform.position  = GameManager.Instance.targetPosition.position;
        ball.applyForce();
    }

}

