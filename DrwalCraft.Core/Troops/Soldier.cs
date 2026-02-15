using DrwalCraft.Core.Mines;

namespace DrwalCraft.Core.Troops;

public class Soldier: Troop{
    protected int _damage;
    public int Damage{get => _damage;}
    public (int, int) Target{
        set{
            var gameObject = GameMap.Map[value.Item1, value.Item2].GameObject;
            if(gameObject is null){
                TravelTarget = value;
                AttackTarget = null;
            }
            else if(gameObject.PlayerId == Players.enemy.PlayerId){
                AttackTarget = gameObject;
            }
            else if(gameObject is Mine mine && mine.CurrentPlayer == Players.enemy.PlayerId){
                AttackTarget = gameObject;
            }
            else{
                TravelTarget = value;
                AttackTarget = null;
            }
        }
    }
    public override void GetAttacked(int damage, GameObject attacker){
        base.GetAttacked(damage, attacker);
        if(AttackTarget is null)
            AttackTarget = attacker;
    }
    public Soldier(Player player, string Icon) : base(player, Icon){
        _actionSpeed = 6;
        TravelTarget = null;
        AttackTarget = null;
    }
    public override void MainAction(){
        if(AttackTarget is null){
            Move();
        }
        else{
            int distX = Math.Abs(AttackTarget.Position.Item1 - Position.Item1);
            int distY = Math.Abs(AttackTarget.Position.Item2 - Position.Item2);
            if(Math.Max(distX, distY) <= _range && ClearRange(Position, AttackTarget.Position)){
                Attack();
            }
            else{
                if(TravelTarget != AttackTarget.Position)
                    TravelTarget = AttackTarget.Position;
                Move();
            }
        }
    }
    
    public virtual void Attack(){
        if(AttackTarget is null) return;
        if(AttackTarget.IsDead){
            AttackTarget = null;
            return;
        }
        if(AttackTarget is Mine mine && mine.CurrentPlayer == Players.game.PlayerId){
            AttackTarget = null;
            return;
        }

        if(_actionProgress == 0){
            AttackTarget.GetAttacked(_damage, this);
            _actionProgress = _actionSpeed;
        }
        else{
            _actionProgress--;
        }
    }
    private bool ClearRange((int, int) troop, (int, int) target){
        int x = Math.Abs(troop.Item1 - target.Item1);
        int y = Math.Abs(troop.Item2 - target.Item2);
        if(x > y){
            if(target.Item1 < troop.Item1)
                (troop, target) = (target, troop);

            if(troop.Item2 < target.Item2){
                for(int i=0; i<=x-y; i++){
                    var positionX = troop.Item1 + i;
                    var positionY = troop.Item2;
                    while(positionY < target.Item2){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionX++;
                        positionY++;
                    }
                    while(positionX < target.Item1){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionX++;
                    }
                }
            }
            else{
                for(int i=0; i<=x-y; i++){
                    var positionX = troop.Item1 + i;
                    var positionY = troop.Item2;
                    while(positionY > target.Item2){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionX++;
                        positionY--;
                    }
                    while(positionX < target.Item1){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionX++;
                    }
                }
            }
        }
        else if(x < y){
            if(target.Item1 < troop.Item1)
                (troop, target) = (target, troop);

            if(troop.Item2 < target.Item2){
                for(int i=0; i<=y-x; i++){
                    var positionX = troop.Item1;
                    var positionY = troop.Item2 + i;
                    while(positionX < target.Item1){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionX++;
                        positionY++;
                    }
                    while(positionY < target.Item2){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionY++;
                    }
                }
            }
            else{
                for(int i=0; i<=y-x; i++){
                    var positionX = troop.Item1;
                    var positionY = troop.Item2 - i;
                    while(positionX < target.Item1){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionX++;
                        positionY--;
                    }
                    while(positionY > target.Item2){
                        var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                        if(gameObject is not null && gameObject is not Troop)
                            return false;
                        positionY--;
                    }
                }
            }
        }
        else{            
            if(target.Item1 < troop.Item1)
                (troop, target) = (target, troop);

            if(troop.Item2 < target.Item2){
                var positionX = troop.Item1;
                var positionY = troop.Item2;
                while(positionY < target.Item2){
                    var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                    if(gameObject is not null && gameObject is not Troop)
                        return false;
                    positionX++;
                    positionY++;
                }
            }
            else{
                var positionX = troop.Item1;
                var positionY = troop.Item2;
                while(positionY > target.Item2){
                    var gameObject = GameMap.TryGet(positionX, positionY)?.GameObject;
                    if(gameObject is not null && gameObject is not Troop)
                        return false;
                    positionX++;
                    positionY--;
                }
            }
        }
        return true;
    }
}