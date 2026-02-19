
using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BoardPlayer : MonoBehaviour, IPieceOwner
{
    [SerializeField, Range(-5, 5)] private int reach;
    public int Reach => reach;
    
    [SerializeField] private int actionsPerTurn = 2;
    [SerializeField] private int actionsAvailable = 2;

    [SerializeField] private ItemSelectableHighlighter itemHighlighter;
    [SerializeField] private List<BoardItem> items = new();
    public List<BoardItem> Items => items;
    EventBinding<SelectableChosenEvent> selectBinding;

    private Grid<BoardNode> workingGrid;

    private bool turnActive;
    public event Action onTurnEnd = delegate {};

    private void Awake()
    {
        selectBinding = new EventBinding<SelectableChosenEvent>(OnSelectableChosenEvent);
    }

    

    private Grid<BoardNode> ctx;
    private bool clone; 
    public void StartTurn(Grid<BoardNode> ctx, bool clone = false)
    {
        this.ctx = ctx;
        this.clone = clone; 
        //TODO: account for wanting to use many items
        if (items.Count > 0)
        {
            EventBus<SelectableChosenEvent>.Register(selectBinding);
            SelectionManager.Instance
                .StartSelection(
                    items.Cast<ISelectable>().ToList(), 
                    0,
                    itemHighlighter
                    ); 
        }
    }
    void OnSelectableChosenEvent(SelectableChosenEvent boardNodeEvent)
    {
        EventBus<SelectableChosenEvent>.Deregister(selectBinding);
        TurnActive(ctx, clone);
    }

    void TurnActive(Grid<BoardNode> ctx, bool clone = false)
    {
        workingGrid = clone ? ctx.Copy() : ctx; 
        actionsAvailable = actionsPerTurn;
        turnActive = true; 
    }

    public void DoAction(Vector2Int coords, IGridAction<BoardNode> action)
    {
        if (actionsAvailable < 1) return; 
        
        workingGrid.Execute(coords, action);
        actionsAvailable--;
    }
    public void Undo()
    {
        if (actionsAvailable == actionsPerTurn) return; 
        
        workingGrid.Undo();
        actionsAvailable++;
    }

    public void EndTurn()
    {
        turnActive = false;
        onTurnEnd?.Invoke();
    }

    private void Update()
    {
        if (!turnActive) return;
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (actionsAvailable > 0) return;
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Z)) Undo();
    }
}