using EventBus;
using UnityEngine;

public struct BoardPositionEvent : IEvent
{
    public Vector2Int position;
}
