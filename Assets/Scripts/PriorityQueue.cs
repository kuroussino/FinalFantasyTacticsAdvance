using UnityEngine;

public class PriorityQueue<T>
{
    #region Variables & Properties

    #region Local
    const int baseHeapSize = 100;


    PriorityNode<T>[] heap;
    int count;

    public int Count => count;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    public PriorityQueue()
    {
        heap = new PriorityNode<T>[baseHeapSize];
        count = 0;
    }
    #endregion

    #region Methods
    public void Enqueue(T data, float priority, float heuristic = 0)
    {
        PriorityNode<T> node = new PriorityNode<T>(data, priority, heuristic);

        if (count == heap.Length)
            ResizeQueue();

        heap[count] = node;
        count++;

        SiftUp(count - 1);
    }

    public T Dequeue()
    {
        PriorityNode<T> node;

        if (count == 0)
            throw new System.Exception("Priority Queue is currently empty!");
        else
        {
            node = heap[0];
            heap[0] = heap[count - 1];
            count--;

            if (count > 0)
                SiftDown(0);
        }

        heap[count] = null;
        return node.Data;
    }

    private void SiftUp(int index)
    {
        if (index != 0)
        {
            int parentIndex = GetParentIndex(index);
            if (heap[parentIndex] > heap[index])
            {
                PriorityNode<T> tmp = heap[parentIndex];
                heap[parentIndex] = heap[index];
                heap[index] = tmp;

                SiftUp(parentIndex);
            }
        }
    }

    private void SiftDown(int index)
    {
        int leftIndex, rightIndex, toBeCheckedIndex;
        PriorityNode<T> tmp;

        leftIndex = GetLeftChildIndex(index);
        rightIndex = GetRightChildIndex(index);

        if (rightIndex >= count)
        {
            if (leftIndex >= count)
                return;
            else
                toBeCheckedIndex = leftIndex;
        }
        else
        {
            if (heap[leftIndex] <= heap[rightIndex])
                toBeCheckedIndex = leftIndex;
            else
                toBeCheckedIndex = rightIndex;
        }

        if (heap[index] > heap[toBeCheckedIndex])
        {
            tmp = heap[toBeCheckedIndex];
            heap[toBeCheckedIndex] = heap[index];
            heap[index] = tmp;

            SiftDown(toBeCheckedIndex);
        }
    }

    private void ResizeQueue()
    {
        PriorityNode<T>[] newHeap = new PriorityNode<T>[2 * count];

        for (int i = 0; i < heap.Length; i++)
            newHeap[i] = heap[i];

        heap = newHeap;
    }

    private int GetParentIndex(int index)
    {
        return (index - 1) / 2;
    }

    private int GetLeftChildIndex(int index)
    {
        return (2 * index) + 1;
    }

    private int GetRightChildIndex(int index)
    {
        return (2 * index) + 2;
    }
    #endregion
}
