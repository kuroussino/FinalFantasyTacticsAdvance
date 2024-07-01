using System;

public class PriorityNode<T>
{
    #region Variables & Properties

    #region Local
    T data;
    float priority;
    float heuristic;

    public T Data => data;
    public float Priority => priority;
    public float Heuristic => heuristic;
    #endregion

    #region Public
    #endregion

    #endregion

    #region Monobehaviour
    public PriorityNode(T data, float priority, float heuristic = 0)
    {
        this.data = data;
        this.priority = priority;
        this.heuristic = heuristic;
    }
    #endregion

    #region Methods
    public static bool operator <(PriorityNode<T> a, PriorityNode<T> b) => a.priority < b.priority ? true : a.priority > b.priority ? false : a.heuristic > b.heuristic;
    public static bool operator >(PriorityNode<T> a, PriorityNode<T> b) => a.priority > b.priority ? true : a.priority < b.priority ? false : a.heuristic < b.heuristic;
    public static bool operator <=(PriorityNode<T> a, PriorityNode<T> b) => a < b || a == b;
    public static bool operator >=(PriorityNode<T> a, PriorityNode<T> b) => a > b || a == b;
    public static bool operator ==(PriorityNode<T> a, PriorityNode<T> b) => (a.priority == b.priority && a.heuristic == b.heuristic);
    public static bool operator !=(PriorityNode<T> a, PriorityNode<T> b) => !(a == b);

    public override bool Equals(object obj)
    {
        PriorityNode<T> PriorityNode = obj as PriorityNode<T>;
        if (PriorityNode == null)
            return false;

        return this == PriorityNode;
    }

    public override int GetHashCode() => HashCode.Combine(data, priority);
    #endregion
}
