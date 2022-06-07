using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] public static TileMap tileMap, tileMap2, tileMap3, tileMap4;
    public static Pathfinding pathFind;
    public static Pathfinding pathfind2;
    public static Pathfinding pathfind3;
    public static Pathfinding pathfind4;
     public TMP_InputField baseIdInput;
    public Dropdown tD;
    public TMP_Dropdown baseDD;
    private Node.NodeObject nodeSprite;
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
    public string[] sName;
    //public GameObject floor;
    private PathAgent[] agents;
    AgentManager[] agents1;
    bool placeStorage = false;
    bool PlaceSpawner = false;
    public bool isGenerating = false;
    public float targetTime = 0.0f;
    public int[][] lol;
    public GameObject agent;
    GameObject terrain;
    public static Rect rect1;
    public static Rect rect2;
    public static Rect rect3;
    public static Rect rect4;
    public static List<Pathfinding> maps;
    public static List<Rect> rects;
    public Camera cam;
    bool canSetP1Tiles = false;
    bool canSetP2Tiles = false;
    // Start is called before the first frame update
    private void Awake()
    {
        baseIdInput.text = "0";
        sName = new string [4];
        tileMap = GameObject.Find("TileMap1").GetComponent<TileMap>();
        tileMap2 = GameObject.Find("TileMap2").GetComponent<TileMap>();
        tileMap3 = GameObject.Find("TileMap3").GetComponent<TileMap>();
        tileMap4 = GameObject.Find("TileMap4").GetComponent<TileMap>();
        maps = new List<Pathfinding>();
        rects = new List<Rect>();
        vis = GameObject.FindGameObjectWithTag("Wall");
        origin = new Vector3(0, 0, 0);
        scene = SceneManager.GetActiveScene();
        pathFind = new Pathfinding(width, height, origin);
        pathfind2 = new Pathfinding(width, height, new Vector3(1000, 0, 0));
        pathfind3 = new Pathfinding(width, height, new Vector3(0, 1000, 0));
        pathfind4 = new Pathfinding(width, height, new Vector3(1000, 1000, 0));
        maps.Add(pathFind);
        maps.Add(pathfind2);
        maps.Add(pathfind3);
        maps.Add(pathfind4);
        
        agents = GameObject.FindObjectsOfType<PathAgent>();
        agents1 = FindObjectsOfType<AgentManager>();
        rect1 = new Rect(pathFind.origin.x, pathFind.origin.y, pathFind.GetMap().GetWidth() * 10f, pathFind.GetMap().GetWidth() * 10f);
        rect2 = new Rect(pathfind2.origin.x, pathfind2.origin.y, pathfind2.GetMap().GetWidth() * 10f, pathfind2.GetMap().GetWidth() * 10f);
        rect3 = new Rect(pathfind3.origin.x, pathfind3.origin.y, pathfind3.GetMap().GetWidth() * 10f, pathfind3.GetMap().GetWidth() * 10f);
        rect4 = new Rect(pathfind4.origin.x, pathfind4.origin.y, pathfind4.GetMap().GetWidth() * 10f, pathfind4.GetMap().GetWidth() * 10f);
        
        //terrain = Resources.Load<GameObject>("GameHelpers/terrainR");

        pathFind.SetNodeSpriteVisual(tileMap);
        pathfind2.SetNodeSpriteVisual(tileMap2);
        pathfind3.SetNodeSpriteVisual(tileMap3);
        pathfind4.SetNodeSpriteVisual(tileMap4);
        //pathFind.Load("rtest_2");
        //pathfind2.Load("path2");
        //pathfind3.Load("path3B");
        //pathfind4.Load("path4A");
        //InitLevel("Master02");

        //terrain.gameObject.transform.localScale = new Vector3((size * width)*2, (size * height)*2, 1);
        //GameObject terr =  Instantiate(terrain, new Vector2(1000, 1000), Quaternion.identity);
        //terr.name = terrain.name;
        tD.onValueChanged.AddListener(delegate
        {
            if(tD.value == 0)
            {
                placeStorage = true;
            }else if(tD.value == 1)
            {
                PlaceSpawner = true;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
       


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
        if (placeStorage)
        {
            buildingTest(Node.NodeObject.Box);
        }
        if (PlaceSpawner)
        {
            BuildSpawner(Node.NodeObject.Brick);
        }

    }
    private void LateUpdate()
    {
        
        
    }
    void PlaceWallWithMouse()
    {
        if (UIManager.currentState == UIManager.UIStates.Save)
        {
            if (Input.GetMouseButtonDown(1))
            {
                mousePressed = true;
            }
            if (mousePressed)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                for (int i = 0; i < maps.Count; i++)
                {
                    maps[i].GetMap().GetXY(new Vector3(mousePos.x, mousePos.y), out int x, out int y);

                    maps[i].SetNodeObject(mousePos, nodeSprite, baseDD.value);

                    
                    //Debug.Log(node.value);
                    
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                mousePressed = false;
            }
        }
        
        

    }
    public void OnSave()
    {
        for (int i = 0; i < maps.Count; i++)
        {

            //sName[i] = saveName.text;
            sName[i] = saveName.text + "_" + i;
            maps[i].Save(sName[i], baseDD.value);
        }
            
       
        //foreach (AgentManagerTest agent in agents1)
        //{
        //    agent.SaveAgent();
        //}
        
    }

    public void OnLoad()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            sName[i] = saveName.text + "_" + i;
            
            maps[i].Load(sName[i]);
        }
        
        //foreach (AgentManagerTest agent in agents1)
        //{
        //    agent.LoadPlayer();
        //}
        
    }

    public void InitLevel(string levelName)
    {
        string[] sName = new string[4];
        for (int i = 0; i < maps.Count; i++)
        {
            sName[i] = levelName + "_" + i;

            maps[i].Load(sName[i]);
        }
    }
    
    public void DrawLine()
    {
        if (Input.GetMouseButtonDown(1))
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
    void buildingTest(Node.NodeObject nodesprite)
    {

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            pathFind.GetMap().GetXY(new Vector3(mousePos.x, mousePos.y), out int x, out int y);
            //for (int i = -2; i < 5; i++)
            //{
            //    for (int j = -2; j < 5; j++)
            //    {
            //        for (int k = 0; k < maps.Count; k++)
            //        {
            //            maps[k].SetNodeObject(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeObject.Ground);
            //        }
                    

            //    }
            //}
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < maps.Count; k++)
                    {
                        maps[k].SetNodeObject(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), nodesprite, int.Parse(baseIdInput.text));
                    }
                }
            }
            
            placeStorage = false;
            
        }
    }
    void BuildSpawner(Node.NodeObject nodesprite)
    {

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            pathFind.GetMap().GetXY(new Vector3(mousePos.x, mousePos.y), out int x, out int y);
            //for (int i = -2; i < 5; i++)
            //{
            //    for (int j = -2; j < 5; j++)
            //    {
            //        for (int k = 0; k < maps.Count; k++)
            //        {
            //            maps[k].SetNodeObject(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), Node.NodeObject.Ground);
            //        }


            //    }
            //}
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < maps.Count; k++)
                    {
                        maps[k].SetNodeObject(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * i, mousePos.y - pathFind.GetMap().GetCellSize() * j), nodesprite, int.Parse(baseIdInput.text));
                    }
                }
            }
            for (int k = 0; k < maps.Count; k++)
            {
                maps[k].SetNodeObject(new Vector3(mousePos.x + pathFind.GetMap().GetCellSize() * 2, mousePos.y - pathFind.GetMap().GetCellSize() * 2), Node.NodeObject.Spawn, int.Parse(baseIdInput.text));
            }
            PlaceSpawner = false;

        }
    }
    public void ActivateTile(int val)
    {
        if (val == 0)
        {
            nodeSprite = Node.NodeObject.None;

        }
        if (val == 1)
        {
            nodeSprite = Node.NodeObject.Wall;

        }
        if (val == 2)
        {
            nodeSprite = Node.NodeObject.Ground;

        }
        if (val == 3)
        {
            nodeSprite = Node.NodeObject.Grass;

        }
        if (val == 4)
        {
            nodeSprite = Node.NodeObject.Resource1;

        }
        if (val == 5)
        {
            nodeSprite = Node.NodeObject.Resource2;

        }
        if (val == 6)
        {
            nodeSprite = Node.NodeObject.Resource3;

        }
        if (val == 7)
        {
            nodeSprite = Node.NodeObject.Box;

        }
        if(val == 8)
        {
            nodeSprite = Node.NodeObject.Spawn;
        }
        if(val == 9)
        {
            nodeSprite = Node.NodeObject.Brick;
        }
        if (val == 10)
        {
            nodeSprite = Node.NodeObject.Base;
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
                //if (Random.Range(0f, 1f) < .20f)
                //{
                //    pathFind.SetNodeObject(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeObject.Box);


                //}
                //if (Random.Range(0f, 1f) < .15f)
                //{
                //    pathFind.SetNodeObject(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeObject.Resource2);


                //}
                //    int count = 0;
                //if (Random.Range(0f, 1f) < .001f && count < 4)
                //{

                //    //Instantiate(agent, pathFind.GetMap().GetPosition(x, y), Quaternion.identity);
                //    count += 1;
                //}
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

                        pathFind.SetNodeObject(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeObject.Wall, int.Parse(baseIdInput.text));

                    }
                    else if (neighbors.Count < 4)
                    {
                        pathFind.SetNodeObject(new Vector3(x * pathFind.GetMap().GetCellSize(), pathFind.GetMap().GetCellSize() * y), Node.NodeObject.None, int.Parse(baseIdInput.text));

                    }
                    Node node = pathFind.GetMap().GetMapNode(x, y);
                    Node.NodeObject nodeSprite = node.GetNodeObject();





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
