using DrwalCraft.Core;

namespace DrwalCraft.Core.Buildings;

public interface IBuilding : IGameObject{

}

public class Building : GameObject, IBuilding{
    public List<Type> Products {get; set;}
    public Building(string Icon, int size) : base(Icon, size:size){
        
    }
}