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
    public string From = "default";
    public string Text = "default";
    ActionType actionType;
    UnitType unitType;
    private int id;
    private (int,int) position;

    public Message(string from, string text)
    {
        this.From = from;
        this.Text = text;
    }
    public Message(ActionType actionType, UnitType unitType, int id, (int,int) position)
    {
        this.actionType = actionType;
        this.unitType = unitType;
        this.id = id;
        this.position = position;
    }
}