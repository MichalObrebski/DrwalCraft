using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Engine.Game;

public abstract class GameObject{
    public byte[]? ObjectIcon {set; get;}

}

public class Tree: GameObject{
    public Tree(){
        var uri = new Uri("../Assets/Icons/Tree.png", UriKind.Relative);
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
    }
}