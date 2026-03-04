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
            if(this == Players.you)
                return (0xFF, 0x00, 0x00); //bgr
            else if(this == Players.enemy)
                return (0x00, 0x00, 0xFF); //bgr
            else
                return (0x3A, 0x7F, 0x3A);
        }
    }
    public Player(int player, int wood = 3000){
        _player = player;
        _objectCount = 0;
        Wood = wood;
    }
    public int GetNewId(){
        _objectCount += 12;
        return _objectCount + _player;
    }
}