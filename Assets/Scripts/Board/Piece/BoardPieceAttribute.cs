using System;
using System.Linq;
using UnityEngine;

public abstract class BoardPieceAttribute
{
    protected BoardPiece parentPiece;
    public BoardPieceAttribute(BoardPiece parent)
    {
        parentPiece = parent;
    }
    
    public abstract void OnAdd();
    public static T Create<T>(BoardPiece parent)
    {
        T attr = (T)System.Activator.CreateInstance(typeof(T), parent);
        return attr;
    }
    public static BoardPieceAttribute Create(Type attributeType, BoardPiece parent)
    {
        if (attributeType == null) throw new ArgumentNullException(nameof(attributeType));
        if (!typeof(BoardPieceAttribute).IsAssignableFrom(attributeType))
            throw new ArgumentException($"Type must derive from {nameof(BoardPieceAttribute)}: {attributeType}", nameof(attributeType));

        return (BoardPieceAttribute)Activator.CreateInstance(attributeType, parent);
    }
}


public class NeutralizingAttribute : BoardPieceAttribute
{
    private BoardPiece parentPiece;
    public NeutralizingAttribute(BoardPiece parentPiece) : base(parentPiece)
    {
        this.parentPiece = parentPiece;
    }
    
    public override void OnAdd()
    {
        SetResolver();
    }

    void SetResolver()
    {
        Debug.Log("Setting Neutralizing Attribute");
        parentPiece.SetResolver(ResolverType.Neutralize);
    }
}