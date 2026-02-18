using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class TakePiece : IGridAction<BoardNode>
{
    private int charge;
    private BoardNode activeNode;
    private BoardPiece takenPiece;
    public BoardPiece TakenPiece => takenPiece;
    private Grid<BoardNode> ctx;
    
    public TakePiece(int charge = 0)
    {
        this.charge = charge;
    }
    
    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        activeNode = active;
        this.ctx = ctx;
        
        takenPiece = new BoardPiece(active.TakePiece(charge));
        Debug.Log($"Took Piece {takenPiece} from {activeNode.Coords}");
    }

    public void Undo()
    {
        GivePiece givePiece = new GivePiece(takenPiece);
        givePiece.Execute(activeNode, ctx);
    }
}

public class GivePiece : IGridAction<BoardNode>
{
    private BoardNode activeNode;
    private BoardPiece incomingPiece;
    private Grid<BoardNode> ctx;
    
    public GivePiece(BoardPiece incomingPiece)
    {
        this.incomingPiece = incomingPiece;
    }

    public void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        this.ctx = ctx;
        activeNode = active;
        
        active.AddPiece(incomingPiece);
        Debug.Log($"Gave Piece {incomingPiece} to {activeNode.Coords}");
    }

    public void Undo()
    {
        TakePiece takePiece = new TakePiece(incomingPiece.Charge);
        takePiece.Execute(activeNode, ctx);
    }
}