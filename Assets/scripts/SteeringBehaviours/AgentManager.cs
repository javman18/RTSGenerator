using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class AgentManager : SteerinBehaviours
{
    public bool isSeek;
    public bool isFlee;
    public bool isPursue;
    public bool isEvade;
    public bool isArrive;
    public bool isCohesion;
    public bool isCollector;
    public bool isTitan;
    public int copperInv = 0;
    public int metalInv = 0;
    public int scrapsInv = 0;
    public int inventory = 0;
    public int resourcesCarryLimt = 1;
    public float nearestResource = Mathf.Infinity;
    public Node closestResource = null;
    public float secondNearest = Mathf.Infinity;
    public float shootingRaange;
    GameObject wall;
    public Node nextClosest = null;
    PathAgent pAgent;
    public bool hasLeader;
    public bool isLeader;
    public int rank;
    public GameObject[] leaders;
    public int squad;
    public int team;
    GameObject fogOfWar;
    public GameObject bullet;
    public GameObject firePoint;
    bool canShoot = true;
    public float startTimeBtwShots=0.1f;
    public float bulletForce = 30;
    public int ammo = 50;
    public GameObject sg;
    public bool isFollowing;
    public Animator anim;
    AgentManager[] agents;
    bool meleeAtack = true;
    public float attackSize = 0.5f;
    public float attackRange = 0.5f;
    public float attackTime = 0.5f;
    public bool isAtacker;
    public bool isShooter;
    public float meleeDamage;
    public float bulletDamage;
    GameObject tmpBullet;
    bool isAgentMoving = false;
    public LayerMask agentsLayers;
    public LayerMask wallsLayer;
    bool isAttacking = false;
    bool isShooting = false;
    public float maxScale = 15f;
    public float healthAmount = 150;
    public bool isLeaderAlive;
    public float fleeTime;
    bool startTime = false;
    bool isFloking;
    bool wallHitted;
    public List<GameObject> bullets;
    public GameObject objectToPool;
    int amountToPool = 20;
    GameObject target;
    bool isCollecting;
    bool showlifebar = true;
    float maxHP;
    GameObject hpb;
    void Start()
    {
        bullets = new List<GameObject>();
        
        anim = GetComponent<Animator>();
        transform.gameObject.tag = "Agent";
        if (showlifebar)
        {

            hpb = Instantiate(Resources.Load<GameObject>("GameHelpers/HealthBar"));

            hpb.transform.GetChild(0).gameObject.GetComponent<HealhBar>().Set(this, healthAmount);
        }
        maxHP = healthAmount;
        transform.gameObject.layer = LayerMask.NameToLayer("Agents");
        agentsLayers = LayerMask.GetMask("Agents");
        wallsLayer = LayerMask.GetMask("Walls");
        InitAgentObjects();
        fogOfWar = GameObject.Find("fogofwarcanvas");
        SaveAgent.agents.Add(this);
        pAgent = GetComponent<PathAgent>();
        Awake();
        SetSelected(false);
        GetComponent<BoxCollider2D>().size = new Vector2(2f, 1.06f);
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/" + this.name);
        agents = FindObjectsOfType<AgentManager>();
        if (isCollector)
        {
            isCollecting = true;
        }
    }

    private void Update()
    {
        if (sg.activeInHierarchy)
        {
            hpb.SetActive(true);
        }
        else
        {
            hpb.SetActive(false);
        }
        agents = FindObjectsOfType<AgentManager>();
        targets = new Transform[agents.Length - 1];
        
        StartCoroutine(CeheckMovement(gameObject));
        if (isAgentMoving == false)
        {
            anim.SetFloat("Speed", 0.0f);
        }
        else
        {
            anim.SetFloat("Speed", 1.0f);
        }
            int count = 0;
        foreach (AgentManager agent in agents)
        {
            if (agent.gameObject != gameObject)
            {

                targets[count] = agent.transform;
                count++;

            }
        }
        
        wall = GameObject.FindGameObjectWithTag("Wall");
        foreach (AgentManager agent in agents)
        {
            if (agent.GetComponent<AgentManager>().rank > 0)
            {
                agent.transform.gameObject.tag = "Leader";
                agent.GetComponent<AgentManager>().isLeader = true;

            }
            else
            {
                agent.GetComponent<AgentManager>().isLeader = false;

            }
        }
       
        leaders = GameObject.FindGameObjectsWithTag("Leader");
        if (rank == 0)
        {
            //DetectWall();
        }
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 10, 1990), Mathf.Clamp(transform.position.y, 10, 1990), 0);
        FollowClosestLeader();

        if (team == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.blue;
        }
        if (team == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.red;
            transform.GetChild(3).gameObject.SetActive(false);
            GetComponent<SpriteRenderer>().color = Color.red;
            if (fogOfWar.transform.GetChild(0).gameObject.activeSelf == false)
            {
                transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(2).gameObject.SetActive(false);
            }

        }

        if (isLeader && team == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.magenta;
        }
        if (isLeader && team == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.green;
        }
        SetBehaviors();
        SetRTSBehaviours();
        if (startTime)
        {
            fleeTime += Time.deltaTime;
        }
        if (healthAmount <= 0)
        {
           gameObject.SetActive(false);
        }
        DetectWall();
       

    }
    void LateUpdate()
    {
       

    }
    private void OnDestroy()
    {
        SaveAgent.agents.Remove(this);
    }

    public void SetBehaviors()
    {
        AgentManager closestEnemy = null;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
       closestEnemy = GetClosestEnemy(agents);
       
        float d = 0;
        if (closestEnemy != null)
        {
            Vector2 desired = closestEnemy.transform.position - transform.position;
            d = desired.magnitude;
        }
        else if(sg.gameObject.activeInHierarchy == false)
        {
            //Wander();
        }
        if (isSeek)
        {

            if (d < seekPerception && d>shootingRaange)
            {
                if (closestEnemy != null)
                {
                   
                    SetSteeringWeight(Seek(closestEnemy.transform.position), 1);
                    Wander();
                }

            }
            else
            {
                pAgent.enabled = true;
            }
        }
        if (isFlee)
        {
            if (d < fleePerception && sg.activeInHierarchy == false)
            {
                if (closestEnemy != null)
                {
                    if (isCollector)
                    {
                        isCollecting = false;
                        
                    }
                    Flee(closestEnemy.transform.position);
                }
            }
            else if (isCollector)
            {
                isCollecting = true;
            }

        }
        
        if (isEvade)
        {
            foreach (AgentManager agent in agents)
            {
                if(agent != gameObject)
                    Evade(agent.transform);
            }
            
        }
        if (isArrive)
        {
            //Flocking();
            Arrive(mousePos);
            // CalcSteerig(steer);
        }
        if (isPursue || isPursue && isShooter && closestEnemy.isLeaderAlive == false && !closestEnemy.isLeader)
        {
            if (closestEnemy != null)
            {
                if (!closestEnemy.isPursue)
                {
                    if (d < seekPerception && sg.activeInHierarchy == false)
                    {
                    
                        Pursue(closestEnemy.transform);
                    }
                }
            }
        }
        
    }
    
    public void Flocking()
    {
        Vector2 align = Align();
        Vector2 separate = Separate();
        Vector2 cohesion = Cohesion();
        vel += separate * separationWeigh + cohesion * cohesionWeight;
    }

    public void Collect(Rect rect, Pathfinding p)
    {
       

        int count = 0;
        
        nearestResource = Mathf.Infinity;
        float nearestStorage = Mathf.Infinity;
        closestResource = null;
        Node closestStorage = null;
        secondNearest = Mathf.Infinity;
        nextClosest = null;
        foreach (Node resource in TileMap.resources)
        {
            if (rect.Contains(transform.position))
            {
                float distance = (p.GetMap().GetPosition(resource.x, resource.y) - transform.position).sqrMagnitude;

                if (distance < nearestResource)
                {
                    if (p.GetMap().GetMapNode(resource.x, resource.y).isResource)
                    {

                        nearestResource = distance;
                        closestResource = p.GetMap().GetMapNode(resource.x, resource.y);
                    }
                }
            }
        }
        Node.NodeObject nodeSprite = Node.NodeObject.None ;
        if (closestResource != null)
        {
            nodeSprite = closestResource.GetNodeObject();
        
           // Debug.DrawLine(this.transform.position, p.GetMap().GetPosition(closestResource.x + 0.5f, closestResource.y + 0.5f), Color.red);
        }
        if (nodeSprite == Node.NodeObject.Metal)
        {
            if ((transform.position - p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y)).magnitude <= 1f && inventory <= resourcesCarryLimt && isCollector)
            {
               
                closestResource.SetNodeObject(Node.NodeObject.None);
                closestResource.isResource = false;
                metalInv++;
                inventory++;
            }
            if (inventory < resourcesCarryLimt)
            {
                Arrive(p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y));
            }

        }
        else if(nodeSprite == Node.NodeObject.Scrap)
        {
            if ((transform.position - p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y)).magnitude <= 1f && inventory <= resourcesCarryLimt && isCollector)
            {

                closestResource.SetNodeObject(Node.NodeObject.None);
                closestResource.isResource = false;
                scrapsInv++;
                inventory++;
            }
            if (inventory < resourcesCarryLimt)
            {
                Arrive(p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y));
            }
        }
        else if(nodeSprite == Node.NodeObject.Copper)
        {
            if ((transform.position - p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y)).magnitude <= 1f && inventory <= resourcesCarryLimt && isCollector)
            {

                closestResource.SetNodeObject(Node.NodeObject.None);
                closestResource.isResource = false;
                copperInv++;
                inventory++;
            }
            if (inventory < resourcesCarryLimt)
            {
                Arrive(p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y));
            }
        }
        
        foreach (Node storage in TileMap.storages)
        {
            if (rect.Contains(transform.position))
            {
                float distance = (p.GetMap().GetPosition(storage.x, storage.y) - transform.position).sqrMagnitude;
                if (distance < nearestStorage)
                {
                    if (p.GetMap().GetMapNode(storage.x, storage.y).isStorage)
                    {


                        nearestStorage = distance;
                        closestStorage = p.GetMap().GetMapNode(storage.x, storage.y);

                    }


                    //Seek(storage.transform.position);
                }
            }
            //float dist = (storage.transform.position - transform.position).sqrMagnitude;
            
        }
        Node.NodeObject nodeSpriteS = Node.NodeObject.None;
        if (closestStorage != null)
        {
            nodeSpriteS = closestStorage.GetNodeObject();

            Debug.DrawLine(this.transform.position, p.GetMap().GetPosition(closestStorage.x + 0.5f, closestStorage.y + 0.5f), Color.red);
        }
        if (nodeSpriteS == Node.NodeObject.Box)
        {
            
            if (inventory >= resourcesCarryLimt)
            {
                
                //pAgent.SetTarget(MapGenerator.pathFind.GetMap().GetPosition(closestStorage.x, closestStorage.y));
                Arrive(p.GetMap().GetPosition(closestStorage.x, closestStorage.y));
            }
            if ((transform.position - p.GetMap().GetPosition(closestStorage.x, closestStorage.y)).magnitude <= 10f)
            {
                Player.copper += copperInv;
                Player.metal += metalInv;
                Player.scraps += scrapsInv;
                copperInv = 0;
                metalInv = 0;
                scrapsInv = 0;
                inventory = 0;
            }
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTitan)
        {
            if(collision.gameObject.tag == "Wall")
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
    private void DestroyWall(Rect rect, Pathfinding p)
    {
        
            float nearestWall = Mathf.Infinity;
            Node closestWall = null;
            foreach (Node wall in TileMap.walls)
            {

                if (rect.Contains(transform.position))
                {
                    float distance = (p.GetMap().GetPosition(wall.x, wall.y) - transform.position).sqrMagnitude;
                    if (distance < nearestWall)
                    {
                        if (!p.GetMap().GetMapNode(wall.x, wall.y).notWall)
                        {

                            nearestWall = distance;
                            closestWall = p.GetMap().GetMapNode(wall.x, wall.y);
                        }
                    }
                }
                
            }
            Node.NodeObject nodeSprite = Node.NodeObject.None;
            if (closestWall != null)
            {
                nodeSprite = closestWall.GetNodeObject();
            }
            
            if (nodeSprite == Node.NodeObject.Wall )
            {
                Vector3 pos = p.GetMap().GetPosition(closestWall.x, closestWall.y);
            Debug.DrawLine(new Vector3(transform.position.x + 15, transform.position.y), new Vector3(pos.x + 10, pos.y));
            if (transform.position.x + (mass*15f) > pos.x && pos.x + 10f > transform.position.x - (mass * 15f) && transform.position.y + (mass * 15f) > pos.y && pos.y + 10f > transform.position.y - (mass * 15f))
                {

                    
                    closestWall.SetNodeObject(Node.NodeObject.None);
                closestWall.notWall = true;
                    
                   
                }
                
            }
        
    }
    public void SetRTSBehaviours()
    {
        AgentManager closestEnemy = GetClosestEnemy(agents);
        float distance = 0;
        if (closestEnemy != null)
        {
            distance = (closestEnemy.transform.position - transform.position).magnitude;
        }
        if (isCollector && sg.activeInHierarchy == false)
        {
            if (isCollecting)
            {
                Collect(MapGenerator.rect1, MapGenerator.pathFind);
                Collect(MapGenerator.rect2, MapGenerator.pathfind2);
                Collect(MapGenerator.rect3, MapGenerator.pathfind3);
                Collect(MapGenerator.rect4, MapGenerator.pathfind4);
            }
            
        }
        if (isAtacker && sg.activeInHierarchy == false  && isLeaderAlive == true || isLeader && isAtacker)
        {
            Attack();
        }
        if (isTitan)
        {
            DestroyWall(MapGenerator.rect1, MapGenerator.pathFind);
            DestroyWall(MapGenerator.rect2, MapGenerator.pathfind2);
            DestroyWall(MapGenerator.rect3, MapGenerator.pathfind3);
            DestroyWall(MapGenerator.rect4, MapGenerator.pathfind4);

        }
        if (isShooter && sg.activeInHierarchy == false && this.GetComponent<AgentManager>().isFollowing == false && isLeaderAlive == true || isLeader && isShooter || isPursue && isShooter)
        {
            ShootBehavior();
        }
        if (isShooter  && isLeaderAlive == false && !isLeader && sg.activeInHierarchy == false ||
            isAtacker && isLeaderAlive == false && !isLeader && sg.activeInHierarchy == false)
        {
            if (closestEnemy != null)
            {
                if(!closestEnemy.isCollector)
                    Flee(closestEnemy.transform.position);
            }
            
            Flocking();
        }
    }
    public void Attack()
    {
        
        AgentManager closest = GetClosestEnemy(agents);

        if (closest != null)
        {
            if (ammo <= 0 && Vector2.Distance(transform.position, closest.transform.position) > 10 && closest.GetComponent<AgentManager>().team != team
                && isAtacker)
            {
                Arrive(closest.transform.position);

            }

            if ((transform.position - closest.transform.position).magnitude <= attackRange && closest.isTitan == false)
            {
                
                if (meleeAtack)
                {
                    isAttacking = true;
                    closest.agentsLayers = LayerMask.GetMask("Agents");
                    StartCoroutine("AttackTime");
                    anim.SetTrigger("Attack");
                    Vector2 direction = closest.transform.position - transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
                    Quaternion neededRotation = Quaternion.LookRotation(direction);
                    Quaternion.RotateTowards(transform.rotation, neededRotation, Time.deltaTime * 10f);
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firePoint.transform.position, attackSize, closest.agentsLayers);
                    foreach(Collider2D enemy in hitEnemies)
                    {
                        if (enemy != null)
                        {
                            if (enemy.GetComponent<AgentManager>().team != team&& enemy.GetComponent<AgentManager>().isTitan == false)
                            {
                                healthAmount -= meleeDamage;
                                //Debug.Log(enemy.name + "hitted");

                            }
                        }
                    }
                }
                else
                {
                    isAttacking = false;
                }


            }
        } 
    }
    public void SetSelected(bool isVisible)
    {
        sg.SetActive(isVisible);
    }
    public void MoveTo(Vector3 target, Pathfinding pathf, Vector3 helperPos)
    {
        
            pAgent.SetTarget(target, helperPos, pathf);
        
      
    }
    public void MoveArrive(Vector3 target)
    {
        pAgent.SetMovePos(target);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "storeage")
        {
            copperInv = 0;
        }
        if (isCollecting)
        {
            if (collision.tag == "Resource")
            {
                if (inventory < resourcesCarryLimt)
                {
                    //collision.gameObject.transform.position = transform.position;
                    collision.gameObject.SetActive(false);
                }

            }
        }
    }
    void DetectWall()
    {
        //Vector2 avoidanceForce = Vector2.zero;
        //Collider2D[] wallsColl = Physics2D.OverlapCircleAll(transform.position, 60f, wallsLayer);
        //GameObject closestLeader = GetClosestLeader(leaders);
        //foreach (Collider2D wall in wallsColl)
        //{
           
        //    if (wall.IsTouching(this.GetComponent<Collider2D>()))
        //    {
        //        avoidanceForce = Vector2.Reflect(vel, steering);
        //        Debug.Log("wall hitted");
        //        wallHitted = true;
        //        //Wander();
        //    } 
        //    if(avoidanceForce != Vector2.zero)
        //    {
        //        Vector2 desired = avoidanceForce.normalized * maxVel;
        //        steering = desired - vel;
        //    }
           
        //}
        float nearetWall = Mathf.Infinity;
        
        GameObject closestwall = null;
      
        foreach (GameObject wall in TileMap.wallsList)
        {
            
           float distance = (wall.transform.position- transform.position).sqrMagnitude;

            if (distance < nearetWall)
            {
                    

                nearetWall = distance;
                closestwall = wall;
                    
            }
            
        }
        
        if (closestwall != null)
        {
            Debug.DrawLine(new Vector3(transform.position.x, transform.position.y), closestwall.transform.position);
            float d = (closestwall.transform.position - transform.position).sqrMagnitude;
            if (d <= 60f)
            {
                Flee(closestwall.transform.position);

            }

            // Debug.DrawLine(this.transform.position, p.GetMap().GetPosition(closestResource.x + 0.5f, closestResource.y + 0.5f), Color.red);
        }
        

        //Vector3[] rayVec = new Vector3[10];
        //rayVec[0] = vel;
        //rayVec[0].Normalize();
        //rayVec[0] *= rayLength;
        //float rayOrient = Mathf.Atan2(vel.y, vel.x);
        //float rightRayO = rayOrient + (20f * Mathf.Deg2Rad);
        //float leftRayO = rayOrient - (20f * Mathf.Rad2Deg);
        //rayVec[1] = Quaternion.AngleAxis(60f, Vector3.forward) * transform.right;
        //rayVec[1].Normalize();
        //rayVec[1] *= rayLength;

        //rayVec[2] = Quaternion.AngleAxis(120f, Vector3.forward) * transform.right;
        //rayVec[2].Normalize();
        //rayVec[2] *= rayLength;

        //rayVec[3] = Quaternion.AngleAxis(150f, Vector3.forward) * transform.right;
        //rayVec[3].Normalize();
        //rayVec[3] *= rayLength;

        //rayVec[4] = Quaternion.AngleAxis(180f, Vector3.forward) * transform.right;
        //rayVec[4].Normalize();
        //rayVec[4] *= rayLength;

        //rayVec[5] = Quaternion.AngleAxis(210f, Vector3.forward) * transform.right;
        //rayVec[5].Normalize();
        //rayVec[5] *= rayLength;

        //rayVec[6] = Quaternion.AngleAxis(240f, Vector3.forward) * transform.right;
        //rayVec[6].Normalize();
        //rayVec[6] *= rayLength;


        //rayVec[7] = Quaternion.AngleAxis(270f, Vector3.forward) * transform.right;
        //rayVec[7].Normalize();
        //rayVec[7] *= rayLength;

        //rayVec[8] = Quaternion.AngleAxis(300f, Vector3.forward) * transform.right;
        //rayVec[8].Normalize();
        //rayVec[8] *= rayLength;

        //rayVec[9] = Quaternion.AngleAxis(330f, Vector3.forward) * transform.right;
        //rayVec[9].Normalize();
        //rayVec[9] *= rayLength;

        //Debug.DrawRay(transform.position, rayVec[0], Color.blue);
        //Debug.DrawRay(transform.position, rayVec[1], Color.red);
        //Debug.DrawRay(transform.position, rayVec[2], Color.black);
        //Debug.DrawRay(transform.position, rayVec[3], Color.blue);
        //Debug.DrawRay(transform.position, rayVec[4], Color.red);
        //Debug.DrawRay(transform.position, rayVec[5], Color.black);
        //Debug.DrawRay(transform.position, rayVec[6], Color.blue);
        //Debug.DrawRay(transform.position, rayVec[7], Color.red);
        //Debug.DrawRay(transform.position, rayVec[8], Color.black);
        //Debug.DrawRay(transform.position, rayVec[9], Color.black);
        //AgentManager friend = GetClosestFriend(agents);
        //for (int i = 0; i < rayVec.Length; i++)
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(transform.position, rayVec[i], rayLength);
        //    if (hit.collider != null)
        //    {
        //        if (hit.collider.gameObject.CompareTag("Wall"))
        //        {
        //            Debug.Log("wall hitted");
        //            isFollowing = false;
        //            pAgent.SetTarget(friend.transform.position);
        //        }
        //    }
        //    else
        //    {
        //        isFollowing = true;
        //    }

        //}

    }

    void FollowClosestLeader()
    {
       
        GameObject closestLeader = GetClosestLeader(leaders);
        if (closestLeader != null)
        {
            if (closestLeader.GetComponent<AgentManager>().healthAmount > 50 && !isLeader)
            {
                fleeTime = 0;
                startTime = false;
                isLeaderAlive = true;
                isFollowing = true;
            }
            if (closestLeader.GetComponent<AgentManager>().healthAmount <= 50 && !isLeader)
            {
                startTime = true;
                isLeaderAlive = false;
                isFollowing = false;
            }
            
            StartCoroutine(CeheckMovement(closestLeader));
            
            if (closestLeader.GetComponent<AgentManager>().isAgentMoving && sg.activeInHierarchy == false)
            {
                LeaderFollow(closestLeader);
            }
            else if(Vector2.Distance(transform.position, closestLeader.transform.position) > 30 && !isTitan && isFollowing && sg.activeInHierarchy == false)
            {
                Arrive(closestLeader.transform.position);
            }
            if(Vector2.Distance(transform.position, closestLeader.transform.position) > 100 && isTitan && isFollowing && sg.activeInHierarchy == false)
            {
                Arrive(closestLeader.transform.position);
            }
            if (closestLeader.GetComponent<AgentManager>().isAgentMoving == false)
            {
                isFollowing = false;
            }
            else
            {
                isFollowing = true;
            }


        }

        
    }

    IEnumerator CeheckMovement(GameObject go)
    {
        Vector2 prevPos = go.transform.position;
        yield return new WaitForSeconds(0.1f);
        Vector2 actualPos = go.transform.position;

        if (prevPos == actualPos) go.GetComponent<AgentManager>().isAgentMoving = false;
        if (prevPos != actualPos) go.GetComponent<AgentManager>().isAgentMoving = true;
    }
    IEnumerator AttackTime()
    {
        meleeAtack = false;
        yield return new WaitForSeconds(attackTime);
        meleeAtack = true;
    }
    IEnumerator ShootTime()
    {
        yield return new WaitForSeconds(startTimeBtwShots);
        canShoot = true;
    }
    public void ShootBehavior()
    {
        
        AgentManager closest = GetClosestEnemy(agents);

        if (closest != null)
        {

            if ((transform.position - closest.transform.position).magnitude <= shootingRaange && ammo > 0)
            {
                
                if (canShoot)
                {
                    canShoot = false;
                    StartCoroutine("ShootTime");
                    target = closest.gameObject;
                    Vector2 direction = target.transform.position - transform.position;
                    GameObject newTarget;
                    
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
                    Quaternion neededRotation = Quaternion.LookRotation(direction);
                    Quaternion.RotateTowards(transform.rotation, neededRotation, Time.deltaTime * 10f);

                    tmpBullet = ObjectPool.Instance.GetPooledObjects(bullets, 20);
                    if(tmpBullet != null)
                    {
                        tmpBullet.transform.position = firePoint.transform.position;
                        tmpBullet.SetActive(true);
                        tmpBullet.GetComponent<Bullet>().id = team;
                        tmpBullet.GetComponent<Bullet>().damage = bulletDamage;
                        ammo--;
                       
                        tmpBullet.transform.localScale = new Vector3(mass * 5f, mass * 5f, 0f);
                        
                        tmpBullet.transform.right = direction;
                        tmpBullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletForce;
                    }
                    

                }

                
            }
            
            

        }
        
        
    }

    void InitAgentObjects()
    {
        GameObject selectedGo = Resources.Load("AgentObjects/selected") as GameObject;
        sg = Instantiate(selectedGo, transform.position, Quaternion.identity);
        sg.transform.parent = this.transform;
        sg.transform.localScale = new Vector3(1.7f, 1.7f, 1);

        GameObject miniMapIcon = Resources.Load("AgentObjects/Minimap Icon") as GameObject;
        GameObject mmi = Instantiate(miniMapIcon, transform.position, Quaternion.identity);
        mmi.transform.parent = this.transform;
        mmi.transform.localScale = new Vector2(1.7f, 1.7f);

        GameObject spriteMask = Resources.Load("AgentObjects/Sprite Mask") as GameObject;
        GameObject sp = Instantiate(spriteMask, transform.position, Quaternion.identity);
        sp.transform.parent = this.transform;

        GameObject fogOfWar = Resources.Load("AgentObjects/FogOfWar 1") as GameObject;
        GameObject fow = Instantiate(fogOfWar, transform.position, Quaternion.identity);
        fow.transform.parent = this.transform;

        GameObject fp = Resources.Load("AgentObjects/FirePoint") as GameObject;
        firePoint = Instantiate(fp, new Vector2(transform.position.x + 3, transform.position.y + 15f), Quaternion.identity);
        firePoint.transform.parent = this.transform;

        bullet = Resources.Load("AgentObjects/Bullet") as GameObject;
        GameObject tmp;
        if (isShooter)
        {
            ObjectPool.Instance.SetPooledObjects(bullets, bullet, 50);
            
        }
        //this.bullet.transform.localScale = new Vector3(1f, 1f, 1f);
        this.transform.localScale = new Vector3(mass * maxScale, mass * maxScale, 0f);
        if (this.mass == 1)
        {
            firePoint.transform.position = new Vector2(transform.position.x + 1.2f, transform.position.y + 20f);
            
        }


    }

    

    public AgentManager GetClosestEnemy(AgentManager[] agentArr)
    {
        float nearestE = Mathf.Infinity;
        AgentManager closestEnemy = null;
        foreach (AgentManager agent in agentArr)
        {
            float distanceToEnemy = (agent.transform.position - transform.position).sqrMagnitude;
            if (distanceToEnemy < nearestE)
            {
                if (agent.GetComponent<AgentManager>().team != this.team)
                {
                    nearestE = distanceToEnemy;
                    closestEnemy = agent;
                }
            }
        }
        return closestEnemy;
    }
    public GameObject  GetClosestLeader(GameObject[] agentArr)
    {
        float nearestL= Mathf.Infinity;
        GameObject closestLeader = null;
        foreach (GameObject agent in agentArr)
        {
            float distanceToLeader = (agent.transform.position - transform.position).sqrMagnitude;
            if (distanceToLeader < nearestL)
            {
                if (agent.GetComponent<AgentManager>().team == this.team && this != isLeader &&
                 agent.GetComponent<AgentManager>().squad == squad)
                {
                    nearestL = distanceToLeader;
                    closestLeader = agent;
                    hasLeader = true;
                }
                
            }
        }
        if(closestLeader == null)
        {
            hasLeader = false;
        }
        return closestLeader;
    }
    public AgentManager GetClosestFriend(AgentManager[] agentArr)
    {
        float nearestE = Mathf.Infinity;
        AgentManager closestEnemy = null;
        foreach (AgentManager agent in agentArr)
        {
            float distanceToEnemy = (agent.transform.position - transform.position).sqrMagnitude;
            if (distanceToEnemy < nearestE)
            {
                if (agent.GetComponent<AgentManager>().team == this.team)
                {
                    nearestE = distanceToEnemy;
                    closestEnemy = agent;
                }
            }
        }
        return closestEnemy;
    }
}