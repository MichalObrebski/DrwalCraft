using System.Printing;

namespace Engine.Game;

public static class GameMap{
    public struct ObjectId{
        public byte Player {set; get;}
        public byte Type {set; get;}
        public GameObject? GameObject {set; get;}

        public ObjectId(byte player, byte type, GameObject? gameObject){
            Player = player;
            Type = type;
            GameObject = gameObject;
        }
    }

    public static ObjectId[,] Map{set; get;}
    public static int Size{set; get;}

    public static int ChunkSize{set; get;}

    public static void Init(int size){
        ChunkSize = 32;
        Size = size;
        Map = new ObjectId[size, size];
        for(int i=0; i<size; i++){
            for(int j=0; j<size; j++){
                Map[i,j] = new ObjectId(0, 0, null);
            }
        }
    }

    public static IEnumerator<int> ModTrees(){
        for(int i=4;i<16;i++){
            for(int j=4;j<16;j++){
                Map[i,j] = new Game.GameMap.ObjectId(0, 0, new Game.Tree());yield return 0;
            }
        }
    }
    public static (int, int)? FindPath((int, int) position, (int, int) target, int range){
        (int, int)? result = null;
        int i = 0;
        while(position != target){
            if(i == range) break;
            (int, int) nextField;
            if(position.Item1 == target.Item1){
                nextField = (position.Item2 < target.Item2) switch {
                    true => (position.Item1, position.Item2 + 1),
                    false => (position.Item1, position.Item2 - 1),
                };
            }
            else if(position.Item2 == target.Item2){
                nextField = (position.Item1 < target.Item1) switch {
                    true => (position.Item1 + 1, position.Item2),
                    false => (position.Item1 - 1, position.Item2),
                };
            }
            else{
                nextField = (position.Item1 < target.Item1, position.Item2 < target.Item2) switch {
                    (true, true) => (position.Item1 + 1, position.Item2 + 1),
                    (true, false) => (position.Item1 + 1, position.Item2 - 1),
                    (false, true) => (position.Item1 - 1, position.Item2 + 1),
                    (false, false) => (position.Item1 - 1, position.Item2 - 1)
                };
            }
            if(result == null) result = nextField;

            // Map[]

            i++;
        }
        return result;
    }
    public static IEnumerator<int> Trees(){
        while(true){
        Map[11,12].GameObject = new Game.Tree();yield return 0;
        Map[11,13].GameObject =  new Game.Tree();yield return 0;
        Map[12,12].GameObject =  new Game.Tree();yield return 0;
        Map[12,13].GameObject =  new Game.Tree();yield return 0;
        Map[13,12].GameObject =  new Game.Tree();yield return 0;
        Map[13,13].GameObject =  new Game.Tree();yield return 0;
        Map[14,12].GameObject =  new Game.Tree();yield return 0;
        Map[15,12].GameObject =  new Game.Tree();yield return 0;
        Map[16,12].GameObject =  new Game.Tree();yield return 0;
        Map[16,13].GameObject =  new Game.Tree();yield return 0;
        Map[17,12].GameObject =  new Game.Tree();yield return 0;
        Map[17,13].GameObject =  new Game.Tree();yield return 0;
        Map[18,12].GameObject =  new Game.Tree();yield return 0;
        Map[18,13].GameObject =  new Game.Tree();yield return 0;
        Map[12,11].GameObject =  new Game.Tree();yield return 0;
        Map[17,11].GameObject =  new Game.Tree();yield return 0;
        Map[12,14].GameObject =  new Game.Tree();yield return 0;
        Map[17,14].GameObject =  new Game.Tree();yield return 0;
        Map[14,11].GameObject =  new Game.Tree();yield return 0;
        Map[14,10].GameObject =  new Game.Tree();yield return 0;
        Map[14,9].GameObject  =  new Game.Tree();yield return 0;
        Map[14,8].GameObject  =  new Game.Tree();yield return 0;
        Map[14,7].GameObject  =  new Game.Tree();yield return 0;
        Map[14,6].GameObject  =  new Game.Tree();yield return 0;
        Map[14,5].GameObject  =  new Game.Tree();yield return 0;
        Map[15,11].GameObject =  new Game.Tree();yield return 0;
        Map[15,10].GameObject =  new Game.Tree();yield return 0;
        Map[15,9].GameObject  =  new Game.Tree();yield return 0;
        Map[15,8].GameObject  =  new Game.Tree();yield return 0;
        Map[15,7].GameObject  =  new Game.Tree();yield return 0;
        Map[15,6].GameObject  =  new Game.Tree();yield return 0;
        Map[15,5].GameObject  =  new Game.Tree();yield return 0;
        Map[14,4].GameObject  =  new Game.Tree();yield return 0;
        Map[15,4].GameObject  =  new Game.Tree();yield return 0;
        Map[13,5].GameObject  =  new Game.Tree();yield return 0;
        Map[16,5].GameObject  =  new Game.Tree();yield return 0;

        for(int i=0;i<24;i++)
            yield return 0;

        for(int i=0; i<Size; i++){
            for(int j=0; j<Size; j++){
                Map[i,j].GameObject = null;
            }
        }
        yield return 0;
    }
}
}
