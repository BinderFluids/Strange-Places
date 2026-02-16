
using System;
using EventBus;
using UnityEngine;
using Registry;
using UnityEditor;

public class BoardNode : MonoBehaviour, IGridNode
{
    private static int count = 0;
    private int id; 
    [SerializeField] private BoardPiece piece;
    [SerializeField] private Transform pieceAnchor;
    private EventBinding<SelectBoardNodeEvent> selectBinding;

    private void Awake()
    {
        id = count; 
        count++;
        Registry<BoardNode>.TryAdd(this); 
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBindingEvent);
        EventBus<SelectBoardNodeEvent>.Register(selectBinding);
    }

    void OnSelectBindingEvent(SelectBoardNodeEvent boardNodeEvent)
    {
        if (boardNodeEvent.selectedNode != this) return;    
    }

    public bool SetPiece(BoardPiece piece)
    {
        if (!IsOccupied())
        {
            this.piece = piece;
            piece.transform.SetParent(pieceAnchor);
            piece.transform.localPosition = Vector3.zero;
            piece.transform.localRotation = Quaternion.identity;
            
            return true;
        }
        return false;
    }
    public BoardPiece TakePiece()
    {
        if (piece == null) return null;
        
        BoardPiece returnPiece = piece; 
        piece = null;
        
        return returnPiece; 
    }
    
    public bool IsOccupied()
    {
        return piece != null; 
    }
    
#if UNITY_EDITOR
    private Color gizmoColor;
    private float gizmoSize;
    public void Init(Color gizmoColor, float gizmoSize)
    {
        this.gizmoColor = gizmoColor;
        this.gizmoSize = gizmoSize;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        
        Vector3 cubeSize = new Vector3(gizmoSize, .01f, gizmoSize);
        Gizmos.DrawCube(transform.position, cubeSize);

    }
#endif
    
    private void OnDestroy()
    {
        Registry<BoardNode>.Remove(this); 
        EventBus<SelectBoardNodeEvent>.Deregister(selectBinding);
    }
}