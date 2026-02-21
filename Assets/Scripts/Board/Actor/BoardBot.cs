
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityUtils;

public delegate bool BoardEvaluatorDelegate(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner, out Vector2Int targetCoords, out IGridAction<BoardNode> action);

public class BoardBot : BoardActor
{
    private BoardEvaluatorDelegate[] evaluationOrder;
    private BoardEvaluatorDelegate[] specialActions;
    private Queue<ActionCoordsPair> actionQueue = new();
    
    [SerializeField] private BoardPlayer player;
    private int turnIndex = 0; 
    
    protected override void OnStartTurn()
    {
        turnIndex++;
        if (turnIndex % 6 == 0)
            actionsAvailable++;
        
        workingGrid = workingGrid.Copy(); 
        evaluationOrder = new BoardEvaluatorDelegate[]
        {
            EqualizeColumn,
            TakeOpenRank,
            PushAdvantage,
            PlaceRandomly
        };
        specialActions = new BoardEvaluatorDelegate[]
        {
            RemoveChargesIfOverFive,
        };

        RunEvaluations();
    }

    //Run evaluations on board snapshot
    async UniTaskVoid RunEvaluations()
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
                RunEvaluations().Forget();
                return;
            }
        }

        await TransferToLiveGrid();
        await UniTask.WaitForSeconds(1f);
        
        EndTurn();
    }

    
    //Run special actions on active board
    public async UniTask TrySpecialActions(Grid<BoardNode> ctx)
    {
        foreach (var eval in specialActions)
            eval(ctx, this, player, out var _, out var _);
    }

    private struct ActionCoordsPair
    {
        public Vector2Int coords;
        public IGridAction<BoardNode> action;
    }
    
    async UniTask TransferToLiveGrid()
    {
        var liveGrid = Board.Instance;
        
        liveGrid.StartObservingAction();
        while (actionQueue.Count > 0)
        {
            var pair = actionQueue.Dequeue();
            liveGrid.Execute(pair.coords, pair.action);
            await UniTask.WaitForSeconds(.25f); 
        }
        liveGrid.StopObservingAction();
    }


    Dictionary<int, int> GetTotalCharges(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner,
        bool neutralize = true)
    {
        Dictionary<int, int> totalColumnCharge = new();
        
        //Ignore null edges
        for (int i = 1; i < ctx.Width - 1; i++)
            totalColumnCharge.Add(i, 0);
        
        //tally up charges
        for (int y = 0; y < ctx.Height; y++)
        for (int x = 0; x < ctx.Width; x++)
        {
            if (ctx.TryGet(x, y, out BoardNode node))
            {
                if (!totalColumnCharge.ContainsKey(x)) continue;
                if (node is NullBoardNode || node is GiveItemBoardNode) continue; 
                if (!node.IsOccupied()) continue;
                
                if (node.Piece.Owner == activeOwner) totalColumnCharge[x] += node.Piece.Charge;
                else totalColumnCharge[x] += neutralize ? -node.Piece.Charge : node.Piece.Charge;
            }
        }
        return totalColumnCharge; 
    }
    
    #region Board Evaluators
    public bool EqualizeColumn(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner, 
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        action = null;
        var totalColumnCharge = GetTotalCharges(ctx, activeOwner, otherOwner, false);
        
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
        action = new GivePiece(new BoardPiece(activeOwner));
        
        return foundTarget;
    }
    
    public bool TakeOpenRank(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner,
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        targetCoords = Vector2Int.zero;
        action = null; 
        
        var totalColumCharge = GetTotalCharges(ctx, activeOwner, otherOwner);
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
        
        if (!foundTarget) return false;

        action = new GivePiece(new BoardPiece(activeOwner));
        targetCoords = new Vector2Int(openRanks.Random(), ctx.Height - 1);
        return true;
    }

    public bool PushAdvantage(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner,
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        targetCoords = Vector2Int.zero;
        action = null;
        
        var totalColumCharge = GetTotalCharges(ctx, activeOwner, otherOwner);

        bool foundTarget = false;
        int currentLargetAdvantage = 0;
        foreach (var kvp in totalColumCharge)
        {
            int column = kvp.Key;
            int charge = kvp.Value;

            if (charge <= 0) continue;
            if (column == 0 || column == workingGrid.Width - 1) continue; 
            if (charge > currentLargetAdvantage)
            {
                foundTarget = true;
                targetCoords = new Vector2Int(column, ctx.Height - 1);
                currentLargetAdvantage = charge;
                action = new GivePiece(new BoardPiece(activeOwner));
            }
        }

        return foundTarget;
    }

    
    public bool PlaceRandomly(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner,
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        targetCoords = new Vector2Int(Random.Range(0, ctx.Width - 1), ctx.Height - 1);
        action = new GivePiece(new BoardPiece(activeOwner));
        
        return true;
    }
    #endregion
    
    #region Special Actions

    bool RemoveChargesIfOverFive(Grid<BoardNode> ctx, IPieceOwner activeOwner, IPieceOwner otherOwner,
        out Vector2Int targetCoords, out IGridAction<BoardNode> action)
    {
        targetCoords = Vector2Int.zero;
        action = null; 
        
        ctx.ForEach(node =>
        {
            if (!node.IsOccupied()) return;
            if (node.Piece.Owner != otherOwner) return;
            if (node.Piece.Charge < 5) return;
            
            node.TakePiece(3);
        });

        return true;
    }
    
    #endregion
}