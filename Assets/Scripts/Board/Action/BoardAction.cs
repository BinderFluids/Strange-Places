using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class TakePiece : IGridAction<BoardNode>
{
    private int charge;
    private BoardNode activeNode;
    private BoardPiece takenPiece;
    
    public TakePiece(int charge = 0)
    {
        this.charge = charge;
    }
    
    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        activeNode = active;
        takenPiece = active.TakePiece(charge); 
    }

    public void Undo()
    {
        activeNode.AddPiece(takenPiece);
    }
}

public class GivePiece<T> : IGridAction<BoardNode> where T : BoardConflictResolver
{
    private BoardNode activeNode;
    private BoardPiece incomingPiece;
    
    public GivePiece(BoardPiece incomingPiece)
    {
        this.incomingPiece = incomingPiece;
    }

    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        activeNode = active;
        active.AddPiece(incomingPiece);
    }

    public void Undo()
    {
        activeNode.TakePiece(incomingPiece.Charge); 
    }
}
public class GivePiece : GivePiece<None>
{
    public GivePiece(BoardPiece incomingPiece) : base(incomingPiece) { }
}