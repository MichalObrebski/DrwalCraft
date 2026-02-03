using System.Windows.Input;
using Engine;
using Engine.Game;
using DrwalCraft.Game;

namespace DrwalCraft.Game;
public class Program{
    [STAThread]
    public static void Main(){
        var DrwalCraftApp = new Engine.App();
        var DrwalCraftWindow = new Engine.MainWindow();
        DrwalCraftWindow.MainMapOnClick = MainMapOnClick;
        DrwalCraftApp.Run(DrwalCraftWindow);
        Engine.Game.GameLoop.UpdateGameLogic = GameLoopLogic;
    }
    public static void GameLoopLogic(){

    }
    public static void MainMapOnClick(MouseButtonEventArgs e, int x, int y){
        if(e.LeftButton == MouseButtonState.Pressed){
            Engine.Game.GameMap.Map[x,y].GameObject = new DrwalCraft.Game.Troops.Soldier();
        }
        else if(e.RightButton == MouseButtonState.Pressed){
            Engine.Game.GameMap.Map[x,y].GameObject = null;
        }
        else{

        }
    }
}