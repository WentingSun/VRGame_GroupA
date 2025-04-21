using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField] private PlanetPool planetPool; // 星球对象池
    [SerializeField] private List<Transform> spawnPoints; // 生成点列表
    [SerializeField] private float spawnInterval = 5f; // 星球生成间隔
    [SerializeField] private Planet currentPlanet;
    public bool isSpawner; //判断是否已经生成一颗星球
    public bool isBooked; // 判断是否预定了下一个要生成的星球
    public string BookedPlanetName;

    public void CurrentPlanetTakeDamage()//just for test
    {
        if (currentPlanet.gameObject.activeSelf)
        {
            currentPlanet.TakeDamage(1);
        }
        
    }


    private void Start()
    {
        if (planetPool == null || spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("PlanetPool or SpawnPoints is not properly assigned!");
            return;
        }

        SpwnARandomPlanet();
    }

    public void SetNextSpawner(string name)
    {
        if (isBooked)
        {
            return;
        }
        else
        {
            isBooked = true;
            BookedPlanetName = name;
        }

    }

    private IEnumerator SpawnPlanets()
    {

        yield return new WaitForSeconds(spawnInterval);
        if (isBooked && BookedPlanetName != null)
        {
            SpawnPlanetsWithName(BookedPlanetName);
            isBooked = false;
        }
        else
        {
            SpwnARandomPlanet();
        }


    }

    private void SpawnPlanetsWithName(string name)
    {
        isSpawner = true;
        Planet planet = planetPool.GetByName(name);
        if (planet != null)
        {
            planet.transform.parent = transform;
            planet.transform.position = transform.position;
            planet.transform.rotation = Quaternion.identity;
            planet.gameObject.SetActive(true); // 激活星球
            planet.SetPlanetSpawner(this);
            currentPlanet = planet;
        }
    }

    public void StartSpawnAPlanet()
    {
        StartCoroutine(SpawnPlanets());
    }

    private void SpwnARandomPlanet()
    {
        isSpawner = true;
        foreach (var spawnPoint in spawnPoints)
        {
            // 从对象池中获取星球
            Planet planet = planetPool.Get();
            if (planet != null)
            {
                planet.transform.parent = spawnPoint.transform;
                planet.transform.position = spawnPoint.position;
                planet.transform.rotation = Quaternion.identity;
                planet.gameObject.SetActive(true); // 激活星球
                planet.SetPlanetSpawner(this);
                currentPlanet = planet;
            }
        }
    }
}