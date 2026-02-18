
using EventBus;
using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    [SerializeField, Range(-5, 5)] private int reach;
    public int Reach => reach;
    [SerializeField] private int actionsAvailable = 2;

    public void UseAction()
    {
        if (actionsAvailable == 0) return;
        actionsAvailable--;  
    }
}