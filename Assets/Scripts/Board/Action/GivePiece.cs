using System.Collections.Generic;
using UnityEngine;

public class GivePiece : IGridAction<BoardNode>
{
    private BoardNode activeNode;
    private BoardPiece incomingPiece;
    private Grid<BoardNode> ctx;
    private List<IGridAction<BoardNode>> giveAttributeActions = new();
    
    public GivePiece(BoardPiece incomingPiece)
    {
        this.incomingPiece = incomingPiece;
    }

    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        this.ctx = ctx;
        activeNode = active;
        
        if (active.IsOccupied())
            foreach (BoardPieceAttribute attribute in incomingPiece.Attributes)
            {
                Debug.Log($"Looping {attribute.GetType()}");
                var newAddAttributeAction = CreateAddAttributeAction(attribute);
                newAddAttributeAction.Execute(activeNode, ctx);
            }
        active.AddPiece(incomingPiece);
        

        Debug.Log($"Gave Piece {incomingPiece} to {activeNode.Coords}");
    }

    public void Undo()
    {
        foreach (IGridAction<BoardNode> attributeAction in giveAttributeActions)
            attributeAction.Undo();
        
        TakePiece takePiece = new TakePiece(incomingPiece.Charge);
        
        takePiece.Execute(activeNode, ctx);
    }
    
    AddAttribute CreateAddAttributeAction(BoardPieceAttribute attribute)
    {
        Debug.Log($"Creating Add Attribute {attribute.GetType()}");
        var addAttributeAction = new AddAttribute(attribute.GetType());
        giveAttributeActions.Add(addAttributeAction);
        return addAttributeAction;
    }
}