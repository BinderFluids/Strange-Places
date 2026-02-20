using System.Collections.Generic;
using EventBus;
using UnityEngine;

public class BoardModifier
{
    private BoardNodeMonobehavior activeNode;
    public BoardNodeMonobehavior ActiveNode => activeNode;
    private EventBinding<SelectBoardNodeEvent> selectBinding;
    private HashSet<BoardNode> dirtyNodes = new(); //TODO: when giving a node a charge, make it so that node can not also have a charge translated out of it
    [SerializeField] private ItemLootTableGiver itemLootTableGiver;
    
    
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
                //TODO: CHECK IF ROW IS 3 AND GIVE ITEM
                if (leftNode is not NullBoardNode)
                    actor.UseAction(activeCoords, new TranslatePiece<Neutralize>(Vector2Int.left, 1));
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (grid.TryGet(activeCoords.x + 1, activeCoords.y, out BoardNode leftNode))
            {
                
                //TODO: CHECK IF ROW IS 3 AND GIVE ITEM
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