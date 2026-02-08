using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrwalCraft.Core;

public static class GameObjectId{
    private static int _player;
    private static int _objectCount;
    public static int PlayerId {get => _player;}
    private static bool IsPrime(int n){
        if(n <= 2) return false;
        if(n % 2 == 0) return false;
        for(int i=3; i*i <= n; i+=2){
            if(n % i == 0) return false;
        }
        return true;
    }
    private static int NthOddPrime(int n){
        int count = 0;
        int num = 1;
        while(count < n){
            num++;
            if(IsPrime(num))
                count ++;
        }
        return num;
    }
    public static void Init(int player){
        _player = NthOddPrime(player);
        _objectCount = 1;
    }
    public static int GetNewId(){
        _objectCount++;
        return _objectCount * 2 * _player;
    }
}
public interface IGameObject{
    public int Id {get;}
    public byte[]? ObjectIcon {get;}
    public (int, int) Position {get;}
}
public abstract class GameObject : IGameObject{
    protected int _hp;
    public int Id {init; get;}
    public int PlayerId {init; get;}
    public byte[]? ObjectIcon {set; get;}
    public byte[][] ObjectIconPart {set; get;}
    public (int, int) Position {set; get;}
    public int Size {set; get;}
    public int Hp{
        get => _hp;
        set{
            _hp = value;
            if(_hp <= 0){
                GameMap.Map[Position.Item1, Position.Item2].SetDefault();
                IsDead = true;
            }
        }
    }
    public bool IsDead;
    public int MaxHp{set; get;}
    public string Name{init; get;}
    public virtual bool IsActive{set; get;}
    public GameObject(string? Icon = null, int? playerId = null, int? objectId = null, int size = 1){
        if(playerId is null)
            PlayerId = GameObjectId.PlayerId;
        else
            PlayerId = playerId.Value;
        if(PlayerId == GameObjectId.PlayerId)
            Id = GameObjectId.GetNewId();
        else
            Id = objectId ?? 1;
        Size = size;
        IsDead = false;

        if(Icon is null) return;
        var fullPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Icons",
            Icon);
        var uri = new Uri(fullPath);
        var sourceImage = new BitmapImage(uri);

        FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
        convertedBitmap.BeginInit();
        convertedBitmap.Source = sourceImage;
        convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
        convertedBitmap.EndInit();

        int width = convertedBitmap.PixelWidth;
        int height = convertedBitmap.PixelHeight;
        int bytesPerPixel = (convertedBitmap.Format.BitsPerPixel + 7) / 8; // Dla Pbgra32 to 4
        int stride = width * bytesPerPixel; // Długość jednego wiersza w bajtach

        // // 4. Przygotowanie tablicy bajtów
        this.ObjectIcon = new byte[height * stride];

        // // 5. Kopiowanie pikseli z Tree.png do tablicy
        convertedBitmap.CopyPixels(ObjectIcon, stride, 0);

        if(this is not Tree){
            int center = height*stride/2 + stride/2;
            int centerX = center % stride;
            int centerY = center / stride;
            for(int i=3; i<height*stride;i+=4)
                if(ObjectIcon[i]<0xFF && ObjectIcon[i] > 0)
                    ObjectIcon[i] = 0xFF;
            for(byte j=(byte)(0x50+Size); j>=0x50; j--){
                for(int i=7; i<height*stride-4;i+=4){
                // int pointX = i % stride;
                // int pointY = i / stride;
                // int distX = centerX - pointX;
                // int distY = centerY - pointY;
                // int dist = distX*distX/16 + distY*distY;
                // if(ObjectIcon[i] == 0 && dist <= height*height/4){
                    if(ObjectIcon[i] == 0 && ((ObjectIcon[i-4] > j && (i-4) / stride == i / stride) || (ObjectIcon[i+4] > j && (i+4) / stride == i / stride))){
                        ObjectIcon[i] = j;
                        if(PlayerId == GameObjectId.PlayerId){
                            ObjectIcon[i-1] = 0x00;
                            ObjectIcon[i-2] = 0x00;
                            ObjectIcon[i-3] = 0xFF;
                        }
                        else{
                            ObjectIcon[i-1] = 0xFF;
                            ObjectIcon[i-2] = 0x00;
                            ObjectIcon[i-3] = 0x00;
                        }
                    }
                }
            }
        }
    
        var chunkSize = 32;
        var chunkStride = chunkSize * 4;
        ObjectIconPart = new byte[Size*Size][];
        
        for(int i=0; i<Size; i++){
            for(int j=0; j<Size; j++){
                int cropX = i * chunkSize;
                int cropY = j * chunkSize;

                ObjectIconPart[i*Size + j] = new byte[chunkSize * chunkStride];

                for (int y = 0; y < chunkSize; y++)
                {
                    int srcIndex =
                        ((cropY + y) * stride) +
                        (cropX * bytesPerPixel);

                    int dstIndex = y * chunkStride;

                    Buffer.BlockCopy(
                        ObjectIcon,
                        srcIndex,
                        ObjectIconPart[i*Size + j],
                        dstIndex,
                        chunkStride
                    );
                }
            }
        }
    }

    public byte[]? GetIconPart(int positionX, int positionY){
        if(ObjectIconPart == null) return null;
        int indexX = positionX - Position.Item1;
        int indexY = positionY - Position.Item2;
        int index = indexX * Size + indexY;
        if(ObjectIconPart.Length>2)
            return ObjectIconPart[index];
        return ObjectIconPart[0];
    }
}

public class Tree: GameObject{
    public Tree() : base("Tree.png"){
        Name = "Tree";
    }
}