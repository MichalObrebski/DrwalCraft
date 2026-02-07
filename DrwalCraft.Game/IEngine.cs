using System.Windows.Input;
using Engine;
using DrwalCraft.Game;
using Microsoft.VisualBasic;
using System.Windows.Media;
using DrwalCraft.Core;
using Engine.Render.GameUIDataContext;

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
    }
    public static void GameLoopLogic(){
        if(soldier1 != null)
            soldier1.Move();
        if(soldier2 != null)
            soldier2.Move();
        if(soldier3 != null)
            soldier3.Move();
    }
    public static void MainMapOnClick(MouseButtonEventArgs e, int x, int y, GameUIDataContext? dataContext){
        if(e.LeftButton == MouseButtonState.Pressed){
            var field = DrwalCraft.Core.GameMap.Map[x,y].GameObject;
            if(field is null) return;
            
            if(field is DrwalCraft.Core.Troops.Soldier s){
                soldier = s;
            }
            if(dataContext != null)
                dataContext.ActiveUnit = field;
        }
        else if(e.RightButton == MouseButtonState.Pressed){
            if(soldier!=null)soldier.TravelTarget = (x,y);
        }
        else{

        }
    }
}