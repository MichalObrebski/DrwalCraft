namespace DrwalCraft.Core.Interfaces;

public interface ICanAttack : ICanReceiveTarget{
    public GameObject? AttackTarget{get;}
    public event EventHandler? AttackTargetChanged;
}