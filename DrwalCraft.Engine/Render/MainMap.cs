using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrwalCraft.Core;
using DrwalCraft.Engine;

namespace DrwalCraft.Engine.Render;

public class MainMap{
    private WriteableBitmap _baseBitmap;
    private int _offsetTop;
    private int _offsetLeft;
    private int _maxOffsetTop;
    private int _maxOffsetLeft;
    private GameUIDataContext.GameUIDataContext _dataContext;
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

    public MainMap(int height, int width, int chunkSize, int mapSize, GameUIDataContext.GameUIDataContext dataContext){
        _dataContext = dataContext;
        Height = height;
        Width = width;
        OffsetTop = 0;
        OffsetLeft = 0;
        _maxOffsetTop = mapSize - (height / chunkSize);
        _maxOffsetLeft = mapSize - (width / chunkSize);
        _baseBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        byte[] pixels = new byte[width * height * 4];
        
        for(int i=0; i<width*height; i++){
            pixels[i*4 + 0] = 0x3A; // B
            pixels[i*4 + 1] = 0x7F; // G
            pixels[i*4 + 2] = 0x3A; // R
            pixels[i*4 + 3] = 255;  // A
        }

        _baseBitmap.WritePixels(new Int32Rect(0,0,width,height), pixels, width*4, 0);
    }

    async public Task<WriteableBitmap> RenderBitmap(){
        var bitmap = _baseBitmap.Clone();
        var chunkSize = DrwalCraft.Core.GameMap.ChunkSize;
        var renderWidth = Width / chunkSize;
        var renderHeight = Height / chunkSize;

        for(int i=0; i<renderWidth; i++){
            for(int j=0; j<renderHeight; j++){
                var mapField = DrwalCraft.Core.GameMap.Map[i+OffsetLeft,j+OffsetTop];
                var gameObject = mapField.GameObject;
                if(gameObject == null) continue;
                var objectSize = chunkSize;// * gameObject.Size;
                var objectIconPart = gameObject.GetIconPart(i+OffsetLeft, j+OffsetTop);
                if(objectIconPart is null) continue;

                if(gameObject.IsActive){
                    var objectIcon = (byte[])objectIconPart.Clone();
                    Highlight(gameObject, objectIcon, objectSize, i+OffsetLeft, j+OffsetTop);

                    var rect = new Int32Rect(i*chunkSize, j*chunkSize, objectSize, objectSize);
                    bitmap.WritePixels(rect, objectIcon, objectSize * 4 ,0);
                }
                else{
                    var rect = new Int32Rect(i*chunkSize, j*chunkSize, objectSize, objectSize);
                    bitmap.WritePixels(rect, objectIconPart, objectSize * 4 ,0);
                }
            }
        }

        return bitmap;
    }
    private void Highlight(GameObject gameObject, byte[] objectIcon, int objectSize, int positionX, int positionY){
        if(gameObject.Position.Item2 == positionY)
            for(int k=0; k<objectSize; k++){
                objectIcon[k*4 + 0] = 0x00; // B
                objectIcon[k*4 + 1] = 0xFF; // G
                objectIcon[k*4 + 2] = 0x00; // R
                objectIcon[k*4 + 3] = 0xFF; // A
            }
        
        if(gameObject.Position.Item1 == positionX)
            for(int k=0; k<objectSize*objectSize; k+=objectSize){
                objectIcon[k*4 + 0] = 0x00; // B
                objectIcon[k*4 + 1] = 0xFF; // G
                objectIcon[k*4 + 2] = 0x00; // R
                objectIcon[k*4 + 3] = 0xFF; // A
            }
        if(gameObject.Position.Item1 + gameObject.Size - 1 == positionX)
            for(int k=objectSize-1; k<objectSize*objectSize; k+=objectSize){
                objectIcon[k*4 + 0] = 0x00; // B
                objectIcon[k*4 + 1] = 0xFF; // G
                objectIcon[k*4 + 2] = 0x00; // R
                objectIcon[k*4 + 3] = 0xFF; // A
            }
        if(gameObject.Position.Item2 + gameObject.Size - 1 == positionY)
            for(int k=objectSize*(objectSize-1); k<objectSize*objectSize; k++){
                objectIcon[k*4 + 0] = 0x00; // B
                objectIcon[k*4 + 1] = 0xFF; // G
                objectIcon[k*4 + 2] = 0x00; // R
                objectIcon[k*4 + 3] = 0xFF; // A
            }
    }
}