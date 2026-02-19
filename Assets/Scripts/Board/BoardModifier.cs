
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
    }
    
    private void Update()
    {
        if (activeNode == null) return;


        Board.Instance.StartObservingAction();
        if (Input.GetKeyDown(KeyCode.W))
            Board.Instance.DoAction(new GivePiece(new BoardPiece(boardPlayer)), activeNode);
        
        if (Input.GetKeyDown(KeyCode.S))
            Board.Instance.DoAction(new TakePiece(1), activeNode);
            
        if (Input.GetKeyDown(KeyCode.A))
            Board.Instance.DoAction(new TranslatePiece<Neutralize>(Vector2Int.left, 1), activeNode);
        
        if (Input.GetKeyDown(KeyCode.D))
            Board.Instance.DoAction(new TranslatePiece<Neutralize>(Vector2Int.right, 1), activeNode);

        if (Input.GetKeyDown(KeyCode.Q))
            Board.Instance.DoAction(new AddAttribute(typeof(NeutralizingAttribute)), activeNode); 
            
        Board.Instance.StopObservingAction();
            
    }

    public void Reset()
    {
        activeNode = null;
    }
}