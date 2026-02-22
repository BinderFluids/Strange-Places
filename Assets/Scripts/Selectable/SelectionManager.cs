using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBus; 
using UnityEngine;
using UnityUtils; 

public class SelectionManager : Singleton<SelectionManager>
{
    [SerializeField] private int selectionCount;
    
    private List<ISelectable> activeItems;
    public IReadOnlyList<ISelectable> ActiveItems => activeItems; 
    
    public Observer<ISelectable> CurrentItem = new(null); 
    private bool active = false;
    private bool endOnSelect;

    [SerializeField] private SelectionBehavior defaultHighligher;
    [SerializeField] private SelectionBehavior currentBehavior; 
    
    public event Action onSelectonEnded = delegate {};
    
    int GetTrueIndex(int index, List<ISelectable> items)
    {
        if (items.Count == 0) return 0; 
        int newIndex = (index % items.Count + items.Count) % items.Count;
        return newIndex;
    }
    
    public void StartSelection(List<ISelectable> items, int index = 0, SelectionBehavior behavior = null, bool endOnSelect = true)
    {
        this.endOnSelect = endOnSelect;
        if (active)
        {
            Debug.LogWarning("Selection already started");
            return; 
        }

        if (behavior == null)
            currentBehavior ??= defaultHighligher;
        else
            currentBehavior = behavior;
        
        active = true; 
        activeItems = items; 
        selectionCount = index;
        
        CurrentItem.Value = activeItems[selectionCount];
        currentBehavior.Activate();
    }

    public void EndSelection()
    {
        if (!active)
        {
            Debug.LogWarning("No selection session to end");
            return;
        }

        onSelectonEnded?.Invoke();
        currentBehavior.Deactivate();
        currentBehavior = null; 
        
        active = false; 
        activeItems.Clear();
    }

    private void Update()
    {
        if (!active) return; 
        
        NavigateSelectables();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentItem.Value.Select();
            if (endOnSelect) EndSelection();
            EventBus<SelectableChosenEvent>.Raise(
                new SelectableChosenEvent
                {
                    SelectedItem = CurrentItem.Value
                }
            ); 
        }
    }

    void NavigateSelectables()
    {
        if (Input.GetKeyDown(KeyCode.D)) ShiftSelection(1);
        if (Input.GetKeyDown(KeyCode.A)) ShiftSelection(-1); 
    }

    private UniTask currentHighlighterTask; 
    void ShiftSelection(int shift)
    {
        //if (currentHighlighterTask.Status == UniTaskStatus.Pending) return;
        
        selectionCount += shift; 
        var index = GetTrueIndex(selectionCount, activeItems);
        CurrentItem.Value = activeItems[index];

        //currentHighlighterTask = currentHighlighter.GetHighlightTask();
    }
}
