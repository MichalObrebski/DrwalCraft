using Engine;
using Engine.Game;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DrwalCraft.Game.Troops;
public interface ITroop : Engine.Game.IGameObject{
    public void MainAction();
    public void Move();
}

public abstract class Troop : GameObject, ITroop{
    protected int range;
    protected int _speed;
    protected int _actionSpeed;
    protected (int, int)? _travelTarget;
    protected int _moveProgress;
    protected Stack<(int, int)> _travelPath = new ();
    public (int, int)? TravelTarget{
        get{
            return _travelTarget;
        }
        set{
            _travelTarget = value;
            if(value == null) return;
            
            _travelPath = Engine.Game.GameMap.BFS(Position, value.Value);
        }
    }
    public Troop(Uri IconUri) : base(IconUri){}
    public GameObject? AttackTarget{get;set;}
    public abstract void MainAction();
    public abstract void Move();
}

public class Soldier: Troop{
    public Soldier() : base(new Uri("../Assets/Icons/Tree.png", UriKind.Relative)){
        _speed = 6;
        _actionSpeed = 1;
        TravelTarget = null;
        AttackTarget = null;
        _moveProgress = 0;
    }
    public override void MainAction()
    {
        //jesli odleglosc od Targetu < range - w jednej lini bez przeszkód
        
    }
    public override void Move(){
        if(TravelTarget == null || TravelTarget == Position) return;
        if(_travelPath.Count == 0) return;

        if(_moveProgress == 0){
            (int, int) nextPosition = _travelPath.Pop();
            if(nextPosition == Position) return;
            GameObject? self = Engine.Game.GameMap.Map[this.Position.Item1, this.Position.Item2].GameObject;
            
            if(Engine.Game.GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject != null){
                List<(int, int)> currentPath = new ();
                int maxRadius;
                if(Engine.Game.GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject is Troop)
                    maxRadius = 2;
                else
                    maxRadius = 4;

                int radius = 0;
                for(; radius < maxRadius - 1 && _travelPath.Count != 0; radius++)
                    currentPath.Add(_travelPath.Pop());
                var correctedPath = Engine.Game.GameMap.CorrectPath(Position, currentPath, radius+1);

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
                Engine.Game.GameMap.Map[this.Position.Item1, this.Position.Item2].GameObject = null;
                Engine.Game.GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject = self;
                this.Position = nextPosition;
                _moveProgress = _speed;
            }
        }

        else{
            _moveProgress --;
        }
    }
}