using System;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using EventBus;
using ScriptableVariables;
using UnityEngine;


public class BoardPlayer : BoardActor
{
    enum Phase
    {
        None,
        Item,
        Board
    }

    [SerializeField] private int defaultReach = 2; 
    [SerializeField] private IntVariable reach;
    public int Reach => reach.Value;
    
    private Phase phase = Phase.None; 
    [SerializeField] private GameObject overHeadCam;
    [SerializeField] private GameObject firstPersonCam;
    [SerializeField] private BoardNodeSelector boardNodeSelector;
    private BoardModifier boardModifier;
    
    [SerializeField] private ItemSelectableBehavior itemSelectableBehavior;
    EventBinding<SelectableChosenEvent> selectBinding;
    private void Awake()
    {
        boardModifier = new BoardModifier(); 
        selectBinding = new EventBinding<SelectableChosenEvent>(OnSelectableChosenEvent);
        reach.OnValueChanged += OnReachValueChanged;
    }
    public void AddItem(BoardItem item) => Items.Add(item);
    public void RemoveItem(BoardItem item) => Items.Remove(item);

    private bool skipReachEvent; 
    void OnReachValueChanged(int value)
    {
        if (skipReachEvent)
        {
            skipReachEvent = false; 
            return;
        }
        if (value > 4)
        {
            skipReachEvent = true;
            reach.Value = 4; 
        }
    }
    
    protected override void OnStartTurn()
    {
        reach.Value = defaultReach; 
        if (Items.Count > 0)
            StartItemPlay();
        else
            StartBoardModification();
    }
    
    void StartItemPlay()
    {
        phase = Phase.Item;
            
        overHeadCam.SetActive(false); 
        firstPersonCam.SetActive(true);
            
        SelectionManager.Instance.onSelectonEnded += OnSelectionEnd;
        EventBus<SelectableChosenEvent>.Register(selectBinding);
        SelectionManager.Instance.StartSelection(
            Items.Cast<ISelectable>().ToList(), 
            0, 
            itemSelectableBehavior
        );
    }
    void StartBoardModification()
    {
        phase = Phase.Board;
        foreach(var item in Items) item.gameObject.SetActive(true);
        overHeadCam.SetActive(true); 
        firstPersonCam.SetActive(false);
    }
    
    
    void OnSelectableChosenEvent(SelectableChosenEvent selectableChosenEvent)
    {
        BoardItem chosenItem = (BoardItem)selectableChosenEvent.SelectedItem;
        Items.Remove(chosenItem);
        Destroy(chosenItem.gameObject);
        
        if (Items.Count > 0)
            SelectionManager.Instance.StartSelection(
                Items.Cast<ISelectable>().ToList(), 
                0, 
                itemSelectableBehavior
            );
        else
            StartBoardModification();
    }
    void OnSelectionEnd()
    {
        if (phase != Phase.Board) return;
        
        SelectionManager.Instance.onSelectonEnded -= OnSelectionEnd;
        EventBus<SelectableChosenEvent>.Deregister(selectBinding);
        StartBoardModification();
    }
    

    void EndItemPhase()
    {
        phase = Phase.Board;
        SelectionManager.Instance.EndSelection();
    }
    
    private void Update()
    {
        switch (phase)
        {
            case Phase.Item: ItemPhaseUpdate(); break;
            case Phase.Board: BoardPhaseUpdate(); break;
            default: break;
        }
    }
    
    void ItemPhaseUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            EndItemPhase();
    }
    
    void BoardPhaseUpdate()
    {
        boardNodeSelector.UpdateSelect(this);
        boardModifier.Update(this);
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (actionsAvailable > 0) return;
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Z)) Undo();
    }

    private void OnDestroy()
    {
        reach.OnValueChanged -= OnReachValueChanged;
    }
}