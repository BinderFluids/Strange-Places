using System;
using EventBus;
using PrimeTween;
using ScriptableVariables;
using UnityEditor.Callbacks;
using UnityEngine;

public class MachineBehavior : MonoBehaviour
{
    [SerializeField] private BoardPlayer player; 
    [SerializeField] private BoardPlayer opponent; 
    [SerializeField] private IntVariable points; 
    [SerializeField] private IntVariable movementCharge;
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
        
        BoardPiece piece = boardPositionEvent.piece;
        int moveDir = 0; 
        if (player == (BoardPlayer)piece.PlayerOwner) moveDir = 1;
        if (opponent == (BoardPlayer)piece.PlayerOwner) moveDir = -1;
            
        float moveVector = moveDistance * moveDir;
        points.Value += movementCharge.Value * moveDir; 
        
        movementTween = Tween
            .PositionX(_transform, _transform.position.x + moveVector, moveDuration)
            .OnComplete(OnMoveComplete);
    }

    void OnMoveComplete()
    {
        movementCharge.Value = 1; 
        onMoveComplete?.Invoke();
    }

    private void OnDestroy()
    {
        EventBus<BoardPositionEvent>.Deregister(boardPositionEventBinding);
    }
}
