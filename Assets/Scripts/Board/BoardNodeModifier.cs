
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class BoardNodeModifier : MonoBehaviour
{
    public static BoardNodeModifier Instance;
    
    [SerializeField] private Board board;
    [SerializeField] private BoardPlayer boardPlayer;
    [SerializeField] private BoardNode activeNode;

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
            activeNode.AddPiece(new BoardPiece());
        if (Input.GetKeyDown(KeyCode.S))
            activeNode.TakeCharge(1);
        if (Input.GetKeyDown(KeyCode.A))
            activeNode.TranslateCharge(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D))
            activeNode.TranslateCharge(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.Space))
            activeNode.TranslatePiece(Vector2Int.up);
            
    }

    public void Reset()
    {
        activeNode = null;
    }
}