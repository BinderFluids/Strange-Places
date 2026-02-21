using System;
using UnityEngine;

public class AddAttribute : BoardAction
{
    private Vector2Int activeCoords;
    private Type attributeType;
    private bool addedAttribute; 
    private Grid<BoardNode> ctx;
    
    public AddAttribute(Type attributeType)
    {
        this.attributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));
    }
    
    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        if (!ctx.TryGet(activeCoords, out BoardNode active)) return;
        
        this.activeCoords = activeCoords;
        this.ctx = ctx; 
        
        BoardPieceAttribute newAttribute = BoardPieceAttribute.Create(attributeType, active.Piece);
        addedAttribute = active.Piece.TryAddAttribute(newAttribute);
    }

    protected override void OnUndo()
    { 
        if (addedAttribute)
            if (ctx.TryGet(activeCoords, out BoardNode activeNode))
                activeNode.Piece.RemoveAttribute(attributeType);
    }
}