using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

//using UnityEditor;

public class AgentManager : SteerinBehaviours /*Tree*/
{
    [Header("Behaviors")]
    public bool isSeek;
    public bool isFlee;
    public bool isPursue;
    public bool isEvade;
    public bool isArrive;
    public bool isCohesion;
    public bool isCollector;
    public bool isTitan;
    private int resource3Inv = 0;
    private int resource2Inv = 0;
    private int resource1Inv = 0;
    private int inventory = 0;
    [Header("Capacidad de carga")]
    public int resourcesCarryLimt = 1;
    private float nearestResource = Mathf.Infinity;
    private Node closestResource = null;
    private float secondNearest = Mathf.Infinity;
    public GameObject particle;
    GameObject wall;
    private Node nextClosest = null;
    PathAgent pAgent;
    public bool hasLeader;
    public bool isLeader;
    public int rank;
    private GameObject[] leaders;
    public int squad;
    public int team;
    GameObject fogOfWar;
    private GameObject bullet;
    private GameObject firePoint;
    private bool canShoot = true;
    [Header("Bala")]
    public bool isShooter;
    public float shootingRaange;
    public float startTimeBtwShots = 0.1f;
    public float bulletForce = 30;
    public int ammo = 50;
    public float bulletDamage;
    public GameObject sg;
    public bool isFollowing;
    public Animator anim;
    public AgentManager[] agents;
    bool meleeAtack = true;
    [Header("Cuerpo a Cuerpo")]
    public float attackSize = 0.5f;
    public float attackRange = 15f;
    public float attackTime = 0.5f;
    public bool isAtacker;
    public float meleeDamage;
    public bool hasTarget = false;
    GameObject tmpBullet;
    public bool isAgentMoving = false;
    public LayerMask agentsLayers;
    public LayerMask wallsLayer;
    public bool isAttacking = false;
    bool isShooting = false;
    public float maxScale = 15f;
    public float healthAmount = 150;
    public bool isLeaderAlive;
    private float fleeTime;
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
    CircleCollider2D attackCol;
    public int priceR1;
    public int priceR2;
    public int priceR3;
    public GameObject[] baseNodes;
    GameObject[] agentsObjs;
    GameObject[] newArray;
    public GameObject shotLight;
    AudioSource audioSource;
    public AudioClip gunShot;

    private GameObject dialogueBox;
    GameObject damagepp;
    void Start()
    {
        bullets = new List<GameObject>();
        isAttacking = false;
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
        SaveAgent.agentsP.Add(this);
        pAgent = GetComponent<PathAgent>();
        Awake();
        SetSelected(false);
        GetComponent<BoxCollider2D>().size = new Vector2(2f, 1.06f);
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/" + this.name);
        if (rank > 0)
        {
            isLeader = true;
            dialogueBox.SetActive(false);
        }
        agents = FindObjectsOfType<AgentManager>();
        if (isCollector)
        {
            isCollecting = true;
        }
        audioSource.volume = 0.01f;
        if (team == 2)
        {
            gameObject.AddComponent<EnemyAI>();

        }
        if(rank > 0 && team == 2)
        {
            RTSManager.enemyLeaders.Add(this);
        }
        if (squad > 0 && team == 2)
        {
            RTSManager.minions.Add(this);
        }
        if (isPursue && team == 2)
        {
            RTSManager.hunters.Add(this);
        }
        if (isTitan && team == 2)
        {
            RTSManager.tanks.Add(this);
        }
        if (isAtacker && !isShooter && team == 2)
        {
            RTSManager.attackers.Add(this);
        }
    }

    private void Update()
    {
        baseNodes = GameObject.FindGameObjectsWithTag("Base");
        agentsObjs = GameObject.FindGameObjectsWithTag("Agent");
        if (isLeader)
        {
            dialogueBox.transform.position = new Vector3(transform.position.x + 20, transform.position.y + 20, transform.position.z);
        }

        if (sg.activeInHierarchy && team == 1)
        {
            hpb.SetActive(true);
        }
        else if (!sg.activeInHierarchy && team == 1)
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
        if (rank == 0 && !isTitan)
        {
            //squad = 0;
            DetectWall();
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 10, 1990), Mathf.Clamp(transform.position.y, 10, 1990), 0);
        FollowClosestLeader();
        SetLeaderText();
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
            if(team == 1)
            {
                RTSManager.spawnedSoldier--;
            }
            if (rank > 0 && team == 2)
            {
                RTSManager.enemyLeaders.Remove(this);
            }
            if (squad > 0 && team == 2)
            {
                RTSManager.minions.Remove(this);
            }
            if (isPursue && team == 2)
            {
                RTSManager.hunters.Remove(this);
            }
            if (isTitan && team == 2)
            {
                RTSManager.tanks.Remove(this);
            }
            if (isAtacker && !isShooter && team == 2)
            {
                RTSManager.attackers.Remove(this);
            }
            gameObject.SetActive(false);
        }
        GetClosestObject(baseNodes);


        audioSource.volume = 0.20f;
        audioSource.spatialBlend = 1;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    void LateUpdate()
    {


    }
    private void OnDestroy()
    {
        //SaveAgent.agentsP.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    /// <summary>
    /// Updates the Steering Behaviors that can be used
    /// </summary>
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
        if (isFlee)
        {
            if (d < fleePerception && sg.activeInHierarchy == false)
            {
                if (closestEnemy != null)
                {
                    if (isCollector)
                    {
                        isCollecting = false;
                        Flee(closestEnemy.transform.position);
                    }

                }
            }
            else if (isCollector)
            {
                isCollecting = true;
            }
            if (isShooter && isLeaderAlive == false && !isPursue && !isLeader && sg.activeInHierarchy == false ||
            isAtacker && isLeaderAlive == false && !isLeader && sg.activeInHierarchy == false)
            {
                if (closestEnemy != null)
                {
                    if (!closestEnemy.isCollector)
                        Flee(closestEnemy.transform.position);
                }

                Flocking();
            }

        }

        if (isEvade)
        {
            foreach (AgentManager agent in agents)
            {
                if (agent != gameObject)
                    Evade(agent.transform);
            }

        }

        if (isPursue || isPursue && isShooter && closestEnemy.isLeaderAlive == false && !closestEnemy.isLeader)
        {
            if (closestEnemy != null)
            {
                if (!closestEnemy.isPursue)
                {
                    if (d < seekPerception && sg.activeInHierarchy == false)
                    {
                        hasTarget = true;
                        Pursue(closestEnemy.transform);
                    }else if(d > seekPerception && sg.activeInHierarchy == false)
                    {
                        hasTarget = false;
                    }
                }
            }
        }

    }
    /// <summary>
    /// SB to move simulating a bird flock
    /// </summary>
    public void Flocking()
    {
        Vector2 align = Align();
        Vector2 separate = Separate();
        Vector2 cohesion = Cohesion();
        vel += separate * separationWeigh + cohesion * cohesionWeight;
    }
    /// <summary>
    /// function to collect the nearest resource on the tilemap (three resource types)
    /// </summary>
    /// <param name="rect">tilemap where the resource is (the game contains four tilemaps)</param>
    /// <param name="p">pathfinding system of the tilemap (optimization purpose)</param>
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
        Node.NodeObject nodeSprite = Node.NodeObject.None;
        if (closestResource != null)
        {
            nodeSprite = closestResource.GetNodeObject();

            // Debug.DrawLine(this.transform.position, p.GetMap().GetPosition(closestResource.x + 0.5f, closestResource.y + 0.5f), Color.red);
        }
        if (nodeSprite == Node.NodeObject.Resource2)
        {
            if ((transform.position - p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y)).magnitude <= 1f && inventory <= resourcesCarryLimt && isCollector)
            {

                closestResource.SetNodeObject(Node.NodeObject.None);
                //closestResource.isResource = false;
                resource2Inv++;
                inventory++;
            }
            if (inventory < resourcesCarryLimt)
            {

                Arrive(p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y));
            }

        }
        else if (nodeSprite == Node.NodeObject.Resource1)
        {
            if ((transform.position - p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y)).magnitude <= 1f && inventory <= resourcesCarryLimt && isCollector)
            {

                closestResource.SetNodeObject(Node.NodeObject.None);
                //closestResource.isResource = false;
                resource1Inv++;
                inventory++;
            }
            if (inventory < resourcesCarryLimt)
            {
                Arrive(p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y));
            }
        }
        else if (nodeSprite == Node.NodeObject.Resource3)
        {
            if ((transform.position - p.GetMap().GetPosition(closestResource.x + .5f, closestResource.y)).magnitude <= 1f && inventory <= resourcesCarryLimt && isCollector)
            {

                closestResource.SetNodeObject(Node.NodeObject.None);
                //closestResource.isResource = false;
                resource3Inv++;
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

            // Debug.DrawLine(this.transform.position, p.GetMap().GetPosition(closestStorage.x + 0.5f, closestStorage.y + 0.5f), Color.red);
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
                if (team == 1)
                {
                    RTSManager.resource3T1 += resource3Inv;
                    RTSManager.resource2T1 += resource2Inv;
                    RTSManager.resource1T1 += resource1Inv;
                }
                else if (team == 2)
                {
                    RTSManager.resource3T2 += resource3Inv;
                    RTSManager.resource2T2 += resource2Inv;
                    RTSManager.resource1T2 += resource1Inv;

                }
                resource3Inv = 0;
                resource2Inv = 0;
                resource1Inv = 0;
                inventory = 0;
            }
        }

    }
    /// <summary>
    /// Titan behaviour collision detection 
    /// </summary>
    /// <param name="collision">collided object</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTitan)
        {
            if (collision.gameObject.tag == "Wall")
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// function to clear the wall after a collision
    /// </summary>
    /// <param name="rect">tilemap where the wall is</param>
    /// <param name="p">tilemap pathfinding system </param>
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

        if (nodeSprite == Node.NodeObject.Wall)
        {
            Vector3 pos = p.GetMap().GetPosition(closestWall.x, closestWall.y);
            ///Debug.DrawLine(new Vector3(transform.position.x + 15, transform.position.y), new Vector3(pos.x + 10, pos.y));
            if (transform.position.x + (mass * 15f) > pos.x && pos.x + 10f > transform.position.x - (mass * 15f) && transform.position.y + (mass * 15f) > pos.y && pos.y + 10f > transform.position.y - (mass * 15f))
            {


                closestWall.SetNodeObject(Node.NodeObject.None);
                closestWall.notWall = true;


            }

        }

    }
    /// <summary>
    /// function to update de RTS behaviors: collector, shooter, attacker
    /// </summary>
    public void SetRTSBehaviours()
    {
        GameObject agentA = GetClosestObject(agentsObjs);
        GameObject hBase = GetClosestObject(baseNodes);
        GameObject leadersClosest = GetClosestObject(leaders);
        List<GameObject> objects = new List<GameObject>();
        objects.Add(agentA);
        objects.Add(hBase);
        objects.Add(leadersClosest);
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
        if (isAtacker && sg.activeInHierarchy == false && isLeaderAlive == true || isLeader && isAtacker || isAtacker && squad == 0)
        {
            float nearestObject = Mathf.Infinity;
            GameObject closestObject = null;
            foreach (GameObject obj in objects)
            {
                if (obj)
                {
                    float distanceToBase = (obj.transform.position - transform.position).sqrMagnitude;
                    if (distanceToBase < nearestObject)
                    {
                        nearestObject = distanceToBase;
                        closestObject = obj;
                    }
                }
            }
            if (closestObject)
            {
                AttackBehavior(closestObject);
                //Debug.Log(closestObject.name +" "+this.name);
                Debug.DrawLine(this.transform.position, closestObject.transform.position);
            }
           
        }
        if (isTitan)
        {
            DestroyWall(MapGenerator.rect1, MapGenerator.pathFind);
            DestroyWall(MapGenerator.rect2, MapGenerator.pathfind2);
            DestroyWall(MapGenerator.rect3, MapGenerator.pathfind3);
            DestroyWall(MapGenerator.rect4, MapGenerator.pathfind4);

        }
        if (isShooter && sg.activeInHierarchy == false && this.GetComponent<AgentManager>().isFollowing == false && isLeaderAlive == true || isLeader && isShooter || isPursue && isShooter || isShooter && isAtacker || isShooter && rank == 0)
        {
            
            float nearestObject = Mathf.Infinity;
            GameObject closestObject = null;
            foreach (GameObject obj in objects)
            {
                if (obj)
                {
                    float distanceToBase = (obj.transform.position - transform.position).sqrMagnitude;
                    if (distanceToBase < nearestObject)
                    {
                        nearestObject = distanceToBase;
                        closestObject = obj;
                    }
                }
            }
            if (closestObject)
            {
                float distanceToObject = (closestObject.transform.position - transform.position).sqrMagnitude;
                
                    ShootBehavior(closestObject);
                    //Debug.Log(closestObject.name +" "+this.name);
                    Debug.DrawLine(this.transform.position, closestObject.transform.position);
                   
                
                    
                    
                
            }
            else
            {
               
                shotLight.SetActive(false);
            }

        }

    }
    /// <summary>
    /// melee combat
    /// </summary>
    /// <param name="objectType">object to attack</param>
    public void AttackBehavior(GameObject objectType)
    {
        
        if (ammo <= 0 && !isFollowing)
        {
            isAttacking = true;
            if ((transform.position - objectType.transform.position).magnitude <= 100)
            {
                if (isArrive)
                {
                    Arrive(objectType.transform.position);
                }
                else if (isSeek)
                {
                    Seek(objectType.transform.position);
                }
            }
        }
        else
        {
            isAttacking = false;
        }
        attackRange = mass * maxScale * 2;
        if ((transform.position - objectType.transform.position).magnitude <= attackRange && ammo <= 0 && isFollowing == false)
        {

           if (meleeAtack)
           {
               isAttacking = true;
               attackCol.enabled = true;
               anim.SetTrigger("Attack");
               //objectType.agentsLayers = LayerMask.GetMask("Agents");
               StartCoroutine("AttackTime");

               Vector2 direction = objectType.transform.position - transform.position;
               float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
               transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
               Quaternion neededRotation = Quaternion.LookRotation(direction);
               Quaternion.RotateTowards(transform.rotation, neededRotation, Time.deltaTime * 10f);

               if (attackCol.IsTouching(objectType.GetComponent<BoxCollider2D>()))
               {
                   if (objectType.tag == "Agent")
                   {
                       Debug.Log("Pelea");
                       GameObject part = Instantiate(particle, transform.position, Quaternion.identity);
                       Destroy(part, 1);
                       objectType.GetComponent<AgentManager>().healthAmount -= meleeDamage;
                       DamagePopup.Create(objectType.transform.position, meleeDamage, damagepp);
                   }
                   else if (objectType.tag == "Base")
                   {
                       objectType.GetComponent<HomeBase>().healthAmount -= meleeDamage;
                       DamagePopup.Create(objectType.transform.position, meleeDamage, damagepp);
                   }
               }

           }
           else
           {
               //attackCol.enabled = false;
               isAttacking = false;
           }


        }
    }
    /// <summary>
    /// function to set the selected agente
    /// </summary>
    /// <param name="isVisible">change if the selected object is active</param>
    public void SetSelected(bool isVisible)
    {
        sg.SetActive(isVisible);
    }
    /// <summary>
    /// function to move the agent with pathfinding
    /// </summary>
    /// <param name="target">position to move</param>
    /// <param name="pathf">pathfinding system to use</param>
    /// <param name="helperPos">pathfindig offset</param>
    public void MoveTo(Vector3 target, Pathfinding pathf, Vector3 helperPos)
    {

        pAgent.SetTarget(target, helperPos, pathf);


    }
    /// <summary>
    /// finction to arrive to the location
    /// </summary>
    /// <param name="target">position to move with arrive</param>
    public void MoveArrive(Vector3 target)
    {
        pAgent.SetMovePos(target);
        //Arrive(target);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponent<Collider2D>() is BoxCollider2D)
        {
            if (collision.tag == "storeage")
            {
                resource3Inv = 0;
            }
            if (collision.tag == "Ceiling")
            {
                Color tmp = collision.GetComponent<SpriteRenderer>().color;
                tmp.a = 0.5f;
                collision.GetComponent<SpriteRenderer>().color = tmp;
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
        if (collision.tag == "Bullet" && collision.GetComponent<Bullet>().id != team)
        {
            GameObject part = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(part, 1);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ceiling")
        {
            Color tmp = collision.GetComponent<SpriteRenderer>().color;
            tmp.a = 1f;
            collision.GetComponent<SpriteRenderer>().color = tmp;
        }
    }
    /// <summary>
    /// wall avoidance
    /// </summary>
    void DetectWall()
    {
        Vector3[] rayVec = new Vector3[3];
        rayVec[0] = vel;
        rayVec[0].Normalize();
        rayVec[0] *= rayLength;
        rayVec[1] = Quaternion.AngleAxis(60f, Vector3.forward) * transform.right;
        rayVec[1].Normalize();
        rayVec[1] *= rayLength;
        rayVec[2] = Quaternion.AngleAxis(120f, Vector3.forward) * transform.right;
        rayVec[2].Normalize();
        rayVec[2] *= rayLength;
        Debug.DrawRay(transform.position, rayVec[0], Color.blue);
        Debug.DrawRay(transform.position, rayVec[1], Color.red);
        Debug.DrawRay(transform.position, rayVec[2], Color.black);
        for (int i = 0; i < rayVec.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayVec[i], rayLength);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Wall"))
                {
                    Debug.Log("wall hitted");
                    //isFollowing = false;
                    Flee(hit.collider.transform.position);
                }
            }
        }
    }
    /// <summary>
    /// leader behaviour
    /// </summary>
    void FollowClosestLeader()
    {

        GameObject closestLeader = GetClosestLeader(leaders);
        if (closestLeader != null)
        {
            hasLeader = true;
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
            
            if (closestLeader.GetComponent<AgentManager>().isAgentMoving == false)
            {
                isFollowing = false;
            }
            else
            {
                isFollowing = true;
            }
            if (closestLeader.GetComponent<AgentManager>().isAgentMoving && sg.activeInHierarchy == false && isFollowing)
            {

                LeaderFollow(closestLeader);
                DetectWall();
            }
            else if (Vector2.Distance(transform.position, closestLeader.transform.position) > 30 && !isTitan && sg.activeInHierarchy == false /*&& isFollowing*/ && !isAttacking)
            {
                Arrive(closestLeader.transform.position);
                DetectWall();
            }
            else if (Vector2.Distance(transform.position, closestLeader.transform.position) > 100 && isTitan && sg.activeInHierarchy == false /*&& isFollowing*/)
            {
                Arrive(closestLeader.transform.position);
            }
           


        }
        else
        {
            hasLeader = false;
        }


    }
    /// <summary>
    /// check if an object is moving
    /// </summary>
    /// <param name="go">object to check</param>
    /// <returns>movement time</returns>
    IEnumerator CeheckMovement(GameObject go)
    {
        Vector2 prevPos = go.transform.position;
        yield return new WaitForSeconds(0.1f);
        Vector2 actualPos = go.transform.position;

        if (prevPos == actualPos) go.GetComponent<AgentManager>().isAgentMoving = false;
        if (prevPos != actualPos) go.GetComponent<AgentManager>().isAgentMoving = true;
    }
    /// <summary>
    /// time between attacks
    /// </summary>
    /// <returns>attack time</returns>
    IEnumerator AttackTime()
    {
        meleeAtack = false;
        yield return new WaitForSeconds(attackTime);
        meleeAtack = true;
    }
    /// <summary>
    /// time between shots
    /// </summary>
    /// <returns>shot time</returns>
    IEnumerator ShootTime()
    {
        shotLight.SetActive(false);
        yield return new WaitForSeconds(startTimeBtwShots);
        shotLight.SetActive(true);

        canShoot = true;
    }
    /// <summary>
    /// shoot behaviour
    /// </summary>
    /// <param name="objectType">object to shoot</param>
    public void ShootBehavior(GameObject objectType)
    {
        
        if ((transform.position - objectType.transform.position).magnitude <= shootingRaange && ammo > 0)
        {
            hasTarget = true;
            if (canShoot)
            {
                canShoot = false;
                StartCoroutine("ShootTime");
                target = objectType.gameObject;
                Vector2 direction = target.transform.position - transform.position;
                GameObject newTarget;
                audioSource.PlayOneShot(gunShot);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
                Quaternion neededRotation = Quaternion.LookRotation(direction);
                Quaternion.RotateTowards(transform.rotation, neededRotation, Time.deltaTime * 10f);

                tmpBullet = ObjectPool.Instance.GetPooledObjects(bullets, 20);
                if (tmpBullet != null)
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
        else
        {
            hasTarget = false;
        }
        shotLight.SetActive(false);
    }
    /// <summary>
    /// leader dialog
    /// </summary>
    void SetLeaderText()
    {

        string[] dialogueMoving = { "Siganme los buenos", "Por aqui", "A la carga", "Sin piedad", "Destrocenlos", "Avancen" };
        string[] dialogueCritical = { "Retirada", "Salvense", "Hay mama", "no resistire mas", "Porque a mi", "cubranme" };
        
        if (isLeader && team == 1)
        {
            if (isAgentMoving)
            {


                dialogueBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = dialogueMoving[UnitSelection.random];
                dialogueBox.SetActive(true);
            }
            else if(healthAmount<=150 && healthAmount > 50)
            {
                dialogueBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = dialogueCritical[UnitSelection.random];
                dialogueBox.SetActive(true);
                
            }
            else
            {
                dialogueBox.SetActive(false);
            }
        }
    }
    /// <summary>
    /// set agent objects to use in the game
    /// </summary>
    void InitAgentObjects()
    {
        //GameObject to set agent as selected
        GameObject selectedGo = Resources.Load("AgentObjects/selected") as GameObject;
        sg = Instantiate(selectedGo, transform.position, Quaternion.identity);
        sg.transform.parent = this.transform;
        sg.transform.localScale = new Vector3(1.7f, 1.7f, 1);
        //GameObject to show in the minimap
        GameObject miniMapIcon = Resources.Load("AgentObjects/Minimap Icon") as GameObject;
        GameObject mmi = Instantiate(miniMapIcon, transform.position, Quaternion.identity);
        mmi.transform.parent = this.transform;
        mmi.transform.localScale = new Vector2(1.7f, 1.7f);
        //GameObject to show on hide ahent on the fog of war
        GameObject spriteMask = Resources.Load("AgentObjects/Sprite Mask") as GameObject;
        GameObject sp = Instantiate(spriteMask, transform.position, Quaternion.identity);
        sp.transform.parent = this.transform;
        //fog of war object
        GameObject fogOfWar = Resources.Load("AgentObjects/FogOfWar 1") as GameObject;
        GameObject fow = Instantiate(fogOfWar, transform.position, Quaternion.identity);
        fow.transform.parent = this.transform;
        //fire point
        GameObject fp = Resources.Load("AgentObjects/FirePoint") as GameObject;
        firePoint = Instantiate(fp, new Vector2(transform.position.x + 3, transform.position.y + 15f), Quaternion.identity);
        firePoint.transform.parent = this.transform;

        particle = Resources.Load("AgentObjects/Particle System") as GameObject;
        bullet = Resources.Load("AgentObjects/Bullet") as GameObject;
        GameObject tmp;
        audioSource = gameObject.AddComponent<AudioSource>();
        gunShot = Resources.Load("SFX/FeelTacticalRifle") as AudioClip;
        audioSource.clip = gunShot;
        audioSource.priority = 0;
        audioSource.volume = 0.30f;
        GameObject sl = Resources.Load("AgentObjects/shotLight") as GameObject;
        shotLight = Instantiate(sl, firePoint.transform.position, Quaternion.identity);
        shotLight.transform.parent = this.transform;
        shotLight.SetActive(false);
        if (isShooter)
        {
            ObjectPool.Instance.SetPooledObjects(bullets, bullet, 50);
            bullet.transform.tag = "Bullet";

        }
        if (isAtacker)
        {
            attackCol = gameObject.AddComponent<CircleCollider2D>();
            attackCol.offset = new Vector2(0.46f, 2.12f);
            attackCol.radius = 1.18f;
            attackCol.isTrigger = true;
        }
        //this.bullet.transform.localScale = new Vector3(1f, 1f, 1f);
        this.transform.localScale = new Vector3(mass * maxScale, mass * maxScale, 0f);
        if (this.mass == 1)
        {
            firePoint.transform.position = new Vector2(transform.position.x + 1.2f, transform.position.y + 20f);

        }
        if (rank > 0)
        {
            GameObject pl = Resources.Load("AgentObjects/Point Light 2D") as GameObject;
            GameObject pl2d = Instantiate(pl, transform.position, Quaternion.identity);
            pl2d.transform.parent = this.transform;
            pl2d.transform.localScale = new Vector2(1.7f, 1.7f);
            GameObject db = Resources.Load("AgentObjects/DialogueBox") as GameObject;
            dialogueBox = Instantiate(db, transform.position, Quaternion.identity);
        }
        damagepp = Resources.Load("AgentObjects/DamagePopup") as GameObject;
        //dialogueBox.transform.parent = this.transform;

    }


    /// <summary>
    /// find closest enemy
    /// </summary>
    /// <param name="agentArr">array to search object</param>
    /// <returns>closet enemy</returns>
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
    /// <summary>
    /// find closet leader
    /// </summary>
    /// <param name="agentArr">array to search object</param>
    /// <returns>closet leader</returns>
    public GameObject GetClosestLeader(GameObject[] agentArr)
    {
        float nearestL = Mathf.Infinity;
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

                }

            }
        }
        if (closestLeader == null)
        {
            //hasLeader = false;
        }
        return closestLeader;
    }
    /// <summary>
    /// find closes agent of the same team
    /// </summary>
    /// <param name="agentArr">array to search object</param>
    /// <returns>closes agent</returns>
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
    /// <summary>
    /// find closet object of different team
    /// </summary>
    /// <param name="objects">array to search object</param>
    /// <returns>closest object</returns>
    public GameObject GetClosestObject(GameObject[] objects)
    {
        float nearestObject = Mathf.Infinity;
        GameObject closestObject = null;


        foreach (GameObject obj in objects)
        {
            float distanceToBase = (obj.transform.position - transform.position).sqrMagnitude;
            if (distanceToBase < nearestObject)
            {
                if (obj.tag == "Agent")
                {
                    if (obj.GetComponent<AgentManager>().team != this.team)
                    {
                        nearestObject = distanceToBase;
                        closestObject = obj;
                    }
                }
                else if (obj.tag == "Base")
                {
                    if (obj.GetComponent<HomeBase>().iD != this.team)
                    {
                        nearestObject = distanceToBase;
                        closestObject = obj;
                    }
                }
                if (obj.tag == "Leader")
                {
                    if (obj.GetComponent<AgentManager>().team != this.team)
                    {
                        nearestObject = distanceToBase;
                        closestObject = obj;
                    }
                }

            }
        }

        if (closestObject)
        {

           
            return closestObject;



        }
        return null;
    }

    

   
}






