namespace DrwalCraft.Core.Animations;

public class Sword : Animation{
    private (int, int) _direction;
    public Sword((int, int) position, (int, int) target):
        base(position, 90){
        _direction = (
            Math.Sign(target.Item1 - _position.Item1),
            Math.Sign(target.Item2 - _position.Item2)
        );
    }
    public Sword(GameObject gameObject, (int, int) target):
        base(gameObject, 90){
        _direction = (
            Math.Sign(target.Item1 - _position.Item1),
            Math.Sign(target.Item2 - _position.Item2)
        );
    }
    public override void Animate(ref byte[] field){
        if(!CanAnimate()) return;

        var bytes = new byte[_chunkSize*_chunkSize*4];
        int progress = (int)(_msDuration - _stopwatch.ElapsedMilliseconds)*_chunkSize/_msDuration/2;
        for(int i = 4; i<_chunkSize/2-2; i++){
            for(int j=i; j<_chunkSize-i; j++){
                int a = i-_chunkSize/2;
                int b = j-_chunkSize/2;
                int c = _chunkSize/2-4;
                if((Math.Floor(Math.Sqrt(a*a + b*b))-4 <= c &&
                    Math.Ceiling(Math.Sqrt(a*a + b*b))+4 >= c) && 
                    j>i+progress){
                    bytes[i*4 + j*_chunkSize*4] = 0xFF;
                    bytes[i*4 + j*_chunkSize*4 + 1] = 0xFF;
                    bytes[i*4 + j*_chunkSize*4 + 2] = 0xFF;
                    bytes[i*4 + j*_chunkSize*4 + 3] = (byte)((double)j/_chunkSize*0xFF/2);
                }
                if(Math.Floor(Math.Sqrt(a*a + b*b)) <= c && j >= _chunkSize-i-3){
                    bytes[i*4 + j*_chunkSize*4] = 0xFF;
                    bytes[i*4 + j*_chunkSize*4 + 1] = 0xFF;
                    bytes[i*4 + j*_chunkSize*4 + 2] = 0xFF;
                    bytes[i*4 + j*_chunkSize*4 + 3] = 0xFF;

                }
            }
        }
        int x = _direction.Item1, y = _direction.Item2;
        double norm = Math.Sqrt(x*x + y*y);
        double sin = x/norm;
        double cos = y/norm;
        double fi = Math.Atan2(cos, sin);
        double theta = Math.PI*1.5 + fi - _stopwatch.ElapsedMilliseconds * Math.PI / 180;
        Rotate(theta, ref bytes);
        for(int i=0;i<_chunkSize*_chunkSize; i++)
            if(bytes[i*4+3]!=0){
                field[i*4]=bytes[i*4];
                field[i*4+1]=bytes[i*4+1];
                field[i*4+2]=bytes[i*4+2];
                field[i*4+3]=bytes[i*4+3];
            }
    }
}