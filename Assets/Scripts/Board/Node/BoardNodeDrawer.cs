using System;
using PrimeTween;
using UnityEngine;

public class BoardNodeDrawer : MonoBehaviour
{
    [SerializeField] private Transform nodeAnchor;
    [SerializeField] private Transform orb;
    [SerializeField] private float orbSizeStep = 0.1f;
    [SerializeField] private Renderer orbRenderer;
    [SerializeField] private Color playerColor = Color.darkGreen;
    [SerializeField] private Color opponentColor = Color.red;
    
    private BoardNode previousNodeState;
    private BoardNode node;

    public void Init(BoardNode node)
    {
        orb.localScale = Vector3.zero;
        
        this.node = node; 
        node.onNodeUpdate += OnNodeUpdate;
    }

    void OnNodeUpdate()
    {
        if (!node.IsOccupied())
        {
            if (orb.localScale != Vector3.zero) Tween.Scale(orb, Vector3.zero, 0.2f);
            return;
        }

        Color targetColor = Color.white;
        if (node.Piece.Owner is BoardPlayer)
            targetColor = playerColor; 
        if (node.Piece.Owner is BoardBot)
            targetColor = opponentColor;
        
        orbRenderer.material.color = targetColor; 
        
        Vector3 targetScale = orbSizeStep * node.Piece.Charge * Vector3.one;  
        if (orb.localScale != targetScale) Tween.Scale(orb, targetScale, 0.2f);
    }
}
