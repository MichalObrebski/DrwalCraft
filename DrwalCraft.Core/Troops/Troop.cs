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

// public enum 

public abstract class Troop : GameObject, ITroop{
    protected int _range;
    protected int _speed;
    protected int _actionSpeed;
    protected (int, int)? _travelTarget;
    public (int, int)? _queuedTravelTarget;
    public GameObject? _attackTarget;
    public GameObject? _queuedAttackTarget;
    public GameObject? AttackTarget{
        get => _attackTarget;
        set
        {
            if (_attackTarget != value)
            {
                _queuedAttackTarget = value;
                AttackTargetChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void SetQueuedAttackTarget(GameObject? attackTarget)
    {
        _attackTarget = attackTarget;
    }
    public event EventHandler AttackTargetChanged;
    
    protected int _moveProgress;
    protected int _actionProgress;
    protected Stack<(int, int)> _travelPath = new ();
    public (int, int)? TravelTarget{
        get{
            return _travelTarget;
        }
        set{
            if (_travelTarget != value)
            {
                _queuedTravelTarget = value;
                TravelTargetChanged?.Invoke(this, EventArgs.Empty);
                //Queues to change travelTarget in Offset number of ticks - neccesery for better clients synchronisation
            }
            if(value == null) return;
            
            _travelPath = GameMap.BFS(Position, value.Value);
        }
    }

    public void SetQueuedTravelTarget ((int,int)?  travelTarget)
    {
        _travelTarget = travelTarget;
    }
    public event EventHandler TravelTargetChanged;
    
    public Troop(string Icon, int? playerId = null, int? objectId = null) : base(Icon, playerId, objectId){}
    public abstract void MainAction();
    public virtual void Move(){
        if(TravelTarget == null || TravelTarget == Position) return;
        if(_travelPath.Count == 0){
            if(Math.Abs(TravelTarget.Value.Item1 - Position.Item1) > 1 && Math.Abs(TravelTarget.Value.Item2 - Position.Item2) > 1) 
                TravelTarget = _travelTarget;
            return;
        }

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

            if(nextPosition != (-1, -1) && GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject == null){
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