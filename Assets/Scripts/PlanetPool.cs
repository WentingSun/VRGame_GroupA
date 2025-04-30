using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlanetPool : MonoBehaviour
{
    [System.Serializable]
    public class PlanetWeight
    {
        public string planetType; // 星球类型
        public GameObject prefab; // 星球预制体
        public int weight; // 权重
    }

    [SerializeField]
    private List<PlanetWeight> planetWeights = new List<PlanetWeight>
    {
        new PlanetWeight { planetType = "normalPlanet", weight = 49 },
        new PlanetWeight { planetType = "SplitPlanet", weight = 15 },
        new PlanetWeight { planetType = "SpeedUpPlanet", weight = 10 },
        new PlanetWeight { planetType = "SlowDownPlanet", weight = 10 },
        new PlanetWeight { planetType = "MoreBallPlanet", weight = 1 },
        new PlanetWeight { planetType = "ExtraScorePlanet", weight = 10 },
        new PlanetWeight { planetType = "ExtraCollisionPlanet", weight = 3 },
        new PlanetWeight { planetType = "ExplosivePlanet", weight = 2 }
    };
    [SerializeField] private List<GameObject> pooledPlanets = new List<GameObject>(); // 对象池中的星球

    public Planet Get()
    {
        // 从权重列表中随机选择一个星球
        GameObject prefab = GetRandomPlanetPrefab();
        if (prefab == null) return null;

        // 查找池中是否有未使用的星球
        foreach (var planet in pooledPlanets)
        {
            if (!planet.activeInHierarchy && planet.name.StartsWith(prefab.name))
            {
                return planet.GetComponent<Planet>();
            }
        }

        // 如果没有可用的星球，则实例化一个新的
        GameObject instance = Instantiate(prefab, transform);
        instance.name = prefab.name; // 确保实例名称与预制体一致
        pooledPlanets.Add(instance);
        return instance.GetComponent<Planet>();
    }

    public Planet GetByName(string planetName)
    {

        foreach (var planet in pooledPlanets)
        {
            if (!planet.activeInHierarchy && planet.name.StartsWith(planetName))
            {
                return planet.GetComponent<Planet>();
            }
        }

        var PlanetWeight = planetWeights.Find(x => x.planetType == planetName);
        Planet result = null;


        if (PlanetWeight != null)
        {
            GameObject instance = Instantiate(PlanetWeight.prefab, transform);
            instance.name = PlanetWeight.planetType;
            pooledPlanets.Add(instance);
            result = instance.GetComponent<Planet>();

        }
        else
        {
            result = Get();
        }
        return result;
    }

    private GameObject GetRandomPlanetPrefab()
    {
        if (planetWeights.Count == 0)
        {
            Debug.LogError("No planet weights assigned!");
            return null;
        }

        // 计算总权重
        int totalWeight = 0;
        foreach (var weight in planetWeights)
        {
            totalWeight += weight.weight;
        }

        // 根据权重随机选择
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var weight in planetWeights)
        {
            currentWeight += weight.weight;
            if (randomValue < currentWeight)
            {
                return weight.prefab;
            }
        }

        return null;
    }
}