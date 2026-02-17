
using EventBus;
using UnityEngine;

public class BoardPlayer : MonoBehaviour
{
    [SerializeField] private int actionsAvailable = 2;

    public void UseAction()
    {
        if (actionsAvailable == 0) return;
        actionsAvailable--;  
    }
}