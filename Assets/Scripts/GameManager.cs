
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using EventBus;
using ScriptableVariables;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityUtils;

public class GameManager : Singleton<GameManager>
{
    private bool gameStarted;
    public bool GameStarted => gameStarted;

    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private EndGame endgame; 
    
    [SerializeField] private IntVariable machineQueuedMoves;
    [SerializeField] private MachineBehavior machine;
    [SerializeField] private Board board;
    [SerializeField] private BoardActor player;
    [SerializeField] private BoardBot opponent; 
    
    private const int maxLoseCount = 50;
    [SerializeField] private IntVariable points;
    [SerializeField, Range(-maxLoseCount, -1)] private int pointsToLose;
    [SerializeField, Range(1, maxLoseCount)] private int pointsToWin;
    
    
    private void Awake()
    {
        points.Value = 0; 
        machine ??= FindFirstObjectByType<MachineBehavior>();
        gameStarted = false; 
    }

    [SerializeField] private PlayableDirector introDirector; 
    [SerializeField] private GameObject gameCameras;
    [SerializeField] private GameObject title;
    [SerializeField] private AudioSource introMusic;
    [SerializeField] private AudioSource gameMusic; 
    private void Update()
    {
        if (Input.anyKeyDown && !gameStarted) StartGame();
    }

    public async void StartGame()
    {
        introDirector.Stop(); 
        gameCameras.SetActive(true);
        title.SetActive(false);
        introMusic.Stop();
        gameMusic.Play();
        
        await UniTask.Delay(TimeSpan.FromSeconds(4f));
        
        if (gameStarted) return;
        gameStarted = true;
        
        StartPlayerTurn();
    }
    

    private Grid<BoardNode> gridSnapshot; 
    void StartPlayerTurn()
    {
        gridSnapshot = board.Grid.Copy(); 
        
        player.onTurnEnd += PlayerEndTurn;
        
        TriggerTurnEvent(GameTurnEvent.ActorType.Board, GameTurnEvent.TurnType.Start);
        player.StartTurn(board.Grid);
        TriggerTurnEvent(GameTurnEvent.ActorType.Player, GameTurnEvent.TurnType.Start);
    }
    void PlayerEndTurn()
    {
        TriggerTurnEvent(GameTurnEvent.ActorType.Player, GameTurnEvent.TurnType.End);
        player.onTurnEnd -= PlayerEndTurn;
        StartOpponentTurn();
    }
    void StartOpponentTurn()
    {
        TriggerTurnEvent(GameTurnEvent.ActorType.Opponent, GameTurnEvent.TurnType.Start);
        opponent.onTurnEnd += TriggerEndOpponentTurn;
        opponent.StartTurn(gridSnapshot);
    }
    void TriggerEndOpponentTurn() => OpponentEndTurn();
    
    async UniTaskVoid OpponentEndTurn()
    {
        float delay = .35f;

        await opponent.PostPlayerTurnActions(board.Grid);
        TriggerTurnEvent(GameTurnEvent.ActorType.Player, GameTurnEvent.TurnType.End);
        opponent.onTurnEnd -= TriggerEndOpponentTurn;
        
        
        TriggerTurnEvent(GameTurnEvent.ActorType.Player, GameTurnEvent.TurnType.ShiftStart);
        ShiftPlayersPieces();
        await UniTask.WaitForSeconds(delay);
        await opponent.PostPlayerPushActions(board.Grid); 
        
        TriggerTurnEvent(GameTurnEvent.ActorType.Player, GameTurnEvent.TurnType.ShiftEnd);
        await UniTask.WaitForSeconds(delay); 
        
        
        TriggerTurnEvent(GameTurnEvent.ActorType.Opponent, GameTurnEvent.TurnType.ShiftStart);
        ShiftOpponentPieces();
        TriggerTurnEvent(GameTurnEvent.ActorType.Opponent, GameTurnEvent.TurnType.ShiftEnd);
        TriggerTurnEvent(GameTurnEvent.ActorType.Board, GameTurnEvent.TurnType.End);
        
        if (machineQueuedMoves.Value != 0)
        {
            await UniTask.WaitForSeconds(1f);
            await machine.Move();
            
            if (points.Value >= pointsToWin)
            {
                endgame.GameWon();;
                return;
            }

            if (points.Value <= pointsToLose)
            {
                endgame.GameLost();
                return;
            }
        } 
        
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
    

    private void TriggerTurnEvent(GameTurnEvent.ActorType actorType, GameTurnEvent.TurnType turnType) =>
        EventBus<GameTurnEvent>.Raise(new GameTurnEvent()
        {
            actorType = actorType,
            turnType = turnType
        });
}


public struct GameTurnEvent : IEvent
{
    public enum TurnType { Start, End, ShiftStart, ShiftEnd }
    public enum ActorType { Board, Player, Opponent }
    public ActorType actorType;
    public TurnType turnType;
}