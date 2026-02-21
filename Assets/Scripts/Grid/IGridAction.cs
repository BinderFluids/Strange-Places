using UnityEngine;

public interface IGridAction<T> where T : IGridNode
{
    void Execute(Vector2Int activeCoords, Grid<T> ctx);
    void Undo(); 
}