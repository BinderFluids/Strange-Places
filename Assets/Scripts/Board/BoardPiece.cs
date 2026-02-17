
using UnityEngine;

[System.Serializable]
public class BoardPiece
{
    [SerializeField] private int charge;
    public int Charge => charge;

    public BoardPiece(int charge = 1)
    {
        this.charge = charge;
    }
    
    public void ChangeCharge(int amt)
    { 
        charge = Mathf.Max(1, charge += amt);
    }
    
    public BoardPiece Pop(int amt)
    {
        if (amt >= charge)
        {
            Debug.LogError("Piece does not have enough charge!");
            return null;
        }
        
        charge -= amt;
        return new BoardPiece(amt);
    }
}