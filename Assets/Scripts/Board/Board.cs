using System;
using UnityEngine;

public class Board : MonoBehaviour
{
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
        
        for (int y = 0; y < gridWidth; y++)
        {
            for (int x = 0; x < gridHeight; x++)
            {
                BoardNode node = Instantiate(boardNodePrefab, transform);
                node.Init(colors[colorIndex % 2], gridCellSize);
                
                Vector3 boardOffset = new Vector3(x * gridCellSize, 0, y * gridCellSize);
                Vector3 squareOffset = new Vector3(gridCellSize / 2, 0, gridCellSize / 2);
                node.transform.localPosition = boardAnchor.position + boardOffset + squareOffset;
                node.transform.SetParent(boardNodeContainer);
                
                colorIndex++;
            }
            colorIndex++;
        }
    }
}