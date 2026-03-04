namespace DrwalCraft.Core.Buildings;

public abstract class Building : GameObject{
    public Building(Player player, string? Icon = null, int size=1) : base(player, Icon, size:size){}
    
}