using UnityEngine;

public class ShiftBoard : BoardActionChain
{
    private Vector2Int direction;
    private BoardActor targetOwner;
    private GridItemsOrder order;
    
    public ShiftBoard(Vector2Int direction, BoardActor targetOwner, GridItemsOrder order)
    {
        this.direction = direction;
        this.targetOwner = targetOwner; 
        this.order = order;
    }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        ctx.ForEach(node =>
        {
            if (node == null) return;
            if (!node.IsOccupied()) return;
            if (node.Piece.Owner != targetOwner) return;
            
            TranslatePiece translation = new TranslatePiece(direction);
            Chain(translation, node.Coords, ctx);
        }, order);
    }
}