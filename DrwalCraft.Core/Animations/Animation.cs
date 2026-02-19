using System.Diagnostics;

namespace DrwalCraft.Core.Animations;

public abstract class Animation{
    protected (int, int) _position;
    protected GameObject? _gameObject;
    protected Stopwatch _stopwatch;
    protected int _msDuration;
    protected int _chunkSize = GameMap.ChunkSize;

    public (int, int) Position{get => _position;}

    private Animation(int duration){
        _stopwatch = Stopwatch.StartNew();
        _msDuration = duration;
    }
    public Animation((int, int) position, int duration) : this(duration){
        _position = position;
        AnimationList.Add(this);
    }
    public Animation(GameObject gameObject, int duration) : this(duration){
        _gameObject = gameObject;
        _gameObject.Maneuvering += ChangePosition;
        _position = gameObject.Position;
        AnimationList.Add(this);
    }
    public abstract void Animate(ref byte[] field);
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
    protected void Rotate(double rad, ref byte[] bytes){//trzeba to zrobić lepiej
        byte[] baseBytes = (byte[])bytes.Clone();
        byte[] rotated = new byte[_chunkSize*_chunkSize*4];
        int middle = _chunkSize*_chunkSize*2-_chunkSize*2;
        for(int i=-_chunkSize/2; i<_chunkSize/2; i++){
            for(int j=-_chunkSize/2; j<_chunkSize/2; j++){
                double x = i * Math.Cos(rad) - j * Math.Sin(rad);
                double y = i * Math.Sin(rad) + j * Math.Cos(rad);
                if(((int)x)*4 + ((int)y)*_chunkSize*4 + middle >= 0 &&
                    ((int)x+1)*4 + ((int)y+1)*_chunkSize*4 + middle + 3 < _chunkSize*_chunkSize*4 && 
                    i*4 + j*_chunkSize*4 + middle >= 0 &&
                    i*4 + j*_chunkSize*4 + middle + 3 <_chunkSize*_chunkSize*4){
                    rotated[((int)x)*4 + ((int)y)*_chunkSize*4 + middle] = baseBytes[i*4 + j*_chunkSize*4 + middle];
                    rotated[((int)x)*4 + ((int)y)*_chunkSize*4 + 1 + middle] = baseBytes[i*4 + j*_chunkSize*4 + 1 + middle];
                    rotated[((int)x)*4 + ((int)y)*_chunkSize*4 + 2 + middle] = baseBytes[i*4 + j*_chunkSize*4 + 2 + middle];
                    rotated[((int)x)*4 + ((int)y)*_chunkSize*4 + 3 + middle] = baseBytes[i*4 + j*_chunkSize*4 + 3 + middle];
                    
                    rotated[((int)Math.Ceiling(x))*4 + ((int)Math.Ceiling(y))*_chunkSize*4 + middle] = baseBytes[i*4 + j*_chunkSize*4 + middle];
                    rotated[((int)Math.Ceiling(x))*4 + ((int)Math.Ceiling(y))*_chunkSize*4 + 1 + middle] = baseBytes[i*4 + j*_chunkSize*4 + 1 + middle];
                    rotated[((int)Math.Ceiling(x))*4 + ((int)Math.Ceiling(y))*_chunkSize*4 + 2 + middle] = baseBytes[i*4 + j*_chunkSize*4 + 2 + middle];
                    rotated[((int)Math.Ceiling(x))*4 + ((int)Math.Ceiling(y))*_chunkSize*4 + 3 + middle] = baseBytes[i*4 + j*_chunkSize*4 + 3 + middle];
                }
            }
        }
        bytes = rotated;
    }
}