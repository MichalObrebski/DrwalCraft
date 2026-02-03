using System.Windows.Input;
using Engine;
using Engine.Game;
using DrwalCraft.Game;
using Microsoft.VisualBasic;
using System.Windows.Media;

namespace DrwalCraft.Game;
public class Program{
    public static Troops.Soldier? soldier;
    [STAThread]
    public static void Main(){
        var DrwalCraftApp = new Engine.App();
        var DrwalCraftWindow = new Engine.MainWindow();
        DrwalCraftWindow.MainMapOnClick = MainMapOnClick;
        DrwalCraftApp.Run(DrwalCraftWindow);
    }
    public static void GameLoopLogic(){
        if(soldier != null)
            soldier.Move();
    }
    public static void MainMapOnClick(MouseButtonEventArgs e, int x, int y){
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