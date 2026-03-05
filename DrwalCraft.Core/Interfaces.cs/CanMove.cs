namespace DrwalCraft.Core;

public interface ICanMove{
    public abstract (int, int) Target {set;}
    public abstract (int, int)? TravelTarget {get;}
    public event EventHandler? TravelTargetChanged;
}