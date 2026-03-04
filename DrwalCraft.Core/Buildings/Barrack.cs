using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Barrack : Factory{
    public Barrack(Player player) : base(player, "Barrack.png", 3){
        Name = "Barrack";
        MaxHp = 500;
        Hp = 500;
        Products = [
            new ItemToCreate(typeof(Knight), 500, 120),
            new ItemToCreate(typeof(Archer), 600, 180),
        ];
        InProduction = false;
        Console.WriteLine("Id barracka" + this.Id);
    }

    public void DoMessage(Type troop)
    {
        ObjectsActions.BarrackAddMessage(this.Id, troop); //tworzy wiadomość do przesłania do serwera o tworzeniu jednostki
    }
    
    public override void MainAction(){
        if(_objectInProduction is not null)
            Production();
    }
    
    protected override void ProductionCompleted(){
        //znajduje najbliższe dostępne pole od baraka w którym się tworzy jednostka
        if(!GameMap.TryGetNearestEmptyField(this, out var field)) return;
        //dodawanie jednostki do mapy
        GameMap.AddObjectToMap(field.Item1, field.Item2, _objectInProduction!);
        _objectInProduction = null;
        InProduction = false;
    }
}