﻿

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IBinaryTree<Node> {

    private Map<Node> map;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public float path = 0;
    public bool visited;
    public bool notWall;
    public bool hasCollider;
    public bool isResource;
    public bool isStorage;
    public bool isAlive;
    public bool isSpawn;
    public bool isCeiling;
    public bool isBase;
    public Node parent;
    public int value = 0;
    int binaryIndex;
    public enum NodeObject
    {
        None,
        Wall,
        Ground,
        Grass,
        Resource1,
        Resource2,
        Resource3,
        Box,
        Spawn,
        Brick,
        Base
    }
    private NodeObject nodeObject;
    public Node(Map<Node> map, int x, int y) {
        this.map = map;
        this.x = x;
        this.y = y;
        notWall = true;
        hasCollider = false;
        isResource = false;
        isStorage = false;
        isAlive = false;
        isSpawn = false;
        isCeiling = false;
        isBase = false;
        this.value = 0;
    }
    public void CalculateF()
    {
        fCost = gCost + hCost;
    }
    
    public void SetWall(bool notWall)
    {
        this.notWall = notWall;
        
        map.MapObjectChanged(x, y);
        
    }
    public void SetNodeObject(NodeObject nodeObject)
    {
        this.nodeObject = nodeObject;

        map.MapObjectChanged(x, y);
        
    }

    public NodeObject GetNodeObject()
    {
        return nodeObject;
    }

    public override string ToString()
    {
        return nodeObject.ToString();
    }
    public int BinaryIndex
    {
        get
        {
            return binaryIndex;
        }
        set
        {
            binaryIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    [System.Serializable]
    public class SaveNode
    {
        public NodeObject nodeSprite;
        public int x;
        public int y;
        public int v;
    }

    public SaveNode Save()
    {
        return new SaveNode
        {
            nodeSprite = nodeObject,
            x = x,
            y = y,
            v = value
        };
    }

    public void Load(SaveNode saveNode)
    {
        nodeObject = saveNode.nodeSprite;
        value = saveNode.v;
    }

    
}
