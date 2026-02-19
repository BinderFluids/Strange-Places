using System;
using UnityEngine;

public class AddAttribute : IGridAction<BoardNode> 
{
    private BoardNode activeNode;
    private Type attributeType;
    private bool addedAttribute; 
    
    public AddAttribute(Type attributeType)
    {
        this.attributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));
    }
    
    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        activeNode = active;
        BoardPieceAttribute newAttribute = BoardPieceAttribute.Create(attributeType, active.Piece);
        addedAttribute = active.Piece.TryAddAttribute(newAttribute);
        Debug.Log($"Added Attribute {attributeType} to {activeNode.Coords} : {addedAttribute}");
    }

    public void Undo()
    {
        Debug.Log($"Undoing Add Attribute {attributeType} to {activeNode.Coords} : {addedAttribute}");
        if (addedAttribute)
            activeNode.Piece.RemoveAttribute(attributeType);
    }
}