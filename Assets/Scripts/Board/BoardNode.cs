
using UnityEngine;

public class BoardNode : MonoBehaviour, IGridNode
{
    [SerializeField] private BoardPiece piece;
    public bool IsOccupied()
    {
        return piece != null; 
    }
}