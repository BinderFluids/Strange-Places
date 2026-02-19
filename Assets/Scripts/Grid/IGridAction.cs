using UnityEngine;

public interface IGridAction<T> where T : IGridNode
{
    void Execute(Vector2Int activeCoords, Grid<T> ctx);
    void Undo(); 
}

public abstract class BoardAction : IGridAction<BoardNode>
{
    protected Vector2Int activeCoords; 
    
    public void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        if (!ctx.TryGet(activeCoords, out _)) return;
        Execute(activeCoords, ctx);
    }
    public abstract void Execute(BoardNode active, Grid<BoardNode> ctx);
    public abstract void Undo(); 
}