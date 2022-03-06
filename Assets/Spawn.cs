using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public bool selectedSpawn = false;
    // Start is called before the first frame update
    void Start()
    {
        SetSelected(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            GetComponent<SpriteRenderer>().color = new Color(170f / 255f, 241f / 255f, 158f / 255f);
            selectedSpawn = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1,1,1);
            selectedSpawn = false;
        }
    }
    public GameObject SpawnObject(GameObject obj)
    {
        obj = Instantiate(obj, transform.position, Quaternion.identity);
        return obj;
    }
    
}
