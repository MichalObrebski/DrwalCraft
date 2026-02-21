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

    public uint? this[int x, int y]{
        get{
            if(x < 0 || y < 0 || x >= _width || y >= _height)
                return null;

            int pixelPosition = x * _bytesPerPixel + y * _stride;
            return BinaryPrimitives.ReadUInt32BigEndian(Bytes.AsSpan(pixelPosition, _bytesPerPixel));
        }
        set{
            if(value is null) return;
            if(x < 0 || y < 0 || x >= _width || y >= _height)
                return;

            int pixelPosition = x * _bytesPerPixel + y * _stride;
            BinaryPrimitives.WriteUInt32BigEndian(Bytes.AsSpan(pixelPosition, _bytesPerPixel), value.Value);
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
}