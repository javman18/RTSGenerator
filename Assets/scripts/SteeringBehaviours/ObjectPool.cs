using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

   public void Awake()
    {
        Instance = this;
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
