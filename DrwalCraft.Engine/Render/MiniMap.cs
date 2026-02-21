using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrwalCraft.Core;
using DrwalCraft.Core.Mines;

namespace DrwalCraft.Engine.Render;

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
        var mapObject = new MapObject(ChunkSize);

        for(int i=0; i<DrwalCraft.Core.GameMap.Size; i++){
            for(int j=0; j<DrwalCraft.Core.GameMap.Size; j++){
                var gameObject = DrwalCraft.Core.GameMap.Map[i,j].GameObject;
                if(gameObject == null) continue;
                var rect = new Int32Rect(i*ChunkSize, j*ChunkSize, ChunkSize, ChunkSize);
                if(gameObject is DrwalCraft.Core.Trees.Tree){
                    bitmap.WritePixels(rect, mapObject.greenSquare, ChunkSize*4 ,0);
                }
                else{
                    if(gameObject is Core.Mines.Mine mine){
                        if(mine.CurrentPlayer == Players.game.PlayerId)
                            bitmap.WritePixels(rect, mapObject.yellowSquare, ChunkSize*4 ,0);
                        else if(mine.CurrentPlayer == Players.you.PlayerId)
                            bitmap.WritePixels(rect, mapObject.blueSquare, ChunkSize*4 ,0);
                        else
                            bitmap.WritePixels(rect, mapObject.redSquare, ChunkSize*4 ,0);
                    }
                    else if(gameObject is DrwalCraft.Core.Troops.Troop)
                        if(gameObject.Owner.PlayerId == Players.you.PlayerId)
                            bitmap.WritePixels(rect, mapObject.blueCircle, ChunkSize*4 ,0);
                        else
                            bitmap.WritePixels(rect, mapObject.redCircle, ChunkSize*4 ,0);
                    else
                        if(gameObject.Owner.PlayerId == Players.you.PlayerId)
                            bitmap.WritePixels(rect, mapObject.blueSquare, ChunkSize*4 ,0);
                        else
                            bitmap.WritePixels(rect, mapObject.redSquare, ChunkSize*4 ,0);
                }
            }
        }

        return bitmap;
    }
    private struct MapObject{
        private int _chunkSize;
        public byte[] blueSquare;
        public byte[] blueCircle;
        public byte[] greenSquare;
        public byte[] yellowSquare;
        public byte[] redSquare;
        public byte[] redCircle;
        public MapObject(int chunkSize){
            _chunkSize = chunkSize;
            byte b = 0xA0;
            byte g = 0x32;
            byte r = 0x32;
            blueSquare = Square(b, g, r);
            blueCircle = Circle(b, g, r);

            b = 0x32;
            g = 0x32;
            r = 0xA0;
            redSquare = Square(b, g, r);
            redCircle = Circle(b, g, r);

            b = 0x32;
            g = 0x55;
            r = 0x32;
            greenSquare = Square(b, g, r);

            b = 0x32;
            g = 0x58;
            r = 0x69;
            yellowSquare = Square(b, g, r);
        }
        private byte[] Square(byte b, byte g, byte r){
            byte[] square = new byte[_chunkSize * _chunkSize * 4];
            
            for(int i=0; i<(_chunkSize * _chunkSize); i++){
                square[i*4 + 0] = b;    // B
                square[i*4 + 1] = g;    // G
                square[i*4 + 2] = r;    // R
                square[i*4 + 3] = 0xFF; // A
            }

            return square;
        }
        private byte[] Circle(byte b, byte g, byte r){
            byte[] circle = Square(b, g, r);

            var ind = 0;
                circle[ind*4 + 0] = 0x3A; // B
                circle[ind*4 + 1] = 0x7F; // G
                circle[ind*4 + 2] = 0x3A; // R
                circle[ind*4 + 3] = 0xFF; // A
            ind = _chunkSize - 1;
                circle[ind*4 + 0] = 0x3A; // B
                circle[ind*4 + 1] = 0x7F; // G
                circle[ind*4 + 2] = 0x3A; // R
                circle[ind*4 + 3] = 0xFF; // A
            ind = _chunkSize * (_chunkSize - 1);
                circle[ind*4 + 0] = 0x3A; // B
                circle[ind*4 + 1] = 0x7F; // G
                circle[ind*4 + 2] = 0x3A; // R
                circle[ind*4 + 3] = 0xFF; // A
            ind = _chunkSize * _chunkSize - 1;
                circle[ind*4 + 0] = 0x3A; // B
                circle[ind*4 + 1] = 0x7F; // G
                circle[ind*4 + 2] = 0x3A; // R
                circle[ind*4 + 3] = 0xFF; // A

            return circle;
        }
    }
}