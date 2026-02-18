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
    public static T Create<T>(BoardPiece parent) => (T)System.Activator.CreateInstance(typeof(T), parent);
}


public class NeutralizingAttribute : BoardPieceAttribute
{
    private BoardPiece parentPiece;
    public NeutralizingAttribute(BoardPiece parentPiece) : base(parentPiece)
    {
        this.parentPiece = parentPiece;
        SetResolver();
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