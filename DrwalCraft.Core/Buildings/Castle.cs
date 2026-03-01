using System.Windows.Documents;
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Buildings;

public class Castle : Building{
    public Castle(Player player) : base(player, "Castle.png", 4){
        Name = "Castle";
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
    //pojawia się taki problem (czasami przy pierwszej produkcji ale może też i później(nie zauważyłem tego narazie)) że jednostka natychmiastowo się pojawia bez czekania na zakończenie czasu produkcji
    //produkcja też się wtedy kończy więc problem leży pewnie tam
    //wstawiłem spinlocka i chyba działa
    public override void Create(ItemToCreate item){
        if(InProduction) return;

        bool lockTaken = false;
        try{
            _productionLock.Enter(ref lockTaken);

            _objectInProduction = item.Make(Owner);
            if(_objectInProduction != null){
                InProduction = true;
                ProductionTime = item.ProductionTime;
                Owner.Wood -= item.PriceWood;
            }
        }
        finally{
            if(lockTaken)
                _productionLock.Exit();
        }
    }
    public override void MainAction(){
        if(_objectInProduction == null) return;

        bool lockTaken = false;
        try{
            _productionLock.Enter(ref lockTaken);
            if(_productionProgress >= _productionTime){        
                if(!GameMap.TryGetNearestEmptyField(this, out var field)){
                    return;
                }
                GameMap.AddObjectToMap(field.Item1, field.Item2, _objectInProduction);
                _objectInProduction = null;
                InProduction = false;
            }
            else{
                ProductionProgress++;
            }
        }
        finally{
            if(lockTaken)
                _productionLock.Exit();
        }
    }
}