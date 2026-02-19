
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class BoardModifier : MonoBehaviour
{
    public static BoardModifier Instance;
    
    [SerializeField] private BoardPlayer boardPlayer;
    [SerializeField] private BoardNodeMonobehavior activeNode;
    public BoardNodeMonobehavior ActiveNode => activeNode;

    public void SetBoardPlayer(BoardPlayer boardPlayer)
    {
        this.boardPlayer = boardPlayer;
    }
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetActiveNode(BoardNodeMonobehavior activeNode)
    {
        this.activeNode = activeNode;
        if (activeNode.Node.IsOccupied())
            Debug.Log(activeNode.Node.Piece);
    }

    void PlayerDoAction(IGridAction<BoardNode> action) =>
        boardPlayer.DoAction(activeNode.Node.Coords, action); 
    
    private void Update()
    {
        if (activeNode == null) return;
        
        Board.Instance.StartObservingAction();
        
        if (Input.GetKeyDown(KeyCode.W))
            PlayerDoAction(new GivePiece(new BoardPiece(boardPlayer)));
            
        if (Input.GetKeyDown(KeyCode.A))
            PlayerDoAction(new TranslatePiece<Neutralize>(Vector2Int.left, 1));
        
        if (Input.GetKeyDown(KeyCode.D))
            PlayerDoAction(new TranslatePiece<Neutralize>(Vector2Int.right, 1));

            
        Board.Instance.StopObservingAction();
    }

    public void Reset()
    {
        activeNode = null;
    }
}