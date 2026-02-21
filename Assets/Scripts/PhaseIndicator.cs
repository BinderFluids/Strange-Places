
using System;
using EventBus;
using TMPro;
using UnityEngine;

public class PhaseIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text actorText;
    [SerializeField] private TMP_Text phaseText;
    
    private EventBinding<GameTurnEvent> turnEventBinding;
    
    private void Start()
    {
        actorText.text = string.Empty;
        phaseText.text = string.Empty;
    }

    void OnTurnEvent(GameTurnEvent e)
    {
        actorText.text = e.actorType.ToString();
        phaseText.text = e.turnType.ToString();
    }
}
        
    