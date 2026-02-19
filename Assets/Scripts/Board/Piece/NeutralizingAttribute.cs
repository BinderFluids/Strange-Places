public class NeutralizingAttribute : BoardPieceAttribute
{
    private BoardPiece parentPiece;
    public NeutralizingAttribute(BoardPiece parentPiece) : base(parentPiece)
    {
        this.parentPiece = parentPiece;
    }
    
    public override void OnAdd()
    {
        parentPiece.SetResolver(ResolverType.Neutralize);
    }
    
    
    public override void OnRemove()
    {
        parentPiece.SetResolver(ResolverType.None);
    }
}