using System.Collections.Concurrent;
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
using Engine.Render.GameUIDataContext;

namespace Engine;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window{
    public Render.MainMap? mainMap;
    public Render.MiniMap? miniMap;
    public Action<MouseButtonEventArgs, int, int, GameUIDataContext?> MainMapOnClick = (MouseButtonEventArgs e, int x, int y, GameUIDataContext? dc) => {};
    public Action ContentRenderd = () => {};
    public CancellationToken ct = new ();
    public MainWindow(){
        InitializeComponent();
    }
    protected override void OnContentRendered(EventArgs e){
        base.OnContentRendered(e);
        const int MapSize = 64;
        DrwalCraft.Core.GameObjectId.Init(1);

        DrwalCraft.Core.GameMap.Init(MapSize);
        var mapLock = new ReaderWriterLockSlim();

        int width = (int)GameMapGrid.ActualWidth;
        width -= width % DrwalCraft.Core.GameMap.ChunkSize;
        GameMapGrid.Width = width;
        int height = (int)GameMapGrid.ActualHeight;
        height -= height % DrwalCraft.Core.GameMap.ChunkSize;
        GameMapGrid.Height = height;
        // UnitNameText.Text = $"h: {height}, w: {width}";
        mainMap = new Render.MainMap(height, width, DrwalCraft.Core.GameMap.ChunkSize, MapSize);

        width = (int)MiniMapImage.Width;
        height = (int)MiniMapImage.Height;
        miniMap = new Render.MiniMap(height, width);

        Render.RenderLoop.StartRenderLoop(GameMapImage, MiniMapImage, mainMap, miniMap, mapLock, ct);
        GameLoop.GameLoop.StartGameLoop(mapLock);
        ContentRenderd();
        
        GameMapImage.MouseDown += MainMapClick;
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
    protected void MainMapClick(object sender, MouseButtonEventArgs e){
        var position = Mouse.GetPosition(GameMapImage);
        var dataContext = this.DataContext as GameUIDataContext;
        if(mainMap != null){
            int ChunkSize = DrwalCraft.Core.GameMap.ChunkSize;
            int x = (int)Math.Ceiling(position.X) / ChunkSize;
            int y = (int)Math.Ceiling(position.Y) / ChunkSize;

            x += mainMap.OffsetLeft;
            y += mainMap.OffsetTop;
            x = x >= DrwalCraft.Core.GameMap.Size? DrwalCraft.Core.GameMap.Size - 1 : x;
            y = y >= DrwalCraft.Core.GameMap.Size? DrwalCraft.Core.GameMap.Size - 1 : y;

            MainMapOnClick(e, x, y, dataContext);
        }
        e.Handled = true;
    }
}