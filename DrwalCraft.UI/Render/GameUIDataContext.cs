using System.ComponentModel;
using DrwalCraft.Core;
using DrwalCraft.UI;

namespace DrwalCraft.UI.Render.GameUIDataContext;

public class GameUIDataContext : INotifyPropertyChanged{
    private DrwalCraft.Core.GameObject? _activeUnit;

    public DrwalCraft.Core.GameObject? ActiveUnit{
        get => _activeUnit;
        set{
            if (_activeUnit == value) return;
            
            if(_activeUnit is not null){
                _activeUnit.IsActive = false;
                _activeUnit.BitingTheDust -= ObjectDestroyed;
            }
            
            _activeUnit = value;

            if(_activeUnit is not null){
                _activeUnit.IsActive = true;
                _activeUnit.BitingTheDust += ObjectDestroyed;
            }

            OnPropertyChanged();
        }
    }
    private int _wood;
    public int Wood{
        get => _wood;
        set{
            _wood = value;
            OnPropertyChanged(nameof(Wood));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string? propertyName = "ActiveUnit"){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ObjectDestroyed(object? sender, EventArgs e){
        ActiveUnit = null;
    }

    public void WoodChangeListener(object? sender, EventArgs e){
        Wood = Players.you.Wood;
    }
}
