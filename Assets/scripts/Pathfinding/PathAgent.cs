using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAgent : MonoBehaviour
{
    public float speed = 20f;
    private int currentIndex;
    private List<Vector3> path;
    bool pathExists;
    public GameObject target;
    AgentManager agent;
    private Vector3 movePos;
    public static bool pathFindig2Pressed;
    
    private void Awake()
    {
        movePos = transform.position;
    }
    void Start()
    {
        agent = GetComponent<AgentManager>();
        //vel = Vector2.zero;
    }
    // Update is called once per frame
    void Update()
    {

        Movement();
       if(pathFindig2Pressed == true && agent.sg.activeInHierarchy)
        {
            Debug.Log("siguele");
            MovePos();
        }
        //transform.position = movePos;
        //MovePos();
        //SetTarget(target.transform.position);
        //SetTargetWithMouse();

    }
    public void Movement()
    {
       
        
        if (path != null)
        {
            
            Vector2 targerPos = path[currentIndex];
            if(Vector3.Distance(transform.position, targerPos) > GetComponent<AgentManager>().mass * (GetComponent<AgentManager>().maxScale * 4))
            {

                //Vector3 dir = ((Vector3)targerPos - transform.position).normalized;
                //Vector2 distance = (targerPos - (Vector2)this.transform.position);
                //float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
                //transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
                //transform.position = transform.position + (Vector3)dir * speed * Time.deltaTime;
                agent.Seek(targerPos);
                //Vector2 seek = agent.Arrive(targerPos);
                //agent.Flocking(seek);

            }
            else
            {
                currentIndex++;
                if (currentIndex >= path.Count)
                {
                    Debug.Log("se detuvo");
                    pathFindig2Pressed = false;
                    path = null;
                    currentIndex = 0;
                }
                
            }
             
        }
        
    }
    public Vector3 GetPosition()
    {

        return transform.position;
    }
    
    public void SetTarget(Vector3 target, Vector3 helperPos, Pathfinding pathf) 
    {
        currentIndex = 0;
        
        path = pathf.FindPath(GetPosition(), target, helperPos);
        if (path != null && path.Count > 1)
        {
            
            path.RemoveAt(0);
        }

    }
    public void SetMovePos(Vector3 movepos)
    {
        
         this.movePos = movepos;
        
        
    }
    public void MovePos()
    {


        agent.Arrive(movePos);
            
        
    }
    public void SetTargetD(Vector3 target)
    {
        currentIndex = 0;

        path = Pathfinding.Instance.FindPathD(new Vector3(transform.position.x, transform.position.y), target);
        if (path != null && path.Count > 1)
        {

            path.RemoveAt(0);
        }
        else{
            this.movePos = target;
        }

    }

    public void SetTargetWithMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //SetTarget(mousePos);

       }
    }

    public void SetTargetWithMouseD()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetTargetD(mousePos);

        }
    }
}
