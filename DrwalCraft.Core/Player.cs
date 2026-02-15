using System.ComponentModel;

namespace DrwalCraft.Core;

public static class Players{
    public static Player you;
    public static Player enemy;
    public static Player game;

    public static Player player1;
    public static Player player2;
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
            OnPropertyChanged("Wood");
            WoodAmmountChanged?.Invoke(this, new EventArgs());
        }
    }
    public int PlayerId {get => _player;}
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