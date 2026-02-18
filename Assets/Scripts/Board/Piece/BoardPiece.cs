
using System;
using System.Collections.Generic;
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
    private List<BoardPieceAttribute> attributes = new();
    public IReadOnlyList<BoardPieceAttribute> Attributes => attributes;
    
    private ResolverType resolverType;
    public ResolverType ResolverType => resolverType;
    
    public BoardPiece(BoardPlayer playerOwner, int charge = 1, List<BoardPieceAttribute> attributes = null)
    {
        this.playerOwner = playerOwner;
        Charge = charge;
        this.attributes = attributes ?? new List<BoardPieceAttribute>();
        resolverType = ResolverType.None;
    }

    public BoardPiece(BoardPiece other)
    {
        playerOwner = other.PlayerOwner;
        Charge = other.Charge;
        attributes = new List<BoardPieceAttribute>(other.Attributes);
        resolverType = other.ResolverType;
    }
    
    public void ChangeCharge(int amt)
    { 
        charge += amt;
    }
    
    public void AddAttribute<T>() where T : BoardPieceAttribute
    {
        if (attributes.Exists(a => a is T)) return;
        
        T attribute = BoardPieceAttribute.Create<T>(this);
        attributes.Add(attribute);
        attribute.OnAdd();
    }
    public void RemoveAttribute<T>() where T : BoardPieceAttribute
    {
        attributes.RemoveAll(a => a is T);
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

        if (amt == 0 || amt == charge || amt > charge)
        {
            BoardPiece returnPiece = new BoardPiece(this);
            Charge = 0; 
            return returnPiece;
        }
        
        Charge -= amt; 
        return new BoardPiece(playerOwner, amt, attributes);
    }
    
    public override string ToString() => $"[{playerOwner}] {Charge}";
}


