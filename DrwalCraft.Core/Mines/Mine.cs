using System.Diagnostics.Metrics;
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Mines;

public class Mine : GameObject{
    public List<Miner> Miners {get; set;}
    public int CurrentPlayer{get; private set;}

    private int _progress;
    public int Progress{
        get => _progress;
        set{
            _progress = value;
            OnPropertyChanged(nameof(Progress));
        }
    }

    private int _miningTime;
    public int MiningTime{
        get => _miningTime;
        set{
            _miningTime = value;
            OnPropertyChanged(nameof(MiningTime));
        }
    }

    public Mine(Player player): base(player, "Mine.png", size: 3){
        Name = "Wood Mine";
        Miners = new();
        MiningTime = 18;
        _mortal = false;
        CurrentPlayer = Players.game.PlayerId;
    }

    public override void GetAttacked(int damage, GameObject attacker){
        base.GetAttacked(0, attacker);
        if(CurrentPlayer == Players.game.PlayerId) return;

        if(Miners.Count > 0){
            var miner = Miners.First();
            var tempHp = miner.Hp;
            miner.Hp -= damage/2;
            var deltaHp = miner.Hp > 0 ? tempHp - miner.Hp : tempHp;
            Hp -= deltaHp * 2;
            if(miner.Hp <= 0){
                Miners.RemoveAt(0);
                OnPropertyChanged(nameof(Miners));
                MaxHp -= miner.MaxHp * 2;
            }
        }
        if(Hp <= 0 || Miners.Count == 0){
            Hp = 0;
            Miners.Clear();
            MaxHp = 0;
            CurrentPlayer = Players.game.PlayerId;
        }
    }

    public void AddMiner(Miner miner){
        if(Miners.Count >= 5) return;
        for(int i=Position.Item1-1; i<=Position.Item1+Size; i++){
            for(int j=Position.Item2-1; j<=Position.Item2+Size; j++){
                if(!GameMap.IndexBoundSafeGet(i, j, out var field) && field is null) continue;
                if(field == miner){
                    if(CurrentPlayer == Players.game.PlayerId)
                        CurrentPlayer = miner.Owner.PlayerId;

                    if(CurrentPlayer == miner.Owner.PlayerId){
                        Miners.Add(miner);
                        Hp += miner.Hp * 2;
                        MaxHp += miner.MaxHp * 2;
                        GameMap.Map[i, j] = null;
                        OnPropertyChanged(nameof(Miners));
                    }
                    return;
                }
            }
        }
    }

    public override void MainAction(){
        if(Miners.Count == 0) return;
        if(_progress >= _miningTime){
            Player? owner = null;
            if(CurrentPlayer == Players.you.PlayerId)
                owner = Players.you;
            else if(CurrentPlayer == Players.enemy.PlayerId){
                owner = Players.enemy;
            }
            if(owner is not null)
                foreach(var _ in Miners){
                    owner.Wood += 10; 
                }
            Progress = 0;
        }
        else{
            Progress++;
        }
    }
}