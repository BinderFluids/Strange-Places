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
        chargeText.text = piece == null ? "Empty" : piece.Charge.ToString();
    }

    private void OnDestroy()
    {
        node.onPieceUpdate -= OnPieceUpdate;
    }
}