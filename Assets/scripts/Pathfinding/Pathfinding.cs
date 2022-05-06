

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class Pathfinding {

    

    public event EventHandler OnLoaded;
    private const int straightCost = 10;
    private const int diagonalCost = 14;
    public static Pathfinding Instance { get; private set; }

    private Map<Node> map;
    private BinaryTree<Node> openList;
    private HashSet<Node> closedList;
    int infinity = 9999999;
    public Vector3 origin;
    
    public Pathfinding(int width, int height, Vector3 o) {
        Instance = this;
        origin = o;
        map = new Map<Node>(width, height, 10f, o, (Map<Node> g, int x, int y ) => new Node(g, x, y));
        //SetNodeSpriteVisual(this, MapGenerator.tileMap);
    }

    public void SetNodeObject(Vector3 worldPos, Node.NodeObject nodeSprite, int value)
    {

        Node nodeObject = map.GetMapObject(worldPos);
        if(nodeObject != null)
        {
            nodeObject.SetNodeObject(nodeSprite);
            nodeObject.value = value;
            //nodeObject.SetWall(!nodeObject.notWall);
        }
    }
    public void SetNodeSpriteVisual(TileMap tilemap)
    {
        
            tilemap.setMap(this, map);
        
        
    }
    /*
    * @param saveString: nombre del mapa que se quere guardar
    * @brief función para guardar los atributos del nodo 
   */
    public void Save(string saveName, int value)
    {
        List<Node.SaveNode> nodeObjectSaveObjectList = new List<Node.SaveNode>();
        for (int x = 0; x < map.GetWidth(); x++)
        {
            for (int y = 0; y < map.GetHeight(); y++)
            {
                Node nodeObject = map.GetMapNode(x, y);
                nodeObject.value = map.GetMapNode(x, y).value;
                nodeObject.notWall = true;
                nodeObjectSaveObjectList.Add(nodeObject.Save());
            }
        }
        SaveObject saveObject = new SaveObject { nodeObjectSaveObjectArray = nodeObjectSaveObjectList.ToArray()};

        SaveSystem.SaveObject(saveName,saveObject);

    }
    /*
     * @param saveString: nombre del mapa que se quere cargar
     * @brief función para cargar los atributos del nodo 
    */
    public void Load(string saveString)
    {
        SaveObject saveObject = SaveSystem.LoadObject<SaveObject>(saveString);
        foreach (Node.SaveNode nodeObjectSaveObject in saveObject.nodeObjectSaveObjectArray)
        {
            Node nodeObject = map.GetMapNode(nodeObjectSaveObject.x, nodeObjectSaveObject.y);
            nodeObject.value = map.GetMapNode(nodeObjectSaveObject.x, nodeObjectSaveObject.y).value;
            nodeObject.notWall = true;

            nodeObject.Load(nodeObjectSaveObject);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public class SaveObject
    {
        public Node.SaveNode[] nodeObjectSaveObjectArray;
        
    }
    //Codigo basado en el video https://www.youtube.com/watch?v=alU04hvz6L4
    public Map<Node> GetMap() {
        return map;
    }
    /*
    * @param start: inicio del camino que se quiere buscar en una posicion real
    * @param end: final del camino que se quiere buscar en una posicion real
    * @param helperPos: offset para poder crear multiples pathfindings
    * @brief convierta los nodos encontrados con la función A* en vectores
    * @return una lista de vectores para crear el camino 
   */
    public List<Vector3> FindPath(Vector3 start, Vector3 end , Vector3 helperPos) {
        
        map.GetXY(start, out int startX, out int startY);
        map.GetXY(end, out int endX, out int endY);

        List<Node> path = AStar(startX, startY, endX, endY);
        if (path == null) {
            return null;
        } else {
            List<Vector3> pathVec = new List<Vector3>();
            foreach (Node pathNode in path) {
                pathVec.Add(new Vector3(pathNode.x, pathNode.y) * map.GetCellSize() + Vector3.one + helperPos * map.GetCellSize() * .5f);
            }
            return pathVec;
        }
    }
    /*
    * @param starX startY: inicio del camino que se quiere buscar
    * @param endX endY: final del camino que se quiere buscar
    * @brief función A* para encontrar el camino mas corto desde un inicio y un final
    * @return una lista de nodos para crear el camino con la funcion BackTrack(end)
   */
    public List<Node> AStar(int startX, int startY, int endX, int endY) {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node begin = map.GetMapNode(startX, startY);
        Node goal = map.GetMapNode(endX, endY);

        if (begin == null || goal == null) {
           
            return null;
        }

        openList = new BinaryTree<Node>(map.MaxSize);
        closedList = new HashSet<Node>();
        openList.Add(begin);
        for (int x = 0; x < map.GetWidth(); x++) {
            for (int y = 0; y < map.GetHeight(); y++) {
                Node pathNode = map.GetMapNode(x, y);
                pathNode.gCost = infinity;
                pathNode.CalculateF();
                pathNode.parent = null;
            }
        }

        begin.gCost = 0;
        begin.hCost = HeuristicM(begin, goal);
        begin.CalculateF();

       
        while (openList.Count > 0) {
            Node current = openList.RemoveFirst();
            closedList.Add(current);
            if (current == goal) {
                sw.Stop();
                UnityEngine.Debug.Log("Camino encontrado en " + sw.ElapsedMilliseconds + " ms");
                return BackTrack(goal);
            }

            foreach (Node neighbourNode in GetNeighbourList(current)) {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.notWall) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = current.gCost + HeuristicM(current, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.parent = current;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = HeuristicM(neighbourNode, goal);
                    neighbourNode.CalculateF();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                    else
                    {
                        openList.UpdeateLeaf(neighbourNode);
                    }
                }
                
            }
        }
        return null;
    }
    /*
    * @param currentNode: nodo que se esta revisando para obtener sus vecinos
    * @brief recorre un nodo para obtener sus vecinos
    * @return una lista de nodos 
   */
    private List<Node> GetNeighbourList(Node currentNode) {
        List<Node> neighbourList = new List<Node>();

        if (currentNode.x - 1 >= 0) {
            // izquierda
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // izquierda abajo
            if (currentNode.y - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            }
            // izquierda arriba
            if (currentNode.y + 1 < map.GetHeight())
            {
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }
        if (currentNode.x + 1 < map.GetWidth()) {
            // derecha
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // derecha abajo
            if (currentNode.y - 1 >= 0)
            {
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            }
            // derecha arriba
            if (currentNode.y + 1 < map.GetHeight())
            {
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }
        // abajo
        if (currentNode.y - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        }
        // arri9
        if (currentNode.y + 1 < map.GetHeight())
        {
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
        }

        return neighbourList;
    }

    public Node GetNode(int x, int y) {
        return map.GetMapNode(x, y);
    }

    private List<Node> BackTrack(Node end) {
        List<Node> path = new List<Node>();
        path.Add(end);
        Node current = end;
        while (current.parent != null) {
            path.Add(current.parent);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    private int HeuristicM(Node a, Node b) {
        int xDist = Mathf.Abs(a.x - b.x);
        int yDist = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDist - yDist);
        return diagonalCost * Mathf.Min(xDist, yDist) + straightCost * remaining;
    }

    //private Node LowesFCost(List<Node> pathNodeList) {
    //    Node lowestFCostNode = pathNodeList[0];
    //    for (int i = 1; i < pathNodeList.Count; i++) {
    //        if (pathNodeList[i].fCost < lowestFCostNode.fCost) {
    //            lowestFCostNode = pathNodeList[i];
    //        }
    //    }
    //    return lowestFCostNode;
    //}

    public List<Node> Dijkstra(int startX, int startY, int endX, int endY)
    {
        Node begin = map.GetMapNode(startX, startY);
        Node goal = map.GetMapNode(endX, endY);
        begin.parent = null;
        begin.path = 0;
        begin.visited = true;

        if (begin == null || goal == null)
        {

            return null;
        }
        for (int x = 0; x < map.GetWidth(); x++)
        {
            for (int y = 0; y < map.GetHeight(); y++)
            {
                Node pathNode = map.GetMapNode(x, y);
                pathNode.gCost = infinity;

                pathNode.parent = null;
            }
        }

        List<Node> q = new List<Node> { begin };
        while (q.Count > 0)
        {
            Node current = q[0];
            if(current == goal)
            {
                return BackTrack(goal);
            }
            q.Remove(current);
            foreach(Node neighbourNode in GetNeighbourList(current))
            {
                if (!neighbourNode.notWall)
                {
                    q.Add(neighbourNode);
                    continue;
                }
                if(current.path + current.gCost < neighbourNode.path)
                {
                    current.path = Vector2.Distance(new Vector2(neighbourNode.x, neighbourNode.y), new Vector2(current.x, current.y));
                    neighbourNode.path = current.path + current.gCost;
                    neighbourNode.parent = current;
                    if (!q.Contains(neighbourNode))
                    {
                        q.Add(neighbourNode);
                    }
                }
            }
            current = q[0];

        }
        return null;
    }

    public List<Vector3> FindPathD(Vector3 start, Vector3 end)
    {
        map.GetXY(start, out int startX, out int startY);
        map.GetXY(end, out int endX, out int endY);

        List<Node> path = Dijkstra(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> pathVec = new List<Vector3>();
            foreach (Node pathNode in path)
            {
                pathVec.Add(new Vector3(pathNode.x, pathNode.y) * map.GetCellSize() + Vector3.one * map.GetCellSize() * .5f);
            }
            return pathVec;
        }
    }

}
