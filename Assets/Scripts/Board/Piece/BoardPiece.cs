
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class BoardPiece
{
    private bool doDebug = true; 
    [SerializeField] private IPieceOwner owner;
    public IPieceOwner Owner => owner;
    private int charge;
    public int Charge => charge;

    void DoDebug(object message)
    {
        //if (doDebug) Debug.Log(message);
    }
    private HashSet<BoardPieceAttribute> attributes = new();
    public HashSet<BoardPieceAttribute> Attributes => attributes;
    
    private ResolverType resolverType;
    public ResolverType ResolverType => resolverType;
    
    public BoardPiece(IPieceOwner owner, int charge = 1, HashSet<BoardPieceAttribute> attributes = null)
    {
        this.owner = owner;
        this.charge = charge;
        if (attributes != null) 
            foreach (var attr in attributes) TryAddAttribute(attr);
        
        resolverType = ResolverType.None;
        DoDebug($"Created Piece {this}");
    }

    public BoardPiece(BoardPiece other)
    {
        owner = other.Owner;
        this.charge = other.Charge;
        
        if (other.attributes != null) 
            foreach (var attr in other.attributes) TryAddAttribute(attr);
    }
    
    public void ChangeCharge(int amt)
    { 
        charge += amt;
    }
    

    public bool TryAddAttribute(BoardPieceAttribute attribute)
    {
        if (attributes.Any(a => a.GetType() == attribute.GetType())) return false;  
        
        var newAttribute = BoardPieceAttribute.Create(attribute.GetType(), this);
        attributes.Add(newAttribute);
        newAttribute.OnAdd();

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
        
        resolverType = type;
    }

    public bool Assimilate(BoardPiece otherPiece)
    {
        if (otherPiece.Owner != owner) return false;
        
        ChangeCharge(otherPiece.Charge);
        attributes.AddRange(otherPiece.Attributes);
        
        return true;
    }
    public BoardPiece Pop(int amt = 0)
    {

        if (amt == 0 || amt == charge || amt > charge)
        {
            BoardPiece returnPiece = new BoardPiece(this);
            charge = 0; 
            return returnPiece;
        }
        
        charge -= amt; 
        return new BoardPiece(owner, amt, attributes);
    }

    public override string ToString()
    {
        string playerCharge = $"[{owner}] {Charge}";
        string resolverTypeString = resolverType == ResolverType.None ? "" : $"({resolverType})";
        return $"{playerCharge} Resolver Type: {resolverTypeString} {string.Join(", ", attributes.Select(a => a.ToString()))}";
    }

    public override bool Equals(object obj)
    {
        if (obj is BoardPiece other)
        {
            bool sameCharge = other.Charge == charge;
            bool sameOwner = other.Owner == owner;
            
            //check if attributes are of the same type
            bool sameAttributes = true;
            if (attributes.Count != other.Attributes.Count) sameAttributes = false;
            else
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    BoardPieceAttribute thisAttribute = attributes.ElementAt(i);
                    if (!other.Attributes
                            .Select(a => a.GetType())
                            .Contains(thisAttribute.GetType()))
                        sameAttributes = false; 
                }
            }
            
            return sameCharge && sameOwner && sameAttributes;
        }

        return false;
    }
    public override int GetHashCode() => HashCode.Combine(owner, charge, attributes);
}


