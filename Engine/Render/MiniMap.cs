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

    async public Task<WriteableBitmap> RenderBitmap(){
        var bitmap = _baseBitmap.Clone();
        int ChunkSize = Height / DrwalCraft.Core.GameMap.Size;

        byte[] pixels = new byte[ChunkSize * ChunkSize * 4];
        for(int i=0; i<(ChunkSize * ChunkSize); i++)
        {
            pixels[i*4 + 0] = 0x88;    // B
            pixels[i*4 + 1] = 0x00;  // G
            pixels[i*4 + 2] = 0x00;    // R
            pixels[i*4 + 3] = 255;  // A
        }
        // var ind = 0;
        //     pixels[ind*4 + 0] = 0x3A;    // B
        //     pixels[ind*4 + 1] = 0x7F;  // G
        //     pixels[ind*4 + 2] = 0x3A;    // R
        //     pixels[ind*4 + 3] = 255;  // A
        // ind = ChunkSize - 1;
        //     pixels[ind*4 + 0] = 0x3A;    // B
        //     pixels[ind*4 + 1] = 0x7F;  // G
        //     pixels[ind*4 + 2] = 0x3A;    // R
        //     pixels[ind*4 + 3] = 255;  // A
        // ind = ChunkSize * (ChunkSize - 1);
        //     pixels[ind*4 + 0] = 0x3A;    // B
        //     pixels[ind*4 + 1] = 0x7F;  // G
        //     pixels[ind*4 + 2] = 0x3A;    // R
        //     pixels[ind*4 + 3] = 255;  // A
        // ind = ChunkSize * ChunkSize - 1;
        //     pixels[ind*4 + 0] = 0x3A;    // B
        //     pixels[ind*4 + 1] = 0x7F;  // G
        //     pixels[ind*4 + 2] = 0x3A;    // R
        //     pixels[ind*4 + 3] = 255;  // A

        for(int i=0; i<DrwalCraft.Core.GameMap.Size; i++){
            for(int j=0; j<DrwalCraft.Core.GameMap.Size; j++){
                var gameObject = DrwalCraft.Core.GameMap.Map[i,j].GameObject;
                if(gameObject == null) continue;
                var rect = new Int32Rect(i*ChunkSize, j*ChunkSize, ChunkSize, ChunkSize);
                bitmap.WritePixels(rect, pixels, ChunkSize*4 ,0);
            }
        }

        return bitmap;
    }
}