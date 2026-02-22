
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
    
    private void Awake()
    {
        binding = new EventBinding<SelectBoardNodeEvent>(OnSelectBoardNodeEvent);
        EventBus<SelectBoardNodeEvent>.Register(binding);
    }

    void OnSelectBoardNodeEvent(SelectBoardNodeEvent e)
    {
        if (playerActionsLeft.Value <= 0)
        {
            HoverOff();
            return;
        }

        if (e.selectedNode != nodeMonobehavior)
        {
            HoverOff();
            return;
        }

        if (!e.selectedNode.Node.IsOccupied() && e.selectedNode.Node.Coords.y > playerReach.Value - 1)
        {
            HoverOff();
            return;
        }
        if (e.selectedNode.Node.IsOccupied())
            if (e.selectedNode.Node.Piece.Owner is BoardBot) return;
        {
            HoverOff();
        }
        
        if (e.type == SelectBoardNodeEvent.Type.HoverOn)
        {
            HoverOn(e);
        }
        else if (e.type == SelectBoardNodeEvent.Type.HoverOff)
        {
            HoverOff();
        }
    }

    void HoverOn(SelectBoardNodeEvent e)
    {
        light.SetActive(true); 
        
        Vector2Int coords = e.selectedNode.Node.Coords;
        
        if (e.selectedNode.Node.Coords.y < playerReach.Value)
            moveUp.SetActive(true);

        if (!e.selectedNode.Node.IsOccupied()) return; 
        if (coords.x != 1)
            moveLeft.SetActive(true);
        if (coords.x == 1 && coords.y == 2)
            moveRight.SetActive(true);
        if (coords.x != 3)
            moveRight.SetActive(true);
        if (coords.x == 3 && coords.y == 2)
            moveLeft.SetActive(true);
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