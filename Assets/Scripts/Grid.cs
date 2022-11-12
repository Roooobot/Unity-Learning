using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //图层
    public LayerMask unwalkableMask;
    //网格世界大小
    public Vector2 gridWorldSize;
    //每个网格节点的中心到边界的距离
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    
    private void Start()
    {
        //计算出世界里网格数
        nodeDiameter = nodeRadius * 2;
        //X方向上网格个数
        gridSizeX = Mathf.RoundToInt( gridWorldSize.x / nodeDiameter);
        //Y方向上网格个数
        gridSizeY = Mathf.RoundToInt( gridWorldSize.y / nodeDiameter);
        CreatGrid();
    }

    private void CreatGrid()
    {
        grid=new Node[gridSizeX, gridSizeY];
        //世界的左下角
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                //计算出当前每个网格中心的位置
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //对每个点进行碰撞检测，判断当前网格是否可以经过
                //如果有物体则判断为不可经过
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x,y]=new Node(walkable,worldPoint);
            }
        }
    }

    //返回该物体所在的网格
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //可以想象成将worldPosition向x正方向平移了gridWorldSize的一半
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
