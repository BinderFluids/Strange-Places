
using System;
using EventBus;
using UnityEngine;
using Registry;
public class BoardNodeMonobehavior : MonoBehaviour
{
    private BoardNode node;
    public BoardNode Node => node;
    
    private EventBinding<SelectBoardNodeEvent> selectBinding;
    

    private void Start()
    {
        Registry<BoardNodeMonobehavior>.TryAdd(this);    
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBindingEvent);
        EventBus<SelectBoardNodeEvent>.Register(selectBinding);
    }
    public void Init(BoardNode node) => this.node = node;

    void OnSelectBindingEvent(SelectBoardNodeEvent boardNodeEvent)
    {
        if (boardNodeEvent.selectedNode != this) return;
        BoardModifier.Instance.SetActiveNode(this); 
    }
    
    private void OnDestroy()
    {
        Registry<BoardNodeMonobehavior>.Remove(this);
        EventBus<SelectBoardNodeEvent>.Deregister(selectBinding);
    }

    #region GIZMOS  
    private Color gizmoColor;
    private float gizmoSize;
    public void InitGizmos(Color gizmoColor, float gizmoSize)
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
    #endregion
}