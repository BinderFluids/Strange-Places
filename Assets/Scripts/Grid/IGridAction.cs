public interface IGridAction<T> where T : IGridNode
{
    void Execute(T active, Grid<T> ctx);
    void Undo(); 
}