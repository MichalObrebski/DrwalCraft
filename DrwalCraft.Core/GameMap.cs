using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrwalCraft.Core;
using DrwalCraft.Core.Buildings;
using DrwalCraft.Core.Mines;

namespace DrwalCraft.Core;

public static class GameMap{
    public struct ObjectId{
        public byte Player {set; get;}
        public byte Type {set; get;}
        public GameObject? GameObject {set; get;}
        public bool IsMainObjectPosition {set; get;}

        public ObjectId(byte player, byte type, GameObject? gameObject){
            Player = player;
            Type = type;
            GameObject = gameObject;
            IsMainObjectPosition = true;
        }
        public void SetDefault(){
            Player = 0;
            GameObject = null;
            Type = 0;
            IsMainObjectPosition = true;
        }
    }

    public static ObjectId[,] Map{set; get;}
    public static int Size{set; get;}

    public static int ChunkSize{set; get;}
    public static PriorityQueue<MapAnimation, (int, int)> mainAnimationQueue = new ();

    public static void Init(int size){
        ChunkSize = 32;
        Size = size;
        Map = new ObjectId[size, size];

        var fullPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Map",
            "Trees.png");
        var uri = new Uri(fullPath);
        var sourceImage = new BitmapImage(uri);

        FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
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
                Map[i,j] = new ObjectId(0, 0, null);
            }
        }
        for(int i=0; i<size; i++){
            for(int j=0; j<size; j++){
                var field = forest[i*4 + j*stride + 2];
                if(field == 0x00)
                    AddObjectToMap(i, j, new Tree());
                else if(field == 0x88)
                    AddObjectToMap(i, j, new Mine(Players.game));
                else if(field == 0xAA)
                    AddObjectToMap(i, j, new Castle(Players.you));
            }
        }
    }
    
    public static void AddObjectToMap(int x, int y, GameObject gameObject, bool addToExistingObjects = true){
        var objectSize = gameObject.Size;
        if(x < 0 || y < 0) return;
        if(x + objectSize > Size || y + objectSize > Size) return;

        for(int i = 0; i < objectSize; i++){
            for(int j = 0; j < objectSize; j++){
                Map[x+i, y+j].GameObject = gameObject;
                Map[x+i, y+j].IsMainObjectPosition = false;
            }
        }
        Map[x,y].IsMainObjectPosition = true;
        gameObject.Position = (x, y);

        if(gameObject is not Tree && addToExistingObjects)
            ExistingObjects.Add(gameObject);
    }
    public static ObjectId? TryGet(int x, int y){
        if(x < 0 || y < 0) return null;
        if(x >= Size || y >= Size) return null;
        return Map[x, y];
    }
    public static (int, int) GetNearestEmptyField(GameObject gameObject){
        var size = gameObject.Size + 2;
        var startingX = gameObject.Position.Item1 - 1;
        var startingY = gameObject.Position.Item2 - 1;

        while(startingX >= 0 || startingY >= 0 || startingX + size <= 64 || startingY + size <= 64){
            for(int i = startingY; i< startingY + size; i++){
                for(int j = startingX; j< startingX + size; j++){
                    var field = TryGet(j, i);
                    if(field is not null && field.Value.GameObject is null)
                        return (j, i);
                }            
            }
            startingX -= 1;
            startingY -= 1;
            size += 2;
        }
        return (-1, -1);
    }
    public static (int, int) GetNearestEmptyField((int, int) position){
        int x, y;
        (x, y) = position;
        if(x < 0 && y < 0 && x >= 64 && y >= 64) return (-1, -1);
        var gameObject = Map[x, y].GameObject;
        if(gameObject == null) return (x, y);
        return GetNearestEmptyField(gameObject);
    }

    public static IEnumerator<int> ModTrees(){
        for(int i=4;i<16;i++){
            for(int j=4;j<16;j++){
                Map[i,j] = new GameMap.ObjectId(0, 0, new DrwalCraft.Core.Tree());yield return 0;
            }
        }
    }
    private enum FieldPosition{
        XEqualYGreater,
        XEqualYSmaller,
        XGreaterYEqual,
        XSmallerYEqual,
        XGreaterYGreater,
        XSmallerYGreater,
        XGreaterYSmaller,
        XSmallerYSmaller,
    }
    public enum MapAnimation{
        TakeDamage,
    }
    public static Stack<(int, int)> BFS((int, int) position, (int, int) target, int checkSurrounding = 2){
        if(checkSurrounding == 0) return new();

        bool isSurrounded = true;
        ForEachNeighbourghingField(target, (field)=>{if(field.GameObject is null) isSurrounded = false;});
        
        if(isSurrounded){
            int x = Math.Sign(target.Item1 - position.Item1);
            int y = Math.Sign(target.Item2 - position.Item2);
            return BFS(position, (target.Item1 - x, target.Item2 - y), checkSurrounding-1);
        }

        
        var visited = new bool[Size, Size];
        var path = new (int, int)[Size, Size];
        var queue = new Queue<((int, int), (int, int))>();

        visited[position.Item1, position.Item2] = true;
        for(int i=0; i<8; i++)
            queue.Enqueue((NeigbourghingField(position, (FieldPosition)i), position));

        while(queue.Count != 0){
            var deq = queue.Dequeue();
            var currnetField = deq.Item1;
            var previousField = deq.Item2;


            if(!IsValid(currnetField) || visited[currnetField.Item1, currnetField.Item2])
                continue;
            
            visited[currnetField.Item1, currnetField.Item2] = true;
            path[currnetField.Item1, currnetField.Item2] = previousField;

            if(currnetField == target)
                break;

            if(Map[currnetField.Item1, currnetField.Item2].GameObject != null)
                continue;

            for(int i=0; i<8; i++)
                queue.Enqueue((NeigbourghingField(currnetField, (FieldPosition)i), currnetField));
        }

        var stack = new Stack<(int, int)>();

        if(visited[target.Item1, target.Item2]){
            if(Map[target.Item1, target.Item2].GameObject == null)
                stack.Push(target);
            
            while(target != position){
                target = path[target.Item1, target.Item2];
                if(target != position)
                    stack.Push(target);
            }
        }

        return stack;
    }
    private static bool IsValid((int, int) field){
        var x = field.Item1;
        var y = field.Item2;
        if(x < 0 || y < 0)
            return false;
        if(x >= Size || y >= Size)
            return false;
        return true;
    }

    public static Queue<(int, int)> CorrectPath((int, int) position, List<(int, int)> currentPath, int radius){
        var visited = new bool[2*radius + 1, 2*radius + 1];
        var path = new (int, int)[2*radius + 1, 2*radius + 1];
        var queue = new Queue<((int, int), (int, int))>();

        var blockPosition = (position.Item1 - radius, position.Item2 - radius);

        visited[radius, radius] = true;
        for(int i=0; i<8; i++)
            queue.Enqueue((NeigbourghingField(position, (FieldPosition)i), position));

        while(queue.Count != 0){
            var deq = queue.Dequeue();
            var currnetField = deq.Item1;
            var previousField = deq.Item2;


            if(currnetField.Item1 - blockPosition.Item1 < 0 || currnetField.Item2 - blockPosition.Item2 < 0)
                continue;
            if(currnetField.Item1 - blockPosition.Item1 > 2 * radius || currnetField.Item2 - blockPosition.Item2 > 2 * radius)
                continue;
            if(!IsValid(currnetField) || visited[currnetField.Item1 - blockPosition.Item1, currnetField.Item2 - blockPosition.Item2])
                continue;
            
            visited[currnetField.Item1 - blockPosition.Item1, currnetField.Item2 - blockPosition.Item2] = true;
            path[currnetField.Item1 - blockPosition.Item1, currnetField.Item2 - blockPosition.Item2] = previousField;

            if(currentPath.Contains(currnetField))
                break;

            if(Map[currnetField.Item1, currnetField.Item2].GameObject != null)
                continue;

            for(int i=0; i<8; i++)
                queue.Enqueue((NeigbourghingField(currnetField, (FieldPosition)i), currnetField));
        }

        var resultQueue = new Queue<(int, int)>();

        (int, int) target = (-1, -1);
        var tempStack = new Stack<(int, int)>();
        foreach(var field in currentPath){
            var relativeField = (field.Item1 - blockPosition.Item1, field.Item2 - blockPosition.Item2);
            if(target != (-1, -1))
                tempStack.Push(field);
            else if(relativeField.Item1 < 0 || relativeField.Item2 < 0 || relativeField.Item1 > 2*radius || relativeField.Item2 > 2*radius){}
            else if(visited[relativeField.Item1, relativeField.Item2])
                target = field;
        }

        foreach(var field in tempStack){
            resultQueue.Enqueue(field);
        }

        if(target != (-1, -1)){
            resultQueue.Enqueue(target);
            
            while(target != position){
                target = path[target.Item1 - blockPosition.Item1, target.Item2 - blockPosition.Item2];
                if(target != position)
                    resultQueue.Enqueue(target);
            }
        }

        return resultQueue;
    }

    private static (int, int) NeigbourghingField((int, int) field, FieldPosition direction){
        return direction switch{
            FieldPosition.XEqualYGreater => (field.Item1, field.Item2 + 1), 
            FieldPosition.XEqualYSmaller => (field.Item1, field.Item2 - 1),
            FieldPosition.XGreaterYEqual => (field.Item1 + 1, field.Item2),
            FieldPosition.XSmallerYEqual => (field.Item1 - 1, field.Item2),
            FieldPosition.XGreaterYGreater => (field.Item1 + 1, field.Item2 + 1),
            FieldPosition.XGreaterYSmaller => (field.Item1 + 1, field.Item2 - 1),
            FieldPosition.XSmallerYGreater => (field.Item1 - 1, field.Item2 + 1),
            FieldPosition.XSmallerYSmaller => (field.Item1 - 1, field.Item2 - 1),
            _ => (-1, -1)
        };
    }
    public static void ForEachNeighbourghingField((int, int)field, Action<ObjectId> action){
        for(int i=0; i<8; i++){
            var temp = NeigbourghingField(field, (FieldPosition)i);
            var neighbourgh = TryGet(temp.Item1, temp.Item2);
            if(neighbourgh is not null)
                action(neighbourgh.Value);
        }
    }
    public static IEnumerator<int> Trees(){
        // while(true){
        Map[11,12].GameObject = new DrwalCraft.Core.Tree();yield return 0;
        Map[11,13].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[12,12].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[12,13].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[13,12].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[13,13].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,12].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,12].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[16,12].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[16,13].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[17,12].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[17,13].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[18,12].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[18,13].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[12,11].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[17,11].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[12,14].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[17,14].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,11].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,10].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,9].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,8].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,7].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,6].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,5].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,11].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,10].GameObject =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,9].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,8].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,7].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,6].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,5].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[14,4].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[15,4].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[13,5].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;
        Map[16,5].GameObject  =  new DrwalCraft.Core.Tree();yield return 0;

        // for(int i=0;i<24;i++)
        //     yield return 0;

        // for(int i=0; i<Size; i++){
        //     for(int j=0; j<Size; j++){
        //         Map[i,j].GameObject = null;
        //     }
        // }
        // yield return 0;
    // }
    }
}
