using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrwalCraft.Core;
using DrwalCraft.Core.Buildings;
using DrwalCraft.Core.Mines;

namespace DrwalCraft.Core;

public static class GameMap{

    public static GameObject?[,] Map{private set; get;} = null!;
    public static int Size{private set; get;}
    public static int ChunkSize{private set; get;}
    public static void Init(int size){
        ChunkSize = 32;
        Size = size;
        Map = new GameObject[size, size];

        var fullPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Map",
            "Trees.png");
        var uri = new Uri(fullPath);
        var sourceImage = new BitmapImage(uri);

        FormatConvertedBitmap convertedBitmap = new();
        convertedBitmap.BeginInit();
        convertedBitmap.Source = sourceImage;
        convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
        convertedBitmap.EndInit();

        int width = convertedBitmap.PixelWidth;
        int height = convertedBitmap.PixelHeight;
        int bytesPerPixel = (convertedBitmap.Format.BitsPerPixel + 7) / 8;
        int stride = width * bytesPerPixel;

        var forest = new byte[height * stride];
        convertedBitmap.CopyPixels(forest, stride, 0);

        for(int i=0; i<size; i++){
            for(int j=0; j<size; j++){
                Map[i,j] = null;
            }
        }
        for(int i=0; i<size; i++){
            for(int j=0; j<size; j++){
                var field = forest[i*4 + j*stride + 2];
                if(field == 0x00)
                    AddObjectToMap(i, j, new Trees.Tree());
                else if(field == 0x88)
                    AddObjectToMap(i, j, new Mine(Players.game));
                else if(field == 0xAA){
                    if(forest[i*4 + j*stride] == 0xFF){
                        if(Players.you.PlayerId < Players.enemy.PlayerId)
                            AddObjectToMap(i, j, new Castle(Players.you));
                        else
                            AddObjectToMap(i, j, new Castle(Players.enemy));
                    }
                    else{
                        if(Players.you.PlayerId > Players.enemy.PlayerId)
                            AddObjectToMap(i, j, new Castle(Players.you));
                        else
                            AddObjectToMap(i, j, new Castle(Players.enemy));
                    }
                }
            }
        }
    }
    public static bool AddObjectToMap(int x, int y, GameObject gameObject, bool addToExistingObjects = true){
        //czy się mieści na mapie
        var objectSize = gameObject.Size;
        if(x < 0 || y < 0) return false;
        if(x + objectSize > Size || y + objectSize > Size) return false;

        //ustawienie pól mapy
        for(int i = 0; i < objectSize; i++){
            for(int j = 0; j < objectSize; j++){
                Map[x+i, y+j] = gameObject;
            }
        }
        gameObject.Position = (x, y);

        //dodanie do listy obiektów wykonujących akcje
        if(gameObject is not Core.Trees.Tree && addToExistingObjects)
            ExistingObjects.Add(gameObject);
        return true;
    }
    public static bool IndexBoundSafeGet(int x, int y, out GameObject? gameObject){
        gameObject = null;
        if(x < 0 || y < 0 || x >= Size || y >= Size)
            return false;

        gameObject = Map[x, y];
        return true;
    }
    public static bool IndexBoundSafeGet((int x, int y) position, out GameObject? gameObject){
        return IndexBoundSafeGet(position.x, position.y, out gameObject);
    }
    public static bool TryGetNearestEmptyField(GameObject gameObject, out (int, int) resultField){
        resultField = (-1, -1);
        // granica sprawdzanego obszsaru
        int startX = gameObject.Position.Item1 - 1;
        int startY = gameObject.Position.Item2 - 1;
        int endX = startX + gameObject.Size;
        int endY = startY + gameObject.Size;

        int x = startX + 1, y = startY;
        int i = 1, j = 0; //kierunek następnego chunka

        while(startX >= 0 || startY >= 0 || endX < 64 || endY < 64){
            //idziemy po obwodzie kwadratu i jak dojdzemy do końca to skręcamy o 90deg
            while(x != startX && y != startY){
                if(IndexBoundSafeGet(x, y, out var field) && field is null){
                    resultField = (x, y);
                    return true;
                }
                x += i;
                y += j;
                if(i != 0 && (x == startX || x == endX) || 
                    j != 0 && (y == startY || y == endY)
                ){
                    (i, j) = (j, -i); // wynika to z mnożenia prze macierz rotacji o 90deg w lewo
                }
            }
            //zwiększenie sprawdzanego obszaru
            startX -= 1;
            startY -= 1;
            endX += 1;
            endY += 1;
        }
        return false;
    }
    public static bool TryGetNearestEmptyField((int x, int y) position, out (int, int) resultField){
        resultField = (-1, -1);
        int x, y;
        (x, y) = position;

        if(x < 0 && y < 0 && x >= 64 && y >= 64)
            return false;
        
        var gameObject = Map[x, y];
        if(gameObject is not null)
            return TryGetNearestEmptyField(gameObject, out resultField);
        
        resultField = (x, y);
        return true;
    }
    public enum FieldPosition{
        XEqualYGreater,
        XEqualYSmaller,
        XGreaterYEqual,
        XSmallerYEqual,
        XGreaterYGreater,
        XSmallerYGreater,
        XGreaterYSmaller,
        XSmallerYSmaller,
    }
    private static (int, int) NeighbouringField((int x, int y) field, FieldPosition direction){
        return direction switch{
            FieldPosition.XEqualYGreater => (field.x, field.y + 1), 
            FieldPosition.XEqualYSmaller => (field.x, field.y - 1),
            FieldPosition.XGreaterYEqual => (field.x + 1, field.y),
            FieldPosition.XSmallerYEqual => (field.x - 1, field.y),
            FieldPosition.XGreaterYGreater => (field.x + 1, field.y + 1),
            FieldPosition.XGreaterYSmaller => (field.x + 1, field.y - 1),
            FieldPosition.XSmallerYGreater => (field.x - 1, field.y + 1),
            FieldPosition.XSmallerYSmaller => (field.x - 1, field.y - 1),
            _ => (-1, -1)
        };
    }
    public static void ForEachNeighbouringField((int x, int y)field, Action<(int, int), GameObject?> action){
        for(int i=0; i<8; i++){
            (int x, int y) = NeighbouringField(field, (FieldPosition)i);;
            if(IndexBoundSafeGet(x, y, out var neighbour))
                action((x, y), neighbour);
        }
    }
    public static IEnumerator<int> Trees(){
        // while(true){
        Map[11,12] = new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[11,13] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[12,12] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[12,13] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[13,12] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[13,13] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,12] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,12] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[16,12] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[16,13] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[17,12] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[17,13] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[18,12] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[18,13] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[12,11] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[17,11] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[12,14] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[17,14] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,11] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,10] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,9]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,8]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,7]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,6]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,5]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,11] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,10] =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,9]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,8]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,7]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,6]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,5]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[14,4]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[15,4]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[13,5]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;
        Map[16,5]  =  new DrwalCraft.Core.Trees.Tree();yield return 0;

        // for(int i=0;i<24;i++)
        //     yield return 0;

        // for(int i=0; i<Size; i++){
        //     for(int j=0; j<Size; j++){
        //         Map[i,j] = null;
        //     }
        // }
        // yield return 0;
    // }
    }
}
