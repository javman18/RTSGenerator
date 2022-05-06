using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    public List<GameObject> pooledObjects;
    //public GameObject objectToPool;
    public int amountToPool;
    public static List<GameObject> resource1List;
    public static List<GameObject> resource3List;
    public static List<GameObject> resource2List;
    public static List<GameObject> wallsList;
    public static List<GameObject> spawnList;
    public static List<GameObject> storageList;
    public static List<GameObject> brickList;
    public static List<GameObject> baseList;
    public int wallAmount;
    public GameObject wall;
    public int resource1Amount;
    public GameObject resource1;
    public int resource2Amount;
    public GameObject resource2;
    public int resource3Amount;
    public GameObject resource3;
    public int boxAmount;
    public GameObject box;
    public int spawnAmount;
    public GameObject spawn;
    public int brickAmount;
    public GameObject brick;
    public int baseAmount;
    public GameObject _base;
    
   

    public void Awake()
    {
        wallsList = new List<GameObject>();
        resource1List = new List<GameObject>();
        resource3List = new List<GameObject>();
        resource2List = new List<GameObject>();
        spawnList = new List<GameObject>();
        storageList = new List<GameObject>();
        brickList = new List<GameObject>();
        baseList = new List<GameObject>();
        Instance = this;
        SetPooledObjects(wallsList, wall, wallAmount);
        SetPooledObjects(resource1List, resource1, resource1Amount);
        SetPooledObjects(resource3List, resource3, resource3Amount);
        SetPooledObjects(resource2List, resource2, resource2Amount);
        SetPooledObjects(spawnList, spawn, spawnAmount);
        SetPooledObjects(brickList, brick, brickAmount);
        SetPooledObjects(baseList, _base, baseAmount);
    } 
    // Start is called before the first frame update
    public void SetPooledObjects(List<GameObject> pooledObj, GameObject objectToPool, int amountToPool)
    {
        
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObj.Add(tmp);
        }

    }
    public GameObject GetPooledObjects(List<GameObject> pooledObj, int amountToPool)
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObj[i].activeInHierarchy)
            {
                return pooledObj[i];
            }
        }
        return null;
    }
    
}
