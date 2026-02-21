
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[Serializable]
public enum GridItemsOrder { TopToBottom, BottomToTop }
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
    public Stack<IGridAction<T>> ActionStack => actionStack;
    
    private HashSet<Vector2Int> dirtyNodes = new(); 
    
    private bool observeAction = false;

    private bool InBounds(int x, int y) => (0 <= x && x < grid.GetLength(0) && 0 <= y && y < grid.GetLength(1));
    private bool IsOccupied(int x, int y) => grid[x, y] != null;

    public bool IsDirty(Vector2Int node)
    {
        return dirtyNodes.Contains(node); 
    }
    public bool IsDirty(T node)
    {
        if (node == null) return false;
        return IsDirty(Find(node));
    }
    
    
    public void SetDirty(Vector2Int node)
    {
        dirtyNodes.Add(node); 
    }

    public void SetDirty(T node)
    {
        if (node == null) throw new NullReferenceException(); 
        SetDirty(Find(node));
    }

    void CleanNodes() => dirtyNodes.Clear(); 
    
    
    public void ForEach(GridItemsStrategy strategy, GridItemsOrder order = GridItemsOrder.BottomToTop)
    {
        CleanNodes();
        
        if (order == GridItemsOrder.TopToBottom)
            for (int y = Height - 1; y >= 0; y--)
            for (int x = 0; x < Width; x++)
                Iterate(x, y, strategy);
        if (order == GridItemsOrder.BottomToTop)
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Iterate(x, y, strategy);
        
        void Iterate(int x, int y, GridItemsStrategy strategy)
        {
            T item = grid[x, y];
            if (item == null) return;
            if (IsDirty(item)) return;
            
            strategy(item);
        }
        
        CleanNodes();
    }
    
    public bool TryGet(int x, int y, out T item)
    {
        if (InBounds(x, y) && IsOccupied(x, y))
        {
            item = grid[x, y];
            return true; 
        }

        item = default;
        return false;
    }
    public bool TryGet(Vector2Int pos, out T item) => TryGet(pos.x, pos.y, out item); 
    
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
                if (!TryGet(x, y, out var current)) continue; 
                if (node.Equals(current)) return new Vector2Int(x, y);
            }
        }

        Debug.LogWarning("Didn't find node, returning zero");
        return Vector2Int.zero; 
    }
    
    public void StartObservingAction()
    {
        observeAction = true; 
    }
    public void StopObservingAction()
    {
        observeAction = false; 
    }
    public void Execute(Vector2Int coords, IGridAction<T> action)
    {
        if (observeAction) actionStack.Push(action);
        action.Execute(coords, this);
    }
    public void Undo()
    {
        StopObservingAction();
        if (actionStack.Count > 0)
        {
            Debug.Log("Undo");
            actionStack.Pop().Undo();
            UpdateNodes();
        }
        StartObservingAction();
    }

    public void UpdateNodes()
    {
        ForEach(node => node.Update());
    }
    
    public Grid<T> Copy()
    {
        var copy = new Grid<T>(Width, Height);

        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
        {
            var item = grid[x, y];
            copy.grid[x, y] = item == null ? default : (T)item.Copy();
        }

        return copy;
    }
}