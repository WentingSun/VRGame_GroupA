using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    //对象池就像这样硬编码在这里吧
    // 使用的时候就是 SmallBallPool.Get()这样
    // Pool 常用的的就三个Method
    // Get() 类似于Instantiate
    // Release() 类似于 Destory. 注意嗷, 如果有些东西你是写在onDestory()中的, 可能需要你将写个OnRelease(), 然后在ReleaseItself中添加这个.
    // Clear() 清空物品池, 游戏结束都不用清理. 基本不用
    // 详情看SmallBall 例子
    public SmallBallPool SmallBallPool;
    public PlanetPool defaultPlanetPool;




    public void testSmallBallPoolGet()
    {
        Debug.Log("test button");
        var ball = SmallBallPool.Get();// 类似于使用了Instantiate()
        
        ball.transform.position  = WorldManager.Instance.RandomMoveTarget.position;
        ball.applyForce();
    }

}

