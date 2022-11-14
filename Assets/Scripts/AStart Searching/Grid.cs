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
     int obstacleProximitypenalty = 40;
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

    int penaltyMin =int.MaxValue;
    int penaltyMax =int.MinValue;

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
                //从网格中心上方发出射线获取该网格内的处于 walkableMask 的对象所处的图层（Layer），并以此获取该网格的移动值
                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, walkableMask))
                {
                    walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }
                //如果是不可行走的，将其网格的移动值上调（目的是为了让离障碍物太近也有惩罚）
                if (!walkable)
                {
                    movementPenalty+=obstacleProximitypenalty;
                }
                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        BlurPenaltyMap(3);
    }

    //模糊过渡网格之间移动值的差距,可以让对象更倾向于行走在路中间
    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;
        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for(int y = 0; y < gridSizeY; y++)
        {
            for(int x =-kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x,0,kernelExtents);
                penaltiesHorizontalPass[0, y] += grid[sampleX,y].movementPenalty;
            }
            for(int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                int addIndex = Mathf.Clamp(x+kernelExtents, 0, gridSizeX-1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
            }
        }
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty =Mathf.RoundToInt((float) penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x,y].movementPenalty=blurredPenalty;

                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if(blurredPenalty < penaltyMin)
                {
                    penaltyMin=blurredPenalty;
                }
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
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));

                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
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
