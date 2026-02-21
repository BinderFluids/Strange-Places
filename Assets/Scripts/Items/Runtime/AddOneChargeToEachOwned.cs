public class AddOneChargeToEachOwned : BoardItem
{
    public override void Use()
    {
        Board.Instance.Grid.ForEach(node =>
        {
            if (!node.IsOccupied()) return;
            if (node.Piece.Owner is not BoardPlayer) return;
            
            node.AddPiece(new BoardPiece(node.Piece.Owner));
        });
    }
}