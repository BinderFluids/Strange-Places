using EventBus;
using ScriptableVariables;
using UnityEngine;

public class EndZoneNode : BoardNode
{
    private EventBinding<GameTurnEvent> turnEventBinding;
    private GameTurnEvent.ActorType targetType;
    public GameTurnEvent.ActorType TargetType => targetType;
    private IPieceOwner targetOwner;
    private IntVariable queuedMachineMoves;
    private int moveDirection;
    
    public EndZoneNode(
        Grid<BoardNode> grid, 
        BoardPiece piece = null
    ) 
        : base(grid, piece) 
    { 
        turnEventBinding = new EventBinding<GameTurnEvent>(OnTurnEvent);
        EventBus<GameTurnEvent>.Register(turnEventBinding);
    }

    public void Init(
        IPieceOwner owner,
        IntVariable queuedMachineMoves,
        int moveDirection
    )
    {
        targetOwner = owner;
        this.targetType = targetType;
        this.queuedMachineMoves = queuedMachineMoves;
        this.moveDirection = moveDirection;
    }

    void OnTurnEvent(GameTurnEvent e)
    {
        if (e is
            {
                actorType: GameTurnEvent.ActorType.Board,
                turnType: GameTurnEvent.TurnType.End
            })
            if (IsOccupied())
                if (piece.Owner == targetOwner)
                {
                    var piece = TakePiece();
                    Debug.Log($"EndZoneNode Took {piece}");
                    grid.UpdateNodes();
                }
    }

    public override void OnBoardEnter()
    {
        if (Piece.Owner != targetOwner) return;
        queuedMachineMoves.Value += moveDirection;
    }
}