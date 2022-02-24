using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    
    
    
    private Map<Node> map;
    private Pathfinding path;
    private Mesh mesh;
    private bool updateTiles;
    
    BoxCollider2D col;
    public static BoxCollider2D[] cols;
    public static List<Node> resources;
    public static List<GameObject> srapsList;
    public static List<GameObject> copperList;
    public static List<GameObject> metalList;
    public static List<Node> storages;
    public static List<Node> walls;
    public static List<GameObject> wallsList;
    public GameObject wall;
    public GameObject scraps;
    public GameObject copper;
    public GameObject metal;
    public GameObject box;
    Camera cam;
    public static int wallCount = 500;
    public static int scrapsCount = 500;
    public static int copperCount = 500;
    public static int metalCount = 500;
   
    private void Awake()
    {
        walls = new List<Node>();
        resources = new List<Node>();
        storages = new List<Node>();
        wallsList = new List<GameObject>();
        srapsList = new List<GameObject>();
        copperList = new List<GameObject>();
        metalList = new List<GameObject>();
        ObjectPool.Instance.SetPooledObjects(wallsList, wall, 500);
        ObjectPool.Instance.SetPooledObjects(srapsList, scraps, 500);
        ObjectPool.Instance.SetPooledObjects(copperList, copper, 500);
        ObjectPool.Instance.SetPooledObjects(metalList, metal, 500);
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    
    public void setMap(Pathfinding path, Map<Node> map)
    {
        this.map = map;
        this.path = path;
        UpdateTiles();
        map.MapChanged += Map_OnMapValueChanged;
        path.OnLoaded += Map_OnLoaded;
    }

    private void Map_OnLoaded(object sender, System.EventArgs e)
    {
        updateTiles = true;
    }

    private void Map_OnMapValueChanged(object sender, Map<Node>.MapChangedE e)
    {
        updateTiles = true;
    }

    private void LateUpdate()
    {
        if (updateTiles)
        {
            updateTiles = false;
            UpdateTiles();
        }
        
    }

    private void UpdateTiles()
    {
        for (int x = 0; x < map.GetWidth(); x++)
        {
            for (int y = 0; y < map.GetHeight(); y++)
            {
               

                Node node = map.GetMapNode(x, y);
                Node.NodeObject nodeObject = node.GetNodeObject();
                
                if (nodeObject == Node.NodeObject.None)
                {

                    if (!node.notWall)
                    {
                        Vector3 pos = map.GetPosition(x + 0.5f, y + 0.5f);
                        foreach (GameObject wall in wallsList)
                        {
                            if (wall.transform.position.x == pos.x && wall.transform.position.y == pos.y)
                            {
                                //Debug.Log(pos.x + "," + pos.y + "," + c.offset.x + "," + c.offset.y);

                                wallCount++;
                                wall.SetActive(false);
                            }
                        }
                        node.notWall = true;
                    }
                    if (node.isResource)
                    {
                        DeleteResource(srapsList, x, y, scrapsCount);
                        DeleteResource(copperList, x, y, copperCount);
                        DeleteResource(metalList, x, y, metalCount);
                        node.isResource = false;
                    }
                    
                    
                    
                    node.isStorage = false;
                    node.isAlive = false;

                }
                

                else if (nodeObject == Node.NodeObject.Wall)
                {
                    if (node.notWall)
                    {
                        GameObject tmpWall = ObjectPool.Instance.GetPooledObjects(wallsList, 500);
                        if (tmpWall != null)
                        {
                            tmpWall.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpWall.transform.tag = "Wall";
                            wallCount--;
                            tmpWall.SetActive(true);
                        }
                            
                        walls.Add(node);
                        //wallsList.Add(tmpWall);
                    }

                    node.notWall = false;

                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);

                    node.isResource = false;
                    node.isStorage = false;
                    node.isAlive = true;
                }
                else if(nodeObject == Node.NodeObject.Scrap  )
                {

                    if (!node.isResource)
                    {
                        GameObject tmpScrap = ObjectPool.Instance.GetPooledObjects(srapsList, 500);
                        if (tmpScrap != null)
                        {
                            tmpScrap.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpScrap.transform.tag = "Resource";
                            scrapsCount--;
                            tmpScrap.SetActive(true);
                        }
                        
                        resources.Add(node);
                    }
                    node.isResource = true;
                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);
                    //node.notWall = false;
                    node.isStorage = false;
                    
                }else if (nodeObject == Node.NodeObject.Metal)
                {
                    if (!node.isResource)
                    {
                        GameObject tmpMetal = ObjectPool.Instance.GetPooledObjects(metalList, 500);
                        if (tmpMetal != null)
                        {
                            tmpMetal.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpMetal.transform.tag = "Resource";
                            metalCount--;
                            tmpMetal.SetActive(true);
                        }
                        resources.Add(node);
                        
                        
                    }
                    node.isResource = true;

                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);
                    //node.notWall = false;
                    node.isStorage = false;
                }
                else if (nodeObject == Node.NodeObject.Copper)
                {
                    if (!node.isResource)
                    {
                        GameObject tmpCopper = ObjectPool.Instance.GetPooledObjects(copperList, 500);
                        if (tmpCopper != null)
                        {
                            tmpCopper.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpCopper.transform.tag = "Resource";
                            copperCount--;
                            tmpCopper.SetActive(true);
                        }
                        resources.Add(node);
                        
                        

                    }
                    node.isResource = true;
                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);
                    //node.notWall = false;
                    node.isStorage = false;
                }
                else if(nodeObject == Node.NodeObject.Ground)
                {
                    
                    
                    
                    node.notWall = true;
                    node.hasCollider = false;
                    node.isResource = false;
                    node.isStorage = false;
                }else if(nodeObject == Node.NodeObject.Box)
                {
                    if (!node.isStorage)
                    {
                        Instantiate(box, map.GetPosition(x + 0.5f, y + 0.5f), Quaternion.identity);
                        storages.Add(node);
                        


                    }
                    node.isStorage = true;
                    //node.notWall = false;
                    node.hasCollider = false;
                    //node.isStorage = true;
                    node.isResource = false;
                }
            }
        }

    }
    public void DeleteResource(List<GameObject> resources, int x, int y, int count)
    {
        Vector3 pos = map.GetPosition(x + 0.5f, y + 0.5f);
        foreach (GameObject resource in resources)
        {
            if (resource.transform.position.x == pos.x && resource.transform.position.y == pos.y)
            {
                //Debug.Log(pos.x + "," + pos.y + "," + c.offset.x + "," + c.offset.y);
                count++;
                resource.SetActive(false);
            }
        }
    }
    
}
