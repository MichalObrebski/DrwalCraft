using System.Text.Json.Serialization;

namespace Messages;

public enum ActionType
{
    CreateUnit,
    MoveUnit
}

public enum UnitType
{
    Soldier
}

public class Message
{
    public string From { get; set; } = "default";
    public string Text { get; set; }= "default";
    public ActionType ActionType { get; set; }
    public UnitType UnitType { get; set; }
    public int Id { get; set; }
    public (int,int) Position { get; set; }

    public Message()
    {
        
    }

    public Message(string from, string text)
    {
        this.From = from;
        this.Text = text;
    }
    public Message(ActionType actionType, UnitType unitType, int id, (int,int) position)
    {
        this.ActionType = actionType;
        this.UnitType = unitType;
        this.Id = id;
        this.Position = position;
    }
}