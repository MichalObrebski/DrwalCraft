using System.Diagnostics;
using System.DirectoryServices;
using System.Threading;

namespace Engine.Game;

public static class GameLoop{
    public static void StartGameLoop(GameMap map, ReaderWriterLockSlim mapLock){
        var gameThread = new Thread(()=>{Loop(map, mapLock);});
        gameThread.IsBackground = true;
        gameThread.Start();
    }

    private static void Loop(GameMap map, ReaderWriterLockSlim mapLock){
        const int TickRate = 12;
        const double TargetDt = 1000.0 / TickRate; 
        
        var timer = Stopwatch.StartNew();
        double accumulator = 0.0;
        long lastTime = 0;
        var trees = map.Trees();
        while (true){
            long currentTime = timer.ElapsedMilliseconds;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            accumulator += deltaTime;

            while (accumulator >= TargetDt){
                mapLock.EnterWriteLock();
                try{
                    UpdateGameLogic(map, mapLock);
                    trees.MoveNext();
                    accumulator -= TargetDt;
                }
                finally{
                    mapLock.ExitWriteLock();
                }
            }

            if (accumulator < TargetDt - 1){
                Thread.Sleep(1);
            }
        }
    }

    private static void UpdateGameLogic(GameMap map, ReaderWriterLockSlim mapLock){
        // To wykona się średnio dokładnie 36 razy na sekundę
    }
}