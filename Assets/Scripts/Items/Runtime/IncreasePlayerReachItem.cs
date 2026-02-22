using ScriptableVariables;
using UnityEngine;

public class IncreasePlayerReachItem : BoardItem
{
    [SerializeField] private IntVariable reach; 
    
    public override void Use()
    {
        reach.Value++;
    }
}