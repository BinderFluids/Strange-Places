using UnityEngine;
using UnityEngine.UIElements;

public enum ResolverType { None, Neutralize, TryPush };

public abstract class BoardConflictResolver : BoardActionChain
{
    protected BoardNode otherNode;
    protected Vector2Int direction;
    protected BoardConflictResolver (BoardNode otherNode, Vector2Int direction)
    {
        this.otherNode = otherNode;
        this.direction = direction;
    }

    public static BoardConflictResolver Create(ResolverType type, BoardNode otherNode, Vector2Int direction)
    {
        switch (type)
        {
            case ResolverType.None: return Create<None>(otherNode, direction);
            case ResolverType.Neutralize: return Create<Neutralize>(otherNode, direction);
            case ResolverType.TryPush: return Create<TryPush>(otherNode, direction);
            default: return Create<None>(otherNode, direction); 
        }
    }
    
    public static T Create<T>(BoardNode otherNode, Vector2Int direction) where T : BoardConflictResolver
    {
        return (T)System.Activator.CreateInstance(typeof(T), otherNode, direction);
    }
}

public class None : BoardConflictResolver
{
    public None(BoardNode otherNode, Vector2Int direction) : base(otherNode, direction)
    {
    }

    public override void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        
    }
}

public class Neutralize : BoardConflictResolver
{

    public Neutralize(BoardNode otherNode, Vector2Int direction) : base(otherNode, direction)
    {
        this.direction = direction;
    }

    public override void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        var takeOtherPiece = new TakePiece(active.Piece.Charge);
        var translateActive = new TranslatePiece(direction);

        Chain(takeOtherPiece, otherNode, ctx); 
        Chain(translateActive, active, ctx);
    }
}

public class TryPush : BoardConflictResolver
{
    public TryPush(BoardNode otherNode, Vector2Int direction) : base(otherNode, direction) { }

    public override void Execute(BoardNode active, Grid<BoardNode> ctx)
    {
        if (active.Piece.Charge > otherNode.Piece.Charge)
        {
            var moveOther = new TranslatePiece(direction);
            var moveActive = new TranslatePiece(direction);
            
            Chain(moveOther, otherNode, ctx); 
            Chain(moveActive, active, ctx);
        }
            
    }
}