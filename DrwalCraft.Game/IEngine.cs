using System.ComponentModel;
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
using DrwalCraft.Core.Animations;
using System.Windows.Automation.Text;

namespace DrwalCraft.Game;
public class Game{
    public static DrwalCraft.Core.Troops.Soldier? soldier;
    public static DrwalCraft.Core.Troops.Soldier? soldier1;
    public static DrwalCraft.Core.Troops.Soldier? soldier2;
    public static DrwalCraft.Core.Troops.Soldier? soldier3;
    
    [STAThread]
    public static void Main(){
        Players.game = new Player(1);
        Players.you = new Player(2);
        Players.enemy = new(3);
        Players.player1 = Players.you;
        Players.player2 = Players.enemy;
        var DrwalCraftApp = new DrwalCraft.Engine.App();
        var DrwalCraftWindow = new DrwalCraft.Engine.MainWindow{
            MainMapOnClick = TestMainMapOnClick,
            ContentRenderd = TestContentRendered,
            MainMapSelection = MainMapSelection
        };
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void TestContentRendered(){
        ContentRendered();
        Core.GameMap.AddObjectToMap(8,6,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(9,6,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(10,6,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(9,7,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(8,7,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(8,5,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(9,5,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(10,5,new Knight(Players.you));
        Core.GameMap.AddObjectToMap(9,8,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(9,9,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(8,8,new Knight(Players.enemy));
        Core.GameMap.AddObjectToMap(8,9,new Knight(Players.enemy));
    }
    public static void TestMainMapOnClick(MouseButtonEventArgs e, int x, int y, GameUIDataContext? dataContext){
        MainMapOnClick(e, x, y, dataContext);
        // AnimationList.Add(new Core.Animations.TakeDamage((x,y)));
    }

    [STAThread]
    public static void Run(){
        var DrwalCraftApp = new DrwalCraft.Engine.App();
        var DrwalCraftWindow = new DrwalCraft.Engine.MainWindow{
            MainMapOnClick = MainMapOnClick,
            ContentRenderd = ContentRendered,
            MainMapSelection = MainMapSelection
        };
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void ContentRendered(){
        Core.GameLoop.GameLoop.UpdateGameLogic = GameLoopLogic;
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
            var field = DrwalCraft.Core.GameMap.Map[x,y].GameObject;
            if(field is null) return;

            if(dataContext != null)
                dataContext.ActiveUnit = field;
        }
        else if(e.RightButton == MouseButtonState.Pressed){
            if(dataContext == null || dataContext.ActiveUnit == null) return;
            
            var activeUnit = dataContext.ActiveUnit;
            if(activeUnit.Owner == Players.you){
                if(activeUnit is Army army)
                    foreach(var troop in army.Troops){
                        if(troop is Soldier soldier)
                            soldier.Target = (x, y);
                        else if(troop is Miner miner)
                            miner.Target = (x, y);
                        else if(troop is Builder builder)
                            builder.TravelTarget = (x, y);
                    }
                else if(activeUnit is Soldier soldier)
                    soldier.Target = (x, y);
                else if(activeUnit is Miner miner)
                    miner.Target = (x, y);
                else if(activeUnit is Troop troop)
                    troop.TravelTarget = (x, y);
            }
        }
        else{

        }
    }
    public static void MainMapSelection(MouseButtonEventArgs e, (int, int) start, (int, int) end, GameUIDataContext? dataContext){
        var army = new DrwalCraft.Core.Army.Army(Players.you);
        for(int i = start.Item1; i <= end.Item1; i++){
            for(int j = start.Item2; j <= end.Item2; j++){
                var gameObject = GameMap.Map[i, j].GameObject;
                if(gameObject is Troop troop && troop.Owner == Players.you){
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