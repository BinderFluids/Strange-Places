
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class BoardPiece
{
    private bool doDebug = false; 
    [SerializeField] private BoardPlayer playerOwner;
    public BoardPlayer PlayerOwner => playerOwner;
    private int charge;
    public int Charge
    {
        get
        {
            return charge;
        }
        private set
        {
            DoDebug($"Setting Charge to {value}\n{Environment.StackTrace}");
            charge = value;
        }
    }

    void DoDebug(object message)
    {
        if (doDebug) Debug.Log(message);
    }
    private HashSet<BoardPieceAttribute> attributes = new();
    public HashSet<BoardPieceAttribute> Attributes => attributes;
    
    private ResolverType resolverType;
    public ResolverType ResolverType => resolverType;
    
    public BoardPiece(BoardPlayer playerOwner, int charge = 1, HashSet<BoardPieceAttribute> attributes = null)
    {
        this.playerOwner = playerOwner;
        Charge = charge;
        this.attributes = attributes ?? new HashSet<BoardPieceAttribute>();
        resolverType = ResolverType.None;
    }

    public BoardPiece(BoardPiece other)
    {
        playerOwner = other.PlayerOwner;
        Charge = other.Charge;
        attributes = new HashSet<BoardPieceAttribute>(other.Attributes);
        resolverType = other.ResolverType;
    }
    
    public void ChangeCharge(int amt)
    { 
        charge += amt;
    }
    

    public bool TryAddAttribute(BoardPieceAttribute attribute)
    {
        if (attributes.Any(a => a.GetType() == attribute.GetType())) return false;  
        
        attributes.Add(attribute);
        attribute.OnAdd();

        return true;
    }
    
    public void RemoveAttribute(Type type)
    {
        if (attributes.Any(a => a.GetType() == type))
        {
            var removedAttribute = attributes.First(a => a.GetType() == type);
            removedAttribute.OnRemove();
            attributes.Remove(removedAttribute);
        }
        
        if (attributes.Count == 0) resolverType = ResolverType.None;
    }
    public void ClearAttributes() => attributes.Clear();

    public void SetResolver(ResolverType type)
    {
        Debug.Log($"Setting Resolver to {type}");
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

        if (amt == 0 || amt == charge || amt > charge)
        {
            BoardPiece returnPiece = new BoardPiece(this);
            Charge = 0; 
            return returnPiece;
        }
        
        Charge -= amt; 
        return new BoardPiece(playerOwner, amt, attributes);
    }

    public override string ToString()
    {
        string playerCharge = $"[{playerOwner}] {Charge}";
        string resolverTypeString = resolverType == ResolverType.None ? "" : $"({resolverType})";
        return $"{playerCharge} Resolver Type: {resolverTypeString} {string.Join(", ", attributes.Select(a => a.ToString()))}";
    }
}


