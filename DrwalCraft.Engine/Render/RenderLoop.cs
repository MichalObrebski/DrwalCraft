using System.Collections.Concurrent;
using System.Diagnostics;
using System.DirectoryServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrwalCraft.Engine;

namespace DrwalCraft.Engine.Render;

public static class RenderLoop{
    public static async Task StartRenderLoop(
    Image gameMapImage,
    Image miniMapImage,
    MainMap mainMap,
    MiniMap miniMap,
    ReaderWriterLockSlim mapLock,
    CancellationToken token
    ){
        ManualResetEventSlim renderState = new (true);
        var time = Stopwatch.StartNew();
        var lastStopwatch = time.ElapsedMilliseconds;
        long currentStopwatch;
        int deltaTime;
        int frameRate = 1000/60;
        while (!token.IsCancellationRequested){
            WriteableBitmap mainBmp;
            WriteableBitmap miniBmp;

            mapLock.EnterReadLock();
            try{
                mainBmp = await mainMap.RenderBitmap();
                miniBmp = await miniMap.RenderBitmap();
            }
            finally{
                mapLock.ExitReadLock();
            }

            renderState.Wait(token);
            renderState.Reset();
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() => {
                gameMapImage.Source = mainBmp;
                miniMapImage.Source = miniBmp;
                renderState.Set();
            });

            currentStopwatch = time.ElapsedMilliseconds;
            deltaTime = (int)(currentStopwatch - lastStopwatch);
            lastStopwatch = currentStopwatch;

            if(frameRate > deltaTime)
                await Task.Delay(frameRate - deltaTime, token);
        }
    }
}