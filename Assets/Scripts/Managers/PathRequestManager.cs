using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//寻路请求管理器，用于处理寻路请求，并将其拆分为多个帧获取路径，避免计算量过大导致卡顿
public class PathRequestManager : Singleton<PathRequestManager>
{
    Queue<PathResult> results = new();
    Pathfinding pathfinding;

    protected override void Awake()
    {
        base.Awake();
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Update()
    {
        if(results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for(int i=0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    //发起寻路请求
    public static void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate
        {
            Instance.pathfinding.FindPath(request, Instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }
    //结束寻路
    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }
    }
}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}


public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}
