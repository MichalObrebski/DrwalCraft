using System.Windows.Input;
using Engine;
using DrwalCraft.Game;
using Microsoft.VisualBasic;
using System.Windows.Media;
using DrwalCraft.Core;
using Engine.Render.GameUIDataContext;
using DrwalCraft.Core.Troops;
using DrwalCraft.Core.Army;

namespace DrwalCraft.Game;
public class Program{
    public static DrwalCraft.Core.Troops.Soldier? soldier;
    public static DrwalCraft.Core.Troops.Soldier? soldier1;
    public static DrwalCraft.Core.Troops.Soldier? soldier2;
    public static DrwalCraft.Core.Troops.Soldier? soldier3;
    [STAThread]
    public static void Main(){
        var DrwalCraftApp = new Engine.App();
        var DrwalCraftWindow = new Engine.MainWindow();
        DrwalCraftWindow.MainMapOnClick = MainMapOnClick;
        DrwalCraftWindow.ContentRenderd = ContentRenderd;
        DrwalCraftWindow.MainMapSelection = MainMapSelection;
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void ContentRenderd(){
        Engine.GameLoop.GameLoop.UpdateGameLogic = GameLoopLogic;
        soldier1 = new DrwalCraft.Core.Troops.Soldier();
        soldier1.Position = (16,16);
        DrwalCraft.Core.GameMap.Map[16,16].GameObject = soldier1;
        soldier2 = new DrwalCraft.Core.Troops.Soldier();
        soldier2.Position = (8,16);
        DrwalCraft.Core.GameMap.Map[8,16].GameObject = soldier2;
        soldier3 = new DrwalCraft.Core.Troops.Soldier();
        soldier3.Position = (8,8);
        soldier3.TravelTarget = (20,10);
        DrwalCraft.Core.GameMap.Map[8,8].GameObject = soldier3;
        DrwalCraft.Core.GameMap.Map[2,2].GameObject = new DrwalCraft.Core.Tree();
        DrwalCraft.Core.GameMap.AddObjectToMap(24, 8, new DrwalCraft.Core.Buildings.Building());
        DrwalCraft.Core.GameMap.AddObjectToMap(12, 8, new Soldier(5, 10));
    }
    public static void GameLoopLogic(){
        if(soldier1 != null)
            soldier1.MainAction();
        if(soldier2 != null)
            soldier2.MainAction();
        if(soldier3 != null)
            soldier3.MainAction();
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
                    army.AddTroop(troop);
                }
            }
        }
        if(army.Troops.Count > 1)
            if(dataContext is not null)
                dataContext.ActiveUnit = army;
    }
}