using EventBus;

public class GiveItemBoardNode : BoardNode
{
    private EventBinding<GameTurnEvent> turnEventBinding;
    
    public GiveItemBoardNode(Grid<BoardNode> grid, BoardPiece piece = null) : base(grid, piece)
    {
        turnEventBinding = new EventBinding<GameTurnEvent>(OnTurnEvent);
        EventBus<GameTurnEvent>.Register(turnEventBinding);
    }

    void OnTurnEvent(GameTurnEvent e)
    {
        if (e is
            {
                actorType: GameTurnEvent.ActorType.Player,
                turnType: GameTurnEvent.TurnType.End
            })
            if (IsOccupied())
                TakePiece();
    }
    public override void OnBoardEnter()
    {
        Board.Instance.AddSecondaryAction(ItemLootTableGiver.Instance);
        ItemLootTableGiver.Instance.Execute();
    }
}