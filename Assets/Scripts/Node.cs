using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    //�Ƿ����ƶ�
    public bool walkable;
    //�ڵ������λ��
    public Vector3 worldPosition;

    public Node(bool _walkable, Vector3 _worldPo)
    {
        this.walkable = _walkable;
        this.worldPosition = _worldPo;
    }
}
