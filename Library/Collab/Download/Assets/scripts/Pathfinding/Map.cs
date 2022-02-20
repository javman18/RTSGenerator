using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Map<T>
{
    public event EventHandler<MapChangedE> MapChanged;
    public class MapChangedE : EventArgs
    {
        public int x;
        public int y;
    }
    private int width;
    private int height;
    private float cellSize;
    private Vector3 origin;
    private T[,] mapArr;
    
    public Map(int w, int h, float cSize, Vector3 o, Func<Map<T>, int, int, T> createMap)
    {
        this.width = w;
        this.height = h;
        this.cellSize = cSize;
        origin = o;
        mapArr = new T[width, height];
        for (int x = 0; x < mapArr.GetLength(0); x++)
        {
            for (int y = 0; y < mapArr.GetLength(1); y++)
            {
                mapArr[x, y] = createMap(this, x, y);
            }
        }

        bool seeMap = false;
        if (seeMap)
        {
            for (int x = 0; x < mapArr.GetLength(0); x++)
            {
                for (int y = 0; y < mapArr.GetLength(1); y++)
                {
                    //Utilities.CreateText(null, mapArr[x, y].ToString(), GetPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetPosition(x, y), GetPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetPosition(x, y), GetPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetPosition(0, height), GetPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetPosition(width, 0), GetPosition(width, height), Color.white, 100f);
            
        }


    }
    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
    public Vector3 GetPosition(float x, float y)
    {
        return new Vector3(x, y) * cellSize + origin;
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            mapArr[x, y] = value;
            if (MapChanged != null)
            {
                MapChanged(this, new MapChangedE { x = x, y = y });
            }
        }
    }

    public void MapObjectChanged(int x, int y)
    {
        if (MapChanged != null)
        {
            MapChanged(this, new MapChangedE { x = x, y = y });
        }
    }

    public void GetXY(Vector3 position, out int x, out int y)
    {
        x = Mathf.FloorToInt((position - origin).x / cellSize);
        y = Mathf.FloorToInt((position - origin).y / cellSize);
    }
    public void SetMapNode(Vector3 worldPosition, T value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public T GetMapNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return mapArr[x, y];
        }
        else
        {
            return default(T);
        }
    }

    public T GetMapObject(Vector3 position)
    {
        int x, y;
        GetXY(position, out x, out y);
        return GetMapNode(x, y);
    }

}