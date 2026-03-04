using System.Windows.Documents;
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Castle : Factory{
    public Castle(Player player) : base(player, "Castle.png", 4){
        MaxHp = 1024;
        Hp = 1024;
        InProduction = false;

        Products = [
            new ItemToCreate(typeof(Builder), 300, 120),
            new ItemToCreate(typeof(Miner), 300, 120)
        ];
    }

    public void DoMessage(Type troop)
    {
        ObjectsActions.CastleAddMessage(this.Id, troop); //tworzy wiadomość do przesłania do serwera o tworzeniu jednostki
    }
    public override void MainAction(){
        if(_objectInProduction is not null)
            Production();
    }
    protected override void ProductionCompleted(){
        //znajduje najbliższe dostępne pole od zamku w którym się tworzy jednostka
        if(!GameMap.TryGetNearestEmptyField(this, out var field)) return;
        //dodawanie jednostki do mapy
        GameMap.AddObjectToMap(field.Item1, field.Item2, _objectInProduction!);
        _objectInProduction = null;
        InProduction = false;
    }
}