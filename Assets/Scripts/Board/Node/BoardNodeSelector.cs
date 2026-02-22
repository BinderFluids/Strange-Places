using EventBus;
using UnityEngine;
using Registry; 
public class BoardNodeSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask = ~0;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private Board board;

    private BoardNodeMonobehavior _previouslyHoveredNode;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }
    
    public void UpdateSelect(BoardActor actor)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        BoardNodeMonobehavior selectedNode = null;
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask, QueryTriggerInteraction.Ignore))
        {
            selectedNode = Registry<BoardNodeMonobehavior>.Get(new Closest(Mathf.Sqrt(0.5f), hit.point));
        }
        
        // Check if hover state changed
        if (selectedNode != _previouslyHoveredNode)
        {
            // Raise HoverOff event for previously hovered node
            if (_previouslyHoveredNode != null)
            {
                EventBus<SelectBoardNodeEvent>.Raise(new SelectBoardNodeEvent
                {
                    selectedNode = _previouslyHoveredNode,
                    type = SelectBoardNodeEvent.Type.HoverOff
                });
            }
            
            // Raise HoverOn event for newly hovered node
            if (selectedNode != null)
            {
                EventBus<SelectBoardNodeEvent>.Raise(new SelectBoardNodeEvent
                {
                    selectedNode = selectedNode,
                    type = SelectBoardNodeEvent.Type.HoverOn
                });
            }
            
            _previouslyHoveredNode = selectedNode;
        }
    
        if (selectedNode == null) return;
        
        if (!Input.GetMouseButtonDown(0)) return; 
        EventBus<SelectBoardNodeEvent>.Raise(new SelectBoardNodeEvent
        {
            selectedNode = selectedNode,
            type = SelectBoardNodeEvent.Type.Select
        });
    }
}

public struct SelectBoardNodeEvent : IEvent
{
    public enum Type {HoverOn, HoverOff, Select}
    public Type type;
    public BoardNodeMonobehavior selectedNode;
}