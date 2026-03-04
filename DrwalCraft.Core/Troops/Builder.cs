using System.Xml.Linq;
using DrwalCraft.Core.Buildings;

namespace DrwalCraft.Core.Troops;

public class Builder : Troop, ICanCreate{
    public List<ItemToCreate> Products {get; set;}
    private ItemToCreate? _itemToCreate = null;
    private Building? _objectInProduction;
    public bool InProduction {set; get;}
    public override (int, int) Target {
        set => TravelTarget = value;
    }
    public GameObject? ObjectInProduction {
        get => _objectInProduction;
        set{
            if(value is Building building)
                _objectInProduction = building;
            else
                _objectInProduction = null;
        }
    }
    public int ProductionTime {get => 0;}
    public int ProductionProgress {get => 0;}

    public Builder(Player player) : base(player, "Builder.png"){
        Name = "Builder";
        MaxHp = 50;
        Hp = 50;
        InProduction = false;
        _speed = 8;
        Products = [
            new ItemToCreate(typeof(Barrack), 2000, 0)
        ];
    }

    public override void MainAction(){
        if(_itemToCreate is not null){
            Build(_itemToCreate);
            _itemToCreate = null;
        }
        if(TravelTarget != null)
            Move();
    }

    public void DoMessage(Type building)
    {
        ObjectsActions.BuildAddMessage(this.Id, _objectInProduction);
    }
    //z metodą create jest taki problem że jest wywoływana z wątku UI przez co nie jest zsynchronizowana z tickiem i moze wystąpić taki problem że zostanie wywołana podczas chodzenia co skutkuje tym że builder stoi obok podczas budowy a po zbudowaniu budynku jest w dwóch miejscach. dodatkowo zaczyna zapierdalać zamiast chodzić po zbudowaniu baraku trzeba zrobić tak żebyśmy mieli pewność że Create i MainAction będą zsynchronizowane nie tylko w builderze ale we wszystkich ICanCreate. 
    //naprawiałem to ale nadal nie działa i nadal builder wali w nos na budowie a potem zapierdala 30m/s
    //dodawałem go jeszcze raz do mapy po zakończeniu budowy przez co był dodawany drugi raz do ExistingObjects więc wykonywał .Move() dwa razy na ture XD
    public void Create(ItemToCreate item){
        if(InProduction) return;

        InProduction = true;
        _itemToCreate = item;
        TravelTarget = null;
    }
    public void Build(ItemToCreate item){
        if(_itemToCreate == null) return;

        ObjectInProduction = item.Make(Owner);
        if(_objectInProduction == null){
            InProduction = false;
            _itemToCreate = null;
            return;
        }

        int size = _objectInProduction.Size;
        if(EnoughSpaceToBuild(size, out var placeToBuild)){
            int x, y; (x, y) = placeToBuild;
            TravelTarget = null;
            GameMap.AddObjectToMap(x, y, new Construction(Owner, _objectInProduction, this));
            ExistingObjects.Remove(this);
            Owner.Wood -= item.PriceWood;
        }
        else{
            InProduction = false;
            _objectInProduction = null;
        }
    }
    private bool EnoughSpaceToBuild(int size, out (int, int) placeToBuild){
        for(int i = Position.Item1 - size + 1; i <= Position.Item1; i++)
            for(int j = Position.Item2 - size + 1; j <= Position.Item2; j++)
                if(IsAbleToBuild(i, j, size)){
                    placeToBuild = (i, j);
                    return true;
                }
        placeToBuild = (-1, -1);
        return false;
    }
    private bool IsAbleToBuild(int x, int y, int size){
        //czy mieści się na mapie
        if(x < 0 || y < 0) return false;
        if(x + size > GameMap.Size || y + size > GameMap.Size) return false;

        //czy nie ma obiektów na polach na których chce budować
        for(int i = x; i < x+size; i++){
            for(int j = y; j < y+size; j++){
                if(GameMap.Map[i,j] != null && GameMap.Map[i,j] != this)
                    return false;
            }
        }

        return true;
    }
}