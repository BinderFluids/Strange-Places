using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class TakePiece : IGridAction<BoardNode>
{
    private int charge;
    private BoardNode activeNode;
    private BoardPiece takenPiece;
    public BoardPiece TakenPiece => takenPiece;
    private int id; 
    
    public TakePiece(int charge = 0)
    {
        this.charge = charge;
        id = Random.Range(0, 1000000);
    }
    
    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        activeNode = active;
        takenPiece = new BoardPiece(active.TakePiece(charge));
        Debug.Log($"Execute: takenPiece.Charge={takenPiece.Charge}");
        
    }

    public void Undo()
    {
        Debug.Log($"{id}: Undone taken pice for take piece was {takenPiece}");
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
        Debug.Log($"Gave {incomingPiece} to {active.Coords}");
    }

    public void Undo()
    {
        Debug.Log($"Undone give piece for charge {incomingPiece.Charge}");
        activeNode.TakePiece(incomingPiece.Charge); 
    }
}