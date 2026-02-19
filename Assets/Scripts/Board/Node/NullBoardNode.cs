public class NullBoardNode : BoardNode
{
    public NullBoardNode(Grid<BoardNode> grid) : base(grid)
    {       
        piece = new BoardPiece(new NoOwner(), int.MaxValue);
    }
}