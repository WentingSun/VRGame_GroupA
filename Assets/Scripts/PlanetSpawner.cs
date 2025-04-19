using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    [SerializeField] private PlanetPool planetPool; // 星球对象池
    [SerializeField] private List<Transform> spawnPoints; // 生成点列表
    [SerializeField] private float spawnInterval = 5f; // 星球生成间隔

    private void Start()
    {
        if (planetPool == null || spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("PlanetPool or SpawnPoints is not properly assigned!");
            return;
        }

        StartCoroutine(SpawnPlanets());
    }

    private IEnumerator SpawnPlanets()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            foreach (var spawnPoint in spawnPoints)
            {
                // 从对象池中获取星球
                Planet planet = planetPool.Get();
                if (planet != null)
                {
                    planet.transform.position = spawnPoint.position;
                    planet.transform.rotation = Quaternion.identity;
                    planet.gameObject.SetActive(true); // 激活星球
                }
            }
        }
    }
}