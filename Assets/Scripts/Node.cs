using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    //是否能移动
    public bool walkable;
    //节点的世界位置
    public Vector3 worldPosition;

    public Node(bool _walkable, Vector3 _worldPo)
    {
        this.walkable = _walkable;
        this.worldPosition = _worldPo;
    }
}
