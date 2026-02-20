
using System;
using UnityEngine;
using EventBus;
using ScriptableVariables;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MachineBehavior machine;
    [SerializeField] private Board board;
    [SerializeField] private BoardActor actor;
    [SerializeField] private BoardActor opponent; 
    
    private const int maxLoseCount = 50;
    [SerializeField] private IntVariable points;
    [SerializeField, Range(-maxLoseCount, -1)] private int pointsToLose;
    [SerializeField, Range(1, maxLoseCount)] private int pointsToWin;

    private void Awake()
    {
        points.Value = 0; 
        
        machine ??= FindFirstObjectByType<MachineBehavior>();
        machine.onMoveComplete += OnMachineMoveComplete;
    }

    public void StartTurn(BoardActor actor)
    {
        actor.onTurnEnd += PlayerEndTurn;
        actor.StartTurn(board.Grid); 
    }

    void PlayerEndTurn()
    {
        actor.onTurnEnd -= PlayerEndTurn;
        ShiftPlayersPieces(); 
        StartTurn(actor); 
    }
    
    private void ShiftPlayersPieces()
    {
        board.StartObservingAction();
        board.Execute(new ShiftBoard(Vector2Int.up, actor), Vector2Int.zero);
        board.StopObservingAction();
    }
    
    public void PieceReachEnd(BoardPiece piece)
    {  
        EventBus<BoardPositionEvent>.Raise(new BoardPositionEvent()
        {
            piece = piece,
            position = Vector2Int.zero
        });
    }

    private void OnMachineMoveComplete()
    {
        if (points.Value >= pointsToWin)
            Debug.Log("You Win!");
        else if (points.Value <= pointsToLose)
            Debug.Log("You Lose!");
    }

    private void OnDestroy()
    {
        machine.onMoveComplete -= OnMachineMoveComplete;
    }
}