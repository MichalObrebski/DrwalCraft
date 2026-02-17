using System.Diagnostics;

namespace DrwalCraft.Core.Animations;

public abstract class Animation{
    protected (int, int) _position;
    protected GameObject? _gameObject;
    protected Stopwatch _stopwatch;
    protected int _msDuration;
    protected int _chunkSize = GameMap.ChunkSize;

    public (int, int) Position{get => _position;}

    public Animation((int, int) position, int duration){
        _stopwatch = Stopwatch.StartNew();
        _position = position;
        _msDuration = duration;
        AnimationList.Add(this);
    }
    public Animation(GameObject gameObject, int duration){
        _stopwatch = Stopwatch.StartNew();
        _gameObject = gameObject;
        _gameObject.Maneuvering += ChangePosition;
        _position = gameObject.Position;
        _msDuration = duration;
        AnimationList.Add(this);
    }
    public abstract void Animate(byte[] field);
    protected bool CanAnimate(){
        if(_stopwatch.ElapsedMilliseconds < _msDuration)
            return true;

        AnimationList.Remove(this);
        if(_gameObject is not null)
            _gameObject.Maneuvering -= ChangePosition;
        return false;
    }
    protected void ChangePosition(object? sender, EventArgs e){
        if(_gameObject is null){
            if(sender is not null)
                ((GameObject)sender).Maneuvering -= ChangePosition;
            return;
        }

        AnimationList.ChangePosition(this, _gameObject.Position);
        _position = _gameObject.Position;
    }
}