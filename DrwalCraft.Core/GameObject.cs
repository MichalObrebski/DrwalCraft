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

public abstract class GameObject : INotifyPropertyChanged{
    protected static ImageBytes _chunkPlaceholder = SetIconPlaceholder(1);
    protected int _hp;
    protected int _maxHp;
    protected (int, int) _position;
    protected bool _mortal = true;

    public int Id {init; get;}
    public Player Owner {init; get;}
    public ImageBytes ObjectIcon {protected set; get;}
    public byte[][] ObjectIconPart {protected set; get;}
    public (int, int) Position {
        get => _position;
        set{
            _position = value;
            Maneuvering?.Invoke(this, new EventArgs());
        }
    }
    public int Size {protected set; get;}
    public int Hp{
        get => _hp;
        set{
            _hp = value;
            if(_hp <= 0 && _mortal){
                GameMap.Map[Position.Item1, Position.Item2] = null;
                BitingTheDust?.Invoke(this, new EventArgs());
                ExistingObjects.Remove(this);
            }
            HpChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hp)));
            OnPropertyChanged(nameof(Hp));
        }
    }
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
    public event PropertyChangedEventHandler? HpChanged;
    public event EventHandler? BitingTheDust;
    public event EventHandler? Maneuvering;
    public virtual void GetAttacked(int damage, GameObject attacker){
        Hp -= damage;
        if(Hp > 0)
            Animations.AnimationList.Add(new TakeDamage(Position, attacker.Position));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string? propertyName){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public abstract void MainAction();
    public GameObject(Player player, string? icon = null, int size = 1){
        Id = player.GetNewId();
        Owner = player;
        Size = size;
        Name = "A man has no name";

        if(icon is null)
            ObjectIcon = SetIconPlaceholder(size);
        else
            this.ObjectIcon = CreateObjectIcon(icon);
        
        if(this is Buildings.Construction)
            Size = 1;

        ObjectIconPart = DivideObjectIcon(ObjectIcon);

        if(this is Buildings.Construction)
            Size = size;
    }

    public virtual byte[] GetIconPart(int positionX, int positionY){
        int indexX = positionX - Position.Item1;
        int indexY = positionY - Position.Item2;
        int index = indexX + indexY * Size;

        if(index < 0 || index >= ObjectIconPart.Length)
            return _chunkPlaceholder.Bytes;
        return ObjectIconPart[index];
    }

    private ImageBytes CreateObjectIcon(string icon){
        var fullPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Icons",
            icon);
        var sourceImage = new BitmapImage(new Uri(fullPath));

        //przerabianie png na bitmape
        FormatConvertedBitmap convertedBitmap = new();
        convertedBitmap.BeginInit();
        convertedBitmap.Source = sourceImage;
        convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
        convertedBitmap.EndInit();

        //wymiary
        int iconWidth = convertedBitmap.PixelWidth;
        int iconHeight = convertedBitmap.PixelHeight;
        int iconStride = iconWidth * (PixelFormats.Bgra32.BitsPerPixel / 8); // Długość jednego wiersza w bajtach

        // Kopiowanie pikseli do tablicy
        var objectIcon = new ImageBytes(Size);
        convertedBitmap.CopyPixels(objectIcon.Bytes, iconStride, 0);

        // dodawanie koloru gracza
        if(Owner != Players.game)
            AddPlayerColors(objectIcon);

        return objectIcon;
    }
    private void AddPlayerColors(ImageBytes objectIcon){
        //ustawienie przezroczystości na 0 lub 255 (nic pomiędzy)
        for(int i = 3; i < objectIcon.Height * objectIcon.Stride; i += 4)
            if(objectIcon.Bytes[i] < 0xFF && objectIcon.Bytes[i] > 0)
                objectIcon.Bytes[i] = 0xFF;

        //kolorowanie pixeli na kolor gracza
        for(byte opacity = (byte)(0x50+Size); opacity >= 0x50; opacity--){//grubość obramówki
            //zaczynamy od drugiego pixela i kończymy na przedostatnim
            // (bo sprawdzamy sąsiadujące więc potrzebujemy mieć pewność że +-1 nie wyjebie programu)
            for(int i = 0; i < objectIcon.Width; i++){
                for(int j = 0; j < objectIcon.Height; j++){
                    if(objectIcon[i, j, 3] == 0 && (//czy pixel jest "pusty" i czy sąsiaduje z "niepustym" (3 odpowiada opacity)
                        i > 0 && objectIcon[i-1, j, 3] > opacity || 
                        i < objectIcon.Width - 1 && objectIcon[i+1, j, 3] > opacity ||
                        j > 0 && objectIcon[i, j-1, 3] > opacity || 
                        j < objectIcon.Height - 1 && objectIcon[i, j+1, 3] > opacity
                    )){
                        objectIcon[i, j, 3] = opacity;//alpha w bgra
                        byte b, g, r;
                        (b, g, r) = Owner.Colors;//barwy gracza
                        objectIcon[i, j, 0] = b;
                        objectIcon[i, j, 1] = g;
                        objectIcon[i, j, 2] = r;
                    }
                }
            }
        }
    }
    private byte[][] DivideObjectIcon(ImageBytes objectIcon){
        var chunkSize = GameMap.ChunkSize;
        var chunkStride = chunkSize * (PixelFormats.Bgra32.BitsPerPixel / 8);
        var iconParts = new byte[Size*Size][];
        
        // pętla po chunkach
        for(int j = 0; j < Size; j++){
            for(int i = 0; i < Size; i++){
                int startX = i * chunkSize;
                int startY = j * chunkSize;

                // alokacja pamięci dla fragmentu
                iconParts[i + j*Size] = new byte[chunkSize * chunkStride];

                //kopiowanie wiersz po wierszu (te poziome)
                for(int y = 0; y < chunkSize; y++){
                    int srcIndex = //początek bloku pamięci w objectIcon
                        ((startY + y) * objectIcon.Stride) +
                        (startX * objectIcon.BytesPerPixel);

                    int dstIndex = y * chunkStride; // index w części ikony (podtablicy) w iconParts

                    Buffer.BlockCopy(
                        ObjectIcon.Bytes,       //src
                        srcIndex,               //src offset
                        iconParts[i + j*Size],  //destination
                        dstIndex,               //dest offset
                        chunkStride             //count
                    );
                }
            }
        }

        return iconParts;
    }
    private static ImageBytes SetIconPlaceholder(int size){
        int squareSize = GameMap.ChunkSize / 2;
        uint color1 = 0xA000A0FF;
        uint color2 = 0x222222FF;
        var objectIcon = new ImageBytes(size);
        //for dla każdego chunka po x
        for(int k = 0; k < size*GameMap.ChunkSize; k += GameMap.ChunkSize){
            //for dla każdego chunka po y
            for(int l = 0; l < size*GameMap.ChunkSize; l += GameMap.ChunkSize){
                //for dla pixeli po x
                for(int i = 0; i < squareSize; i++){
                    //for dla pixeli po y
                    for(int j = 0; j < squareSize; j++){
                        //kolorowanie kwadratów
                        objectIcon[k + i, j + l] = color1;                          //lewy górny
                        objectIcon[k + i + squareSize, j + l] = color2;             //prawy górny
                        objectIcon[k + i, j + l + squareSize] = color2;             //lewy dolny
                        objectIcon[k + i + squareSize, j + l + squareSize] = color1;//prawy dolny
                    }
                }
            }
        }
        return objectIcon;
    }
}