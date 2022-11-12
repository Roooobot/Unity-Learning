using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//���ڴ���С��ʱ�����鿴�������Test
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    //��ʼ�㣬Ŀ���
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

        //������ʼ�ͽ����������
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        //�����������ýڵ�ļ��ϣ�������һ���Ž�openSet
        Heap<Node> openSet = new(grid.MaxSize);
        HashSet<Node> closeSet = new();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            #region �ò���ԭ��ʹ��List��openSetʱʹ��
            /*
            //����openSetѡ��fCost��С�ķŽ�closeSet�У�
            //�����fCost��ͬ�Ľڵ㣬ѡ��hCost��С�ģ���ѡ����ͬ�ڵ��о���Ŀ���Ͻ��ĵ㣩��
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
            //�����ǰ�ڵ���Ŀ��ڵ������
            if (currentNode == targetNode)
            {
                //Test
                sw.Stop();
                print("Path  found:" + sw.ElapsedMilliseconds + "ms");

                RetracePath(startNode, targetNode);
                return;
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
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                //����ǰ neighbour �� newMovementCostToNeighbour С��ԭ gCost �����޸� gCost
                //�޸ĵ�Ŀ��ڵ�ľ��룬�޸ĸ��ڵ�
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                }
                //�����ǰ neighbour �������� openSet �У�����ӽ�ȥ
                if (!openSet.Contains(neighbour))
                    openSet.Add(neighbour);
            }
        }
    }
    //����·����ȫ������ڵ�
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
