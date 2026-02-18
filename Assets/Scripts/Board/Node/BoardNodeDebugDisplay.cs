using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class BoardNodeDebugDisplay : MonoBehaviour
{
    [SerializeField] private BoardNode node;
    [SerializeField] private TMP_Text coordText;
    [SerializeField] private TMP_Text chargeText;

    private void Start()
    {
        node.onPieceUpdate += OnPieceUpdate;
        coordText.text = node.Coords.ToString();
    }

    void OnPieceUpdate(BoardPiece piece)
    {
        chargeText.text = piece == null ? "Empty" : piece.Charge.ToString();

        if (piece == null)
        {
            SetColor(Color.black);
            return;
        }
        if (piece.PlayerOwner.gameObject.name == "Player")
            SetColor(Color.green);
        else if (piece.PlayerOwner.gameObject.name == "Opponent")
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
        node.onPieceUpdate -= OnPieceUpdate;
    }
}