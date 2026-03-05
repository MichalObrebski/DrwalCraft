namespace DrwalCraft.Core;

public interface ICanAttack{
    public (int, int) Target{set;}
    public GameObject? AttackTarget{get;}
    public event EventHandler? AttackTargetChanged;
}