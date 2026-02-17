namespace DrwalCraft.Core.Animations;

public class Sword : Animation{
    public Sword((int, int) position):
        base(position, 64){}
    public Sword(GameObject gameObject):
        base(gameObject, 64){}
    public override void Animate(byte[] field){
        if(!CanAnimate()) return;

        for(int i = 0; i<_chunkSize/2; i++){
            for(int j=i; j<_chunkSize-i; j++){
                int a = i-_chunkSize/2;
                int b = j-_chunkSize/2;
                int c = _chunkSize/2;
                if(Math.Floor(Math.Sqrt(a*a + b*b)) == c ||
                    Math.Ceiling(Math.Sqrt(a*a + b*b)) == c){
                    field[i*4 + j*_chunkSize*4] = 0xFF;
                    field[i*4 + j*_chunkSize*4 + 1] = 0xFF;
                    field[i*4 + j*_chunkSize*4 + 2] = 0xFF;
                    field[i*4 + j*_chunkSize*4 + 3] = (byte)((double)j/_chunkSize*0xFF);
                }
                if(Math.Floor(Math.Sqrt(a*a + b*b)) <= c && j >= _chunkSize-i-3){
                    field[i*4 + j*_chunkSize*4] = 0xAA;
                    field[i*4 + j*_chunkSize*4 + 1] = 0xAA;
                    field[i*4 + j*_chunkSize*4 + 2] = 0xAA;
                    field[i*4 + j*_chunkSize*4 + 3] = 0xFF;

                }
            }
        }
    }
}