using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private TileMap tileMap, tileMap2;
    public static Pathfinding pathFind;
    public static Pathfinding pathfind2;

    private Node.NodeSprite nodeSprite;
    //Map<int> map;
    public int width = 100;
    public int height = 100;
    public float size = 10;
    private Vector3 origin;
    public GameObject wall;
    GameObject vis;
    public float counter = 0;
    private Scene scene;
    // GameObject agent;
    bool mousePressed = false;
    public TMP_InputField saveName;
    //public GameObject floor;
    private PathAgent[] agents;
    AgentManager[] agents1;
    bool placeTower = false;
    public bool isGenerating = false;
    public float targetTime = 0.0f;
    public int[][] lol;
    public GameObject agent;
    GameObject terrain;
    public static Rect rect1;
    public static Rect rect2;
    List<Pathfinding> maps;
    public Camera cam;
    bool canSetP1Tiles = false;
    bool canSetP2Tiles = false;
    // Start is called before the first frame update
    private void Awake()
    {
        maps = new List<Pathfinding>();
        vis = GameObject.FindGameObjectWithTag("Wall");
        origin = new Vector3(0, 0, 0);
        scene = SceneManager.GetActiveScene();
        pathFind = new Pathfinding(width, height, origin);
        pathfind2 = new Pathfinding(width, height, new Vector3(1000, 0, 0));
        maps.Add(pathFind);
        maps.Add(pathfind2);
        Debug.Log(maps.Count);
        agents = GameObject.FindObjectsOfType<PathAgent>();
        agents1 = FindObjectsOfType<AgentManager>();
        
        rect1 = new Rect(pathFind.origin.x, pathFind.origin.y, pathFind.GetMap().GetWidth() * 10f, pathFind.GetMap().GetWidth() * 10f);
        rect2 = new Rect(pathfind2.origin.x, pathfind2.origin.y, pathfind2.GetMap().GetWidth() * 10f, pathfind2.GetMap().GetWidth() * 10f);
        //pathfindingDebugStepVisual.Setup(pathfinding2.GetGrid());\
        
        pathFind.Load("rtest_2");
        
        terrain = Resources.Load<GameObject>("GameHelpers/terrain");

        //for (int i = 0; i < maps.Count; i++)
        //{
        //    maps[i].SetNodeSpriteVisual(tileMap);
        //}
        pathFind.SetNodeSpriteVisual(tileMap);
        pathfind2.SetNodeSpriteVisual(tileMap2);
        //pathfind2.Load("bigworld");
        terrain.gameObject.transform.localScale = new Vector3((size * width)*2, size * height, 1);
        GameObject terr =  Instantiate(terrain, new Vector2(1000, 500), Quaternion.identity);
        terr.name = terrain.name;
       
    }

    // Update is called once per frame
    void Update()
    {
       

        if(cam.transform.position.x <1000)
        {
            if(canSetP1Tiles == false)
            {
                canSetP1Tiles = true;
                
                canSetP2Tiles = false;
            }
            
        }else if(cam.transform.position.x >= 1000)
        {
            if (canSetP2Tiles == false)
            {
                canSetP2Tiles = true;
                
                canSetP1Tiles = false;
            }
        }

        targetTime -= Time.deltaTime;
        //DrawLine();
        if(targetTime <= 0.0f)
        {
            isGenerating = false;
        }
        if (isGenerating)
        {

            GenerateMap();
        }
        
        PlaceWallWithMouse();
        if (placeTower)
        {
            buildingTest();
        }
        
    }
    private void LateUpdate()
    {
        
        //for (int x = 0; x < pathFind.GetMap().GetWidth(); x++)
        //{
        //    for (int y = 0; y < pathFind.GetMap().GetHeight(); y++)
        //    {
        //        Node node = pathFind.GetMap().GetMapNode(x, y);
        //        Node.NodeSprite nodeSprite = node.GetNodeSprite();
        //        if (nodeSprite == Node.NodeSprite.Wall)
        //        {

        //            Vector3 playerPos = player.transform.position;
        //            Vector3 pos = pathFind.GetMap().GetPosition(x, y);
        //            if (playerPos.x + 10 > pos.x && pos.x + 10 > playerPos.x - 10 && playerPos.y + 10 > pos.y && pos.y + 10 > playerPos.y - 10)
        //            {
        //                BoxCollider2D col;
        //                if (!node.hasCollider)
        //                {

        //                    col = vis.gameObject.AddComponent<BoxCollider2D>();
        //                    //walls.Add(node);
        //                    col.offset = pathFind.GetMap().GetPosition(x + 0.5f, y + 0.5f);
        //                    col.size = new Vector2(pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize());
        //                    col.usedByComposite = true;
        //                    node.hasCollider = true;
        //                }
        //                Debug.DrawLine(pos, playerPos);
        //                Debug.Log("colision izq");
        //            }
        //            foreach (PathAgent agent in agents)
        //            {
        //                if (agent.transform.position.x + 10 > pos.x && pos.x + 10 > agent.transform.position.x - 10 && agent.transform.position.y + 10 > pos.y && pos.y + 10 > agent.transform.position.y - 10)
        //                {
        //                    BoxCollider2D col;
        //                    if (!node.hasCollider)
        //                    {

        //                        col = vis.gameObject.AddComponent<BoxCollider2D>();
        //                        //walls.Add(node);
        //                        col.offset = pathFind.GetMap().GetPosition(x + 0.5f, y + 0.5f);
        //                        col.size = new Vector2(pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize());
        //                        col.usedByComposite = true;
        //                        node.hasCollider = true;
        //                    }
        //                    //saDebug.DrawLine(pos, playerPos);
        //                    Debug.Log("colision izq");
        //                }
        //            }


        //        }
        //    }
        //}
    }
    void PlaceWallWithMouse()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            mousePressed = true;
        }
        if (mousePressed )
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathFind.GetMap().GetXY(new Vector3(mousePos.x, mousePos.y), out int x, out int y);

            pathFind.SetNodeSprite(mousePos, nodeSprite);
            pathfind2.GetMap().GetXY(new Vector3(mousePos.x, mousePos.y), out int x2, out int y2);

            pathfind2.SetNodeSprite(mousePos, nodeSprite);
        }
        if (Input.GetMouseButtonUp(1))
        {
            mousePressed = false;
        }
            
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            placeTower = true;

        }

    }
    public void OnSave()
    {
        pathFind.Save(saveName.text);
        pathfind2.Save(saveName.text);
        //foreach (AgentManagerTest agent in agents1)
        //{
        //    agent.SaveAgent();
        //}
        
    }

    public void OnLoad()
    {
        pathFind.Load(saveName.text);
        pathfind2.Load(saveName.text);
        //foreach (AgentManagerTest agent in agents1)
        //{
        //    agent.LoadPlayer();
        //}
        
    }
    
    public void DrawLine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathFind.GetMap().GetXY(mousePos, out int x, out int y);
            foreach (PathAgent pAgent in agents)
            {
                pathFind.GetMap().GetXY(pAgent.transform.position, out int px, out int py);
                List<Node> path = pathFind.AStar(px, py, x, y);

                if (path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(path[i].x, path[i].y) * size + new Vector3((origin.x * 2) + (size), (origin.y * 2) + (size)) * .5f,
                            new Vector3(path[i + 1].x, path[i + 1].y) * size + new Vector3((origin.x * 2) + (size), (origin.y * 2) + (size)) * .5f, Color.red, .5f);
                        //Debug.Log(path[i + 1].x + "," + path[i + 1].y);
                    }
                }
            }

        }
    }
    void buildingTest()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathFind.GetMap().GetXY(new Vector3(mousePos.x, mousePos.y), out int x, out int y);
            for (int i = -2; i < 5; i++)
            {
                for (int j = -2; j < 5; j++)
                {
                    pathFind.SetNodeSprite(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeSprite.Ground);

                }
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    pathFind.SetNodeSprite(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeSprite.Box);

                }
            }
            placeTower = false;
            //for (int i = -2; i < 0; i++)
            //{
            //    for (int j = -2; j < 0; j++)
            //    {
            //        pathFind.SetNodeSprite(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeSprite.Wall);

            //    }
            //}
            //for (int i = 3; i < 5; i++)
            //{
            //    for (int j = 3; j < 5; j++)
            //    {
            //        pathFind.SetNodeSprite(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeSprite.Wall);

            //    }
            //}
            //for (int i = -2; i < 0; i++)
            //{
            //    for (int j = 3; j < 5; j++)
            //    {
            //        pathFind.SetNodeSprite(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeSprite.Wall);

            //    }
            //}
            //for (int i = 3; i < 5; i++)
            //{
            //    for (int j = -2; j < 0; j++)
            //    {
            //        pathFind.SetNodeSprite(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeSprite.Wall);

            //    }
            //}
        }
    }
    public void ActivateTile(int val)
    {
        if (val == 0)
        {
            nodeSprite = Node.NodeSprite.None;

        }
        if (val == 1)
        {
            nodeSprite = Node.NodeSprite.Wall;

        }
        if (val == 2)
        {
            nodeSprite = Node.NodeSprite.Ground;

        }
        if (val == 3)
        {
            nodeSprite = Node.NodeSprite.Grass;

        }
        if (val == 4)
        {
            nodeSprite = Node.NodeSprite.Scrap;

        }
        if (val == 5)
        {
            nodeSprite = Node.NodeSprite.Metal;

        }
        if (val == 6)
        {
            nodeSprite = Node.NodeSprite.Copper;

        }
        if (val == 7)
        {
            nodeSprite = Node.NodeSprite.Box;

        }
    }
    public void CreateRandomMap()
    {
        targetTime = 2f;
        isGenerating = true;
        for (int x = 0; x < pathFind.GetMap().GetWidth(); x++)
        {
            for (int y = 0; y < pathFind.GetMap().GetHeight(); y++)
            {
                pathFind.GetMap().GetMapNode(x, y).isAlive = false;

                if (Random.Range(0f, 1f) < .47f)
                {
                    pathFind.GetMap().GetMapNode(x, y).isAlive = true;
                }
                if (Random.Range(0f, 1f) < .20f)
                {
                    pathFind.SetNodeSprite(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeSprite.Box);


                }
                if (Random.Range(0f, 1f) < .15f)
                {
                    pathFind.SetNodeSprite(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeSprite.Metal);


                }
                    int count = 0;
                if (Random.Range(0f, 1f) < .001f && count < 4)
                {

                    //Instantiate(agent, pathFind.GetMap().GetPosition(x, y), Quaternion.identity);
                    count += 1;
                }
            }
        }

    }
    public void GenerateMap()
    {
        for (int i = 0; i < 1; i++)
        {

            for (int x = 0; x < pathFind.GetMap().GetWidth(); x++)
            {
                for (int y = 0; y < pathFind.GetMap().GetHeight(); y++)
                {
                    //GetLivingNeighbors(pathFind.GetMap().GetMapNode(x, y));
                    //pathFind.GetMap().GetMapNode(x, y).DrawAlive();
                    List<Node> neighbors = GetLivingNeighbors(pathFind.GetMap().GetMapNode(x, y));
                    //living = pathFind.GetMap().GetMapNode(x, y).isAlive;
                    if (neighbors.Count > 4)
                    {

                        pathFind.SetNodeSprite(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeSprite.Wall);

                    }
                    else if (neighbors.Count < 4)
                    {
                        pathFind.SetNodeSprite(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeSprite.None);

                    }
                    Node node = pathFind.GetMap().GetMapNode(x, y);
                    Node.NodeSprite nodeSprite = node.GetNodeSprite();





                }

            }
            //SetNextState();
        }
    }
    private List<Node> GetLivingNeighbors(Node currentNode)
    {
        List<Node> neighbourList = new List<Node>();

        if (currentNode.x - 1 >= 0)
        {
            // izquierda
            if (pathFind.GetMap().GetMapNode(currentNode.x - 1, currentNode.y).isAlive)
            {
                neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x - 1, currentNode.y));
            }
            // izquierda abajo
            if (currentNode.y - 1 >= 0)
            {
                if (pathFind.GetMap().GetMapNode(currentNode.x - 1, currentNode.y - 1).isAlive)
                {
                    neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x - 1, currentNode.y - 1));
                }
            }
            // izquierda arriba
            if (currentNode.y + 1 < pathFind.GetMap().GetHeight())
            {
                if (pathFind.GetMap().GetMapNode(currentNode.x - 1, currentNode.y + 1).isAlive)
                {
                    neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x - 1, currentNode.y + 1));
                }
            }
        }
        if (currentNode.x + 1 < pathFind.GetMap().GetWidth())
        {
            // derecha
            if (pathFind.GetMap().GetMapNode(currentNode.x + 1, currentNode.y).isAlive)
            {
                neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x + 1, currentNode.y));
            }
            // derecha abajo
            if (currentNode.y - 1 >= 0)
            {
                if (pathFind.GetMap().GetMapNode(currentNode.x + 1, currentNode.y - 1).isAlive)
                {
                    neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x + 1, currentNode.y - 1));
                }

            }
            // derecha arriba
            if (currentNode.y + 1 < pathFind.GetMap().GetHeight())
            {
                if (pathFind.GetMap().GetMapNode(currentNode.x + 1, currentNode.y + 1).isAlive)
                {
                    neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x + 1, currentNode.y + 1));
                }

            }
        }
        // abajo
        if (currentNode.y - 1 >= 0)
        {
            if (pathFind.GetMap().GetMapNode(currentNode.x, currentNode.y - 1).isAlive)
            {
                neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x, currentNode.y - 1));
            }

        }
        // arri9
        if (currentNode.y + 1 < pathFind.GetMap().GetHeight())
        {
            if (pathFind.GetMap().GetMapNode(currentNode.x, currentNode.y + 1).isAlive)
            {
                neighbourList.Add(pathFind.GetMap().GetMapNode(currentNode.x, currentNode.y + 1));
            }
        }

        return neighbourList;
    }
}
