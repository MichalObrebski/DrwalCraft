using System.Diagnostics;
using System.Runtime;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DrwalCraft.Core.Troops;

public abstract class Troop : GameObject, ICanMove{
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
            if(value != null)
                value.BitingTheDust += (_, _) => { AttackTarget = null; };
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
    protected ObjectMovement _travelPath;
    public (int, int)? TravelTarget{
        get{
            return _travelTarget;
        }
        set{
            if (_travelTarget != value)
            {
                _queuedTravelTarget = value;
                TravelTargetChanged?.Invoke(this, EventArgs.Empty);
                //Queues to change travelTarget (by SetQueuedTravelTarget) in Offset number of ticks
            }
            //if(value == null) return;
            //_travelPath = GameMap.BFS(Position, value.Value); => is now in SetQueuedTravelTarget
        }
    }
    public abstract (int, int) Target{set;}

    public void SetQueuedTravelTarget ((int,int)?  travelTarget)
    {
        _travelTarget = travelTarget;
        if(travelTarget != null)
            _travelPath = new(this, travelTarget.Value);
        else
            _travelPath.Clear();
    }
    public event EventHandler TravelTargetChanged;
    
    public Troop(Player player, string Icon) : base(player, Icon){}
    public virtual void Move(){
        if(TravelTarget == null || TravelTarget == Position) return;
        // if(_travelPath.Count == 0){
        //     if(Math.Abs(TravelTarget.Value.Item1 - Position.Item1) > 1 && Math.Abs(TravelTarget.Value.Item2 - Position.Item2) > 1) 
        //         TravelTarget = _travelTarget;
        //     return;
        // }

        if(_moveProgress == 0){
            if(_travelPath.Move()){
                _moveProgress = _speed;
            }
            else{
                TravelTarget = null;
            }
        }

        else{
            _moveProgress --;
        }
    }
}