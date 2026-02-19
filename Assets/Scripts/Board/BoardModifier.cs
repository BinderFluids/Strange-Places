
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class BoardModifier : MonoBehaviour
{
    public static BoardModifier Instance;
    
    [SerializeField] private BoardPlayer boardPlayer;
    [SerializeField] private BoardNode activeNode;
    public BoardNode ActiveNode => activeNode;

    public void SetBoardPlayer(BoardPlayer boardPlayer)
    {
        this.boardPlayer = boardPlayer;
    }
    
    private void Awake()
    {
        Instance = this;
    }

    public void SetActiveNode(BoardNode activeNode)
    {
        this.activeNode = activeNode;
        if (activeNode.IsOccupied())
            Debug.Log(activeNode.Piece);
    }

    void PlayerDoAction(IGridAction<BoardNode> action) =>
        boardPlayer.DoAction(action, activeNode); 
    
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