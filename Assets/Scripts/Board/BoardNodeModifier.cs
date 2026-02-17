
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class BoardNodeModifier : MonoBehaviour
{
    public static BoardNodeModifier Instance;
    
    [SerializeField] private BoardPlayer boardPlayer;
    [SerializeField] private BoardNode activeNode;

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
    }
    
    private void Update()
    {
        if (activeNode == null) return;
        
        
        if (Input.GetKeyDown(KeyCode.W))
            activeNode.AddPiece(new BoardPiece(boardPlayer));
        if (Input.GetKeyDown(KeyCode.S))
            activeNode.TakeCharge(1);
        if (Input.GetKeyDown(KeyCode.A))
            activeNode.TranslateCharge(Vector2Int.left, new PushOtherPiece());
        if (Input.GetKeyDown(KeyCode.D))
            activeNode.TranslateCharge(Vector2Int.right, new PushOtherPiece());
            
    }

    public void Reset()
    {
        activeNode = null;
    }
}