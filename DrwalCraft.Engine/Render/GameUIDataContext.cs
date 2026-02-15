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
            
            if(_activeUnit is not null){
                _activeUnit.IsActive = false;
                _activeUnit.HpChanged -= HpChangeListener;
            }
            
            _activeUnit = value;

            if(_activeUnit is not null){
                _activeUnit.IsActive = true;
                _activeUnit.HpChanged += HpChangeListener;
            }

            OnPropertyChanged();
        }
    }
    private int _wood;
    public int Wood{
        get => _wood;
        set{
            _wood = value;
            OnPropertyChanged("Wood");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string? propertyName = "ActiveUnit"){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void HpChangeListener(object? sender, PropertyChangedEventArgs e){
        if(_activeUnit is null) return;
        if(_activeUnit.Hp > 0) return;
        ActiveUnit = null;
    }

    public void WoodChangeListener(object? sender, EventArgs e){
        Wood = Players.you.Wood;
    }
}
