using System;
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
        if (piece == null)
            chargeText.text = "Empty";
        else
            chargeText.text = piece.Charge.ToString(); 
    }
}