using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class BoardConflictResolver : BoardActionChain
{
    protected BoardNode otherNode;
    protected Vector2Int direction;
    protected int charge;
    protected BoardConflictResolver (BoardNode otherNode, Vector2Int direction, int charge = 0)
    {
        this.otherNode = otherNode;
        this.direction = direction;
        this.charge = charge;
    }

    public static BoardConflictResolver Create(ResolverType type, BoardNode otherNode, Vector2Int direction, int charge = 0)
    {
        switch (type)
        {
            case ResolverType.None: return Create<None>(otherNode, direction, charge);
            case ResolverType.Neutralize: return Create<Neutralize>(otherNode, direction, charge);
            case ResolverType.TryPush: return Create<TryPush>(otherNode, direction, charge);
            default: return Create<None>(otherNode, direction, charge); 
        }
    }
    
    public static T Create<T>(BoardNode otherNode, Vector2Int direction, int charge = 0) where T : BoardConflictResolver
    {
        return (T)System.Activator.CreateInstance(typeof(T), otherNode, direction, charge);
    }
}