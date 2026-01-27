using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Engine.Render;

public class MiniMap{
    private WriteableBitmap _baseBitmap;
    public int Height {init; get;}
    public int Width {init; get; }
    public int OffsetTop {set; get;}
    public int OffsetLeft {set; get;}

    public MiniMap(int height, int width){
        Height = height;
        Width = width;
        OffsetTop = 0;
        OffsetLeft = 0;
        _baseBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
        byte[] pixels = new byte[width * height * 4];
        for(int i=0; i<width*height; i++)
        {
            pixels[i*4 + 0] = 0x3A;    // B
            pixels[i*4 + 1] = 0x7F;  // G
            pixels[i*4 + 2] = 0x3A;    // R
            pixels[i*4 + 3] = 255;  // A
        }

        _baseBitmap.WritePixels(new Int32Rect(0,0,width,height), pixels, width*4, 0);
    }

    public WriteableBitmap RenderBitmap(Game.GameMap gameMap){
        var bitmap = _baseBitmap.Clone();
        int ChunkSize = Height / gameMap.Size;

        byte[] pixels = new byte[ChunkSize * ChunkSize * 4];
        for(int i=0; i<(ChunkSize * ChunkSize); i++)
        {
            pixels[i*4 + 0] = 0x88;    // B
            pixels[i*4 + 1] = 0x00;  // G
            pixels[i*4 + 2] = 0x00;    // R
            pixels[i*4 + 3] = 255;  // A
        }

        for(int i=0; i<gameMap.Size; i++){
            for(int j=0; j<gameMap.Size; j++){
                var gameObject = gameMap.Map[i,j].GameObject;
                if(gameObject == null) continue;
                var rect = new Int32Rect(i*ChunkSize, j*ChunkSize, ChunkSize, ChunkSize);
                bitmap.WritePixels(rect, pixels, ChunkSize*4 ,0);
            }
        }

        return bitmap;
    }
}