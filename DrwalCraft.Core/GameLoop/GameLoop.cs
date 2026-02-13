using System.Diagnostics;
using DrwalCraft.Core;

namespace DrwalCraft.DrwalCraftCore.GameLoop;

public static class GameLoop{
    public static Action UpdateGameLogic = () => {};
    public static void StartGameLoop(ReaderWriterLockSlim mapLock){
        var gameThread = new Thread(()=>{Loop(mapLock);});
        gameThread.IsBackground = true;
        gameThread.Start();
    }

    public static int CurrentTick = 0;
    public const int OffsetTik = 3;

    private static void Loop(ReaderWriterLockSlim mapLock){
        const int TickRate = 12;
        const double TargetDt = 1000.0 / TickRate;
        
        var timer = Stopwatch.StartNew();
        double accumulator = 0.0;
        long lastTime = 0;
        var trees = DrwalCraft.Core.GameMap.Trees();
        while (true){
            long currentTime = timer.ElapsedMilliseconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            accumulator += deltaTime;

            while (accumulator >= TargetDt){
                mapLock.EnterWriteLock();
                try{
                    GameMap.mainAnimationQueue = new ();
                    UpdateGameLogic();
                    // trees.MoveNext();
                    CurrentTick++;
                    accumulator -= TargetDt;
                }
                finally{
                    mapLock.ExitWriteLock();
                }
            }

            if (accumulator < TargetDt - 1){
                Thread.Sleep((int)(TargetDt - accumulator));
            }
        }
    }

}