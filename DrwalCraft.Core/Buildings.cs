using DrwalCraft.Core;

namespace DrwalCraft.Core.Buildings;

public interface IBuilding : IGameObject{

}

public class Building : GameObject, IBuilding{
    public Building() : base(new Uri("../Assets/Icons/Castle.png", UriKind.Relative)){
        Name = "Castle";
        Size = 4;
    }
}