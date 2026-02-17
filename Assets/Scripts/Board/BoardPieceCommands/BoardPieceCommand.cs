using System;

public abstract class BoardPieceCommand : ICommand
{
    public event Action onExecute;
    public event Action onUndo;
    public abstract void Execute();
    public abstract void Undo();
}

public class BoardPiecePlace : BoardPieceCommand
{
    public override void Execute()
    {
    }

    public override void Undo()
    {
    }
}