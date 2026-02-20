
public class BoardBot : BoardActor
{
    protected override void OnStartTurn()
    {
        workingGrid = workingGrid.Copy(); 
    }
}