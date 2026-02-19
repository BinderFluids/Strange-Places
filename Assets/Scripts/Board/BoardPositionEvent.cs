using EventBus;
using UnityEngine;

public struct BoardPositionEvent : IEvent
{
    public BoardPiece piece;
    public Vector2Int position;
}
