namespace DrwalCraft.Core.Animations;

public class TakeDamage : Animation{
    public TakeDamage((int, int) position):
        base(position, 96){}
    public TakeDamage(GameObject gameObject):
        base(gameObject, 96){}
    public override void Animate(byte[] field){
        if(!CanAnimate()) return;

        for(int k = 12; k<=_chunkSize-12; k++){
            // \
            field[(k)*_chunkSize*4 + k*4]=0x00;
            field[(k)*_chunkSize*4 + k*4 + 1]=0x00;
            field[(k)*_chunkSize*4 + k*4 + 2]=0xFF;
            field[(k)*_chunkSize*4 + k*4 + 3]=0xFF;

            field[(k)*_chunkSize*4 + k*4 + 4]=0x00;
            field[(k)*_chunkSize*4 + k*4 + 4 + 1]=0x00;
            field[(k)*_chunkSize*4 + k*4 + 4 + 2]=0xFF;
            field[(k)*_chunkSize*4 + k*4 + 4 + 3]=0xFF;

            // field[(k)*_chunkSize*4 + k*4 - 4]=0x00;
            // field[(k)*_chunkSize*4 + k*4 - 4 + 1]=0x00;
            // field[(k)*_chunkSize*4 + k*4 - 4 + 2]=0xFF;
            // field[(k)*_chunkSize*4 + k*4 - 4 + 3]=0xFF;
            // field[(k)*_chunkSize*4 + k*4]=0x00;

            // -
            field[_chunkSize*_chunkSize*2 + k*4]=0x00;
            field[_chunkSize*_chunkSize*2 + k*4 + 1]=0x00;
            field[_chunkSize*_chunkSize*2 + k*4 + 2]=0xFF;
            field[_chunkSize*_chunkSize*2 + k*4 + 3]=0xFF;

            // field[_chunkSize*_chunkSize*2 + k*4 + 4]=0x00;
            // field[_chunkSize*_chunkSize*2 + k*4 + 4 + 1]=0x00;
            // field[_chunkSize*_chunkSize*2 + k*4 + 4 + 2]=0xFF;
            // field[_chunkSize*_chunkSize*2 + k*4 + 4 + 3]=0xFF;

            // field[_chunkSize*_chunkSize*2 + k*4 - 4]=0x00;
            // field[_chunkSize*_chunkSize*2 + k*4 - 4 + 1]=0x00;
            // field[_chunkSize*_chunkSize*2 + k*4 - 4 + 2]=0xFF;
            // field[_chunkSize*_chunkSize*2 + k*4 - 4 + 3]=0xFF;
            // field[_chunkSize*_chunkSize*2 + k*4]=0x00;

            // |
            field[(k)*_chunkSize*4 + _chunkSize*2]=0x00;
            field[(k)*_chunkSize*4 + _chunkSize*2 + 1]=0x00;
            field[(k)*_chunkSize*4 + _chunkSize*2 + 2]=0xFF;
            field[(k)*_chunkSize*4 + _chunkSize*2 + 3]=0xFF;

            // field[(k)*_chunkSize*4 + _chunkSize*2 + 4]=0x00;
            // field[(k)*_chunkSize*4 + _chunkSize*2 + 4 + 1]=0x00;
            // field[(k)*_chunkSize*4 + _chunkSize*2 + 4 + 2]=0xFF;
            // field[(k)*_chunkSize*4 + _chunkSize*2 + 4 + 3]=0xFF;

            // field[(k)*_chunkSize*4 + _chunkSize*2 - 4]=0x00;
            // field[(k)*_chunkSize*4 + _chunkSize*2 - 4 + 1]=0x00;
            // field[(k)*_chunkSize*4 + _chunkSize*2 - 4 + 2]=0xFF;
            // field[(k)*_chunkSize*4 + _chunkSize*2 - 4 + 3]=0xFF;
            // field[(k)*_chunkSize*4 + _chunkSize*2]=0x00;

            // /
            field[(k+1)*_chunkSize*4 - k*4]=0x00;
            field[(k+1)*_chunkSize*4 - k*4 + 1]=0x00;
            field[(k+1)*_chunkSize*4 - k*4 + 2]=0xFF;
            field[(k+1)*_chunkSize*4 - k*4 + 3]=0xFF;

            field[(k+1)*_chunkSize*4 - k*4 + 4]=0x00;
            field[(k+1)*_chunkSize*4 - k*4 + 4 + 1]=0x00;
            field[(k+1)*_chunkSize*4 - k*4 + 4 + 2]=0xFF;
            field[(k+1)*_chunkSize*4 - k*4 + 4 + 3]=0xFF;

            // field[(k+1)*_chunkSize*4 - k*4 - 4]=0x00;
            // field[(k+1)*_chunkSize*4 - k*4 - 4 + 1]=0x00;
            // field[(k+1)*_chunkSize*4 - k*4 - 4 + 2]=0xFF;
            // field[(k+1)*_chunkSize*4 - k*4 - 4 + 3]=0xFF;
        }
    }
}