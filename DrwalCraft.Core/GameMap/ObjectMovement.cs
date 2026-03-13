using System.Data;
using DrwalCraft.Core.Interfaces;

namespace DrwalCraft.Core;

public class ObjectMovement{
    private GameObject _gameObject;
    private (int, int) _target;
    private GameObject? _targetObject;
    private List<(int,int)> _path;

    public ObjectMovement(GameObject gameObject, (int, int) target){
        _gameObject = gameObject;
        _target = target;
        GameMap.IndexBoundSafeGet(target, out _targetObject);
        _path = GetNewPath(gameObject.Position, target);
    }
    public bool Move(){
        if(_path.Count == 0) return false;
        (int, int) nextPosition = _path.First();
        //czy pozycja nie jest poza mapą
        if(!GameMap.IndexBoundSafeGet(nextPosition, out var nextField))
            return false;
        //czy da się przejść a jak nie to poprawia ścieżkę
        if(nextField is not null){
            if(CorrectPath(_gameObject.Position))
                return Move();
            else
                return false;
        }

        //przesunięcie obiektu na mapie i zmiana pozycji
        if(_gameObject.Position != nextPosition){
            GameMap.Map[nextPosition.Item1, nextPosition.Item2] = _gameObject;
            GameMap.Map[_gameObject.Position.X, _gameObject.Position.Y] = null;
            _gameObject.Position = nextPosition;
        }
        //zdjęcie pozycji z listy
        _path.RemoveAt(0);
        return true;
    }
    public void Clear(){
        _path.Clear();
    }

    private List<(int, int)> GetNewPath((int x, int y) position, (int x, int y) target){        
        bool[,] visited = new bool[GameMap.Size, GameMap.Size];
        (int x, int y)[,] path = new (int x, int y)[GameMap.Size, GameMap.Size];
        Queue<((int x, int y) curr, (int x, int y) prev)> queue = new();

        // kolejkowanie pól sąsiadujących z position
        visited[position.x, position.y] = true;
        GameMap.ForEachNeighbouringField(position, (neighbourgh, _) => {
            queue.Enqueue((neighbourgh, position));
        });

        // main loop dla bfs
        while(queue.Count != 0){
            //dequeue
            var (curr, prev) = queue.Dequeue();
            var currnetField = curr;
            var previousField = prev;

            int x, y;
            (x, y) = currnetField;
            
            //czy pole jest dostępne (nie jest out of bound i nie było jeszcze odwiedzone)
            if(!GameMap.IndexBoundSafeGet(currnetField, out var fieldValue) || visited[x, y])
                continue;
            
            visited[x, y] = true;
            path[x, y] = previousField;

            //czy doszliśmy do celu
            if(currnetField == target || (_targetObject is not null && fieldValue == _targetObject)){
                target = currnetField;
                break;
            }
            
            //czy można przejść przez pole (puste lub obiekt na tym polu się porusza)
            if(fieldValue is not null && !(fieldValue is Interfaces.ICanMove fieldValueMove && fieldValueMove.IsMoving))
                continue;

            //kolejkowanie sąsiadujących pól
            GameMap.ForEachNeighbouringField(currnetField, (neighbour, _) => {
                queue.Enqueue((neighbour, currnetField));
            });
        }

        // tworzenie ścieżki
        var list = new List<(int x, int y)>();

        if(visited[target.x, target.y]){
            //dodawanie targetu do ścieżki jeżeli się da
            if(GameMap.Map[target.x, target.y] == null)
                list.Insert(0, target);
            
            //wracanie ścieżką od targetu do początku i dodawanie kolejnych przejść
            while(target != position){
                target = path[target.x, target.y];
                if(target != position)
                    list.Insert(0, target);
            }
        }

        return list;
    }

    private bool CorrectPath((int x, int y) position){
        bool[,] visited = new bool[GameMap.Size, GameMap.Size];
        (int x, int y)[,] path = new (int x, int y)[GameMap.Size, GameMap.Size];
        Queue<((int x, int y) curr, (int x, int y) prev)> queue = new();
        (int x, int y) target;

        // kolejkowanie pól sąsiadujących z position
        visited[position.x, position.y] = true;
        GameMap.ForEachNeighbouringField(position, (neighbourgh, _) => {
            queue.Enqueue((neighbourgh, position));
        });

        // main loop dla bfs
        while(true){
            if(queue.Count == 0)
                return false;
            //dequeue
            var (curr, prev) = queue.Dequeue();
            var currnetField = curr;
            var previousField = prev;

            int x, y;
            (x, y) = currnetField;
            
            //czy pole jest dostępne (nie jest out of bound i nie było jeszcze odwiedzone)
            if(!GameMap.IndexBoundSafeGet(currnetField, out var fieldValue) || visited[x, y])
                continue;
            
            visited[x, y] = true;
            path[x, y] = previousField;

            //czy dotarliśmy do celu
            if(currnetField == _target || (_targetObject is not null && fieldValue == _targetObject)){
                target = currnetField;
                // tworzenie nowej ścieżki
                _path.Clear();
                //dodawanie targetu do ścieżki jeżeli się da
                if(GameMap.Map[target.x, target.y] == null)
                    _path.Insert(0, target);
                break;
            }

            //czy można przejść przez pole
            if(fieldValue is not null)//trzeba umożliwić "chodzenie" przez jednostki jak zaszliśmy wystarczająco daleko
                continue;

            //czy wróciliśmy na ścieżkę
            if(_path.Contains(currnetField)){
                target = currnetField;
                //usunięcie poprzedniej ścieżki aż do momentu powrotu na pierwotną trasę
                _path.RemoveRange(0, _path.IndexOf(currnetField));
                break;
            }

            //kolejkowanie sąsiadujących pól
            GameMap.ForEachNeighbouringField(currnetField, (neighbour, _) => {
                queue.Enqueue((neighbour, currnetField));
            });
        }
        //wracanie ścieżką od targetu do początku i dodawanie kolejnych przejść
        while(target != position){
            target = path[target.x, target.y];
            if(target != position)
                _path.Insert(0, target);
        }
        return true;
    }
}