using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    //是否能移动
    public bool walkable;
    //节点的世界位置
    public Vector3 worldPosition;
    //节点在世界网格数组中的位置
    public int gridX;
    public int gridY;

    public Node parent;
    //gCost离起点的距离;hCost离终点的距离 ;fCost寻路消耗
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    //在堆内的索引
    int heapIndex;
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    //创建网格节点，参数：是否能经过，世界位置，世界网格二维数组的x,y
    public Node(bool _walkable, Vector3 _worldPo, int _gridX, int _gridY)
    {
        this.walkable = _walkable;
        this.worldPosition = _worldPo;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }
    //用于比较两个节点的 fCost
    //返回为正则说明该节点的优先级大于 nodeToCompare 的优先级（即该节点的 fCost 值较小）
    public int CompareTo(Node nodeToCompare)
    {
        //如果 compare < 0，则说明这个节点的 fCost 小于 nodeToCompare 的 fCost
        //如果 compare = 0，则说明这个节点的 fCost 等于 nodeToCompare 的 fCost
        //如果 compare > 0，则说明这个节点的 fCost 大于 nodeToCompare 的 fCost
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
