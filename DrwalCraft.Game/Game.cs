using System.ComponentModel;
using System.Runtime.InteropServices.ObjectiveC;
using System.Windows.Input;
using DrwalCraft.Game;
using Messages;
using Microsoft.VisualBasic;
using System.Windows.Media;
using DrwalCraft.Core;
using DrwalCraft.UI.Render.GameUIDataContext;
using DrwalCraft.Core.Troops;
using DrwalCraft.Core.Groups;
using DrwalCraft.Core.Animations;
using System.Windows.Automation.Text;
using DrwalCraft.Core.GameLoop;
using DrwalCraft.Core.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace DrwalCraft.Game;
public class Game{
    public static DrwalCraft.Core.Troops.Soldier? soldier;
    public static DrwalCraft.Core.Troops.Soldier? soldier1;
    public static DrwalCraft.Core.Troops.Soldier? soldier2;
    public static DrwalCraft.Core.Troops.Soldier? soldier3;
    
    [STAThread]
    public static void Main(){
        Players.game = new Player(1);
        Players.you = new Player(2, 50000);
        Players.enemy = new(3);
        Players.player1 = Players.you;
        Players.player2 = Players.enemy;

        var mapLock = new ReaderWriterLockSlim();
        RunEngine(mapLock);

        var DrwalCraftApp = new DrwalCraft.UI.App();
        var DrwalCraftWindow = new DrwalCraft.UI.MainWindow{
            MainMapOnClick = TestMainMapOnClick,
            ContentRenderd = TestContentRendered,
            MainMapSelection = MainMapSelection,
            mapLock = mapLock
        };
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void TestContentRendered(){
        ContentRendered();
        Core.GameMap.AddObjectToMap(8,6,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(9,6,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(10,6,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(8,5,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(9,5,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(10,5,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(10,7,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(10,8,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(12,7,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(12,8,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(11,7,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(11,8,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(9,7,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(8,7,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(9,8,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(9,9,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(8,8,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(8,9,new Knight(Players.enemy));
        var miner = new Miner(Players.enemy);
        Core.GameMap.AddObjectToMap(14,20,miner);
        miner.Target = (14,24);
        var miner2 = new Miner(Players.enemy);
        Core.GameMap.AddObjectToMap(15,20,miner2);
        miner2.Target = (14,24);
    }
    public static void TestMainMapOnClick(MouseButtonEventArgs e, int x, int y, GameUIDataContext? dataContext){
        MainMapOnClick(e, x, y, dataContext);
        // AnimationList.Add(new Core.Animations.TakeDamage((x,y)));
    }

    [STAThread]
    public static void Run(){
        var mapLock = new ReaderWriterLockSlim();
        RunEngine(mapLock);

        var DrwalCraftApp = new DrwalCraft.UI.App();
        var DrwalCraftWindow = new DrwalCraft.UI.MainWindow{
            MainMapOnClick = MainMapOnClick,
            ContentRenderd = ContentRendered,
            MainMapSelection = MainMapSelection,
            mapLock = mapLock
        };
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    /// <summary>
    /// mapLock = null wtw gdy nie ma innego wątku czytającego/piszącego na GameMap
    /// </summary>
    public static void RunEngine(ReaderWriterLockSlim? mapLock){
        GameMap.Init(64);
        GameLoop.StartGameLoop(mapLock);
        GameLoop.UpdateGameLogic = GameLoopLogic;
    }
    public static void ContentRendered(){
        // Core.GameMap.AddObjectToMap(8,8,new Miner(Players.enemy));
        // Core.GameMap.AddObjectToMap(8,9,new Miner(Players.enemy));
        // Core.GameMap.AddObjectToMap(8,10,new Miner(Players.enemy));

        // soldier1 = new DrwalCraft.Core.Troops.Knight(Players.you);
        // DrwalCraft.Core.GameMap.AddObjectToMap(16, 16, soldier1);

        // soldier2 = new Knight(Players.enemy);
        // DrwalCraft.Core.GameMap.AddObjectToMap(12, 8, soldier2);
        // // soldier2.TravelTarget = (20,30);

        // soldier3 = new DrwalCraft.Core.Troops.Knight(Players.you);

        // DrwalCraft.Core.GameMap.AddObjectToMap(1, 2, new Builder(Players.enemy));
        // DrwalCraft.Core.GameMap.AddObjectToMap(2, 2,  new DrwalCraft.Core.Tree());
        // // DrwalCraft.Core.GameMap.AddObjectToMap(28, 24, new DrwalCraft.Core.Buildings.Castle(Players.you));
        // DrwalCraft.Core.GameMap.AddObjectToMap(24, 14, new DrwalCraft.Core.Troops.Miner(Players.you));
        // DrwalCraft.Core.GameMap.AddObjectToMap(20, 14, new DrwalCraft.Core.Troops.Miner(Players.you));
        // DrwalCraft.Core.GameMap.AddObjectToMap(16, 20, new DrwalCraft.Core.Buildings.Barrack(Players.you));
        }
    public static void GameLoopLogic(){
        ExistingObjects.TickAction();
    }
    
    public static void MainMapOnClick(MouseButtonEventArgs e, int x, int y, GameUIDataContext? dataContext){
        if(e.LeftButton == MouseButtonState.Pressed){
            var field = DrwalCraft.Core.GameMap.Map[x,y];
            if(field is null) return;

            if(dataContext != null)
                dataContext.ActiveUnit = field;
        }
        else if(e.RightButton == MouseButtonState.Pressed){
            if(dataContext == null || dataContext.ActiveUnit == null) return;
            
            var activeUnit = dataContext.ActiveUnit;
            if(activeUnit.Owner == Players.you){
                if(activeUnit is ICanReceiveTarget unit)
                    unit.Target = (x, y);
            }
        }
        else{

        }
    }
    public static void MainMapSelection(MouseButtonEventArgs e, (int, int) start, (int, int) end, GameUIDataContext? dataContext){
        var army = new DrwalCraft.Core.Groups.Army(Players.you);
        for(int i = start.Item1; i <= end.Item1; i++){
            for(int j = start.Item2; j <= end.Item2; j++){
                var gameObject = GameMap.Map[i, j];
                if(gameObject is Troop troop && troop.Owner == Players.you){
                    army.TryAddTroop(troop);
                }
            }
        }
        if(dataContext is null) return;
        
        if(army.Count > 1)
            dataContext.ActiveUnit = army;
        else if(army.Count == 1 && army.Units[0] != dataContext.ActiveUnit)
            dataContext.ActiveUnit = army.Units[0];
    }
}