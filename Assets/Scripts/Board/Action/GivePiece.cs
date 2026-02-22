using System.Collections.Generic;
using UnityEngine;


public class GivePiece : BoardAction
{
    private Vector2Int activeCoords;
    private BoardPiece incomingPiece;
    private Grid<BoardNode> ctx;
    private List<IGridAction<BoardNode>> giveAttributeActions = new();
    
    public GivePiece(BoardPiece incomingPiece)
    {
        this.incomingPiece = incomingPiece;
    }

    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        this.ctx = ctx;
        this.activeCoords = activeCoords;
        if (!ctx.TryGet(activeCoords, out BoardNode active)) return;
        
        BoardPiece pieceToAdd = new BoardPiece(incomingPiece.Owner, incomingPiece.Charge);

        if (active.IsOccupied())
            TryAddAttributes(active); 
        
        bool wasOccupied = active.IsOccupied();
        active.AddPiece(pieceToAdd);
        if (!wasOccupied) TryAddAttributes(active); 
        
        active.OnBoardEnter();
        ctx.UpdateNodes();
        DoDebug($"Gave Piece {incomingPiece} to {activeCoords}");
    }

    void TryAddAttributes(BoardNode active)
    {
        foreach (BoardPieceAttribute attribute in incomingPiece.Attributes)
        {
            var newAddAttributeAction = CreateAddAttributeAction(attribute);
            newAddAttributeAction.Execute(activeCoords, ctx);
        }
    }

    protected override void OnUndo()
    {
        if (!ctx.TryGet(activeCoords, out BoardNode active)) return; 
        
        foreach (IGridAction<BoardNode> attributeAction in giveAttributeActions)
            attributeAction.Undo();
        
        TakePiece takePiece = new TakePiece(incomingPiece.Charge);
        
        takePiece.Execute(activeCoords, ctx);
        ctx.UpdateNodes();
    }
    
    AddAttribute CreateAddAttributeAction(BoardPieceAttribute attribute)
    {
        var addAttributeAction = new AddAttribute(attribute.GetType());
        giveAttributeActions.Add(addAttributeAction);
        return addAttributeAction;
    }
}