using System;
using EventBus;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    enum BoardInteractionState { None, Take, Place }
    [SerializeField] private BoardInteractionState boardInteractionState;
    [SerializeField] private BoardPiece heldPiece;
    [SerializeField] private Transform heldPieceAnchor; 
    private bool hasPiece => heldPiece != null;
    
    private EventBinding<SelectBoardNodeEvent> boardNodeSelectBinding;
    

    private void Awake()
    {
        boardNodeSelectBinding = new EventBinding<SelectBoardNodeEvent>(OnBoardNodeSelect);
        EventBus<SelectBoardNodeEvent>.Register(boardNodeSelectBinding);
    }
    
    void OnBoardNodeSelect(SelectBoardNodeEvent boardNodeEvent)
    {
        if (hasPiece)
            HandlePlacePiece(boardNodeEvent);
        else
            HandleTakePiece(boardNodeEvent);
    }
    
    void HandleTakePiece(SelectBoardNodeEvent boardNodeEvent)
    {
        if (heldPiece != null) return;
        if (!boardNodeEvent.selectedNode.IsOccupied()) return;
        
        BoardPiece piece = boardNodeEvent.selectedNode.TakePiece();
        
        piece.transform.SetParent(heldPieceAnchor);
        piece.transform.localPosition = Vector3.zero;
        piece.transform.localRotation = Quaternion.identity;
        heldPiece = piece;
    }
    
    void HandlePlacePiece(SelectBoardNodeEvent boardNodeEvent)
    {
        if (heldPiece == null) return;
        bool couldSetPiece = boardNodeEvent.selectedNode.SetPiece(heldPiece);
        if (couldSetPiece)
            heldPiece = null;
    }
}
