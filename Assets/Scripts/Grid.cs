using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //ͼ��
    public LayerMask unwalkableMask;
    //���������С
    public Vector2 gridWorldSize;
    //ÿ������ڵ�����ĵ��߽�ľ���
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    
    private void Start()
    {
        //�����������������
        nodeDiameter = nodeRadius * 2;
        //X�������������
        gridSizeX = Mathf.RoundToInt( gridWorldSize.x / nodeDiameter);
        //Y�������������
        gridSizeY = Mathf.RoundToInt( gridWorldSize.y / nodeDiameter);
        CreatGrid();
    }

    private void CreatGrid()
    {
        grid=new Node[gridSizeX, gridSizeY];
        //��������½�
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                //�������ǰÿ���������ĵ�λ��
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //��ÿ���������ײ��⣬�жϵ�ǰ�����Ƿ���Ծ���
                //������������ж�Ϊ���ɾ���
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x,y]=new Node(walkable,worldPoint);
            }
        }
    }

    //���ظ��������ڵ�����
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //��������ɽ�worldPosition��x������ƽ����gridWorldSize��һ��
        float percentX =(worldPosition.x+gridWorldSize.x/2)/gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x,y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1f,gridWorldSize.y));
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
