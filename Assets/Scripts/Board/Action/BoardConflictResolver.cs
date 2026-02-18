using UnityEngine;
using UnityEngine.InputSystem;
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
        //Destroy both pieces if they have the same charge
        if (active.Piece.Charge == otherNode.Piece.Charge)
        {
            var takeActivePiece = new TakePiece();
            var takeOtherPiece = new TakePiece();
            
            Chain(takeActivePiece, active, ctx);
            Chain(takeOtherPiece, otherNode, ctx);
            return;
        }
        
        //Active piece has more charge, so get rid of other piece and move active piece
        if (active.Piece.Charge > otherNode.Piece.Charge)
        {
            var translateActive = new TranslatePiece(direction);
            var neutralizeOtherPiece = new TakePiece(active.Piece.Charge);
            var neutralizeActivePieceAfterTranslation = new TakePiece(otherNode.Piece.Charge);
            int otherPieceCharge = otherNode.Piece.Charge;
            
            Chain(neutralizeOtherPiece, otherNode, ctx);
            Chain(translateActive, active, ctx);
            Chain(neutralizeActivePieceAfterTranslation, otherNode, ctx);
            
            return; 
        }

        //Other piece has more charge, so remove other charges and delete active piece
        if (active.Piece.Charge < otherNode.Piece.Charge)
        {
            var takeActivePiece = new TakePiece();
            var neutralizeOtherPiece = new TakePiece(active.Piece.Charge);
            
            Chain(neutralizeOtherPiece, otherNode, ctx);
            Chain(takeActivePiece, active, ctx);
            return; 
        }
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