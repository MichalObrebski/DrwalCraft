using System.Text.Json.Serialization;

namespace Messages;

public enum ActionType
{
    CreateUnit,
    MoveUnit,
    AttackUnit
}

public enum UnitType
{
    Soldier
}

public class Message
{
    public string From { get; set; } = "default";
    public string Text { get; set; }= "default";
    public ActionType ActionType { get; set; } =  ActionType.CreateUnit;
    public UnitType UnitType { get; set; }
    public int Id { get; set; }
    public int? PositionX { get; set; }
    public int? PositionY { get; set; }
    public int? AttackTargetId { get; set; }
    public Message()
    {
        
    }

    public Message(string from, string text)
    {
        //do testow
        this.From = from;
        this.Text = text;
    }
    public Message(ActionType actionType, UnitType unitType, int id, int? attackTargetId)
    {
        this.ActionType = actionType;
        this.UnitType = unitType;
        this.Id = id;
        this.AttackTargetId = attackTargetId;
    }
    public Message(ActionType actionType, UnitType unitType, int id, (int,int)? position)
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
    }
}