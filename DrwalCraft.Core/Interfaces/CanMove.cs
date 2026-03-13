namespace DrwalCraft.Core.Interfaces;

public interface ICanMove : ICanReceiveTarget{
    public (int, int)? TravelTarget { get; }
    public bool IsMoving { get; }
    public event EventHandler? TravelTargetChanged;
}