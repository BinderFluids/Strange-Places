using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using EventBus;
using UnityEngine;


public class BoardPlayer : BoardActor
{
    enum Phase
    {
        None,
        Item,
        Board
    }

    private Phase phase = Phase.None; 
    [SerializeField] private GameObject overHeadCam;
    [SerializeField] private GameObject firstPersonCam;
    [SerializeField] private BoardNodeSelector boardNodeSelector;
    private BoardModifier boardModifier;
    
    [SerializeField] private ItemSelectableHighlighter itemSelectableHighlighter;
    EventBinding<SelectableChosenEvent> selectBinding;
    private void Awake()
    {
        boardModifier = new BoardModifier(); 
        selectBinding = new EventBinding<SelectableChosenEvent>(OnSelectableChosenEvent);
    }

    protected override void OnStartTurn()
    {
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
            itemSelectableHighlighter
        );
    }
    void StartBoardModification()
    {
        phase = Phase.Board;
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
                itemSelectableHighlighter
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
}