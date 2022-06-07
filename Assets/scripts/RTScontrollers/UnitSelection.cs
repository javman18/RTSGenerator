using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] private Transform selectedAreaT;
    Vector3 startPos;
    private List<AgentManager> selectedAgents;
    public static float time = 0;
    public static int random = 0;
    private void Awake()
    {
        selectedAgents = new List<AgentManager>();
        selectedAreaT.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedAreaT.gameObject.SetActive(true);
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 lowerLeft = new Vector3(Mathf.Min(startPos.x, currentMousPos.x), Mathf.Min(startPos.y, currentMousPos.y));
            Vector3 upperRight = new Vector3(Mathf.Max(startPos.x, currentMousPos.x), Mathf.Max(startPos.y, currentMousPos.y));
            selectedAreaT.position = new Vector3(lowerLeft.x, lowerLeft.y);
            selectedAreaT.localScale = upperRight - lowerLeft;
        }
        if (Input.GetMouseButtonUp(0))
        {
            selectedAreaT.gameObject.SetActive(false);
            //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition) + " " + startPos);
            Collider2D[] agentCols = Physics2D.OverlapAreaAll(startPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));

            foreach (AgentManager agent in selectedAgents)
            {
                agent.SetSelected(false);
            }
            selectedAgents.Clear();
            foreach (Collider2D collider2D in agentCols)
            {
                AgentManager agent = collider2D.GetComponent<AgentManager>();
                if (collider2D is BoxCollider2D)
                {
                    if (agent != null)
                    {
                        if (agent.team == 1)
                        {
                            agent.SetSelected(true);
                            selectedAgents.Add(agent);
                        }
                        
                    }
                }
            }
        }
        Vector3 target = new Vector3();
        if (Input.GetMouseButtonDown(1))
        {
            time += Time.deltaTime;
            random = Random.Range(0, 6);
            Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.DrawLine(new Vector3(movePos.x, 2000), new Vector3(movePos.x, 0));
            Debug.DrawLine(new Vector3(2000, movePos.y), new Vector3(0, movePos.y));
            Debug.Log(new Vector3(MapGenerator.rect2.xMin, MapGenerator.rect2.yMin));
            Debug.DrawLine(new Vector3(MapGenerator.rect2.xMin, MapGenerator.rect2.yMin), new Vector3(MapGenerator.rect2.width, MapGenerator.rect2.height), Color.red);
            Debug.DrawLine(new Vector3(MapGenerator.rect3.xMin, MapGenerator.rect3.yMin), new Vector3(MapGenerator.rect2.width, MapGenerator.rect2.height), Color.red);
            foreach (AgentManager agent in selectedAgents)
            {
                
                if (MapGenerator.rect1.Contains(movePos) && MapGenerator.rect1.Contains(agent.transform.position))
                {
                    PathAgent.pathFindig2Pressed = false;
                    agent.MoveTo(movePos, MapGenerator.pathFind, Vector3.zero);
                    //Debug.Log("si esta dentro en el 1");
                }
                else if (MapGenerator.rect2.Contains(movePos) && MapGenerator.rect2.Contains(agent.transform.position))
                {
                    PathAgent.pathFindig2Pressed = false;
                    //Debug.Log("si esta dentro en el 2");
                    agent.MoveTo(movePos, MapGenerator.pathfind2, new Vector3(200, 0, 0));
                }
                else if (MapGenerator.rect3.Contains(movePos) && MapGenerator.rect3.Contains(agent.transform.position))
                {
                    PathAgent.pathFindig2Pressed = false;
                    //Debug.Log("si esta dentro en el 2");
                    agent.MoveTo(movePos, MapGenerator.pathfind3, new Vector3(0, 200, 0));
                }
                else if (MapGenerator.rect4.Contains(movePos) && MapGenerator.rect4.Contains(agent.transform.position))
                {
                    PathAgent.pathFindig2Pressed = false;
                    //Debug.Log("si esta dentro en el 2");
                    agent.MoveTo(movePos, MapGenerator.pathfind4, new Vector3(200, 200, 0));
                }
                else if (MapGenerator.rect1.Contains(movePos) && (MapGenerator.rect2.Contains(agent.transform.position) || MapGenerator.rect3.Contains(agent.transform.position) || MapGenerator.rect4.Contains(agent.transform.position))
                    || MapGenerator.rect2.Contains(movePos) && (MapGenerator.rect1.Contains(agent.transform.position) || MapGenerator.rect3.Contains(agent.transform.position) || MapGenerator.rect4.Contains(agent.transform.position))
                    || MapGenerator.rect3.Contains(movePos) && (MapGenerator.rect1.Contains(agent.transform.position) || MapGenerator.rect2.Contains(agent.transform.position) || MapGenerator.rect4.Contains(agent.transform.position))
                    || MapGenerator.rect4.Contains(movePos) && (MapGenerator.rect1.Contains(agent.transform.position) || MapGenerator.rect2.Contains(agent.transform.position) || MapGenerator.rect3.Contains(agent.transform.position)))
                {
                    PathAgent.pathFindig2Pressed = true;
                    
                    agent.MoveArrive(movePos);
                    
                }




            }
        }
        //foreach (AgentManager agent in selectedAgents)
        //{
        //   if( MapGenerator.rect1.Contains(agent.transform.position))
        //   {
        //        agent.MoveTo(target, MapGenerator.pathFind, Vector3.zero);
        //   }
        //    else if (MapGenerator.rect2.Contains(agent.transform.position))
        //    {
        //        agent.MoveTo(target, MapGenerator.pathfind2, new Vector3(200, 0, 0));
        //    }
        //}
    }
}
