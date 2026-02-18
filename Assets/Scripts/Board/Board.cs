using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;
    
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
    
    private Stack<IGridAction<BoardNode>> actionStack = new();
    [SerializeField] private List<string> actionStackString = new();
    
    private bool observeAction;
    public bool ObserveAction => observeAction;

    public void StartObservingAction()
    {
        observeAction = true; 
    }
    public void StopObservingAction()
    {
        observeAction = false; 
    }
    
    private void Awake()
    {
        Instance = this; 
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

    public void DoAction(IGridAction<BoardNode> action, BoardNode active)
    {
        if (observeAction) actionStack.Push(action);
        grid.ExecuteGridAction(active, action); 
    }

    public void Undo()
    {
        Debug.Log("Undo");
        StopObservingAction();
        if (actionStack.Count > 0) actionStack.Pop().Undo();
        StartObservingAction();
    }

    void UpdatePieces()
    {
        grid.ForEach(node => node.PieceUpdated());
        
    }
    void VisualizeStack()
    {
        actionStackString.Clear();
        foreach (BoardActionChain action in actionStack)
            actionStackString.Add(action.ToString());
    }
    
    private void Update()
    {
        StartObservingAction();
        if (Input.GetKeyDown(KeyCode.U)) {
            try {
                DoAction(new ShiftBoard(Vector2Int.up, player), null);
            } catch(System.Exception e) {
                Debug.LogException(e);
            }
        }
        if (Input.GetKeyDown(KeyCode.Z)) Undo();
        StopObservingAction();
    }
}