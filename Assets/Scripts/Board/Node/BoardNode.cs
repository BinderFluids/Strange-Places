
using System;
using EventBus;
using UnityEngine;
using Registry;
using UnityEngine.UIElements;

public class BoardNode : MonoBehaviour, IGridNode
{
    [SerializeField] private Transform pieceAnchor;
    private BoardPiece piece;
    public BoardPiece Piece => piece;

    private Grid<BoardNode> grid;
    public Vector2Int Coords => grid.Find(this); 
    private EventBinding<SelectBoardNodeEvent> selectBinding;
    public event Action<BoardPiece> onPieceUpdate =  delegate { };

    
    
    private void Awake()
    {
        Registry<BoardNode>.TryAdd(this); 
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBindingEvent);
        EventBus<SelectBoardNodeEvent>.Register(selectBinding);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (piece == null) return;
            if (BoardModifier.Instance.ActiveNode != this) return;
            piece.AddAttribute(new NeutralizingAttribute(piece));
            PieceUpdated();
        }
    }

    void OnSelectBindingEvent(SelectBoardNodeEvent boardNodeEvent)
    {
        if (boardNodeEvent.selectedNode != this) return;
        BoardModifier.Instance.SetActiveNode(this); 
    }
    public void Init(Grid<BoardNode> grid)
    {
        this.grid = grid;
    }

    public void PieceUpdated()
    {
        onPieceUpdate?.Invoke(piece);
    }

    public BoardPiece TakePiece(int amt = 0)
    {
        BoardPiece returnPiece = piece.Pop(amt);
        if (piece.Charge == 0) piece = null;
        
        PieceUpdated();
        return returnPiece;
    }
    public void AddPiece(BoardPiece incomingPiece)
    {
        if (piece != null)
        {
            Debug.Log($"Add {incomingPiece} to {Coords}");
            if (incomingPiece.PlayerOwner == piece.PlayerOwner)
                piece.Assimilate(incomingPiece);
        }
        else
        {
            piece = incomingPiece;
        }
        
        PieceUpdated();
    }
    
    
    public bool IsOccupied()
    {
        return piece != null;
    }
    
#if UNITY_EDITOR
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
#endif
    
    private void OnDestroy()
    {
        Registry<BoardNode>.Remove(this); 
        EventBus<SelectBoardNodeEvent>.Deregister(selectBinding);
    }
}