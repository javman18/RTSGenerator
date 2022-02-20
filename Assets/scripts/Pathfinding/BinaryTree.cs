using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BinaryTree<T> where T : IBinaryTree<T>
{
    T[] leaves;
    int leafCount;

    public BinaryTree(int maxTreeSize)
    {
        leaves = new T[maxTreeSize];
    }
    public void Add(T leaf)
    {
        leaf.BinaryIndex = leafCount;
        leaves[leafCount] = leaf;
        SortUp(leaf);
        leafCount++;

    }
    public T RemoveFirst()
    {
        T root = leaves[0];
        leafCount--;
        leaves[0] = leaves[leafCount];
        leaves[0].BinaryIndex = 0;
        SortDown(leaves[0]);
        return root;
    }
    public void UpdeateLeaf(T leaf)
    {
        SortUp(leaf);

    }
    public int Count
    {
        get
        {
            return leafCount;
        }
    }
    public bool Contains(T leaf)
    {
        return Equals(leaves[leaf.BinaryIndex], leaf);
    }
    void SortDown(T leaf)
    {
        while (true)
        {
            int leftChild = leaf.BinaryIndex * 2 + 1;
            int rightChild = leaf.BinaryIndex * 2 + 2;
            int swapIndex = 0;
            if(leftChild < leafCount)
            {
                swapIndex = leftChild;

                if(rightChild < leafCount)
                {
                    if (leaves[leftChild].CompareTo(leaves[rightChild]) < 0)
                    {
                        swapIndex = rightChild;
                    }
                }
                if(leaf.CompareTo(leaves[swapIndex])< 0)
                {
                    Swap(leaf, leaves[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
    void SortUp(T leaf)
    {
        int parentIndex = (leaf.BinaryIndex-1) / 2;
        while (true)
        {
            T parentLeaf = leaves[parentIndex];
            if(leaf.CompareTo(parentLeaf) > 0)
            {
                Swap(leaf, parentLeaf);
            }
            else
            {
                break;
            }
            parentIndex = (leaf.BinaryIndex - 1) / 2;
        }
    }
    void Swap(T leafA, T leafB)
    {
        leaves[leafA.BinaryIndex] = leafB;
        leaves[leafB.BinaryIndex] = leafA;
        int leafAIndex = leafA.BinaryIndex;
        leafA.BinaryIndex = leafB.BinaryIndex;
        leafB.BinaryIndex = leafAIndex;
    }
}

public interface IBinaryTree<T> : IComparable<T>
{
    int BinaryIndex { get; set; }
}
