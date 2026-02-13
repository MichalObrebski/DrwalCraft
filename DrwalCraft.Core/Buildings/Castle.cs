
namespace DrwalCraft.Core.Buildings;

public class Castle : Building{
    public Castle() : base("Castle.png", 4){
        Name = "Castle";
        MaxHp = 1024;
        Hp = 1024;
    }

    public override void MainAction(){
        return;
    }

    public override void Produce(Type item){
        return;
    }
}