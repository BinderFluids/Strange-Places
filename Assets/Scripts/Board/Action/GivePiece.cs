using System.Collections.Generic;
using UnityEngine;

public class GivePiece : IGridAction<BoardNode>
{
    private bool doDebug = false; 
    private Vector2Int activeCoords;
    private BoardPiece incomingPiece;
    private Grid<BoardNode> ctx;
    private List<IGridAction<BoardNode>> giveAttributeActions = new();
    
    public GivePiece(BoardPiece incomingPiece)
    {
        this.incomingPiece = incomingPiece;
    }

    public void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        this.ctx = ctx;
        this.activeCoords = activeCoords;
        if (!ctx.TryGet(activeCoords, out BoardNode active)) return;
        
        if (active.IsOccupied())
            foreach (BoardPieceAttribute attribute in incomingPiece.Attributes)
            {
                var newAddAttributeAction = CreateAddAttributeAction(attribute);
                newAddAttributeAction.Execute(activeCoords, ctx);
            }
        active.AddPiece(incomingPiece);
        

        if (doDebug) Debug.Log($"Gave Piece {incomingPiece} to {activeCoords}");
    }

    public void Undo()
    {
        if (!ctx.TryGet(activeCoords, out BoardNode active)) return; 
        
        foreach (IGridAction<BoardNode> attributeAction in giveAttributeActions)
            attributeAction.Undo();
        
        TakePiece takePiece = new TakePiece(incomingPiece.Charge);
        
        takePiece.Execute(activeCoords, ctx);
    }
    
    AddAttribute CreateAddAttributeAction(BoardPieceAttribute attribute)
    {
        var addAttributeAction = new AddAttribute(attribute.GetType());
        giveAttributeActions.Add(addAttributeAction);
        return addAttributeAction;
    }
}