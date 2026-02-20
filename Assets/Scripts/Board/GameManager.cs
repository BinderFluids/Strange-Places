
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using EventBus;
using ScriptableVariables;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MachineBehavior machine;
    [SerializeField] private Board board;
    [SerializeField] private BoardActor player;
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


    public void StartGame() => StartPlayerTurn();

    private Grid<BoardNode> gridSnapshot; 
    void StartPlayerTurn()
    {
        gridSnapshot = board.Grid.Copy(); 
        
        player.onTurnEnd += PlayerEndTurn;
        player.StartTurn(board.Grid); 
    }
    void PlayerEndTurn()
    {
        player.onTurnEnd -= PlayerEndTurn;
        StartOpponentTurn();
    }
    void StartOpponentTurn()
    {
        opponent.onTurnEnd += TriggerEndOpponentTurn;
        opponent.StartTurn(gridSnapshot);
    }
    void TriggerEndOpponentTurn() => OpponentEndTurn();
    
    async UniTaskVoid OpponentEndTurn()
    {
        float delay = .5f; 
        opponent.onTurnEnd -= TriggerEndOpponentTurn;
        
        ShiftPlayersPieces();
        await UniTask.WaitForSeconds(delay); 
        
        ShiftOpponentPieces();
        await UniTask.WaitForSeconds(delay); 
        
        StartPlayerTurn(); 
    }
    
    private void ShiftPlayersPieces()
    {
        board.StartObservingAction();
        board.Execute(Vector2Int.zero, new ShiftBoard(Vector2Int.up, player, GridItemsOrder.TopToBottom));
        board.StopObservingAction();
    }

    private void ShiftOpponentPieces()
    {
        board.StartObservingAction();
        board.Execute(Vector2Int.zero, new ShiftBoard(Vector2Int.down, opponent, GridItemsOrder.BottomToTop));
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