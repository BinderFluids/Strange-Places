using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class ItemSelectableHighlighter : SelectionHighlighter
{
    [SerializeField] private SplineContainer splineContainer; 
    
    protected override void SelectionChanged(ISelectable newSelection)
    {
        if (newSelection is not BoardItem selectedItem) return;
        
        var activeItems = SelectionManager.Instance.ActiveItems.Cast<BoardItem>();
        var otherItems = 
            SelectionManager.Instance.ActiveItems.Where(x => x != selectedItem);

        for (int i = 0; i < activeItems.Count(); i++)
        {
            BoardItem thisItem = activeItems.ElementAt(i); 
            float distanceOnSpline = (i + 1) / SelectionManager.Instance.ActiveItems.Count;
            Vector3 worldPosition = splineContainer.EvaluatePosition(distanceOnSpline); 
            thisItem.transform.position = worldPosition;
        }

        selectedItem.transform.position += Vector3.up; 
    }
}