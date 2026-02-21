using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class BoardNodeDebugDisplay : MonoBehaviour
{
    private bool enabled = false; 
    [SerializeField] private BoardNodeMonobehavior nodeMonobehavior;
    [SerializeField] private TMP_Text coordText;
    [SerializeField] private TMP_Text chargeText;

    public void Init()
    {
        if (!enabled)
        {
            coordText.text = string.Empty;
            chargeText.text = string.Empty;
            return; 
        }
        nodeMonobehavior.Node.onNodeUpdate += OnNodeUpdate;
        coordText.text = nodeMonobehavior.Node.Coords.ToString();
    }

    void OnNodeUpdate()
    {
        BoardPiece piece = nodeMonobehavior.Node.Piece;
        chargeText.text = piece == null ? "Empty" : piece.Charge.ToString();

        if (nodeMonobehavior.Node is GiveItemBoardNode)
        {
            SetColor(Color.darkGreen);
            coordText.text = "Item!";
            chargeText.text = string.Empty;
            return; 
        }
        
        if (piece == null)
        {
            SetColor(Color.black);
            return;
        }

        if (nodeMonobehavior.Node is NullBoardNode)
        {
            SetColor(Color.black);
            coordText.text = "BLOCKED";
            chargeText.text = string.Empty;
            return; 
        }

        BoardActor actor = piece.Owner as BoardActor; 
        if (actor.gameObject.name == "Player")
            SetColor(Color.green);
        else if (actor.gameObject.name == "Bot")
            SetColor(Color.red);

        if (piece.Attributes.Any(a => a is NeutralizingAttribute))
            chargeText.color = Color.yellow;
    }

    void SetColor(Color color)
    {
        coordText.color = color;
        chargeText.color = color;
    }

    private void OnDestroy()
    {
        nodeMonobehavior.Node.onNodeUpdate -= OnNodeUpdate;
    }
    
    #region GIZMOS  
    private Color gizmoColor;
    private float gizmoSize;
    public void InitGizmos(Color gizmoColor, float gizmoSize)
    {
        this.gizmoColor = gizmoColor;
        this.gizmoSize = gizmoSize;
    }
    
    private void OnDrawGizmos()
    {
        if (nodeMonobehavior.Node != null)
        {
            if (nodeMonobehavior.Node is NullBoardNode)
                gizmoColor = Color.black;
            if (nodeMonobehavior.Node is GiveItemBoardNode)
                gizmoColor = Color.darkGreen;
            if (nodeMonobehavior.Node is EndZoneNode endZoneNode)
            {
                if (endZoneNode.TargetType == GameTurnEvent.ActorType.Player)
                    gizmoColor = Color.green;
                if (endZoneNode.TargetType == GameTurnEvent.ActorType.Opponent)
                    gizmoColor = Color.red;
            }
        
        }
        Gizmos.color = gizmoColor;
        
        Vector3 cubeSize = new Vector3(gizmoSize, .01f, gizmoSize);
        Gizmos.DrawCube(transform.position, cubeSize);
    }
    #endregion
}