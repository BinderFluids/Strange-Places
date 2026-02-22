using System;
using System.Collections.Generic;
using EventBus;
using UnityEngine;

public class BoardModifier
{
    private BoardNodeMonobehavior activeNode;
    public BoardNodeMonobehavior ActiveNode => activeNode;
    private EventBinding<SelectBoardNodeEvent> selectBinding;
    private EventBinding<AddAttributeToBoardModifier> addAttributeBinding;
    private EventBinding<GameTurnEvent> turnEventBinding;
    private HashSet<BoardNode> dirtyNodes = new(); //TODO: when giving a node a charge, make it so that node can not also have a charge translated out of it
    private HashSet<Type> attributes = new();
    
    public BoardModifier()
    {
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBindingEvent);
        EventBus<SelectBoardNodeEvent>.Register(selectBinding);
        
        addAttributeBinding = new EventBinding<AddAttributeToBoardModifier>(OnAddAttributeToBoardModifier);
        EventBus<AddAttributeToBoardModifier>.Register(addAttributeBinding);
        
        turnEventBinding = new EventBinding<GameTurnEvent>(OnGameTurnEvent);
        EventBus<GameTurnEvent>.Register(turnEventBinding);
    }

    void OnGameTurnEvent(GameTurnEvent e)
    {
        if (e is
            {
                actorType: GameTurnEvent.ActorType.Board,
                turnType: GameTurnEvent.TurnType.End
            }) ;
        attributes.Clear();
    }
    
    void OnSelectBindingEvent(SelectBoardNodeEvent e)
    {
        if (e.selectedNode.Node.IsOccupied())
        {
            if (e.selectedNode.Node.Piece.Owner is BoardPlayer)
                activeNode = e.selectedNode;
        }
        else
        {
            activeNode = e.selectedNode;
        }
    }
    
    public void Update(BoardPlayer actor)
    {
        if (activeNode == null) return;
        
        Board.Instance.StartObservingAction();
        Vector2Int activeCoords = activeNode.Node.Coords;
        var grid = Board.Instance.Grid;

        bool inReach = false;
        if (actor.Reach > 0)
            inReach = activeNode.Node.Coords.y < actor.Reach; 

        
        if (Input.GetKeyDown(KeyCode.A) && activeNode.Node.IsOccupied())
        {
            if (grid.TryGet(activeCoords.x - 1, activeCoords.y, out BoardNode leftNode))
            {
                if (leftNode is not NullBoardNode)
                    actor.UseAction(activeCoords, new TranslatePiece<Neutralize>(Vector2Int.left, 1));
            }
        }
        if (Input.GetKeyDown(KeyCode.D) && activeNode.Node.IsOccupied())
        {
            if (grid.TryGet(activeCoords.x + 1, activeCoords.y, out BoardNode leftNode))
            {
                if (leftNode is not NullBoardNode)
                    actor.UseAction(activeCoords, new TranslatePiece<Neutralize>(Vector2Int.right, 1));
            }
        }

        if (!inReach) return;
        if (Input.GetKeyDown(KeyCode.W))
        {
            BoardAction.doDebug = true; 
            BoardPiece newPiece = new BoardPiece(actor, 1, CreateNewAttributes());
            GivePiece givePiece = new GivePiece(newPiece);
            Debug.Log($"New Piece: {newPiece}");
            actor.UseAction(activeCoords, givePiece);
            BoardAction.doDebug = false;
        }

            
        Board.Instance.StopObservingAction();
    }
    
    void OnAddAttributeToBoardModifier(AddAttributeToBoardModifier e) 
    {
        Debug.Log($"Adding attribute {e.attributeType.Name}");
        attributes.Add(e.attributeType);
    }
    HashSet<BoardPieceAttribute> CreateNewAttributes()
    {
        HashSet<BoardPieceAttribute> newAttributes = new();
        foreach (Type attributeType in attributes)
            newAttributes.Add(BoardPieceAttribute.Create(attributeType, activeNode.Node.Piece));
        return newAttributes;
    }
}

public struct AddAttributeToBoardModifier : IEvent 
{
    public Type attributeType;
}