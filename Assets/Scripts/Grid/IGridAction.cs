using UnityEngine;

public interface IGridAction<T> where T : IGridNode
{
    void Execute(Vector2Int activeCoords, Grid<T> ctx);
    void Undo(); 
}

public abstract class BoardAction : IGridAction<BoardNode>
{
    private Vector2Int activeCoords; 
    public Vector2Int ActiveCoords => activeCoords;
    
    public void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        this.activeCoords = activeCoords;
        if (!ctx.TryGet(activeCoords, out _)) return;
        Execute(activeCoords, ctx);
    }
    public abstract void Execute(BoardNode active, Grid<BoardNode> ctx);
    public abstract void Undo(); 
}