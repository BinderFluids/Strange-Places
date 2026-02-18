
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardPiece
{

    [SerializeField] private BoardPlayer playerOwner;
    public BoardPlayer PlayerOwner => playerOwner;
    [SerializeField] private int charge;
    public int Charge => charge;
    
    private List<BoardPieceAttribute> attributes = new();
    public IReadOnlyList<BoardPieceAttribute> Attributes => attributes;
    
    private ResolverType resolverType;
    public ResolverType ResolverType => resolverType;
    
    public BoardPiece(BoardPlayer playerOwner, int charge = 1, List<BoardPieceAttribute> attributes = null)
    {
        this.playerOwner = playerOwner;
        this.charge = charge;
        this.attributes = attributes ?? new List<BoardPieceAttribute>();
        resolverType = ResolverType.None;
    }
    
    public void ChangeCharge(int amt)
    { 
        charge = Mathf.Max(1, charge += amt);
    }
    
    public void AddAttribute(BoardPieceAttribute attribute)
    {
        attributes.Add(attribute);
    }
    public void ClearAttributes() => attributes.Clear();

    public void SetResolver(ResolverType type)
    {
        resolverType = type;
    }

    public bool Assimilate(BoardPiece otherPiece)
    {
        if (otherPiece.PlayerOwner != playerOwner) return false;
        
        ChangeCharge(otherPiece.Charge);
        attributes.AddRange(otherPiece.Attributes);
        return true;
    }
    public BoardPiece Pop(int amt)
    {
        if (amt > charge)
        {
            Debug.LogError("Piece does not have enough charge!");
            return null;
        }

        if (amt == charge)
            return this; 
        
        charge -= amt;
        return new BoardPiece(playerOwner, amt, attributes);
    }
    
    public override string ToString() => $"[{playerOwner.ToString()}] {charge}";
}


