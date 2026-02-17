using System;

public interface ICommand
{
    event Action onExecute;
    event Action onUndo;
    void Execute();
    void Undo();
}