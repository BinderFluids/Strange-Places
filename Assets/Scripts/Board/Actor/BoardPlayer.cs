using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using EventBus;
using UnityEngine;

public class BoardPlayer : BoardActor
{
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

    void OnSelectableChosenEvent(SelectableChosenEvent selectableChosenEvent)
    {
        BoardItem chosenItem = (BoardItem)selectableChosenEvent.SelectedItem;
        Items.Remove(chosenItem);
        chosenItem?.Use(workingGrid);
        
        EventBus<SelectableChosenEvent>.Deregister(selectBinding);
        StartBoardModification();
    }

    protected override void OnStartTurn()
    {
        if (Items.Count > 0)
        {
            overHeadCam.SetActive(false); 
            firstPersonCam.SetActive(true);
            
            EventBus<SelectableChosenEvent>.Register(selectBinding);
            SelectionManager.Instance.StartSelection(
                Items.Cast<ISelectable>().ToList(), 
                0, 
                itemSelectableHighlighter
            );
            
            return;
        }
        StartBoardModification();
    }

    protected override void OnEndTurn()
    {
        
    }

    void StartBoardModification()
    {
        
        overHeadCam.SetActive(true); 
        firstPersonCam.SetActive(false);
        turnActive = true;
    }
    
    private void Update()
    {   
        if (!turnActive) return;
        
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