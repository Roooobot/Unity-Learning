using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//用于创建小计时器，查看性能如何Test
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    //寻路方法
    public void FindPath(PathRequest request,Action<PathResult>callback)
    {
        //Test
        Stopwatch sw = new();
        sw.Start();
        //用于存放路径节点
        Vector3[] waypoints = new Vector3[0];
        //用于判断是否已经寻路结束
        bool pathSuccess = false;
        //创建开始和结束的网格点
        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);
        //如果开始节点和目标节点都能行走则开始寻路
        if (startNode.walkable && targetNode.walkable)
        {
            //创建两个放置节点的集合，并将第一个放进openSet
            Heap<Node> openSet = new(grid.MaxSize);
            HashSet<Node> closeSet = new();
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closeSet.Add(currentNode);
                //如果当前节点是目标节点则跳出循环
                if (currentNode == targetNode)
                {
                    //Test寻路消耗时间
                    sw.Stop();
                    print("Path  found:" + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
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
                    //再加上当前 neighbour 的移动惩罚值
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    //若当前 neighbour 的 newMovementCostToNeighbour 小于原 gCost ，则修改 gCost
                    //修改到目标节点的距离，修改父节点
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        //如果当前 neighbour 不存在于 openSet 中，则添加进去
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints,pathSuccess,request.callback));
    }

    //返回路径的全部网格节点
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints= SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }
    //简化路径
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints= new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
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
