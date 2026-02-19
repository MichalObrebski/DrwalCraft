using System.Security.Cryptography;

namespace DrwalCraft.Core.Animations;

public class TakeDamage : Animation{
    private (int, int) _innerPosition;
    private double _theta;
    private (int, int) _attackDirection;
    
    private void SharedConstructorCode((int, int) attackerPosition){
        // kierunek (x,y), x,y in {-1,0,1}
        (int, int) attackDirection = (
            Math.Sign(attackerPosition.Item1 - _position.Item1),
            Math.Sign(attackerPosition.Item2 - _position.Item2)
        );

        //mamy 9 kwadratów rozmiaru 4 równomiernie rozstawionych. środkowy jest na pozycji (14,14)-(18,18)
        (int, int) basePosition = (14, 14);
        (int, int) rangePosition = (//zakres punktów startowych dla rysowania (minX, minY)
            basePosition.Item1 + 8*attackDirection.Item1,
            basePosition.Item2 + 8*attackDirection.Item2
        );

        var rand = new Random();
        _innerPosition = (//dokładna pozycja środka rysunku
            rand.Next(0, 4) + rangePosition.Item1,
            rand.Next(0, 4) + rangePosition.Item2
        );
        _theta = rand.NextDouble() * Math.PI*2;//obrót rysunku
    }
    public TakeDamage((int, int) position, (int, int) attackerPosition):
        base(position, 180){
        SharedConstructorCode(attackerPosition);
    }
    public TakeDamage(GameObject gameObject, (int, int) attackerPosition):
        base(gameObject, 180){
        SharedConstructorCode(attackerPosition);
    }
    public override void Animate(ref byte[] field){
        if(!CanAnimate()) return;
        
        //jakieś coś a algebry liniowej
        double x = Math.Cos(_theta);
        double y = Math.Sin(_theta);
        int middle = _innerPosition.Item2*_chunkSize*4-(_chunkSize-_innerPosition.Item1)*4;
        //narazie nie dam fade out bo animacje są rysowane na tej samej bitmapie co obiekty
        //trzeba dodać kolejną nad obiektami która będzie służyła tylko do animacji
        byte progress = 0xFF;//(byte)(255-128*_stopwatch.ElapsedMilliseconds/_msDuration);
        
        //rysowanie obróconej kreski
        for(int i=-12/2+1; i<12/2; i++){
            field[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 + middle] = 0x40;
            field[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 + 1 + middle] = 0x28;
            field[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 + 2 + middle] = 0xAA;
            field[(int)(x*i)*4 + (int)(i*y)*_chunkSize*4 + 3 + middle] = progress;

            field[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 + middle] = 0x40;
            field[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 + 1 + middle] = 0x28;
            field[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 + 2 + middle] = 0xAA;
            field[(int)(x*i)*4 + ((int)(i*y)-1)*_chunkSize*4 + 3 + middle] = progress;

            field[(int)(x*i+1)*4 + ((int)(i*y))*_chunkSize*4 + middle] = 0x40;
            field[(int)(x*i+1)*4 + ((int)(i*y))*_chunkSize*4 + 1 + middle] = 0x28;
            field[(int)(x*i+1)*4 + ((int)(i*y))*_chunkSize*4 + 2 + middle] = 0xAA;
            field[(int)(x*i+1)*4 + ((int)(i*y))*_chunkSize*4 + 3 + middle] = progress;

            field[(int)(y/2*i)*4 + (int)(i*-x/2)*_chunkSize*4 + middle] = 0x40;
            field[(int)(y/2*i)*4 + (int)(i*-x/2)*_chunkSize*4 + 1 + middle] = 0x28;
            field[(int)(y/2*i)*4 + (int)(i*-x/2)*_chunkSize*4 + 2 + middle] = 0xAA;
            field[(int)(y/2*i)*4 + (int)(i*-x/2)*_chunkSize*4 + 3 + middle] = progress;

            field[(int)(y/2*i)*4 + ((int)(i*-x/2)-1)*_chunkSize*4 + middle] = 0x40;
            field[(int)(y/2*i)*4 + ((int)(i*-x/2)-1)*_chunkSize*4 + 1 + middle] = 0x28;
            field[(int)(y/2*i)*4 + ((int)(i*-x/2)-1)*_chunkSize*4 + 2 + middle] = 0xAA;
            field[(int)(y/2*i)*4 + ((int)(i*-x/2)-1)*_chunkSize*4 + 3 + middle] = progress;

            field[(int)(y/2*i+1)*4 + ((int)(i*-x/2))*_chunkSize*4 + middle] = 0x40;
            field[(int)(y/2*i+1)*4 + ((int)(i*-x/2))*_chunkSize*4 + 1 + middle] = 0x28;
            field[(int)(y/2*i+1)*4 + ((int)(i*-x/2))*_chunkSize*4 + 2 + middle] = 0xAA;
            field[(int)(y/2*i+1)*4 + ((int)(i*-x/2))*_chunkSize*4 + 3 + middle] = progress;
        }
    }
}