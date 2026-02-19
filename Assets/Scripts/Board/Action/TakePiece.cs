using System.ComponentModel.Design;
using System.Linq;
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