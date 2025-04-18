using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : Singleton<WorldManager>
{
    public Transform RoomBallWorld;
    public Transform FootBallWorld;
    public Transform RoomBallWorldShell;
    public Transform FootBallWorldShell;
    public Transform RandomMoveTarget;

    void Update()
    {
        if (RoomBallWorld == null)
        {
            RoomBallWorld = GameObject.Find("RoomBallWorldShell").transform;
        }

        if (FootBallWorld == null)
        {
            FootBallWorld = GameObject.Find("FootBallWorldShell").transform;

        }

        if (RandomMoveTarget == null)
        {
            RandomMoveTarget = GameObject.Find("RandomMoveTarget").transform;
        }
    }
}
