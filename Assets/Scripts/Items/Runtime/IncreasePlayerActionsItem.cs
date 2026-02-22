using ScriptableVariables;
using UnityEngine;

public class IncreasePlayerActionsItem : BoardItem
{
    [SerializeField] private IntVariable actionsAvailable;
    [SerializeField] private int increaseValue; 
    
    public override void Use()
    {
        actionsAvailable.Value += increaseValue; 
    }
}