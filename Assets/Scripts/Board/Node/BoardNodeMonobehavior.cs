
using System;
using EventBus;
using UnityEngine;
using Registry;
public class BoardNodeMonobehavior : MonoBehaviour
{
    [SerializeField] private BoardNodeDrawer drawer;
    public BoardNodeDrawer Drawer => drawer;
    
    private BoardNode node;
    public BoardNode Node => node;
    private EventBinding<SelectBoardNodeEvent> selectBinding;
    

    private void Start()
    {
        Registry<BoardNodeMonobehavior>.TryAdd(this);    
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBindingEvent);
        EventBus<SelectBoardNodeEvent>.Register(selectBinding);
    }
    public void Init(BoardNode node)
    {
        
        this.node = node;
        if (Node is NullBoardNode)
            gameObject.SetActive(false); 
    }

    void OnSelectBindingEvent(SelectBoardNodeEvent boardNodeEvent)
    {
        if (boardNodeEvent.selectedNode != this) return;
    }
    
    private void OnDestroy()
    {
        Registry<BoardNodeMonobehavior>.Remove(this);
        EventBus<SelectBoardNodeEvent>.Deregister(selectBinding);
    }
}