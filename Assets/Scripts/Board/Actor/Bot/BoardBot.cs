
using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

public delegate bool BoardEvaluatorDelegate(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner, out Vector2Int targetCoords, out IGridAction<BoardNode> action);
public class BoardBot : BoardActor
{
    private BoardEvaluatorDelegate[] evaluationOrder;
    private BoardEvaluatorDelegate[] specialActions;
    private Queue<ActionCoordsPair> actionQueue = new();
    
    [SerializeField] private BoardPlayer player; 
    
    protected override void OnStartTurn()
    {
        workingGrid = workingGrid.Copy(); 
        evaluationOrder = new BoardEvaluatorDelegate[]
        {
            EqualizeColumn,
            TakeOpenRank,
            PlaceRandomly
        };

        RunEvaluations();
    }

    void RunEvaluations()
    {
        foreach (var eval in evaluationOrder)
        {
            if (actionsAvailable == 0) continue;    

            if (eval(workingGrid, this, player, out var targetCoords, out var action))
            {
                Debug.Log("Executing " + eval.Method.Name);
                UseAction(targetCoords, action);
                actionQueue.Enqueue(new ActionCoordsPair()
                {
                    coords = targetCoords, 
                    action = action
                });
                RunEvaluations();
                return;
            }
        }
        
        EndTurn();
    }

    private struct ActionCoordsPair
    {
        public Vector2Int coords;
        public IGridAction<BoardNode> action;
    }

    protected override void OnEndTurn()
    {
        var liveGrid = Board.Instance;
        
        liveGrid.StartObservingAction();
        while (actionQueue.Count > 0)
        {
            var pair = actionQueue.Dequeue();
            liveGrid.Execute(pair.coords, pair.action);
        }
        liveGrid.StopObservingAction();
    }


    Dictionary<int, int> GetTotalColumnCharge(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner)
    {
        Dictionary<int, int> totalColumnCharge = new(); 
        for (int i = 0; i < ctx.Width; i++) totalColumnCharge.Add(i, 0);
        
        //tally up charges
        for (int y = 0; y < ctx.Height; y++)
        for (int x = 0; x < ctx.Width; x++)
        {
            if (ctx.TryGet(x, y, out BoardNode node))
            {
                if (!node.IsOccupied()) continue;
                Debug.Log($"Column Charge Dictionary found {node.Piece} at ({x}, {y}).");
                
                if (node.Piece.Owner == activeOwner) totalColumnCharge[x] += node.Piece.Charge;
                else totalColumnCharge[x] -= node.Piece.Charge;
            }
        }

        foreach (var kvp in totalColumnCharge) Debug.Log($"New Column Dictionary -> {kvp.Key}: {kvp.Value}");
        return totalColumnCharge; 
    }
    
    public bool EqualizeColumn(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner, 
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        action = new GivePiece(new BoardPiece(activeOwner));
        var totalColumnCharge = GetTotalColumnCharge(ctx, activeOwner, otherOwner);
        
        //find the column with the greatest difference
        int greatestDifference = 0;
        int targetColumn = 0;
        bool foundTarget = false;
        foreach (var kvp in totalColumnCharge)
        {
            int charge = kvp.Value;
            if (charge >= 0) continue; //active owner is in control of the rank

            if (charge < greatestDifference)
            {
                foundTarget = true;
                targetColumn = kvp.Key;
                greatestDifference = charge;
            }
        }

        //target coords is the bot's first rank in line with the target column
        targetCoords = new Vector2Int(targetColumn, ctx.Height - 1);
        
        return foundTarget;
    }
    
    public bool TakeOpenRank(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner,
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        targetCoords = Vector2Int.zero; 
        
        var totalColumCharge = GetTotalColumnCharge(ctx, activeOwner, otherOwner);
        List<int> openRanks = new();

        bool foundTarget = false; 
        foreach (var kvp in totalColumCharge)
        {
            if (kvp.Value == 0)
            {
                foundTarget = true; 
                openRanks.Add(kvp.Key);
            }
        }
        
        action = new GivePiece(new BoardPiece(activeOwner));
        if (!foundTarget) return false;
        
        targetCoords = new Vector2Int(openRanks.Random(), ctx.Height - 1);
        return true;
    }

    public bool PlaceRandomly(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner,
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        targetCoords = new Vector2Int(Random.Range(0, ctx.Width - 1), ctx.Height - 1);
        action = new GivePiece(new BoardPiece(activeOwner));
        
        return true;
    }
}