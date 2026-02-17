using System.Collections;
using System.Collections.Concurrent;

namespace DrwalCraft.Core.Animations;

public static class AnimationList{
    private static ConcurrentDictionary<(int, int), List<Animation>> _animations = new();

    public static void Add(Animation newAnimation, (int, int)? position = null){
        position ??= newAnimation.Position;
        var animations = _animations.GetOrAdd(position.Value, []);
        animations.Add(newAnimation);
    }
    public static void Remove(Animation animation){
        (int, int) position = animation.Position;
        if(_animations.TryGetValue(position, out var animations)){
            animations.Remove(animation);
            if(animations.Count == 0)
                _animations.Remove(position, out var _);
        }
    }

    public static IEnumerator<KeyValuePair<(int, int), List<Animation>>> GetEnumerator(){
        var items = _animations.Order().ToList();
        foreach(var item in items){
            yield return item;
        }
    }

    public static void Draw(int x, int y, byte[] bytes){
        if(_animations.TryGetValue((x,y), out var animations)){
            for(int i=animations.Count-1; i>=0; i--){
                animations[i].Animate(bytes);
            }
        }
    }

    public static void ChangePosition(Animation animation, (int, int) newPosition){
        if(_animations.TryGetValue(animation.Position, out var list)){
            if(list.Remove(animation)){
                Add(animation, newPosition);
            }
        }
    }
}