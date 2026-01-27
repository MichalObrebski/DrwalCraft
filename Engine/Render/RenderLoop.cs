using System.Diagnostics;
using System.DirectoryServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Engine;

namespace Engine.Render;

public static class RenderLoop
{
    public static void StartRenderLoop(
        Image gameMapImage,
        Image miniMapImage,
        MainMap mainMap,
        MiniMap miniMap,
        Game.GameMap map,
        ReaderWriterLockSlim mapLock
    ){
        var renderThread = new Thread(async () => {
                while(true){
                    mapLock.EnterReadLock();
                    try{
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>{
                            gameMapImage.Source = mainMap.RenderBitmap(map);
                            miniMapImage.Source = miniMap.RenderBitmap(map);
                        });
                    }
                    finally{
                        mapLock.ExitReadLock();
                    }
                    Thread.Sleep(5);
                }
        });

        renderThread.IsBackground = true;
        renderThread.Start();
    }
}

//     private static void Loop(Game.GameMap map, ReaderWriterLockSlim mapLock){
//         const double TargetDt = 1000.0;// / 36.0; 
        
//         var timer = Stopwatch.StartNew();
//         double accumulator = 0.0;
//         long lastTime = 0;
//         var trees = map.Trees();
//         while (true){
//             long currentTime = timer.ElapsedMilliseconds;
//             double deltaTime = currentTime - lastTime;
//             lastTime = currentTime;

//             accumulator += deltaTime;

//             while (accumulator >= TargetDt){
//                 mapLock.EnterWriteLock();
//                 try{
//                     UpdateGameLogic(map, mapLock);
//                     trees.MoveNext();
//                     accumulator -= TargetDt;
//                 }
//                 finally{
//                     mapLock.ExitWriteLock();
//                 }
//             }

//             if (accumulator < TargetDt - 1){
//                 Thread.Sleep(1);
//             }
//         }
//     }

//     private static void UpdateGameLogic(GameMap map, ReaderWriterLockSlim mapLock){
//         // To wykona się średnio dokładnie 36 razy na sekundę
//     }
// }