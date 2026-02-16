
[System.Serializable]
public class Grid<T> where T : IGridNode
{
    private T[,] grid;
    public T this[int x, int y] => Get(x, y);
    
    public Grid(int width, int height)
    {
        grid = new T[width, height];
    }

    private bool InBounds(int x, int y) => (0 <= x && x < grid.GetLength(0) && 0 <= y && y < grid.GetLength(1));
    private bool IsOccupied(int x, int y) => grid[x, y] != null;

    public T Get(int x, int y)
    {
        if (InBounds(x, y) && IsOccupied(x, y))
        {
            return grid[x, y];
        }
        return default;
    }
    
    public bool Set(int x, int y, T item)
    {
        if (InBounds(x, y) && !IsOccupied(x, y))
        {
            grid[x, y] = item;
            return true;
        }
        return false;
    }
    public bool Clear(int x, int y) => Set(x, y, default);
}