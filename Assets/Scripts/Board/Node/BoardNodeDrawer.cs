using System;
using System.Linq;
using EventBus;
using PrimeTween;
using UnityEngine;

public class BoardNodeDrawer : MonoBehaviour
{
    [SerializeField] private AnimationCurve sizeCurve;
    [SerializeField] private Transform pieceAnchor; 
    [SerializeField] private Transform orb;
    [SerializeField] private float orbSizeStep = 0.1f;
    [SerializeField] private Renderer orbRenderer;
    [SerializeField] private Color playerColor = Color.darkGreen;
    [SerializeField] private Color opponentColor = Color.red;
    [SerializeField] private Color neutralizedColor = Color.yellow;
    [SerializeField] private Color highlightedColor = Color.blue;
    [SerializeField] private Color dullColor = Color.yellow;
    [SerializeField] private Renderer[] highlighters; 
    
    private BoardNode previousNodeState;
    private BoardNode node;
    private EventBinding<SelectBoardNodeEvent> selectBinding;

    public void Init(BoardNode node)
    {
        orb.localScale = Vector3.zero;

        if (node is NullBoardNode) return;
        this.node = node; 
        selectBinding = new EventBinding<SelectBoardNodeEvent>(OnSelectBoardNodeEvent);
    }

    void OnSelectBoardNodeEvent(SelectBoardNodeEvent e)
    {
        if (e.selectedNode.Node != node) 
            foreach (var highlighter in highlighters) highlighter.material.color = dullColor;
        else
            foreach (var highlighter in highlighters) highlighter.material.color = highlightedColor;
    }

    private Tween positionTween;
    private Tween scaleTween;
    void Update()
    {
        if (node == null) return; 
        
        float duration = 0.2f;
        if (!node.IsOccupied())
        {
            if (orb.localScale != Vector3.zero) Tween.Scale(orb, Vector3.zero, duration);
            return;
        }

        Color targetColor = Color.white;
        if (node.Piece.Owner is BoardPlayer)
            targetColor = playerColor; 
        if (node.Piece.Owner is BoardBot)
            targetColor = opponentColor;
        if (node.Piece.Attributes.Any(a => a is NeutralizingAttribute))
            targetColor = neutralizedColor;
        
        
        orbRenderer.material.color = targetColor; 
        
        Vector3 targetScale = orbSizeStep * node.Piece.Charge * Vector3.one;
        Vector3 offset = orbSizeStep * node.Piece.Charge * Vector3.up;
        
        if (orb.localScale != targetScale && !scaleTween.isAlive)
            scaleTween = Tween.Scale(orb, targetScale, duration);
        if (orb.localPosition != offset && !positionTween.isAlive)
            positionTween = Tween.LocalPosition(orb, offset, duration);
    }

    private void OnDestroy()
    {
        //node.onNodeUpdate -= OnNodeUpdate;
        EventBus<SelectBoardNodeEvent>.Deregister(selectBinding);
    }
}
