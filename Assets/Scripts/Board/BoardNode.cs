
using System;
using UnityEngine;

public class BoardNode : MonoBehaviour, IGridNode
{
    [SerializeField] private BoardPiece piece;

    private Color gizmoColor;
    private float gizmoSize;
    public void Init(Color gizmoColor, float gizmoSize)
    {
        this.gizmoColor = gizmoColor;
        this.gizmoSize = gizmoSize;
    }
    
    public bool IsOccupied()
    {
        return piece != null; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        
        Vector3 cubeSize = new Vector3(gizmoSize, .01f, gizmoSize);
        Gizmos.DrawCube(transform.position, cubeSize);
    }
}