using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    //�Ƿ����ƶ�
    public bool walkable;
    //�ڵ������λ��
    public Vector3 worldPosition;
    //�ڵ����������������е�λ��
    public int gridX;
    public int gridY;

    public Node parent;
    //gCost�����ľ���;hCost���յ�ľ��� ;fCostѰ·����
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }
    //�ڶ��ڵ�����
    int heapIndex;
    public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

    //��������ڵ㣬�������Ƿ��ܾ���������λ�ã����������ά�����x,y
    public Node(bool _walkable, Vector3 _worldPo, int _gridX, int _gridY)
    {
        this.walkable = _walkable;
        this.worldPosition = _worldPo;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }
    //���ڱȽ������ڵ�� fCost
    //����Ϊ����˵���ýڵ�����ȼ����� nodeToCompare �����ȼ������ýڵ�� fCost ֵ��С��
    public int CompareTo(Node nodeToCompare)
    {
        //��� compare < 0����˵������ڵ�� fCost С�� nodeToCompare �� fCost
        //��� compare = 0����˵������ڵ�� fCost ���� nodeToCompare �� fCost
        //��� compare > 0����˵������ڵ�� fCost ���� nodeToCompare �� fCost
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
