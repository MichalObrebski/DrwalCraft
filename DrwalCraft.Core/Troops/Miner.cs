namespace DrwalCraft.Core.Troops;

public class Miner : Troop
{
    public Mines.Mine? _queuedTargetMine;
    private Mines.Mine? _targetMine;
    public Mines.Mine? TargetMine {
        get
        {
            return _targetMine;
        }
        set
        {
            if (_targetMine != value)
            {
                _queuedTargetMine = value;
                TargetMineChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void setQueuedTargetMine(Mines.Mine? value)
    {
        _targetMine = value;
    }
    public event EventHandler TargetMineChanged;
    
    public (int, int) Target{
        set{
            var gameObject = GameMap.Map[value.Item1, value.Item2].GameObject;
            TravelTarget = value;

            if(gameObject is Mines.Mine mine){
                TargetMine = mine;
            }
            else{
                TargetMine = null;
            }
        }
    }
    public Miner(int? playerId = null, int? objectId = null) : base("Miner.png", playerId, objectId){
        Name = "Tree miner";
        MaxHp = 64;
        Hp = 64;
        _speed = 8;
    }

    public override void MainAction(){
        if(TravelTarget is not null)
            Move();
        if(TargetMine is not null){
            bool flag = false;
            GameMap.ForEachNeighbourghingField(Position, (field) => {
                if(field.GameObject == TargetMine)
                    flag = true;
            });
            if(flag){
                TargetMine.AddMiner(this);
            }
        }
    }
}