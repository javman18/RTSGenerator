using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public bool selectedSpawn = false;
    public int iD = 0;
    // Start is called before the first frame update
    void Start()
    {
        SetSelected(false, new Color(1,1,1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelected(bool selected, Color color)
    {
        if (iD == 1 || iD==0)
        {
            if (selected)
            {
                GetComponent<SpriteRenderer>().color = color;
                selectedSpawn = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                selectedSpawn = false;
            }
        }
    }
    public GameObject SpawnObject(GameObject obj)
    {
        obj = Instantiate(obj, transform.position, Quaternion.identity);
        return obj;
    }
    
}
