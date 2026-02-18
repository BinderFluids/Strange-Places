using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class TakePiece : IGridAction<BoardNode>
{
    private int charge;
    private BoardNode activeNode;
    private BoardPiece takenPiece;
    public BoardPiece TakenPiece => takenPiece;
    
    public TakePiece(int charge = 0)
    {
        this.charge = charge;
    }
    
    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        activeNode = active;
        takenPiece = active.TakePiece(charge); 
        Debug.Log($"Took {takenPiece} from {activeNode}");
    }

    public void Undo()
    {
        activeNode.AddPiece(takenPiece);
    }
}

public class GivePiece : IGridAction<BoardNode>
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
        Debug.Log($"Gave {incomingPiece} to {active}");
    }

    public void Undo()
    {
        activeNode.TakePiece(incomingPiece.Charge); 
    }
}