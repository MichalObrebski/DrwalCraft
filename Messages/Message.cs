using System.Text.Json.Serialization;

namespace Messages;

public enum MessageType
{
    PlayerAction,
    ServerSnapshot
}

public enum ActionType
{
    CreateUnitBarrack,
    MoveUnit,
    AttackUnit,
    GoMine,
    Build
}

public enum UnitType
{
    Soldier,
    Knight,
    Archer,
    TreeMiner,
    Builder,
    Barrack
}

public class Message
{
    public string From { get; set; } = "default";
    public string Text { get; set; }= "default";
    public int Tick { get; set; }
    public ActionType ActionType { get; set; }
    public UnitType UnitType { get; set; }
    public int Id { get; set; }
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public int? TargetId { get; set; }
    
    public Message()
    {
        
    }

    public Message(string from, string text)
    {
        //do testow
        this.From = from;
        this.Text = text;
    }
    
    //konstruktor do wszelkich czynności gdzie targetem jest obiekt: atak, pójście do kopalni
    public Message(ActionType actionType, UnitType unitType, int id, int? targetId, int tick = 0)
    {
        this.ActionType = actionType;
        this.UnitType = unitType;
        this.Id = id;
        this.TargetId = targetId;
        this.Tick = tick;
    }
    
    //konstruktor do ruchu
    public Message(ActionType actionType, UnitType unitType, int id, (int,int)? position, int tick = 0)
    {
        this.ActionType = actionType;
        this.UnitType = unitType;
        this.Id = id;
        if(position==null)this.PositionX = this.PositionY = null;
        else
        {
            this.PositionX = (((int, int))position).Item1;
            this.PositionY = (((int,int))position).Item2;
        }
        this.Tick = tick;
    }
    
    //konstruktor do tworzenia jednostki w baraku i tworzenia budowli przez buildera 
    public Message(int id, ActionType actionType, UnitType unitType, int tick = 0)
    {
        this.Id = id;
        this.ActionType = actionType;
        this.UnitType = unitType;
        this.Tick = tick;
    }
    
}
