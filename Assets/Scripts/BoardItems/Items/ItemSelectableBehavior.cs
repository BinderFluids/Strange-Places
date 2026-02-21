using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using PrimeTween;

public class ItemSelectableBehavior : SelectionBehavior
{
    [SerializeField] private float tweenDuration; 
    [SerializeField] private SplineContainer splineContainer; 
    private List<BoardItem> activeItems = new();

    protected override void OnActivate()
    {
        activeItems.Clear();
        activeItems = SelectionManager.Instance.ActiveItems.Cast<BoardItem>().ToList();
        var activeItem = SelectionManager.Instance.CurrentItem.Value as BoardItem;
        SetPositions(activeItem);
    }

    protected override void SelectionChanged(ISelectable newSelection)
    {
        SetPositions(newSelection);
    }

    void SetPositions(ISelectable newSelection)
    {
        if (newSelection is not BoardItem selectedItem) return;

        float step = 1f / activeItems.Count; 
        for (int i = 0; i < activeItems.Count; i++)
        {
            BoardItem thisItem = activeItems[i];
            bool wasActive = thisItem.gameObject.activeSelf; 
            thisItem.gameObject.SetActive(true);
            
            Vector3 worldPosition = splineContainer.EvaluatePosition(step * i);
            if (thisItem == selectedItem) worldPosition += Vector3.up * 0.1f; 
            if (!wasActive)
            {
                thisItem.transform.position = worldPosition;
                continue; 
            }
            
            if (thisItem.transform.position != worldPosition)
                Tween.Position(thisItem.transform, worldPosition, tweenDuration); 
        }
    }
}