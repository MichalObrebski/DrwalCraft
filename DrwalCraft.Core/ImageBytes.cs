using System.Buffers.Binary;
using System.Collections;
using System.Runtime.InteropServices;

namespace DrwalCraft.Core;

public class ImageBytes{
    private int _bytesPerPixel = 4;
    private int _chunkSize = GameMap.ChunkSize;
    private int _stride;
    private int _height;
    private int _width;
    public byte[] Bytes;
    public int BytesPerPixel {get => _bytesPerPixel;}
    public int Height {get => _height;}
    public int Width {get => _width;}
    public int Stride {get => _stride;}

    public uint this[int x, int y]{
        get{
            int pixelPosition = x * _bytesPerPixel + y * _stride;
            return BinaryPrimitives.ReadUInt32BigEndian(Bytes.AsSpan(pixelPosition, _bytesPerPixel));
        }
        set{
            int pixelPosition = x * _bytesPerPixel + y * _stride;
            BinaryPrimitives.WriteUInt32BigEndian(Bytes.AsSpan(pixelPosition, _bytesPerPixel), value);
        }
    }
    public byte this[int x, int y, int n]{
        get => Bytes[x * _bytesPerPixel + y * _stride + n];
        set => Bytes[x * _bytesPerPixel + y * _stride + n] = value;
    }

    public ImageBytes(int width, int height){
        _width = width * _chunkSize;
        _height = height * _chunkSize;
        _stride = _width * _bytesPerPixel;
        Bytes = new byte[_height * _stride];
    }
    public ImageBytes(int size) : this(size, size){}

    public bool IndexBoundSafeGetPixel(int x, int y, out uint? pixel){
        if(x < 0 || y < 0 || x >= _width || y >= _height){
            pixel = null;
            return false;
        }
        pixel = this[x, y];
        return true;
    }
    public bool IndexBoundSafeSetPixel(int x, int y, uint value){
        if(x < 0 || y < 0 || x >= _width || y >= _height){
            return false;
        }
        this[x, y] = value;
        return true;
    }
}