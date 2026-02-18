
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class BoardPiece
{

    [SerializeField] private BoardPlayer playerOwner;
    public BoardPlayer PlayerOwner => playerOwner;
    private int charge;
    private int id; 
    public int Id => id;
    public int Charge
    {
        get
        {
            return charge;
        }
        private set
        {
            Debug.Log($"{id}: Setting Charge to {value}\n{Environment.StackTrace}");
            charge = value;
        }
    }

    private List<BoardPieceAttribute> attributes = new();
    public IReadOnlyList<BoardPieceAttribute> Attributes => attributes;
    
    private ResolverType resolverType;
    public ResolverType ResolverType => resolverType;
    
    public BoardPiece(BoardPlayer playerOwner, int charge = 1, List<BoardPieceAttribute> attributes = null)
    {
        id = Random.Range(0, 1000000); 
        this.playerOwner = playerOwner;
        Charge = charge;
        this.attributes = attributes ?? new List<BoardPieceAttribute>();
        resolverType = ResolverType.None;
        
        Debug.Log($"Created {this}");
    }

    public BoardPiece(BoardPiece other)
    {
        id = Random.Range(0, 1000000); 
        playerOwner = other.PlayerOwner;
        Charge = other.Charge;
        attributes = new List<BoardPieceAttribute>(other.Attributes);
        resolverType = other.ResolverType;
        
        Debug.Log($"Created {this}");
    }
    
    public void ChangeCharge(int amt)
    { 
        charge += amt;
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
    public BoardPiece Pop(int amt = 0)
    {

        if (amt == 0 || amt == charge)
        {
            BoardPiece returnPiece = new BoardPiece(this);
            Charge = 0; 
            return returnPiece;
        }
        
        if (amt > charge)
        {
            Debug.LogError("Piece does not have enough charge!");
            return null;
        }
        
        Charge -= amt; 
        return new BoardPiece(playerOwner, amt, attributes);
    }
    
    public override string ToString() => $"{id}: [{playerOwner.ToString()}] {Charge}";
}


