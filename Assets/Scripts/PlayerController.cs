using System;
using System.Diagnostics.Contracts;
using EventBus;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    [SerializeField] private BoardPlayer boardPlayer;
    [SerializeField] private Transform heldPieceAnchor; 
    
    private EventBinding<SelectBoardNodeEvent> boardNodeSelectBinding;
    

    private void Awake()
    {
        boardNodeSelectBinding = new EventBinding<SelectBoardNodeEvent>(OnBoardNodeSelect);
        EventBus<SelectBoardNodeEvent>.Register(boardNodeSelectBinding);
    }
    
    void OnBoardNodeSelect(SelectBoardNodeEvent boardNodeEvent)
    {
        BoardNode boardNode = boardNodeEvent.selectedNode;
        
    }
}
