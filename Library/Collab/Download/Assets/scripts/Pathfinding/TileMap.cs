using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    [System.Serializable]
    public struct Tiles
    {
        public Node.NodeSprite nodeSprite;
        public Vector2Int startPoint;
        public Vector2Int endPoint;

    }

    private struct TilesPosition
    {
        public Vector2 start;
        public Vector2 end;

    }
    [SerializeField] private Tiles[] tiles;
    private Map<Node> map;
    private Mesh mesh;
    private bool updateTiles;
    private Dictionary<Node.NodeSprite, TilesPosition> tilesPosDict;
    private Texture texture;
    private MeshRenderer ren;
    BoxCollider2D col;
    public static BoxCollider2D[] cols;
    public static List<Node> resources;
    public static List<Node> storages;
    public static List<Node> walls;
    private void Awake()
    {
        walls = new List<Node>();
        resources = new List<Node>();
        storages = new List<Node>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        ren = GetComponent<MeshRenderer>();
        texture = ren.material.mainTexture;
        float textureWidth = texture.width;
        float textureHeigh = texture.height;
        tilesPosDict = new Dictionary<Node.NodeSprite, TilesPosition>();
        foreach (Tiles tile in tiles)
        {
            tilesPosDict[tile.nodeSprite] = new TilesPosition {
                start = new Vector2(tile.startPoint.x / textureWidth, tile.startPoint.y / textureHeigh),
                end = new Vector2(tile.endPoint.x / textureWidth, tile.endPoint.y / textureHeigh),
            };
        }
    }

    public void setMap(Pathfinding path, Map<Node> map)
    {
        this.map = map;
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
        CreateEmptyMeshArrays(map.GetWidth() * map.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triengles);
        for (int x = 0; x < map.GetWidth(); x++)
        {
            for (int y = 0; y < map.GetHeight(); y++)
            {
                int index = x * map.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * map.GetCellSize();

                Node node = map.GetMapNode(x, y);
                Node.NodeSprite nodeSprite = node.GetNodeSprite();
                Vector2 tileStart, tileEnd;
                if (nodeSprite == Node.NodeSprite.None)
                {
                    cols = GetComponents<BoxCollider2D>();
                   
                    if (node.hasCollider)
                    {
                        Vector3 pos = map.GetPosition(x + 0.5f, y + 0.5f);
                        foreach (BoxCollider2D c in cols)
                        {
                            if (c.offset.x == pos.x && c.offset.y == pos.y)
                            {
                                //Debug.Log(pos.x + "," + pos.y + "," + c.offset.x + "," + c.offset.y);
                                Destroy(c);
                            }
                        }
                        
                    }
                    
                    tileStart = Vector2.zero;
                    tileEnd = Vector2.zero;
                    quadSize = Vector2.zero;
                    node.notWall = true;
                    node.hasCollider = false;
                    node.isResource = false;
                    node.isStorage = false;
                    node.isAlive = false;

                }
                else
                {
                    TilesPosition pos = tilesPosDict[nodeSprite];
                    tileStart = pos.start;
                    tileEnd = pos.end;
                }
                AddToMeshArrays(vertices, uv, triengles, index, map.GetPosition(x, y) + quadSize * .5f, 0f, quadSize, tileStart, tileEnd);

                if (nodeSprite == Node.NodeSprite.Wall)
                {
                    if (!node.hasCollider)
                    {

                        col = gameObject.AddComponent<BoxCollider2D>();
                        //walls.Add(node);
                        col.offset = new Vector2(node.x + 0.5f, node.y + 0.5f);
                        col.size = new Vector2(10f, 10f);
                        col.usedByComposite = true;
                        node.hasCollider = true;
                        
                    }
                    walls.Add(node);
                    
                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);
                    node.notWall = false;
                    node.isResource = false;
                    node.isStorage = false;
                    node.isAlive = true;
                }
                else if(nodeSprite == Node.NodeSprite.Scrap || nodeSprite == Node.NodeSprite.Metal || nodeSprite == Node.NodeSprite.Copper)
                {

                    if (!node.isResource)
                    {
                        resources.Add(node);
                        node.isResource = true;
                        
                    }
                    
                    //Debug.Log(node.x + " , " + node.y + " , " + nodeSprite);
                    node.notWall = false;
                    node.isStorage = false;
                    
                }
                else if(nodeSprite == Node.NodeSprite.Ground)
                {
                    
                    cols = GetComponents<BoxCollider2D>();

                    if (node.hasCollider)
                    {
                        Vector3 pos1 = map.GetPosition(x + 0.5f, y + 0.5f);
                        foreach (BoxCollider2D c in cols)
                        {
                            if (c.offset.x == pos1.x && c.offset.y == pos1.y)
                            {
                                //Debug.Log(pos.x + "," + pos.y + "," + c.offset.x + "," + c.offset.y);
                                Destroy(c);
                            }
                        }

                    }
                    
                    node.notWall = true;
                    node.hasCollider = false;
                    node.isResource = false;
                    node.isStorage = false;
                }else if(nodeSprite == Node.NodeSprite.Box)
                {
                    if (!node.isStorage)
                    {
                        storages.Add(node);
                        node.isStorage = true;


                    }
                    
                    node.notWall = false;
                    node.hasCollider = false;
                    //node.isStorage = true;
                    node.isResource = false;
                }
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triengles;
    }
    
    public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
    {
        vertices = new Vector3[4 * quadCount];
        uvs = new Vector2[4 * quadCount];
        triangles = new int[6 * quadCount];
    }
    public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
    {
        int vIndex = index * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        baseSize *= .5f;

        bool skewed = baseSize.x != baseSize.y;
        if (skewed)
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
        }
        else
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
            vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
            vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
            vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
        }
        uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
        uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
        uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
        uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

        int tIndex = index * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex3;
        triangles[tIndex + 2] = vIndex1;

        triangles[tIndex + 3] = vIndex1;
        triangles[tIndex + 4] = vIndex3;
        triangles[tIndex + 5] = vIndex2;
    }

    private static Quaternion GetQuaternionEuler(float rotFloat)
    {
        int rot = Mathf.RoundToInt(rotFloat);
        rot = rot % 360;
        if (rot < 0) rot += 360;
        if (cachedQuaternionEulerArr == null) CacheQuaternionEuler();
        return cachedQuaternionEulerArr[rot];
    }

    private static Quaternion[] cachedQuaternionEulerArr;
    private static void CacheQuaternionEuler()
    {
        if (cachedQuaternionEulerArr != null) return;
        cachedQuaternionEulerArr = new Quaternion[360];
        for (int i = 0; i < 360; i++)
        {
            cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
        }
    }
}
