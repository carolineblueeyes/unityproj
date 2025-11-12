// Файл: TileState.cs
// Просто создайте этот файл, больше ничего с ним делать не нужно.

public class TileState
{
    public string Letter;
    public bool IsInteractable;

    public TileState(string letter, bool isInteractable)
    {
        this.Letter = letter;
        this.IsInteractable = isInteractable;
    }
}
