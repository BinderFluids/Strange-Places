using EventBus;

public class NeutralizeNewPiecesItem : BoardItem
{
    public override void Use()
    {
        EventBus<AddAttributeToBoardModifier>.Raise(new AddAttributeToBoardModifier()
        {
            attributeType =  typeof(NeutralizingAttribute)
        });
    }
}