
using System;
using EventBus;
using UnityEngine;

public class BoardNode : IGridNode
{
    protected BoardPiece piece;
    public BoardPiece Piece => piece;

    protected Grid<BoardNode> grid;
    public Vector2Int Coords => grid.Find(this); 
    private EventBinding<SelectBoardNodeEvent> selectBinding;
    public event Action onNodeUpdate =  delegate { };
    
    public BoardNode(Grid<BoardNode> grid, BoardPiece piece = null)
    {
        this.grid = grid; 
        this.piece = piece;
    }
    
    public void Update()
    {
        onNodeUpdate?.Invoke();
    }

    public IGridNode Copy()
    {
        return new BoardNode(grid, piece);
    }

    public BoardNode CopyBoardNode()
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
            if (incomingPiece.Owner == piece.Owner)
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

    public virtual void OnBoardEnter() { }

    // public override bool Equals(object obj)
    // {
    //     if (obj is not BoardNode otherNode) return false;
    //     
    //     bool sameCoords = Coords == otherNode.Coords;
    //     bool samePiece = otherNode.Piece == piece;
    //     return sameCoords && samePiece;
    // }
    // public override int GetHashCode()
            //    // {
            //    //     return HashCode.Combine(Coords, Piece);
            //    // }
}