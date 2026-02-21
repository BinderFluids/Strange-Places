using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TakePiece : BoardAction
{
    private bool doDebug = false;
    private int charge;
    private Vector2Int activeCoords;
    private BoardPiece takenPiece;
    public BoardPiece TakenPiece => takenPiece;
    private Grid<BoardNode> ctx;
    
    public TakePiece(int charge = 0)
    {
        this.charge = charge;
    }
    
    public override void Execute(Vector2Int activeCoords, Grid<BoardNode> ctx)
    {
        this.ctx = ctx;
        this.activeCoords = activeCoords;
        if (!ctx.TryGet(activeCoords, out BoardNode active)) return;
        
        takenPiece = new BoardPiece(active.TakePiece(charge));
        if (doDebug) Debug.Log($"Took Piece {takenPiece} from {activeCoords}");
        ctx.UpdateNodes();
    }

    protected override void OnUndo()
    {
        GivePiece givePiece = new GivePiece(takenPiece);
        givePiece.Execute(activeCoords, ctx);
        ctx.UpdateNodes();
    }
}