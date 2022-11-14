using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //Test
    public bool displayGizmos;

    //图层
    public LayerMask unwalkableMask;
    //网格世界大小
    public Vector2 gridWorldSize;
    //每个网格节点的中心到边界的距离
    public float nodeRadius;
    Node[,] grid;
    //数组用于存放不同图层的移动惩罚值
    public TerrainType[] walkableRegions;
    //可行走的图层，（目前是在编辑器界面设置）
    LayerMask walkableMask;
    //字典集，用于存放不同图层的移动值（即图层值+惩罚值）
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    //网格边长
    float nodeDiameter;
    //地图X轴的网格数，地图Y轴的网格数
    int gridSizeX, gridSizeY;
    //世界里的网格总数
    public int MaxSize { get { return gridSizeX * gridSizeY; } }

    private void Awake()
    {
        //计算出世界里网格数
        nodeDiameter = nodeRadius * 2;
        //X方向上网格个数
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        //Y方向上网格个数
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        //将每个网格的图层数值加上移动的惩罚值获得图层的移动值；并将其存放于字典集中
        foreach(TerrainType region in walkableRegions)
        {
            walkableMask.value += region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
        }
        CreatGrid();
    }
    //将世界分成X*Y个网格
    private void CreatGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        //世界的左下角
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //计算出当前每个网格中心的位置
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //对每个点进行范围碰撞检测，判断当前网格是否可以经过
                //如果范围内有物体则判断为不可经过
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                //当前网格的移动值
                int movementPenalty = 0;
                //如果是可以行走的网格，则从网格中心上方发出射线获取该网格内的处于 walkableMask 的对象所处的图层（Layer），并以此获取该网格的移动值
                if (walkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                        walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer,out movementPenalty);
                    }
                }
                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
    }

    //返回一个节点的周围的节点的列表
    public List<Node> GetNeighbous(Node node)
    {
        List<Node> neighbous = new List<Node>();
        //以当前节点为中心，查看3*3的网格节点
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //跳过中心节点
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                //如果grid[checkX,checkY]是在世界网格内，则添加进列表
                if ((checkX >= 0 && checkX < gridSizeX) && (checkY >= 0 && checkY < gridSizeY))
                {
                    neighbous.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbous;
    }
    //返回该物体所在的网格
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //可以想象成将worldPosition向x正方向平移了gridWorldSize的一半
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y));
        if (grid != null && displayGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
    //创建一个类用于将移动惩罚值和对应的图层联系起来
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
