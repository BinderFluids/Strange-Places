public interface IGridNode
{
    bool IsOccupied();
    void Update();
    IGridNode Copy(); 
}