using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    [SerializeField] private GameManager manager; 
    [SerializeField] private BoardPlayer player;
    [SerializeField] private BoardPlayer opponent; 
    [SerializeField] private BoardNodeMonobehavior boardNodePrefab;
    [SerializeField] private Transform boardNodeContainer;
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private float gridCellSize;
    [Min(1), SerializeField] private int gridWidth;
    [Min(1), SerializeField] private int gridHeight;
    
    private Grid<BoardNode> grid;
    public Grid<BoardNode> Grid => grid;
    
    private Stack<IGridAction<BoardNode>> actionStack = new();
    [SerializeField] private List<string> actionStackString = new();
    
    private bool observeAction;
    public bool ObserveAction => observeAction;
    private void Awake()
    {
        Instance = this; 
        InitGrid();
    }

    private void Start()
    {
        manager.StartTurn(player); 
    }

    void InitGrid()
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
                BoardNodeMonobehavior nodeBehavior = Instantiate(boardNodePrefab, transform);
                BoardNode newNode = new BoardNode(grid);
                grid.Set(x, y, newNode);
                nodeBehavior.Init(newNode); 
#if UNITY_EDITOR
                nodeBehavior.InitGizmos(colors[colorIndex % 2], gridCellSize);
                BoardNodeDebugDisplay debugDisplay = nodeBehavior.gameObject.GetComponent<BoardNodeDebugDisplay>();
                debugDisplay.Init(); 
#endif
                Vector3 boardOffsetLocal = new Vector3(x * gridCellSize, 0f, y * gridCellSize);
                Vector3 squareOffsetLocal = new Vector3(gridCellSize / 2f, 0f, gridCellSize / 2f);
                Vector3 cellLocalPosInAnchorSpace = boardOffsetLocal + squareOffsetLocal;

                nodeBehavior.transform.position = boardAnchor.TransformPoint(cellLocalPosInAnchorSpace);

                colorIndex++;
            }
            colorIndex++;
        }
    }

    public void StartObservingAction() => grid.StartObservingAction();
    public void StopObservingAction() => grid.StopObservingAction();
    public void Execute(IGridAction<BoardNode> action, Vector2Int coords) => grid.Execute(coords, action);
    public void Undo() => grid.Undo();
}