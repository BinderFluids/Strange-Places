
using System;
using EventBus;
using UnityEngine;
using Registry;

public class BoardNode : MonoBehaviour, IGridNode
{
    [SerializeField] private BoardPiece piece;
    private EventBinding<SelectBoardNodeEvent> selectBinding;

    private void Awake()
    {
        Registry<BoardNode>.TryAdd(this); 
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBindingEvent);
        EventBus<SelectBoardNodeEvent>.Register(selectBinding);
    }

    void OnSelectBindingEvent(SelectBoardNodeEvent boardNodeEvent)
    {
        if (boardNodeEvent.selectedNode != this) return;
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