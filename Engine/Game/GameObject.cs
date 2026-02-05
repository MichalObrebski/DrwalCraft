using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Engine.Game;

public static class GameObjectId{
    private static int _player;
    private static int _objectCount;
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
    public int Id {init; get;}
    public byte[]? ObjectIcon {set; get;}
    public (int, int) Position {set; get;}
    public GameObject(Uri? IconUri = null){
        Id = GameObjectId.GetNewId();

        if(IconUri is not null){
            var sourceImage = new BitmapImage(IconUri);

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
        }
    }
}


public class Tree: GameObject{
    public Tree() : base(new Uri("../Assets/Icons/Tree.png", UriKind.Relative)){
        
    }
}