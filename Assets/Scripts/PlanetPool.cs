using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPool : BasePool<Planet>
{
    //如果不是一个Planet对应一个Pool的话就要考虑使用下面的逻辑

    // public List<GameObject> PlanetPrefabs;
    // [SerializeField] bool isSelected = false;//如果有个星球已经确认被认为是下次应该创建的, 值为true

    // private GameObject RandomGetAPlanetPrefabsFromList()
    // {
    //     return null; // todo 随机从已有的Prefabs中选一个要的星球
    // }

    // protected override PlanetPool OnCreatePoolItem()
    // {
    //     if (isSelected)
    //     {
    //         return null;
    //     }else{
    //         return base.OnCreatePoolItem();
    //     }
        
    // }

}
