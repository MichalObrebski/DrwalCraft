using System.Diagnostics.Metrics;
using DrwalCraft.Core.Troops;

namespace DrwalCraft.Core.Mines;

public class Mine : GameObject{
    public List<Miner> Miners {get; set;}
    public int Player{get; private set;}

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
        _canDie = false;
        Player = Players.game.PlayerId;
    }

    public override void GetAttacked(int damage, GameObject attacker){
        base.GetAttacked(0, attacker);
        if(Player == Players.game.PlayerId) return;

        if(Miners.Count > 0){
            var miner = Miners.First();
            var tempHp = miner.Hp;
            miner.Hp -= damage/2;
            var deltaHp = miner.Hp > 0 ? tempHp - miner.Hp : tempHp;
            Hp -= deltaHp * 2;
            if(miner.IsDead){
                Miners.RemoveAt(0);
                OnPropertyChanged(nameof(Miners));
                MaxHp -= miner.MaxHp;
            }
        }
        if(Hp <= 0 || Miners.Count == 0){
            Hp = 0;
            MaxHp = 0;
            Player = Players.game.PlayerId;
        }
    }

    public void AddMiner(Miner miner){
        if(Miners.Count >= 5) return;
        for(int i=Position.Item1-1; i<=Position.Item1+Size; i++){
            for(int j=Position.Item2-1; j<=Position.Item2+Size; j++){
                var field = GameMap.TryGet(i, j);
                if(field is null) continue;
                if(field.Value.GameObject == miner){
                    if(Player == Players.game.PlayerId)
                        Player = miner.PlayerId;

                    if(Player != miner.PlayerId)
                        return;

                    Miners.Add(miner);
                    Hp += miner.Hp * 2;
                    MaxHp += miner.MaxHp * 2;
                    GameMap.Map[i, j].GameObject = null;
                    OnPropertyChanged(nameof(Miners));
                    return;
                }
            }
        }
    }

    public override void MainAction(){
        if(Miners.Count == 0) return;
        if(_progress >= _miningTime){
            Console.WriteLine(Miners.Count);
            Progress = 0;
        }
        else{
            Progress++;
        }
    }
}