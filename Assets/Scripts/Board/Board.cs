using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardPlayer player;
    [SerializeField] private BoardPlayer opponent; 
    [SerializeField] private BoardNode boardNodePrefab;
    [SerializeField] private Transform boardNodeContainer;
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private float gridCellSize;
    [Min(1), SerializeField] private int gridWidth;
    [Min(1), SerializeField] private int gridHeight;
    
    private Grid<BoardNode> grid;
    public Grid<BoardNode> Grid => grid;
    
    private void Awake()
    {
        grid = new Grid<BoardNode>(gridWidth, gridHeight);
        int colorIndex = 0;
        Color[] colors =
        {
            Color.red,
            Color.blue
        };
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                BoardNode node = Instantiate(boardNodePrefab, transform);
                grid.Set(x, y, node);
                node.Init(grid);
#if UNITY_EDITOR
                node.InitGizmos(colors[colorIndex % 2], gridCellSize);
#endif
                
                Vector3 boardOffsetLocal = new Vector3(x * gridCellSize, 0f, y * gridCellSize);
                Vector3 squareOffsetLocal = new Vector3(gridCellSize / 2f, 0f, gridCellSize / 2f);
                Vector3 cellLocalPosInAnchorSpace = boardOffsetLocal + squareOffsetLocal;

                node.transform.position = boardAnchor.TransformPoint(cellLocalPosInAnchorSpace);

                colorIndex++;
            }
            colorIndex++;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            AdvancePlayerCharges();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AdvanceOpponentCharges();
        }
    }

    void AdvancePlayerCharges()
    {
        for (int y = grid.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                BoardNode node = grid.Get(x, y);
                if (node.Piece == null || node.Piece.PlayerOwner != player) continue;
                node.TranslatePiece(Vector2Int.up, new PushOtherPiece());
            }
        }
    }
    
    void AdvanceOpponentCharges()
    {
        for (int y = 0; y <= grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                BoardNode node = grid.Get(x, y);
                if (node.Piece == null || node.Piece.PlayerOwner != opponent) continue;
                node.TranslatePiece(Vector2Int.down, new PushOtherPiece());
            }
        }
    }
}