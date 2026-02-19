using UnityEngine;

public class None : BoardConflictResolver
{
    public None(BoardNode otherNode, Vector2Int direction, int charge) : base(otherNode, direction, charge) { }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        
    }
}