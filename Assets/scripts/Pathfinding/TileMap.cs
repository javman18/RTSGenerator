using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileMap : MonoBehaviour
{
    
    
    
    private Map<Node> map;
    private Pathfinding path;
    private Mesh mesh;
    private bool updateTiles;
   
    BoxCollider2D col;
    public static BoxCollider2D[] cols;
    public static List<Node> resources;
    public static List<GameObject> resource1List;
    public static List<GameObject> resource3List;
    public static List<GameObject> resource2List;
    public static List<Node> storages;
    public static List<Node> walls;
    public static List<GameObject> wallsList;
    public static List<GameObject> spawnList;
    public static List<GameObject> storageList;
    public static List<GameObject> brickList;
    public static List<GameObject> baseList;
    public GameObject wall;
    public GameObject resource1;
    public GameObject resource3;
    public GameObject resource2;
    public GameObject box;
    public GameObject spawn;
    public GameObject brick;
    public GameObject _base;
    Camera cam;
    public static int wallCount = 1000;
    public static int resource1Count = 500;
    public static int resource3Count = 500;
    public static int resource2Count = 500;
    
   
    private void Awake()
    {
        walls = new List<Node>();
        resources = new List<Node>();
        storages = new List<Node>();
        wallsList = new List<GameObject>();
        resource1List = new List<GameObject>();
        resource3List = new List<GameObject>();
        resource2List = new List<GameObject>();
        spawnList = new List<GameObject>();
        storageList = new List<GameObject>();
        brickList = new List<GameObject>();
        baseList = new List<GameObject>();
        //ObjectPool.Instance.SetPooledObjects(wallsList, wall, 500);
        //ObjectPool.Instance.SetPooledObjects(resource1List, resource1, 500);
        //ObjectPool.Instance.SetPooledObjects(resource3List, resource3, 500);
        //ObjectPool.Instance.SetPooledObjects(resource2List, resource2, 500);
        //ObjectPool.Instance.SetPooledObjects(spawnList, spawn, 20);
        //ObjectPool.Instance.SetPooledObjects(brickList, brick, 100);
        //ObjectPool.Instance.SetPooledObjects(baseList, _base, 100);

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
                        foreach (GameObject wall in ObjectPool.wallsList)
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
                        DeleteResource(ObjectPool.resource1List, x, y, resource1Count);
                        DeleteResource(ObjectPool.resource3List, x, y, resource3Count);
                        DeleteResource(ObjectPool.resource2List, x, y, resource2Count);
                        node.isResource = false;
                    }
                    if (node.isSpawn)
                    {
                        Vector3 pos = map.GetPosition(x + 0.5f, y + 0.5f);
                        foreach (GameObject spawn in ObjectPool.spawnList)
                        {
                            if (spawn.transform.position.x == pos.x && spawn.transform.position.y == pos.y)
                            {
                                //Debug.Log(pos.x + "," + pos.y + "," + c.offset.x + "," + c.offset.y);

                                
                                spawn.SetActive(false);
                            }
                        }
                        node.isSpawn = false;
                    }
                    if (node.isStorage)
                    {
                        Vector3 pos = map.GetPosition(x + 0.5f, y + 0.5f);
                        foreach (GameObject storage in ObjectPool.storageList)
                        {
                            if (storage.transform.position.x == pos.x && storage.transform.position.y == pos.y)
                            {
                                //Debug.Log(pos.x + "," + pos.y + "," + c.offset.x + "," + c.offset.y);


                                storage.SetActive(false);
                            }
                        }
                        node.isStorage = false;
                    }

                    if (node.isCeiling)
                    {
                        Vector3 pos = map.GetPosition(x + 0.5f, y + 0.5f);
                        foreach (GameObject brick in ObjectPool.brickList)
                        {
                            if (brick.transform.position.x == pos.x && brick.transform.position.y == pos.y)
                            {
                                //Debug.Log(pos.x + "," + pos.y + "," + c.offset.x + "," + c.offset.y);


                                brick.SetActive(false);
                            }
                        }
                        node.isCeiling = false;
                    }

                    //node.isStorage = false;
                    node.isAlive = false;

                }
                

                else if (nodeObject == Node.NodeObject.Wall /*&& wallCount > 0*/)
                {
                    if (node.notWall)
                    {
                        GameObject tmpWall = ObjectPool.Instance.GetPooledObjects(ObjectPool.wallsList, ObjectPool.Instance.wallAmount);
                        if (tmpWall != null)
                        {
                            tmpWall.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpWall.transform.tag = "Wall";
                            wallCount--;
                            tmpWall.SetActive(true);
                            tmpWall.transform.parent = this.transform;
                        }
                            
                        walls.Add(node);
                        node.notWall = false;
                        node.isAlive = true;
                        //wallsList.Add(tmpWall);
                    }

                   

                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);

                    node.isResource = false;
                    node.isStorage = false;
                    
                }
                else if(nodeObject == Node.NodeObject.Resource1 /*&& resource1Count > 0*/)
                {

                    if (!node.isResource)
                    {
                        GameObject tmpResource1 = ObjectPool.Instance.GetPooledObjects(ObjectPool.resource1List, ObjectPool.Instance.resource1Amount);
                        if (tmpResource1 != null)
                        {
                            tmpResource1.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpResource1.transform.tag = "Resource";
                            resource1Count--;
                            tmpResource1.SetActive(true);
                        }
                        
                        resources.Add(node);
                    }
                    node.isResource = true;
                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);
                    //node.notWall = false;
                    node.isStorage = false;
                    
                }else if (nodeObject == Node.NodeObject.Resource2 /*&& resource2Count > 0*/)
                {
                    if (!node.isResource)
                    {
                        GameObject tmpResource2 = ObjectPool.Instance.GetPooledObjects(ObjectPool.resource2List, ObjectPool.Instance.resource2Amount);
                        if (tmpResource2 != null)
                        {
                            tmpResource2.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpResource2.transform.tag = "Resource";
                            resource2Count--;
                            tmpResource2.SetActive(true);
                        }
                        resources.Add(node);
                        
                        
                    }
                    node.isResource = true;

                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);
                    //node.notWall = false;
                    node.isStorage = false;
                }
                else if (nodeObject == Node.NodeObject.Resource3 /*&& resource3Count > 0*/)
                {
                    if (!node.isResource)
                    {
                        GameObject tmpResource3 = ObjectPool.Instance.GetPooledObjects(ObjectPool.resource3List, ObjectPool.Instance.resource3Amount);
                        if (tmpResource3 != null)
                        {
                            tmpResource3.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpResource3.transform.tag = "Resource";
                            resource3Count--;
                            tmpResource3.SetActive(true);
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
                        GameObject st = Instantiate(box, map.GetPosition(x + 0.5f, y + 0.5f), Quaternion.identity);
                        storages.Add(node);
                        storageList.Add(st);


                    }
                    node.isStorage = true;
                    //node.notWall = false;
                    node.hasCollider = false;
                    //node.isStorage = true;
                    node.isResource = false;
                }
                else if (nodeObject == Node.NodeObject.Spawn)
                {
                    GameObject tmpSpawn = ObjectPool.Instance.GetPooledObjects(ObjectPool.spawnList, ObjectPool.Instance.spawnAmount);
                    if (!node.isSpawn)
                    {
                        if (tmpSpawn != null)
                        {
                            tmpSpawn.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpSpawn.transform.tag = "Spawn";
                            resource3Count--;
                            tmpSpawn.SetActive(true);
                        }

                    }

                    node.isSpawn = true;
                    node.isStorage = false;
                    //node.notWall = false;
                    
                    //node.isStorage = true;
                    node.isResource = false;
                }
                else if (nodeObject == Node.NodeObject.Brick)
                {
                    GameObject tmpBrick = ObjectPool.Instance.GetPooledObjects(ObjectPool.brickList, ObjectPool.Instance.brickAmount);
                    if (!node.isCeiling)
                    {
                        if (tmpBrick != null)
                        {
                            tmpBrick.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);
                            tmpBrick.transform.tag = "Ceiling";
                           
                            tmpBrick.SetActive(true);
                        }

                    }

                    node.isCeiling = true;
                    node.isStorage = false;
                    //node.notWall = false;
                    
                    //node.isStorage = true;
                    node.isResource = false;
                }
                else if (nodeObject == Node.NodeObject.Base)
                {
                    GameObject tmpBase = ObjectPool.Instance.GetPooledObjects(ObjectPool.baseList, ObjectPool.Instance.baseAmount);
                    if (!node.isBase)
                    {
                        if (tmpBase != null)
                        {
                            tmpBase.transform.position = map.GetPosition(x + 0.5f, y + 0.5f);

                            tmpBase.transform.tag = "Base";
                            tmpBase.SetActive(true);
                            //if (baseIdInput.gameObject.activeInHierarchy)
                            //{
                            //    node.value = int.Parse(baseIdInput.text);
                            //    Debug.Log("Hola");
                            //}
                            //Debug.Log(node.value);
                            //tmpBase.GetComponent<HomeBase>().iD = node.value;
                            
                            //Debug.Log(node.value);
                            tmpBase.GetComponent<HomeBase>().iD = node.value;
                            tmpBase.GetComponent<HomeBase>().healthAmount = 200;
                            if (tmpBase.GetComponent<HomeBase>().iD == 2)
                            {
                                tmpBase.GetComponent<SpriteRenderer>().color = Color.red;
                                
                            }
                        }


                    }

                    node.isCeiling = true;
                    node.isStorage = false;
                    node.isBase = true;
                    //node.notWall = false;

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
