using EventBus;
using NUnit.Framework;
using ScriptableVariables;

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
        GameTurnEvent.ActorType targetType,
        IntVariable queuedMachineMoves,
        [Range(0, 1)] int moveDirection
    )
    {
        targetOwner = owner;
        this.targetType = targetType;
        this.queuedMachineMoves = queuedMachineMoves;
        this.moveDirection = moveDirection;
    }

    void OnTurnEvent(GameTurnEvent e)
    {
        if (e.actorType == targetType && e.turnType == GameTurnEvent.TurnType.ShiftEnd)
            if (IsOccupied())
                if (piece.Owner == targetOwner)
                    TakePiece();
    }

    public override void OnBoardEnter()
    {
        if (Piece.Owner != targetOwner) return;
        queuedMachineMoves.Value += moveDirection;
    }
}