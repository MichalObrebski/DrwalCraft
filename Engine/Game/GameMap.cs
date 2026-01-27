namespace Engine.Game;

public class GameMap{
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

    public ObjectId[,] Map{set; get;}
    public int Size{init; get;}

    public int ChunkSize{init; get;}

    public GameMap(int size){
        ChunkSize = 32;
        Size = size;
        Map = new ObjectId[size, size];
        for(int i=0; i<size; i++){
            for(int j=0; j<size; j++){
                Map[i,j] = new ObjectId(0, 0, null);
            }
        }
    }

    public IEnumerator<int> ModTrees(){
        for(int i=4;i<16;i++){
            for(int j=4;j<16;j++){
                this.Map[i,j] = new Game.GameMap.ObjectId(0, 0, new Game.Tree());yield return 0;
            }
        }
    }
    public IEnumerator<int> Trees(){
        while(true){
        this.Map[11,12].GameObject = new Game.Tree();yield return 0;
        this.Map[11,13].GameObject =  new Game.Tree();yield return 0;
        this.Map[12,12].GameObject =  new Game.Tree();yield return 0;
        this.Map[12,13].GameObject =  new Game.Tree();yield return 0;
        this.Map[13,12].GameObject =  new Game.Tree();yield return 0;
        this.Map[13,13].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,12].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,12].GameObject =  new Game.Tree();yield return 0;
        this.Map[16,12].GameObject =  new Game.Tree();yield return 0;
        this.Map[16,13].GameObject =  new Game.Tree();yield return 0;
        this.Map[17,12].GameObject =  new Game.Tree();yield return 0;
        this.Map[17,13].GameObject =  new Game.Tree();yield return 0;
        this.Map[18,12].GameObject =  new Game.Tree();yield return 0;
        this.Map[18,13].GameObject =  new Game.Tree();yield return 0;
        this.Map[12,11].GameObject =  new Game.Tree();yield return 0;
        this.Map[17,11].GameObject =  new Game.Tree();yield return 0;
        this.Map[12,14].GameObject =  new Game.Tree();yield return 0;
        this.Map[17,14].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,11].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,10].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,9].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,8].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,7].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,6].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,5].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,11].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,10].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,9].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,8].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,7].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,6].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,5].GameObject =  new Game.Tree();yield return 0;
        this.Map[14,4].GameObject =  new Game.Tree();yield return 0;
        this.Map[15,4].GameObject =  new Game.Tree();yield return 0;
        this.Map[13,5].GameObject =  new Game.Tree();yield return 0;
        this.Map[16,5].GameObject =  new Game.Tree();yield return 0;

        for(int i=0;i<24;i++)
            yield return 0;

        for(int i=0; i<Size; i++){
            for(int j=0; j<Size; j++){
                this.Map[i,j].GameObject = null;
            }
        }
        yield return 0;
    }
}
}
