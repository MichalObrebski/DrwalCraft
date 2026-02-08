using System.Diagnostics;
using System.Runtime;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DrwalCraft.Core.Troops;
public interface ITroop : IGameObject{
    public void MainAction();
    public void Move();
}

public abstract class Troop : GameObject, ITroop{
    protected int _range;
    protected int _speed;
    protected int _actionSpeed;
    protected (int, int)? _travelTarget;
    protected int _moveProgress;
    protected int _actionProgress;
    protected Stack<(int, int)> _travelPath = new ();
    public (int, int)? TravelTarget{
        get{
            return _travelTarget;
        }
        set{
            _travelTarget = value;
            if(value == null) return;
            
            _travelPath = GameMap.BFS(Position, value.Value);
        }
    }
    public Troop(Uri IconUri, int? playerId = null, int? objectId = null) : base(IconUri, playerId, objectId){}
    public abstract void MainAction();
    public virtual void Move(){
        if(TravelTarget == null || TravelTarget == Position) return;
        if(_travelPath.Count == 0) return;

        if(_moveProgress == 0){
            (int, int) nextPosition = _travelPath.Pop();
            if(nextPosition == Position) return;
            GameObject? self = GameMap.Map[this.Position.Item1, this.Position.Item2].GameObject;
            
            if(GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject != null){
                List<(int, int)> currentPath = new ();
                int maxRadius;
                if(GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject is Troop)
                    maxRadius = 2;
                else
                    maxRadius = 4;

                int radius = 0;
                for(; radius < maxRadius - 1 && _travelPath.Count != 0; radius++)
                    currentPath.Add(_travelPath.Pop());
                var correctedPath = GameMap.CorrectPath(Position, currentPath, radius+1);

                if(correctedPath.Count == 0){
                    currentPath.Reverse();
                    foreach(var field in currentPath)
                        _travelPath.Push(field);
                    _moveProgress = _speed;
                    _travelPath.Push(nextPosition);
                    return;
                }
                else{
                    foreach(var field in correctedPath)
                        _travelPath.Push(field);
                    nextPosition = _travelPath.Pop();
                }
            }

            if(nextPosition != (-1, -1)){
                GameMap.Map[this.Position.Item1, this.Position.Item2].GameObject = null;
                GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject = self;
                this.Position = nextPosition;
                _moveProgress = _speed;
            }
        }

        else{
            _moveProgress --;
        }
    }
}

public class Soldier: Troop{
    private int _damage;
    public GameObject? AttackTarget{get;set;}
    public (int, int) Target{
        set{
            var gameObject = GameMap.Map[value.Item1, value.Item2].GameObject;
            if(gameObject is null){
                TravelTarget = value;
                AttackTarget = null;
            }
            else if(gameObject.PlayerId != GameObjectId.PlayerId && gameObject.PlayerId != 0){
                AttackTarget = gameObject;
            }
            else{
                TravelTarget = value;
                AttackTarget = null;
            }
        }
    }
    public Soldier(int? playerId = null, int? objectId = null) : base(new Uri("../Assets/Icons/Knight.png", UriKind.Relative), playerId, objectId){
        _speed = 6;
        MaxHp = 50;
        Hp = 50;
        _actionSpeed = 6;
        TravelTarget = null;
        AttackTarget = null;
        _moveProgress = 0;
        _range = 1;
        _damage = 16;
        Name = "Soldier";
    }
    public override void MainAction(){
        if(AttackTarget is null){
            Move();
        }
        else{
            int distX = Math.Abs(AttackTarget.Position.Item1 - Position.Item1);
            int distY = Math.Abs(AttackTarget.Position.Item2 - Position.Item2);
            if(Math.Max(distX, distY) <= _range){
                Attack();
            }
            else{
                if(TravelTarget != AttackTarget.Position)
                    TravelTarget = AttackTarget.Position;
                Move();
            }
        }
    }
    
    public void Attack(){
        if(AttackTarget is null) return;
        if(AttackTarget.IsDead){
            AttackTarget = null;
            return;
        }

        if(_actionProgress == 0){
            AttackTarget.Hp -= _damage;
            _actionProgress = _actionSpeed;
        }
        else{
            _actionProgress--;
        }
    }
}