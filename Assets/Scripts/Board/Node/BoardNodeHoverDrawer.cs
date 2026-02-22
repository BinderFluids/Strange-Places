
using System;
using EventBus;
using ScriptableVariables;
using UnityEngine;

public class BoardNodeHoverDrawer : MonoBehaviour
{
    EventBinding<SelectBoardNodeEvent> binding;
    [SerializeField] private IntVariable playerReach;
    [SerializeField] private IntVariable playerActionsLeft; 
    [SerializeField] private BoardNodeMonobehavior nodeMonobehavior;
    [SerializeField] private GameObject light;
    [SerializeField] private GameObject moveRight;
    [SerializeField] private GameObject moveLeft;
    [SerializeField] private GameObject moveUp;
    [SerializeField] private Vector2Int nodeCoords;
    
    private void Awake()
    {
        binding = new EventBinding<SelectBoardNodeEvent>(OnSelectBoardNodeEvent);
        EventBus<SelectBoardNodeEvent>.Register(binding);
    }

    void OnSelectBoardNodeEvent(SelectBoardNodeEvent e)
    {
        //not my ndoe
        if (e.selectedNode != nodeMonobehavior)
        {
            HoverOff();
            return; 
        }
        
        
        //hover off obv
        if (e.type == SelectBoardNodeEvent.Type.HoverOff)
            HoverOff();
        //no actions
        if (playerActionsLeft.Value <= 0)
            return;
        //too far forward and no piece
        if (!e.selectedNode.Node.IsOccupied() && e.selectedNode.Node.Coords.y > playerReach.Value - 1)
            return;
        if (e.selectedNode.Node.IsOccupied())
            if (e.selectedNode.Node.Piece.Owner is BoardBot)
                return;
        
        if (e.type == SelectBoardNodeEvent.Type.HoverOn) HoverOn(e);
    }

    void HoverOn(SelectBoardNodeEvent e)
    {
        light.SetActive(true); 
        nodeCoords = e.selectedNode.Node.Coords;
        
        Vector2Int coords = e.selectedNode.Node.Coords;
        
        if (e.selectedNode.Node.Coords.y < playerReach.Value)
            moveUp.SetActive(true);

        if (!e.selectedNode.Node.IsOccupied()) return; 
        if (coords.x != 1)
            moveLeft.SetActive(true);
        if (coords is { x: 1, y: 2} )
            moveLeft.SetActive(true);
        if (coords.x != 3)
            moveRight.SetActive(true);
        if (coords.x == 3 && coords.y == 2)
            moveRight.SetActive(true);
    }

    void HoverOff()
    {
        light.SetActive(false); 
        moveRight.SetActive(false);
        moveLeft.SetActive(false);
        moveUp.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus<SelectBoardNodeEvent>.Deregister(binding);
    }
}