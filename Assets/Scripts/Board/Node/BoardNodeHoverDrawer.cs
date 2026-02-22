
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

    private void Awake()
    {
        binding = new EventBinding<SelectBoardNodeEvent>(OnSelectBoardNodeEvent);
        EventBus<SelectBoardNodeEvent>.Register(binding);
    }

    void OnSelectBoardNodeEvent(SelectBoardNodeEvent e)
    {
        if (playerActionsLeft.Value <= 0)
        {
            light.SetActive(false);
            return;
        }
        if (e.selectedNode != nodeMonobehavior) return;
        if (!e.selectedNode.Node.IsOccupied() && e.selectedNode.Node.Coords.y > playerReach.Value - 1) return;
        if (e.selectedNode.Node.IsOccupied())
            if (e.selectedNode.Node.Piece.Owner is BoardBot) return;
        
        if (e.type == SelectBoardNodeEvent.Type.HoverOn)
        {
            light.SetActive(true); 
        }
        else if (e.type == SelectBoardNodeEvent.Type.HoverOff)
        {
            light.SetActive(false);
        }
    }
    

    private void OnDestroy()
    {
        EventBus<SelectBoardNodeEvent>.Deregister(binding);
    }
}