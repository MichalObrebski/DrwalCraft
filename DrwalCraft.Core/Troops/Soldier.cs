namespace DrwalCraft.Core.Troops;

public class Soldier: Troop{
    protected int _damage;
    public int Damage{get => _damage;}
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
    public Soldier(string Icon, int? playerId = null, int? objectId = null) : base(Icon, playerId, objectId){
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
    
    public virtual void Attack(){
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