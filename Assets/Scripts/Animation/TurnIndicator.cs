using System;
using EventBus;
using TMPro;
using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
    private EventBinding<GameTurnEvent> turnEventBinding;
    [SerializeField] private TMP_Text actorText;
    [SerializeField] private TMP_Text phaseText;
    
    private void Awake()
    {
        turnEventBinding = new EventBinding<GameTurnEvent>(OnTurnEvent);
        EventBus<GameTurnEvent>.Register(turnEventBinding);
    }

    void OnTurnEvent(GameTurnEvent e)
    {
        actorText.text = e.actorType.ToString();
        if (e.turnType == GameTurnEvent.TurnType.ShiftStart) phaseText.text = "Push";
        else phaseText.text = string.Empty;
    }

    private void OnDestroy()
    {
        EventBus<GameTurnEvent>.Deregister(turnEventBinding);
    }
}
