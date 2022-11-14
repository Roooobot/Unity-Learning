using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//最小堆
public class Heap<T> where T : IHeapItem<T>
{
    //泛型数组
    T[] items;
    //当前堆内 item 的个数
    int currentItemCount;
    public int Count { get { return currentItemCount; } }
    //构造函数
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }
    //往堆内添加 item
    public void Add(T item)
    {
        //从0开始给 item 的 HeapIndex 作为索引
        item.HeapIndex = currentItemCount;
        //将 item 放进数组
        items[currentItemCount] = item;
        //排序
        SortUp(item);
        currentItemCount++;
    }
    //返回并移除堆顶 item
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }
    //
    public void UpdateItem(T item)
    {
        SortUp(item);
    }
    //用于判断 item 是否已经在堆内
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }
    //向堆中的父节点进行比较
    //将当前 item 的索引与父 item 的索引进行比较；
    //如果当前 item 的索引小于父 item 的索引则交换
    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else { break; }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    //向堆中的子节点进行比较
    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;
            //在左右子节点中选择较小的用于比较
            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }
                //如果 item 的索引小于子节点的索引则交换
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else { return; }
            }
            else { return; }
        }
    }
    //交换两个 item 在数组中的位置，交换两个 item 的索引
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}
//泛型接口，每个加入堆的 item 都会有一个索引
//有一个 CompareTo() 方法，用于比较
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

