using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using PrimeTween;

public class ItemSelectableHighlighter : SelectionHighlighter
{
    [SerializeField] private float tweenDuration; 
    [SerializeField] private SplineContainer splineContainer; 
    private List<BoardItem> activeItems = new();

    protected override void OnActivate()
    {
        activeItems.Clear();
        activeItems = SelectionManager.Instance.ActiveItems.Cast<BoardItem>().ToList();
        activeItems.ForEach(item => item.gameObject.SetActive(true));
        
        TweenToSelectedPosition((SelectionManager.Instance.CurrentItem.Value as BoardItem).transform);
    }

    protected override void SelectionChanged(ISelectable newSelection)
    {
        if (newSelection is not BoardItem selectedItem) return;
        
        for (int i = 0; i < activeItems.Count; i++)
        {
            BoardItem thisItem = activeItems.ElementAt(i); 
            thisItem.gameObject.SetActive(true);
            if (thisItem == selectedItem) continue; 
            
            float distanceOnSpline = i / (float)SelectionManager.Instance.ActiveItems.Count;
            Vector3 worldPosition = splineContainer.EvaluatePosition(distanceOnSpline);
            
            if(thisItem.transform.position != worldPosition)
                Tween.Position(thisItem.transform, worldPosition, tweenDuration); 
        }
        
        TweenToSelectedPosition(selectedItem.transform);
    }

    void TweenToSelectedPosition(Transform _transform)
    {
        Vector3 selectedWorldPosition = _transform.position + transform.up * .2f; 
        Tween.Position(_transform, selectedWorldPosition, tweenDuration); 
    }

    protected override void OnDeactivate()
    {
        activeItems.ForEach(item => item.gameObject.SetActive(false)); 
    }
}