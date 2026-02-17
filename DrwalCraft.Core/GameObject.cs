using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrwalCraft.Core;
using DrwalCraft.Core.Animations;

namespace DrwalCraft.Core;

public enum GameObjectFor
{
    Army,
}

public interface IGameObject{
    public int Id {get;}
    public byte[]? ObjectIcon {get;}
    public (int, int) Position {get;}
}
public abstract class GameObject : IGameObject, INotifyPropertyChanged{
    protected int _hp;
    protected (int, int) _position;
    public int Id {init; get;}
    public int PlayerId {init; get;}
    public byte[]? ObjectIcon {set; get;}
    public byte[][] ObjectIconPart {set; get;}
    public (int, int) Position {
        get => _position;
        set{
            _position = value;
            Maneuvering?.Invoke(this, new EventArgs());
        }
    }
    public int Size {set; get;}
    public event PropertyChangedEventHandler? HpChanged;
    public event EventHandler BitingTheDust;
    public event EventHandler Maneuvering;
    protected bool _canDie = true;
    public int Hp{
        get => _hp;
        set{
            _hp = value;
            if(_hp <= 0 && _canDie){
                GameMap.Map[Position.Item1, Position.Item2].SetDefault();
                IsDead = true;
                BitingTheDust?.Invoke(this, new EventArgs());
                ExistingObjects.Remove(this);
            }
            HpChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hp)));
            OnPropertyChanged("Hp");
        }
    }
    public virtual void GetAttacked(int damage, GameObject attacker){
        Hp -= damage;
        if(Hp > 0)
            Animations.AnimationList.Add(new TakeDamage(Position));
    }
    public bool IsDead;
    protected int _maxHp;
    public int MaxHp{
        set{
            _maxHp = value;
            OnPropertyChanged(nameof(MaxHp));
        }
        get => _maxHp;
    }
    public string Name{init; get;}
    public static int Price{set; get;}
    public virtual bool IsActive{set; get;}
    public PriorityQueue<GameMap.MapAnimation, (int, int)> objectAnimations = new ();

    //konstruktor do armii, zeby nie zwiekszac liczby obiektów dla determinizmu ID jednostek
    public GameObject(GameObjectFor X)
    {
        
        if (X != GameObjectFor.Army)
            return;
        PlayerId = -1;
        IsDead = false;
        Size = 1;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string? propertyName){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public abstract void MainAction();
    public GameObject(Player player, string? Icon = null, int size = 1){
        Id = player.GetNewId();
        PlayerId = player.PlayerId;
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

        if(this is not Tree && this is not Core.Mines.Mine){
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
                        if(PlayerId == Players.you.PlayerId){
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

        if(this is Buildings.Construction)
            Size = 1;

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

        if(this is Buildings.Construction)
            Size = size;
    }

    public virtual byte[]? GetIconPart(int positionX, int positionY){
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
    public static byte[]? objectStaticIcon;
    public Tree() : base(Players.game){
        if(objectStaticIcon is null)
            SetIcon();
        Name = "Tree";
    }
    public override void MainAction(){
        ExistingObjects.Remove(this);
    }
    private static void SetIcon(){
        var fullPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Icons",
            "Tree.png");
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
        objectStaticIcon = new byte[height * stride];

        // // 5. Kopiowanie pikseli z Tree.png do tablicy
        convertedBitmap.CopyPixels(objectStaticIcon, stride, 0);
    }
    public override byte[]? GetIconPart(int x, int y){
        return objectStaticIcon;
    }
}