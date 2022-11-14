using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//��С��
public class Heap<T> where T : IHeapItem<T>
{
    //��������
    T[] items;
    //��ǰ���� item �ĸ���
    int currentItemCount;
    public int Count { get { return currentItemCount; } }
    //���캯��
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }
    //��������� item
    public void Add(T item)
    {
        //��0��ʼ�� item �� HeapIndex ��Ϊ����
        item.HeapIndex = currentItemCount;
        //�� item �Ž�����
        items[currentItemCount] = item;
        //����
        SortUp(item);
        currentItemCount++;
    }
    //���ز��Ƴ��Ѷ� item
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
    //�����ж� item �Ƿ��Ѿ��ڶ���
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }
    //����еĸ��ڵ���бȽ�
    //����ǰ item �������븸 item ���������бȽϣ�
    //�����ǰ item ������С�ڸ� item �������򽻻�
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
    //����е��ӽڵ���бȽ�
    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;
            //�������ӽڵ���ѡ���С�����ڱȽ�
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
                //��� item ������С���ӽڵ�������򽻻�
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else { return; }
            }
            else { return; }
        }
    }
    //�������� item �������е�λ�ã��������� item ������
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}
//���ͽӿڣ�ÿ������ѵ� item ������һ������
//��һ�� CompareTo() ���������ڱȽ�
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}

