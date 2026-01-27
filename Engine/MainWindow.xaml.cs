using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Engine;
using Engine.Game;

namespace Engine;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window{
    public MainWindow(){
        InitializeComponent();
    }
    protected override void OnContentRendered(EventArgs e){
        base.OnContentRendered(e);

        var gameMap = new Game.GameMap(64);
        var mapLock = new ReaderWriterLockSlim();
        // Game.GameLoop.StartGameLoop(gameMap, mapLock);

        int width = (int)GameMapGrid.ActualWidth;
        int height = (int)GameMapGrid.ActualHeight;
        var mainMap = new Render.MainMap(height, width);

        width = (int)MiniMapImage.Width;
        height = (int)MiniMapImage.Height;
        UnitNameText.Text = $"h: {height}, w: {width}";
        var miniMap = new Render.MiniMap(height, width);
        // MiniMapImage.Source = miniMap.RenderBitmap(gameMap, mapLock);

        Render.RenderLoop.StartRenderLoop(GameMapImage, MiniMapImage, mainMap, miniMap, gameMap, mapLock);
        Game.GameLoop.StartGameLoop(gameMap, mapLock);
    }
}