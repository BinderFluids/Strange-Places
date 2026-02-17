using EventBus;
using UnityEngine;
using Registry; 
public class BoardNodeSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask = ~0;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private BoardPlayer boardPlayer;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (cam == null) return;

        if (!Input.GetMouseButtonDown(0)) return; 
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, mask, QueryTriggerInteraction.Ignore)) return; 
        
        BoardNode selectedNode = Registry<BoardNode>.Get(new Closest(Mathf.Sqrt(0.5f), hit.point));
        if (selectedNode == null) return; 
        
        EventBus<SelectBoardNodeEvent>.Raise(new SelectBoardNodeEvent
        {
            selectedNode = selectedNode,
            boardPlayer = boardPlayer,
            //boardPiece = boardPiecePrefab
        });
    }
    
    
}

public struct SelectBoardNodeEvent : IEvent
{
    public BoardNode selectedNode;
    public BoardPlayer boardPlayer;
    public BoardPiece boardPiece;
}