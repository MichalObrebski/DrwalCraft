using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Engine;

namespace Engine.Render;

public class MainMap{
    private WriteableBitmap _baseBitmap;
    public int Height {init; get;}
    public int Width {init; get; }
    public int OffsetTop {set; get;}
    public int OffsetLeft {set; get;}

    // public WriteableBitmap SourceBitmap{set; get;}

    public MainMap(int height, int width){
        Height = height;
        Width = width;
        OffsetTop = 0;
        OffsetLeft = 0;
        _baseBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        byte[] pixels = new byte[width * height * 4];
        var tree = new Game.Tree();
        // for(int i=0; i<1024; i++)
        //     pixels[i] = tree.ObjectIcon[i];
        
        for(int i=0; i<width*height; i++)
        {
            pixels[i*4 + 0] = 0x3A;    // B
            pixels[i*4 + 1] = 0x7F;  // G
            pixels[i*4 + 2] = 0x3A;    // R
            pixels[i*4 + 3] = 255;  // A
        }

        _baseBitmap.WritePixels(new Int32Rect(0,0,width,height), pixels, width*4, 0);
        // SourceBitmap = _baseBitmap.Clone();
    }

    public WriteableBitmap RenderBitmap(Game.GameMap gameMap){
        var bitmap = _baseBitmap.Clone();

        for(int i=0; i<gameMap.Size; i++){
            for(int j=0; j<gameMap.Size; j++){
                var gameObject = gameMap.Map[i,j].GameObject;
                if(gameObject == null) continue;
                var rect = new Int32Rect(i*gameMap.ChunkSize, j*gameMap.ChunkSize, gameMap.ChunkSize, gameMap.ChunkSize);
                bitmap.WritePixels(rect, gameObject.ObjectIcon, gameMap.ChunkSize*4 ,0);
            }
        }

        return bitmap;
    }
}