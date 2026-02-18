using System.Linq;

public abstract class BoardPieceAttribute
{
    public abstract void OnEnterNode(BoardNode node); 
    public abstract void OnExitNode(BoardNode node);
    public abstract void OnChargeChange();
}


public class NeutralizingAttribute : BoardPieceAttribute
{
    public NeutralizingAttribute(BoardPiece parentPiece)
    {
        //parentPiece.SetResolver<NeutralizeCharges>();
    }
    
    public override void OnEnterNode(BoardNode node)
    {
        if (!node.Piece.Attributes.Any(attr => attr is NeutralizingAttribute))
            node.Piece.AddAttribute(new NeutralizingAttribute(node.Piece));
    }

    public override void OnExitNode(BoardNode node)
    {
    }

    public override void OnChargeChange()
    {
    }
}