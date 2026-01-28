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

namespace Engine;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window{
    private Render.MainMap? mainMap;
    private Game.GameMap? gameMap;
    private Render.MiniMap miniMap;
    public MainWindow(){
        InitializeComponent();
    }
    protected override void OnContentRendered(EventArgs e){
        base.OnContentRendered(e);
        const int MapSize = 64;

        gameMap = new Game.GameMap(MapSize);
        var mapLock = new ReaderWriterLockSlim();

        int width = (int)GameMapGrid.ActualWidth;
        width -= width % gameMap.ChunkSize;
        GameMapGrid.Width = width;
        int height = (int)GameMapGrid.ActualHeight;
        height -= height % gameMap.ChunkSize;
        GameMapGrid.Height = height;
        UnitNameText.Text = $"h: {height}, w: {width}";
        mainMap = new Render.MainMap(height, width, gameMap.ChunkSize, MapSize);

        width = (int)MiniMapImage.Width;
        height = (int)MiniMapImage.Height;
        miniMap = new Render.MiniMap(height, width);

        Render.RenderLoop.StartRenderLoop(GameMapImage, MiniMapImage, mainMap, miniMap, gameMap, mapLock);
        Game.GameLoop.StartGameLoop(gameMap, mapLock);
        
        GameMapImage.MouseDown += MainMapOnClick;
    }
    private void MainMapMouseWheel(object sender, MouseWheelEventArgs e){
        const int scrollSpeed = 120;
        int vertical = 0;
        int horizontal = 0;
        
        if(Keyboard.IsKeyDown(Key.LeftCtrl))
            horizontal = e.Delta;
        else
            vertical = e.Delta;
            
        if(mainMap != null){
            mainMap.OffsetTop -= vertical / scrollSpeed;
            mainMap.OffsetLeft -= horizontal / scrollSpeed;
        }

        e.Handled = true;
    }
    protected void MainMapOnClick(object sender, MouseButtonEventArgs e){
        var position = Mouse.GetPosition(GameMapImage);
        if(gameMap != null && mainMap != null){
            int ChunkSize = gameMap.ChunkSize;
            int x = (int)Math.Ceiling(position.X) / 32;
            int y = (int)Math.Ceiling(position.Y) / 32;

            x += mainMap.OffsetLeft;
            y += mainMap.OffsetTop;

            gameMap.Map[x,y].GameObject = new Game.Tree();
        }
        e.Handled = true;
    }
}