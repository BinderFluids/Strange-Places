
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
        Vector2Int activeCoords = activeNode.Node.Coords;
        var grid = Board.Instance.Grid;

        bool inReach = false;
        if (boardPlayer.Reach > 0)
            inReach = activeNode.Node.Coords.y < boardPlayer.Reach; 
        // if (boardPlayer.Reach < 0)
        //     inReach = activeNode.Node.Coords.y > 




        if (Input.GetKeyDown(KeyCode.A))
        {
            if (grid.TryGet(activeCoords.x - 1, activeCoords.y, out BoardNode leftNode))
            {
                if (leftNode is not NullBoardNode)
                    PlayerDoAction(new TranslatePiece<Neutralize>(Vector2Int.left, 1));
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (grid.TryGet(activeCoords.x + 1, activeCoords.y, out BoardNode leftNode))
            {
                if (leftNode is not NullBoardNode)
                    PlayerDoAction(new TranslatePiece<Neutralize>(Vector2Int.right, 1));
            }
        }

        if (!inReach) return; 
        if (Input.GetKeyDown(KeyCode.W))
            PlayerDoAction(new GivePiece(new BoardPiece(boardPlayer)));

            
        Board.Instance.StopObservingAction();
    }

    public void Reset()
    {
        activeNode = null;
    }
}