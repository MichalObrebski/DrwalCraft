namespace DrwalCraft.Core.Animations;

/*
bawię się tutaj i testuje animacje
*/
public class Line : Animation{
    public Line((int, int) position):
        base(position, 900){}
    public Line(GameObject gameObject):
        base(gameObject, 900){}
    
    public override void Animate(ref byte[] bytes){
        // if(!CanAnimate()) return;

        int u = 1;
        int v = 0;
        double theta = 1.0/4.0 * Math.PI + _stopwatch.ElapsedMilliseconds * Math.PI / 1800;
        double x = u * Math.Cos(theta) - v * Math.Sin(theta);
        double y = u * Math.Sin(theta) + v * Math.Cos(theta);
        for(int i=-_chunkSize/2+1; i<_chunkSize/2; i++){
            bytes[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 +  _chunkSize*_chunkSize*2-_chunkSize*2] = 0x00;
            bytes[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 + 1 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0x00;
            bytes[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 + 2 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0x00;
            bytes[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 + 3 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0xFF;

            bytes[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 +  _chunkSize*_chunkSize*2-_chunkSize*2] = 0x80;
            bytes[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 + 1 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0x80;
            bytes[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 + 2 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0x80;
            bytes[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 + 3 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0xFF;

            bytes[((int)(x*i)-1)*4 + (int)(i*y)*_chunkSize*4 +  _chunkSize*_chunkSize*2-_chunkSize*2] = 0x80;
            bytes[((int)(x*i)-1)*4 + (int)(i*y)*_chunkSize*4 + 1 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0x80;
            bytes[((int)(x*i)-1)*4 + (int)(i*y)*_chunkSize*4 + 2 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0x80;
            bytes[((int)(x*i)-1)*4 + (int)(i*y)*_chunkSize*4 + 3 + _chunkSize*_chunkSize*2-_chunkSize*2] = 0xFF;
        }
        // Rotate(Math.PI/2, ref bytes);
    }
}