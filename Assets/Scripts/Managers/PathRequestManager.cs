using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//寻路请求管理器，用于处理寻路请求，并将其拆分为多个帧获取路径，避免计算量过大导致卡顿
public class PathRequestManager : Singleton<PathRequestManager>
{
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[],bool> callback;

        public PathRequest(Vector3 _start,Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }

    Queue<PathRequest> pathRequestQueue = new();
    PathRequest currentPathRequest;
    Pathfinding pathfinding;
    bool isProcessingPath;

    protected override void Awake()
    {
        base.Awake();
        pathfinding = GetComponent<Pathfinding>();
}
    //发起寻路请求
    public static void RequestPath(Vector3 pathStart,Vector3 pathEnd, Action<Vector3[],bool> callback )
    {
        PathRequest newRequest = new(pathStart, pathEnd, callback);
        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }
    //获取剩下部分的寻路请求
    private void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }
    //结束寻路
    public void FinishedProcessingPath(Vector3[]path,bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath=false;
        TryProcessNext();
    }
}
