
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoardPiece
{

    [SerializeField] private BoardPlayer playerOwner;
    public BoardPlayer PlayerOwner => playerOwner;
    [SerializeField] private int charge;
    public int Charge => charge;
    
    private PieceConflictResolver resolver;
    public PieceConflictResolver Resolver => resolver;

    private List<BoardPieceAttribute> attributes;
    public IReadOnlyList<BoardPieceAttribute> Attributes => attributes;
    
    public BoardPiece(BoardPlayer playerOwner, int charge = 1, List<BoardPieceAttribute> attributes = null)
    {
        this.playerOwner = playerOwner;
        this.charge = charge;
        this.attributes = attributes ?? new List<BoardPieceAttribute>();
    }
    
    public void ChangeCharge(int amt)
    { 
        charge = Mathf.Max(1, charge += amt);
    }
    
    public void AddAttribute(BoardPieceAttribute attribute)
    {
        attributes.Add(attribute);
    }

    public void SetResolver(PieceConflictResolver resolver)
    {
        this.resolver = resolver;
    }
    
    public BoardPiece Pop(int amt)
    {
        if (amt >= charge)
        {
            Debug.LogError("Piece does not have enough charge!");
            return null;
        }
        
        charge -= amt;
        return new BoardPiece(playerOwner, amt, attributes);
    }
}


