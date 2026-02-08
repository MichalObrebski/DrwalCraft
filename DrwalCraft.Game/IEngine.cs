using System.Runtime.InteropServices.ObjectiveC;
using System.Windows.Input;
using Engine;
using Engine.Game;
using DrwalCraft.Game;
using Messages;
using Microsoft.VisualBasic;
using System.Windows.Media;

namespace DrwalCraft.Game;
public class Game{
    
    private readonly PriorityQueue<Message, int> InQueue;
    private readonly PriorityQueue<string, int> OutQueue;
    private readonly Lock InQueueLock;
    private readonly Lock OutQueueLock;
    private readonly SemaphoreSlim OutSemaphore;
    private readonly SemaphoreSlim InSemaphore;
    
    public Game(PriorityQueue<Message, int> inQueue, PriorityQueue<string, int> outQueue, Lock inQueueLock,
        Lock outQueueLock, SemaphoreSlim outSemaphore, SemaphoreSlim inSemaphore)
    {
        InQueue = inQueue;
        OutQueue = outQueue;
        InQueueLock = inQueueLock;
        OutQueueLock = outQueueLock;
        OutSemaphore =  outSemaphore;
        InSemaphore =  inSemaphore;
    }
    
    public static Troops.Soldier? soldier;
    [STAThread]
    public static void Main(){
        var DrwalCraftApp = new Engine.App();
        var DrwalCraftWindow = new Engine.MainWindow();
        DrwalCraftWindow.MainMapOnClick = MainMapOnClick;
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void GameLoopLogic(){
        //zdejmuj polecenia z kolejki i je wykonuj
        if(soldier != null)
            soldier.Move();
    }
    public static void MainMapOnClick(MouseButtonEventArgs e, int x, int y){
        //dodaj polecenie do kolejki o nowym zolnierzu
        Engine.Game.GameLoop.UpdateGameLogic = GameLoopLogic;
        if(e.LeftButton == MouseButtonState.Pressed){
            soldier = new DrwalCraft.Game.Troops.Soldier();
            soldier.Position = (x,y);
            Engine.Game.GameMap.Map[x,y].GameObject = soldier;
        }
        else if(e.RightButton == MouseButtonState.Pressed){
            Engine.Game.GameMap.Map[x,y].GameObject = null;
        }
        else{

        }
    }
}