using EventBus;
using UnityEngine;

public class BoardModifier
{
    private BoardNodeMonobehavior activeNode;
    public BoardNodeMonobehavior ActiveNode => activeNode;
    private EventBinding<SelectBoardNodeEvent> selectBinding;


    public BoardModifier()
    {
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBindingEvent);
        EventBus<SelectBoardNodeEvent>.Register(selectBinding);
    }

    void OnSelectBindingEvent(SelectBoardNodeEvent boardNodeEvent)
    {
        activeNode = boardNodeEvent.selectedNode;
        if (activeNode.Node.IsOccupied())
            Debug.Log(activeNode.Node.Piece);
    }

    
    public void Update(BoardActor actor)
    {
        if (activeNode == null) return;
        
        Board.Instance.StartObservingAction();
        Vector2Int activeCoords = activeNode.Node.Coords;
        var grid = Board.Instance.Grid;

        bool inReach = false;
        if (actor.Reach > 0)
            inReach = activeNode.Node.Coords.y < actor.Reach; 
        // if (boardPlayer.Reach < 0)
        //     inReach = activeNode.Node.Coords.y > 

        
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (grid.TryGet(activeCoords.x - 1, activeCoords.y, out BoardNode leftNode))
            {
                if (leftNode is not NullBoardNode)
                    actor.UseAction(activeCoords, new TranslatePiece<Neutralize>(Vector2Int.left, 1));
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (grid.TryGet(activeCoords.x + 1, activeCoords.y, out BoardNode leftNode))
            {
                if (leftNode is not NullBoardNode)
                    actor.UseAction(activeCoords, new TranslatePiece<Neutralize>(Vector2Int.right, 1));
            }
        }

        if (!inReach) return; 
        if (Input.GetKeyDown(KeyCode.W))
            actor.UseAction(activeCoords, new GivePiece(new BoardPiece(actor)));

            
        Board.Instance.StopObservingAction();
    }
}