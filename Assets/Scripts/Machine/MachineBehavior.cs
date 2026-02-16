using System;
using EventBus;
using PrimeTween;
using ScriptableVariables;
using UnityEngine;

public class MachineBehavior : MonoBehaviour
{
    [SerializeField] private IntVariable points;
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveDuration;

    [SerializeField] private Transform _transform;
    private EventBinding<BoardPositionEvent> boardPositionEventBinding;
    private Tween movementTween;

    public event Action onMoveComplete;
    
    private void Awake()
    {
        _transform ??= GetComponent<Transform>();
        boardPositionEventBinding = new EventBinding<BoardPositionEvent>(OnBoardPositionEvent);
        EventBus<BoardPositionEvent>.Register(boardPositionEventBinding);
    }

    private void OnBoardPositionEvent(BoardPositionEvent boardPositionEvent)
    {
        if (movementTween.isAlive) return;
        
        int moveDir = 0; 
        switch (boardPositionEvent.piece.pieceType)
        {
            case BoardPiece.PieceType.Opponent:
                moveDir = -1;
                break;
            case BoardPiece.PieceType.Player:
                moveDir = 1; 
                break;
            default:
                break;
        }   
        float moveVector = moveDistance * moveDir;
        points.Value += moveDir;
        
        movementTween = Tween
            .PositionX(_transform, _transform.position.x + moveVector, moveDuration)
            .OnComplete(onMoveComplete);
    }

    private void OnDestroy()
    {
        EventBus<BoardPositionEvent>.Deregister(boardPositionEventBinding);
    }
}
