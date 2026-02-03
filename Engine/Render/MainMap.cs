using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Engine;

namespace Engine.Render;

public class MainMap{
    private WriteableBitmap _baseBitmap;
    private int _offsetTop;
    private int _offsetLeft;
    private int _maxOffsetTop;
    private int _maxOffsetLeft;
    public int Height {init; get;}
    public int Width {init; get; }
    public int OffsetTop {
        get => _offsetTop;
        set{
            if(value < _maxOffsetTop)
                _offsetTop = value > 0 ? value : 0;
            else
                _offsetTop = _maxOffsetTop;
        }
    }
    public int OffsetLeft {
        get => _offsetLeft;
        set{
            if(value < _maxOffsetLeft)
                _offsetLeft = value > 0 ? value : 0;
            else
                _offsetLeft = _maxOffsetLeft;
        }
    }

    public MainMap(int height, int width, int chunkSize, int mapSize){
        Height = height;
        Width = width;
        OffsetTop = 0;
        OffsetLeft = 0;
        _maxOffsetTop = mapSize - (height / chunkSize);
        _maxOffsetLeft = mapSize - (width / chunkSize);
        _baseBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        byte[] pixels = new byte[width * height * 4];
        
        for(int i=0; i<width*height; i++)
        {
            pixels[i*4 + 0] = 0x3A; // B
            pixels[i*4 + 1] = 0x7F; // G
            pixels[i*4 + 2] = 0x3A; // R
            pixels[i*4 + 3] = 255;  // A
        }

        _baseBitmap.WritePixels(new Int32Rect(0,0,width,height), pixels, width*4, 0);
    }

    async public Task<WriteableBitmap> RenderBitmap(){
        var bitmap = _baseBitmap.Clone();
        var ChunkSize = Game.GameMap.ChunkSize;
        var renderWidth = Width / ChunkSize;
        var renderHeight = Height / ChunkSize;

        for(int i=0; i<renderWidth; i++){
            for(int j=0; j<renderHeight; j++){
                var gameObject = Game.GameMap.Map[i+OffsetLeft,j+OffsetTop].GameObject;
                if(gameObject == null) continue;
                var rect = new Int32Rect(i*ChunkSize, j*ChunkSize, ChunkSize, ChunkSize);
                bitmap.WritePixels(rect, gameObject.ObjectIcon, ChunkSize*4 ,0);
            }
        }

        return bitmap;
    }
}