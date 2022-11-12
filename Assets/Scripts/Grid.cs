using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //Test
    public bool onlyDisplayPathGizmos;

    //ͼ��
    public LayerMask unwalkableMask;
    //���������С
    public Vector2 gridWorldSize;
    //ÿ������ڵ�����ĵ��߽�ľ���
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public int MaxSize { get { return gridSizeX * gridSizeY; } }

    private void Start()
    {
        //�����������������
        nodeDiameter = nodeRadius * 2;
        //X�������������
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        //Y�������������
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreatGrid();
    }
    //������ֳ�X*Y������
    private void CreatGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        //��������½�
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //�������ǰÿ���������ĵ�λ��
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //��ÿ������з�Χ��ײ��⣬�жϵ�ǰ�����Ƿ���Ծ���
                //�����Χ�����������ж�Ϊ���ɾ���
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }
    //����һ���ڵ����Χ�Ľڵ���б�
    public List<Node> GetNeighbous(Node node)
    {
        List<Node> neighbous = new List<Node>();
        //�Ե�ǰ�ڵ�Ϊ���ģ��鿴3*3������ڵ�
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //�������Ľڵ�
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                //���grid[checkX,checkY]�������������ڣ�����ӽ��б�
                if ((checkX >= 0 && checkX < gridSizeX) && (checkY >= 0 && checkY < gridSizeY))
                {
                    neighbous.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbous;
    }
    //���ظ��������ڵ�����
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //��������ɽ�worldPosition��x������ƽ����gridWorldSize��һ��
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y));
        //Test
        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
        else
        {

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null)
                        if (path.Contains(n))
                            Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }

        }
    }
}
