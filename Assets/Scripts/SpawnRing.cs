using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SpawnRing : MonoBehaviour
{
    public SpawnRingPosition spawnRingPosition;
    [SerializeField] private List<PlanetSpawner> PlanetSpawners = new List<PlanetSpawner>();
    [SerializeField] private List<PlanetGenerationPointMovement> planetGenerationPointMovements = new List<PlanetGenerationPointMovement>();
    [SerializeField] private List<bool> PlanetStates = new List<bool>();


    void Start()
    {
        Initialisation();
    }

    private void Initialisation()
    {
        foreach (var spawner in GetComponentsInChildren<PlanetSpawner>())
        {
            if (spawner.gameObject != this.gameObject)
            {
                PlanetSpawners.Add(spawner);
                planetGenerationPointMovements.Add(spawner.GetComponent<PlanetGenerationPointMovement>());
                spawner.ring = this;
                PlanetStates.Add(true);
            }
        }

        UniformSpawner();

    }

    private void UniformSpawner()
    {
        Vector3 IniAxis = planetGenerationPointMovements[0].axisDirection;
        for (int i = 0; i < PlanetSpawners.Count; i++)
        {
            if (planetGenerationPointMovements[i].axisDirection != IniAxis)
            {
                planetGenerationPointMovements[i].axisDirection = IniAxis;
            }
            planetGenerationPointMovements[i].numOfSameOrbit = PlanetSpawners.Count;
            planetGenerationPointMovements[i].OrbitIndex = i;
        }
    }

    public void SetPlanetState(PlanetSpawner PlanetSpawner, bool State = false)
    {
        int index = PlanetSpawners.IndexOf(PlanetSpawner);
        PlanetStates[index] = State;
        bool allFalse = PlanetStates.All(x => x == false);
        if (allFalse)
        {
            GainRingReware(spawnRingPosition);
            for (int i = 0; i < PlanetStates.Count; i++)
            {
                PlanetStates[i]=true;
            }
        }
    }

    private void GainRingReware(SpawnRingPosition spawnRingPosition)
    {
        switch (spawnRingPosition)
        {
            case SpawnRingPosition.Inner:
            GameManager.Instance.SendGameEvent(GameEvent.GetResurrection);
            break;

            case SpawnRingPosition.Central:
            GameManager.Instance.SendGameEvent(GameEvent.GetProtectShell);
            break;

            case SpawnRingPosition.Outer:
            GameManager.Instance.SendGameEvent(GameEvent.RewardTenBall);
            break;
        }
    }
}


public enum SpawnRingPosition
{
    Inner,
    Central,
    Outer
}
