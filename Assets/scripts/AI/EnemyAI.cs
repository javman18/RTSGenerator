using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] baseNodes;
    void Start()
    {

        //if (GetComponent<AgentManager>().rank > 0)
        //{
        //    GameObject nBase = GetComponent<AgentManager>().GetClosestObject(GetComponent<AgentManager>().baseNodes);


        //    GetComponent<AgentManager>().MoveTo(nBase.transform.position, MapGenerator.pathFind, Vector3.zero);

        //}
        //GetComponent<PathAgent>().SetTarget()
        InvokeRepeating("GoToBase", 4f, 0.1f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        baseNodes = GameObject.FindGameObjectsWithTag("Base");
        GameObject nBase = GetClosestBase(baseNodes);
        if (GetComponent<AgentManager>().rank > 0)
        {
            if (nBase)
            {
                if (!GetComponent<AgentManager>().hasTarget)
                {
                    if (MapGenerator.rect1.Contains(nBase.transform.position) && (MapGenerator.rect2.Contains(transform.position) || MapGenerator.rect3.Contains(transform.position) || MapGenerator.rect4.Contains(transform.position))
                     || MapGenerator.rect2.Contains(nBase.transform.position) && (MapGenerator.rect1.Contains(transform.position) || MapGenerator.rect3.Contains(transform.position) || MapGenerator.rect4.Contains(transform.position))
                     || MapGenerator.rect3.Contains(nBase.transform.position) && (MapGenerator.rect1.Contains(transform.position) || MapGenerator.rect2.Contains(transform.position) || MapGenerator.rect4.Contains(transform.position))
                     || MapGenerator.rect4.Contains(nBase.transform.position) && (MapGenerator.rect1.Contains(transform.position) || MapGenerator.rect2.Contains(transform.position) || MapGenerator.rect3.Contains(transform.position)))
                    {
                        //PathAgent.pathFindig2Pressed = true;
                        Debug.Log("si esta en diferente mapa");
                        GetComponent<AgentManager>().Arrive(nBase.transform.position);

                    }
                }
            }
        }
    }

    public GameObject GetClosestBase(GameObject[] objects)
    {
        float nearestObject = Mathf.Infinity;
        GameObject closestObject = null;


        foreach (GameObject obj in objects)
        {
            float distanceToBase = (obj.transform.position - transform.position).sqrMagnitude;
            if (distanceToBase < nearestObject)
            {
                
                
                    if (obj.GetComponent<HomeBase>().iD != GetComponent<AgentManager>().team)
                    {
                        nearestObject = distanceToBase;
                        closestObject = obj;
                    }
                

            }
        }

        

            //Debug.Log(closestObject.name +" "+this.name);
            //Debug.DrawLine(this.transform.position, closestObject.transform.position);
            return closestObject;  
    }

    void GoToBase()
    {
        baseNodes = GameObject.FindGameObjectsWithTag("Base");
        if (GetComponent<AgentManager>().rank > 0)
        {
            GameObject nBase = GetClosestBase(baseNodes);

            if (nBase)
            {
                if (!GetComponent<AgentManager>().hasTarget)
                {
                    if (MapGenerator.rect1.Contains(nBase.transform.position) && MapGenerator.rect1.Contains(transform.position))
                    {
                        GetComponent<AgentManager>().MoveTo(nBase.transform.position, MapGenerator.pathFind, Vector3.zero);
                    }
                    else if (MapGenerator.rect2.Contains(nBase.transform.position) && MapGenerator.rect2.Contains(transform.position))
                    {

                        //Debug.Log("si esta dentro en el 2");
                        GetComponent<AgentManager>().MoveTo(nBase.transform.position, MapGenerator.pathfind2, new Vector3(200, 0, 0));
                    }
                    else if (MapGenerator.rect3.Contains(nBase.transform.position) && MapGenerator.rect3.Contains(transform.position))
                    {

                        //Debug.Log("si esta dentro en el 2");
                        GetComponent<AgentManager>().MoveTo(nBase.transform.position, MapGenerator.pathfind3, new Vector3(0, 200, 0));
                    }
                    else if (MapGenerator.rect4.Contains(nBase.transform.position) && MapGenerator.rect4.Contains(transform.position))
                    {

                        //Debug.Log("si esta dentro en el 2");
                        GetComponent<AgentManager>().MoveTo(nBase.transform.position, MapGenerator.pathfind4, new Vector3(200, 200, 0));
                    }
                }
            }


        }
    }
}
