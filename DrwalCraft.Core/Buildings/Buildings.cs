using DrwalCraft.Core;

namespace DrwalCraft.Core.Buildings;

public interface IBuilding : IGameObject{

}

public class Building : GameObject, IBuilding{
    public Building() : base("Castle.png", size:4){
        Name = "Castle";
    }
}