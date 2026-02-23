using System.Data;

namespace DrwalCraft.Core;

public class ObjectMovement{
    private GameObject _gameObject;
    private (int, int) _target;
    private GameObject? _targetObject;
    private List<(int,int)> _path;

    public ObjectMovement(GameObject gameObject, (int, int) target){
        _path = GetNewPath(gameObject.Position, target);
        _gameObject = gameObject;
        _target = target;
        GameMap.IndexBoundSafeGet(target, out _targetObject);
    }
    public bool Move(){
        if(_path.Count == 0) return false;
        (int, int) nextPosition = _path[0];
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
        GameMap.Map[nextPosition.Item1, nextPosition.Item2] = _gameObject;
        GameMap.Map[_gameObject.Position.Item1, _gameObject.Position.Item2] = null;
        _gameObject.Position = nextPosition;
        //zdjęcie pozycji z listy
        _path.RemoveAt(0);
        return true;
    }

    private List<(int, int)> GetNewPath((int, int) position, (int, int) target){        
        bool[,] visited = new bool[GameMap.Size, GameMap.Size];
        (int, int)[,] path = new (int, int)[GameMap.Size, GameMap.Size];
        Queue<((int, int), (int, int))> queue = new();

        // kolejkowanie pól sąsiadujących z position
        visited[position.Item1, position.Item2] = true;
        GameMap.ForEachNeighbouringField(position, (neighbourgh, _) => {
            queue.Enqueue((neighbourgh, position));
        });

        // main loop dla bfs
        while(queue.Count != 0){
            //dequeue
            var deq = queue.Dequeue();
            var currnetField = deq.Item1;
            var previousField = deq.Item2;

            int x, y;
            (x, y) = currnetField;
            
            //czy pole jest dostępne (nie jest out of bound i nie było jeszcze odwiedzone)
            if(!GameMap.IndexBoundSafeGet(currnetField, out var fieldValue) || visited[x, y])
                continue;
            
            visited[x, y] = true;
            path[x, y] = previousField;

            //czy doszliśmy do celu
            if(currnetField == target || (_targetObject is not null && fieldValue == _targetObject))
                break;

            //czy można przejść przez pole
            if(fieldValue is not null && fieldValue is not Troops.Troop)
                continue;

            //kolejkowanie sąsiadujących pól
            GameMap.ForEachNeighbouringField(currnetField, (neighbour, _) => {
                queue.Enqueue((neighbour, currnetField));
            });
        }

        // tworzenie ścieżki
        var list = new List<(int, int)>();

        if(visited[target.Item1, target.Item2]){
            //dodawanie targetu do ścieżki jeżeli się da
            if(GameMap.Map[target.Item1, target.Item2] == null)
                list.Insert(0, target);
            
            //wracanie ścieżką od targetu do początku i dodawanie kolejnych przejść
            while(target != position){
                target = path[target.Item1, target.Item2];
                if(target != position)
                    list.Insert(0, target);
            }
        }

        return list;
    }

    private bool CorrectPath((int, int) position){
        bool[,] visited = new bool[GameMap.Size, GameMap.Size];
        (int, int)[,] path = new (int, int)[GameMap.Size, GameMap.Size];
        Queue<((int, int), (int, int))> queue = new();
        (int, int) target;

        // kolejkowanie pól sąsiadujących z position
        visited[position.Item1, position.Item2] = true;
        GameMap.ForEachNeighbouringField(position, (neighbourgh, _) => {
            queue.Enqueue((neighbourgh, position));
        });

        // main loop dla bfs
        while(true){
            if(queue.Count == 0)
                return false;
            //dequeue
            var deq = queue.Dequeue();
            var currnetField = deq.Item1;
            var previousField = deq.Item2;

            int x, y;
            (x, y) = currnetField;
            
            //czy pole jest dostępne (nie jest out of bound i nie było jeszcze odwiedzone)
            if(!GameMap.IndexBoundSafeGet(currnetField, out var fieldValue) || visited[x, y])
                continue;

            //czy można przejść przez pole
            if(fieldValue is not null)//trzeba umożliwić "chodzenie" przez jednostki jak zaszliśmy wystarczająco daleko
                continue;
            
            visited[x, y] = true;
            path[x, y] = previousField;

            //czy wróciliśmy na ścieżkę
            if(_path.Contains(currnetField)){
                target = currnetField;
                //usunięcie poprzedniej ścieżki aż do momentu powrotu na pierwotną trasę
                _path.RemoveRange(0, _path.IndexOf(currnetField));
                break;
            }

            //czy dotarliśmy do celu
            if(currnetField == _target || (_targetObject is not null && fieldValue == _targetObject)){
                target = currnetField;
                // tworzenie ścieżki
                _path = new ();

                if(visited[target.Item1, target.Item2]){
                    //dodawanie targetu do ścieżki jeżeli się da
                    if(GameMap.Map[target.Item1, target.Item2] == null)
                        _path.Insert(0, target);
                }
                break;
            }

            //kolejkowanie sąsiadujących pól
            GameMap.ForEachNeighbouringField(currnetField, (neighbour, _) => {
                queue.Enqueue((neighbour, currnetField));
            });
        }
        //wracanie ścieżką od targetu do początku i dodawanie kolejnych przejść
        while(target != position){
            target = path[target.Item1, target.Item2];
            if(target != position)
                _path.Insert(0, target);
        }
        return true;
    }
}