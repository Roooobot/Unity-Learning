using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//用于创建小计时器，查看性能如何Test
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    //开始点，目标点
    public Transform seeker, target;
    Grid grid;
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
        FindPath(seeker.position, target.position);
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Test
        Stopwatch sw = new();
        sw.Start();

        //创建开始和结束的网格点
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        //创建两个放置节点的集合，并将第一个放进openSet
        Heap<Node> openSet = new(grid.MaxSize);
        HashSet<Node> closeSet = new();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            #region 该部分原来使用List做openSet时使用
            /*
            //遍历openSet选出fCost最小的放进closeSet中；
            //如果有fCost相同的节点，选择hCost较小的（即选择相同节点中距离目标点较近的点）；
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
             */
            #endregion
            closeSet.Add(currentNode);
            //如果当前节点是目标节点则结束
            if (currentNode == targetNode)
            {
                //Test
                sw.Stop();
                print("Path  found:" + sw.ElapsedMilliseconds + "ms");

                RetracePath(startNode, targetNode);
                return;
            }
            //遍历当前节点的周围节点，将可以经过的节点放进openSet中
            //已经在 openSet 或 closeSet 的节点除外
            foreach (Node neighbour in grid.GetNeighbous(currentNode))
            {
                //跳过不能经过的节点和已经存在于 closeSet 的节点
                if (!neighbour.walkable || closeSet.Contains(neighbour))
                {
                    continue;
                }
                //当前 neighbour 与开始节点的距离（从开始节点到父节点，再到当前 neighbour 的距离）
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                //若当前 neighbour 的 newMovementCostToNeighbour 小于原 gCost ，则修改 gCost
                //修改到目标节点的距离，修改父节点
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                }
                //如果当前 neighbour 不存在于 openSet 中，则添加进去
                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
            }
        }
    }
    //返回路径的全部网格节点
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }

    //返回A点到B点的距离；
    //斜方向单位距离为14，正方向单位距离为10
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
