using System.ComponentModel;
using DrwalCraft.Core;
using DrwalCraft.Core.Army;
using DrwalCraft.Engine;

namespace DrwalCraft.Engine.Render.GameUIDataContext;

public class GameUIDataContext : INotifyPropertyChanged{
    private DrwalCraft.Core.GameObject? _activeUnit;

    public DrwalCraft.Core.GameObject? ActiveUnit{
        get => _activeUnit;
        set{
            if (_activeUnit == value) return;
            
            if(_activeUnit is not null)
                _activeUnit.IsActive = false;
            
            _activeUnit = value;

            if(_activeUnit is not null)
                _activeUnit.IsActive = true;

            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string? propertyName = "ActiveUnit"){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
