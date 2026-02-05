using System.Windows.Input;
using Engine;
using Engine.Game;
using DrwalCraft.Game;
using Microsoft.VisualBasic;
using System.Windows.Media;
using DrwalCraft.Game.Troops;

namespace DrwalCraft.Game;
public class Program{
    public static Troops.Soldier? soldier;
    public static Soldier? soldier1;
    public static Soldier? soldier2;
    public static Soldier? soldier3;
    [STAThread]
    public static void Main(){
        var DrwalCraftApp = new Engine.App();
        var DrwalCraftWindow = new Engine.MainWindow();
        DrwalCraftWindow.MainMapOnClick = MainMapOnClick;
        DrwalCraftWindow.ContentRenderd = ContentRenderd;
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void ContentRenderd(){
        Engine.Game.GameLoop.UpdateGameLogic = GameLoopLogic;
        soldier1 = new Troops.Soldier();
        soldier1.Position = (16,16);
        Engine.Game.GameMap.Map[16,16].GameObject = soldier1;
        soldier2 = new Troops.Soldier();
        soldier2.Position = (8,16);
        Engine.Game.GameMap.Map[8,16].GameObject = soldier2;
        soldier3 = new Troops.Soldier();
        soldier3.Position = (8,8);
        soldier3.TravelTarget = (20,10);
        Engine.Game.GameMap.Map[8,8].GameObject = soldier3;
        Engine.Game.GameMap.Map[2,2].GameObject = new Engine.Game.Tree();
    }
    public static void GameLoopLogic(){
        if(soldier1 != null)
            soldier1.Move();
        if(soldier2 != null)
            soldier2.Move();
        if(soldier3 != null)
            soldier3.Move();
    }
    public static void MainMapOnClick(MouseButtonEventArgs e, int x, int y){
        if(e.LeftButton == MouseButtonState.Pressed){
            var field = Engine.Game.GameMap.Map[x,y].GameObject;
            if(field is Soldier s){
                soldier = s;
            }
        }
        else if(e.RightButton == MouseButtonState.Pressed){
            if(soldier!=null)soldier.TravelTarget = (x,y);
        }
        else{

        }
    }
}