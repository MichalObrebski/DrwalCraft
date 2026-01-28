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