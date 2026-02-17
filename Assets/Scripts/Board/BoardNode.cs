
using System;
using System.Collections.Generic;
using EventBus;
using UnityEngine;
using Registry;

public class BoardNode : MonoBehaviour, IGridNode
{
    private Grid<BoardNode> grid;
    public Vector2Int Coords => grid.Find(this); 
    private BoardPiece piece;
    public BoardPiece Piece => piece;
    [SerializeField] private Transform pieceAnchor;
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
            if (BoardNodeModifier.Instance.ActiveNode != this) return;
            piece.AddAttribute(new NeutralizingAttribute(piece));
            PieceUpdated();
        }
    }

    void OnSelectBindingEvent(SelectBoardNodeEvent boardNodeEvent)
    {
        if (boardNodeEvent.selectedNode != this) return;
        BoardNodeModifier.Instance.SetActiveNode(this); 
    }
    public void Init(Grid<BoardNode> grid)
    {
        this.grid = grid;
    }

    void PieceUpdated()
    {
        onPieceUpdate?.Invoke(piece);
    }
    public void AddPiece(BoardPiece incomingPiece)
    {
        if (piece != null)
        {
            if (incomingPiece.PlayerOwner == piece.PlayerOwner)
                piece.ChangeCharge(incomingPiece.Charge);
        }
        else
        {
            piece = incomingPiece;
        }

        foreach (var attribute in incomingPiece.Attributes)
            attribute.OnEnterNode(this); 
        
        onPieceUpdate?.Invoke(piece);
    }
    public BoardPiece TakePiece()
    {
        if (piece == null) return null;
        BoardPiece returnPiece = piece;
        piece = null;
        PieceUpdated();
        return returnPiece;
    }
    public BoardPiece TakeCharge(int amt)
    {
        BoardPiece returnPiece; 
        if (piece == null) return null;
        if (piece.Charge <= amt)
        {
            returnPiece = piece;
            piece = null;
            PieceUpdated();
            return returnPiece;
        }
        returnPiece = piece.Pop(amt);
        PieceUpdated();
        return returnPiece; 
    }

    public void TranslatePiece(Vector2Int direction, PieceConflictResolver resolver)
    {
        if (piece == null)
            return;
        
        Vector2Int currentNodeCoords = grid.Find(this);
        Vector2Int newNodeCoords = currentNodeCoords + direction; 
        
        BoardNode newNode = grid.Get(newNodeCoords);
        if (newNode == null)
        {
            TakePiece(); 
            return;
        }

        if (newNode.IsOccupied())
        {
            if (newNode.Piece.PlayerOwner != piece.PlayerOwner)
            {
                PieceConflictResolver usedResolver = piece.Resolver ?? newNode.piece.Resolver ?? resolver;
                usedResolver.ResolveConflict(this, newNode, direction, grid);
                return;
            }
        }

        BoardPiece returnPiece = TakePiece();
        if (returnPiece == null)
            return;

        PieceUpdated();
        newNode.AddPiece(returnPiece);
    }
    public void TranslateCharge(Vector2Int direction, PieceConflictResolver resolver, int amt = 1)
    {
        if (piece == null)
            return;
        
        if (amt >= piece.Charge)
        {
            TranslatePiece(direction, resolver);
            return;
        }
        
        Vector2Int currentNodeCoords = grid.Find(this);
        Vector2Int newNodeCoords = currentNodeCoords + direction;  
        
        BoardNode newNode = grid.Get(newNodeCoords);
        if (newNode == null)
            return;
        foreach (var attribute in piece.Attributes)
            attribute.OnExitNode(this); 
        
        if (newNode.IsOccupied())
        {
            if (newNode.Piece.PlayerOwner != piece.PlayerOwner)
            {
                resolver.ResolveConflict(this, newNode, direction, grid);
                return;
            }
        }

        BoardPiece returnPiece = TakeCharge(amt); 
        if (returnPiece == null)
            return;
        
        PieceUpdated();
        newNode.AddPiece(returnPiece);
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