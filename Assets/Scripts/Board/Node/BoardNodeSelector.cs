using EventBus;
using UnityEngine;
using Registry; 
public class BoardNodeSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask = ~0;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private Board board;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }
    public void UpdateSelect(BoardActor actor)
    {
        if (!GameManager.Instance.GameStarted) return;
        if (cam == null) return;

        if (!Input.GetMouseButtonDown(0)) return; 
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask, QueryTriggerInteraction.Ignore)) return; 
        
        BoardNodeMonobehavior selectedNode = Registry<BoardNodeMonobehavior>.Get(new Closest(Mathf.Sqrt(0.5f), hit.point));
        
        if (selectedNode == null) return;
        
        EventBus<SelectBoardNodeEvent>.Raise(new SelectBoardNodeEvent
        {
            selectedNode = selectedNode,
            BoardActor = actor,
        });
    }
}

public struct SelectBoardNodeEvent : IEvent
{
    public BoardNodeMonobehavior selectedNode;
    public BoardActor BoardActor;
    public BoardPiece boardPiece;
}