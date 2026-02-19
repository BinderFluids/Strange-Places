using ScriptableVariables;
using UnityEngine;

public class IncreaseMachineChargeItem : BoardItem
{
    [SerializeField] private IntVariable machineMovementNumber;
    
    public override void Use(BoardNodeGrid ctx)
    {
        machineMovementNumber.Value++;
    }
}