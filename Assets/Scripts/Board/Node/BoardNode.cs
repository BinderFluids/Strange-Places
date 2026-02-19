
using System;
using EventBus;
using UnityEngine;
using Registry;
using UnityEngine.UIElements;

public class BoardNode : IGridNode
{
    protected BoardPiece piece;
    public BoardPiece Piece => piece;

    private Grid<BoardNode> grid;
    public Vector2Int Coords => grid.Find(this); 
    private EventBinding<SelectBoardNodeEvent> selectBinding;
    public event Action<BoardPiece> onPieceUpdate =  delegate { };
    
    public BoardNode(Grid<BoardNode> grid, BoardPiece piece = null)
    {
        this.grid = grid; 
        this.piece = piece;
    }
    
    public void Update()
    {
        onPieceUpdate?.Invoke(piece);
    }

    public IGridNode Copy()
    {
        return new BoardNode(grid, piece);
    }

    public BoardPiece TakePiece(int amt = 0)
    {
        BoardPiece returnPiece = piece.Pop(amt);
        if (piece.Charge == 0) piece = null;
        
        Update();
        return returnPiece;
    }
    public void AddPiece(BoardPiece incomingPiece)
    {
        if (piece != null)
        {
            if (incomingPiece.PlayerOwner == piece.PlayerOwner)
                piece.Assimilate(incomingPiece);
        }
        else
        {
            piece = incomingPiece;
        }
        
        Update();
    }
    
    public bool IsOccupied()
    {
        return piece != null;
    }
}