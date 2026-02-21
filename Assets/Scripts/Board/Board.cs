using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using ScriptableVariables;
using UnityEditor.Rendering;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;

    private enum BoardNodeType
    {
        Null,
        Normal,
        GiveItem,
        OpponentEndZone,
        PlayerEndZone
    }

    private int[,] boardMap =
    {
        {
            0, 3, 3, 3, 0
        },
        {
            0, 1, 1, 1, 0
        },
        {
            2, 1, 1, 1, 2
        },
        {
            0, 1, 1, 1, 0
        },
        {
            0, 4, 4, 4, 0
        }
    };

    [SerializeField] private IntVariable machineQueuedMovement;
    [SerializeField] private BoardPlayer player;
    [SerializeField] private BoardBot opponent;
    [SerializeField] private bool generateNullNodes;
    [SerializeField] private GameManager manager; 
    [SerializeField] private BoardNodeMonobehavior boardNodePrefab;
    [SerializeField] private Transform boardNodeContainer;
    [SerializeField] private Transform boardAnchor;
    [SerializeField] private float gridCellSize;
    
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
        manager.StartGame(); 
    }

    void InitGrid()
    {
        int gridHeight = boardMap.GetLength(0); 
        int gridWidth = boardMap.GetLength(1);
        grid = new Grid<BoardNode>(gridWidth, gridHeight);
        
        int colorIndex = 0;
        Color[] colors =
        {
            Color.purple,
            Color.blue
        };
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                BoardNodeType type = (BoardNodeType)boardMap[y, x];
                BoardNodeMonobehavior nodeBehavior = Instantiate(boardNodePrefab, transform);

                BoardNode newNode;
                switch (type)
                {
                    case BoardNodeType.Null:
                        newNode = new NullBoardNode(grid);
                        break;
                    case BoardNodeType.Normal:
                        newNode = new BoardNode(grid);
                        break;
                    case BoardNodeType.GiveItem:
                        newNode = new GiveItemBoardNode(grid);
                        break;
                    case BoardNodeType.PlayerEndZone:
                        newNode = new EndZoneNode(grid);
                        ((EndZoneNode)newNode)
                            .Init(
                                player, 
                                GameTurnEvent.ActorType.Player, 
                                machineQueuedMovement,
                                1
                                );
                        break;
                    case BoardNodeType.OpponentEndZone: 
                        newNode = new EndZoneNode(grid);
                        ((EndZoneNode)newNode)
                            .Init(
                                opponent, 
                                GameTurnEvent.ActorType.Opponent, 
                                machineQueuedMovement,
                                -1
                            );
                        break;
                    default:
                        Debug.LogWarning("Defaulting to Normal BoardNode ");
                        newNode = new BoardNode(grid);
                        break;
                }
                
                grid.Set(x, y, newNode);
                nodeBehavior.Init(newNode); 
#if UNITY_EDITOR
                BoardNodeDebugDisplay debugDisplay = nodeBehavior.gameObject.GetComponent<BoardNodeDebugDisplay>();
                debugDisplay.InitGizmos(colors[colorIndex % 2], gridCellSize);
                debugDisplay.Init(); 
#endif
                Vector3 boardOffsetLocal = new Vector3(x * gridCellSize, 0f, y * gridCellSize);
                Vector3 squareOffsetLocal = new Vector3(gridCellSize / 2f, 0f, gridCellSize / 2f);
                Vector3 cellLocalPosInAnchorSpace = boardOffsetLocal + squareOffsetLocal;

                nodeBehavior.transform.position = boardAnchor.TransformPoint(cellLocalPosInAnchorSpace);

                colorIndex++;
            }

            if (gridWidth % 2 == 0) colorIndex++; 
        }
    }

    public void AddSecondaryAction(ISecondaryAction action)
    {
        (grid.ActionStack.Peek() as BoardAction).AddSecondaryAction(action);
    }
    public void StartObservingAction() => grid.StartObservingAction();
    public void StopObservingAction() => grid.StopObservingAction();
    public void Execute(Vector2Int coords, IGridAction<BoardNode> action) => grid.Execute(coords, action);
    public void Undo() => grid.Undo();
}