using System.ComponentModel;
using Engine;

namespace Engine.Render.GameUIDataContext;

public class GameUIDataContext : INotifyPropertyChanged{
    private DrwalCraft.Core.GameObject? _activeUnit;

    public DrwalCraft.Core.GameObject? ActiveUnit{
        get => _activeUnit;
        set{
            if (_activeUnit == value) return;
            _activeUnit = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string? propertyName = "ActiveUnit"){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
