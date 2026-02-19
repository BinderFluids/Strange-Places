using UnityEngine;

public interface IGridAction<T> where T : IGridNode
{
    void Execute(T active, Grid<T> ctx);
    void Undo(); 
}

public abstract class BoardAction : IGridAction<BoardNode>
{
    protected Vector2Int activeCoords; 
    
    public void Execute(Vector2Int coords, Grid<BoardNode> ctx)
    {
        if (ctx.Get(coords) == null) return;
        Execute(ctx.Get(coords), ctx);
    }
    public abstract void Execute(BoardNode active, Grid<BoardNode> ctx);
    public abstract void Undo(); 
}