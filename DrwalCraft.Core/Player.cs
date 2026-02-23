using System.ComponentModel;

namespace DrwalCraft.Core;

public static class Players{
    public static Player you = null!;
    public static Player enemy = null!;
    public static Player game = null!;

    public static Player player1 = null!;
    public static Player player2 = null!;
}
public class Player : INotifyPropertyChanged{
    private int _player;
    private int _objectCount;
    private int _wood;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? WoodAmmountChanged;

    protected void OnPropertyChanged(string? propertyName){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public int Wood{
        get => _wood;
        set{
            _wood = value;
            OnPropertyChanged(nameof(Wood));
            WoodAmmountChanged?.Invoke(this, new EventArgs());
        }
    }
    public int PlayerId {get => _player;}
    public (byte, byte, byte) Colors{
        get{
            if(this == Players.player1)
                return (0xFF, 0x00, 0x00); //bgr
            else
                return (0x00, 0x00, 0xFF); //bgr
        }
    }
    public Player(int player){
        _player = player;
        _objectCount = 0;
        Wood = 3000;
    }
    public int GetNewId(){
        _objectCount += 12;
        return _objectCount + _player;
    }
}