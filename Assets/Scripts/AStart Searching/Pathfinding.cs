using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���ڴ���С��ʱ�����鿴�������Test
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    //Ѱ·����
    public void FindPath(PathRequest request,Action<PathResult>callback)
    {
        //Test
        Stopwatch sw = new();
        sw.Start();
        //���ڴ��·���ڵ�
        Vector3[] waypoints = new Vector3[0];
        //�����ж��Ƿ��Ѿ�Ѱ·����
        bool pathSuccess = false;
        //������ʼ�ͽ����������
        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);
        //�����ʼ�ڵ��Ŀ��ڵ㶼��������ʼѰ·
        if (startNode.walkable && targetNode.walkable)
        {
            //�����������ýڵ�ļ��ϣ�������һ���Ž�openSet
            Heap<Node> openSet = new(grid.MaxSize);
            HashSet<Node> closeSet = new();
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closeSet.Add(currentNode);
                //�����ǰ�ڵ���Ŀ��ڵ�������ѭ��
                if (currentNode == targetNode)
                {
                    //TestѰ·����ʱ��
                    sw.Stop();
                    print("Path  found:" + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }
                //������ǰ�ڵ����Χ�ڵ㣬�����Ծ����Ľڵ�Ž�openSet��
                //�Ѿ��� openSet �� closeSet �Ľڵ����
                foreach (Node neighbour in grid.GetNeighbous(currentNode))
                {
                    //�������ܾ����Ľڵ���Ѿ������� closeSet �Ľڵ�
                    if (!neighbour.walkable || closeSet.Contains(neighbour))
                    {
                        continue;
                    }
                    //��ǰ neighbour �뿪ʼ�ڵ�ľ��루�ӿ�ʼ�ڵ㵽���ڵ㣬�ٵ���ǰ neighbour �ľ��룩
                    //�ټ��ϵ�ǰ neighbour ���ƶ��ͷ�ֵ
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    //����ǰ neighbour �� newMovementCostToNeighbour С��ԭ gCost �����޸� gCost
                    //�޸ĵ�Ŀ��ڵ�ľ��룬�޸ĸ��ڵ�
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        //�����ǰ neighbour �������� openSet �У�����ӽ�ȥ
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

    //����·����ȫ������ڵ�
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
    //��·��
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
    //����A�㵽B��ľ��룻
    //б����λ����Ϊ14��������λ����Ϊ10
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
