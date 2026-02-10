using System.Runtime.InteropServices.ObjectiveC;
using System.Windows.Input;
using DrwalCraft.Game;
using Messages;
using Microsoft.VisualBasic;
using System.Windows.Media;
using DrwalCraft.Core;
using DrwalCraft.Engine.Render.GameUIDataContext;
using DrwalCraft.Core.Troops;
using DrwalCraft.Core.Army;

namespace DrwalCraft.Game;
public class Game{
    public static DrwalCraft.Core.Troops.Soldier? soldier;
    public static DrwalCraft.Core.Troops.Soldier? soldier1;
    public static DrwalCraft.Core.Troops.Soldier? soldier2;
    public static DrwalCraft.Core.Troops.Soldier? soldier3;
    
    private readonly PriorityQueue<Message, int> InQueue;
    private readonly PriorityQueue<Message, int> OutQueue;
    private readonly Lock InQueueLock;
    private readonly Lock OutQueueLock;
    private readonly SemaphoreSlim OutSemaphore;
    private readonly SemaphoreSlim InSemaphore;
    
    public Game(PriorityQueue<Message, int> inQueue, PriorityQueue<Message, int> outQueue, Lock inQueueLock,
        Lock outQueueLock, SemaphoreSlim outSemaphore, SemaphoreSlim inSemaphore)
    {
        InQueue = inQueue;
        OutQueue = outQueue;
        InQueueLock = inQueueLock;
        OutQueueLock = outQueueLock;
        OutSemaphore =  outSemaphore;
        InSemaphore =  inSemaphore;
    }

    [STAThread]
    public static void Main(){
        var DrwalCraftApp = new DrwalCraft.Engine.App();
        var DrwalCraftWindow = new DrwalCraft.Engine.MainWindow();
        DrwalCraftWindow.MainMapOnClick = MainMapOnClick;
        DrwalCraftWindow.ContentRenderd = ContentRenderd;
        DrwalCraftWindow.MainMapSelection = MainMapSelection;
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void ContentRenderd(){
        DrwalCraft.Engine.GameLoop.GameLoop.UpdateGameLogic = GameLoopLogic;
        soldier1 = new DrwalCraft.Core.Troops.Knight();
        DrwalCraft.Core.GameMap.AddObjectToMap(16, 16, soldier1);

        soldier2 = new Knight(5, 10);
        DrwalCraft.Core.GameMap.AddObjectToMap(12, 8, soldier2);
        soldier2.TravelTarget = (20,30);

        soldier3 = new DrwalCraft.Core.Troops.Knight();

        DrwalCraft.Core.GameMap.AddObjectToMap(8, 8, soldier3);
        DrwalCraft.Core.GameMap.AddObjectToMap(2, 2,  new DrwalCraft.Core.Tree());
        DrwalCraft.Core.GameMap.AddObjectToMap(24, 48, new DrwalCraft.Core.Buildings.Castle());
        DrwalCraft.Core.GameMap.AddObjectToMap(16, 20, new DrwalCraft.Core.Buildings.Barrack());
    }
    public static void GameLoopLogic(){
        ExistingObjects.TickAction();
        //zdejmuj polecenia z kolejki i je wykonuj
    }

    public static void MainMapOnClick(MouseButtonEventArgs e, int x, int y, GameUIDataContext? dataContext){
        //dodaj polecenie do kolejki o nowym zolnierzu
        if(e.LeftButton == MouseButtonState.Pressed){
            var field = DrwalCraft.Core.GameMap.Map[x,y].GameObject;
            if(field is null) return;

            if(dataContext != null)
                dataContext.ActiveUnit = field;
        }
        else if(e.RightButton == MouseButtonState.Pressed){
            if(dataContext == null || dataContext.ActiveUnit == null) return;
            
            var activeUnit = dataContext.ActiveUnit;
            if(activeUnit is Army army)
                foreach(var troop in army.Troops){
                    if(troop is Soldier soldier)
                        soldier.Target = (x, y);
                }
            else if(activeUnit is Soldier soldier)
                soldier.Target = (x, y);
        }
        else{

        }
    }
    public static void MainMapSelection(MouseButtonEventArgs e, (int, int) start, (int, int) end, GameUIDataContext? dataContext){
        var army = new DrwalCraft.Core.Army.Army();
        for(int i = start.Item1; i <= end.Item1; i++){
            for(int j = start.Item2; j <= end.Item2; j++){
                var gameObject = GameMap.Map[i, j].GameObject;
                if(gameObject is Troop troop && troop.PlayerId == GameObjectId.PlayerId){
                    army.TryAddTroop(troop);
                }
            }
        }
        if(dataContext is null) return;

        if(army.Troops.Count > 1)
            dataContext.ActiveUnit = army;
        else if(army.Troops.Count == 1 && army.Troops[0] != dataContext.ActiveUnit)
            dataContext.ActiveUnit = army.Troops[0];
    }
}