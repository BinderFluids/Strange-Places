
using UnityEngine;
using EventBus;

public class BoardManager : MonoBehaviour
{
    private Board board;
    
    private void Awake()
    {
        board = new Board();
    }

    public void PieceReachEnd(BoardPiece piece)
    {
        EventBus<BoardPositionEvent>.Raise(new BoardPositionEvent()
        {
            piece = piece,
            position = Vector2Int.zero
        });
    }
    
    
}