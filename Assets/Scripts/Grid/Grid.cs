
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[System.Serializable]
public class Grid<T> where T : IGridNode
{
    public delegate void GridItemsStrategy(T item);
    private T[,] grid;
    
    public int Width => grid.GetLength(0);
    public int Height => grid.GetLength(1);
    public Grid(int width, int height)
    {
        grid = new T[width, height];
    }
    private Stack<IGridAction<T>> actionStack = new();
    private HashSet<IGridNode> dirtyNodes = new(); 
    

    private bool InBounds(int x, int y) => (0 <= x && x < grid.GetLength(0) && 0 <= y && y < grid.GetLength(1));
    private bool IsOccupied(int x, int y) => grid[x, y] != null;

    public bool IsDirty(IGridNode node)
    {
        return dirtyNodes.Contains(node); 
    }
    public void SetDirty(IGridNode node)
    {
        dirtyNodes.Add(node); 
    }

    void CleanNodes() => dirtyNodes.Clear(); 
    
    public void ForEach(GridItemsStrategy strategy)
    {
        for (int y = Height - 1; y >= 0; y--)
        for (int x = 0; x < Width; x++)
        {
            T item = grid[x, y];
            if (item == null) continue;
            if (IsDirty(item)) continue;

            strategy(item);
        }
        CleanNodes();
    }

    public void ExecuteGridAction(T item, IGridAction<T> action)
    {
        actionStack.Push(action);
        action.Execute(item, this); 
    }
    public void Undo()
    {
        if (actionStack.Count < 1) return;
        actionStack.Pop().Undo();
    }
    
    public T Get(int x, int y)
    {
        if (InBounds(x, y) && IsOccupied(x, y))
        {
            return grid[x, y];
        }
        return default;
    }
    public T Get(Vector2Int pos) => Get(pos.x, pos.y);
    
    public bool Set(int x, int y, T item)
    {
        if (InBounds(x, y) && !IsOccupied(x, y))
        {
            grid[x, y] = item;
            return true;
        }
        return false;
    }
    public bool Clear(int x, int y) => Set(x, y, default);
    
    public Vector2Int Find(T node)
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                T current = Get(x, y);
                if (current == null) continue;
                if (node.Equals(current)) return new Vector2Int(x, y);
            }
        }

        Debug.LogWarning("Didn't find node, returning zero");
        return Vector2Int.zero; 
    }
}