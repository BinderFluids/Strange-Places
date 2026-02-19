
using System.Collections.Generic;

public class BoardNodeGrid : Grid<BoardNode>
{
    public BoardNodeGrid(int width, int height) : base(width, height) {}
    
    public void TransferChanges(Queue<BoardAction> changes)
    {
        while (changes.Count > 0)
        {
            var newAction = changes.Dequeue(); 
            Execute(newAction.ActiveCoords, newAction);
        }
    }

}