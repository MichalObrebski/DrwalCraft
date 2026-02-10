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
using DrwalCraft.Engine;
using DrwalCraft.Engine.Render.GameUIDataContext;

namespace DrwalCraft.Engine;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window{
    public Render.MainMap? mainMap;
    public Render.MiniMap? miniMap;
    public Action<MouseButtonEventArgs, int, int, GameUIDataContext?> MainMapOnClick = (MouseButtonEventArgs e, int x, int y, GameUIDataContext? dc) => {};
    public Action<MouseButtonEventArgs, (int, int), (int, int), GameUIDataContext?> MainMapSelection = (MouseButtonEventArgs e, (int, int) start, (int, int) end, GameUIDataContext? dataContext) => {};
    public Action ContentRenderd = () => {};
    public CancellationToken ct = new ();
    public bool mouseDown = false;
    public Point mouseDownPosition;
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
        mainMap = new Render.MainMap(height, width, DrwalCraft.Core.GameMap.ChunkSize, MapSize, this.DataContext as GameUIDataContext);

        width = (int)MiniMapImage.Width;
        height = (int)MiniMapImage.Height;
        miniMap = new Render.MiniMap(height, width);

        Render.RenderLoop.StartRenderLoop(GameMapImage, MiniMapImage, mainMap, miniMap, mapLock, ct);
        DrwalCraft.Engine.GameLoop.GameLoop.StartGameLoop(mapLock);
        ContentRenderd();
        
        GameMapImage.MouseDown += MainMapClick;
        GameMapImage.MouseDown += MainMapMouseDown;
        GameMapImage.MouseUp += MainMapMouseUp;
        GameMapImage.MouseMove += MainMapMouseMove;
        
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
    }
    protected void MainMapMouseDown(object sender, MouseButtonEventArgs e){
        if(e.LeftButton != MouseButtonState.Pressed) return;

        mouseDown = true;
        mouseDownPosition = e.GetPosition(GameMapImage);
        GameMapImage.CaptureMouse();
        
        Canvas.SetLeft(MainMapSelectionBox, mouseDownPosition.X);
        Canvas.SetTop(MainMapSelectionBox, mouseDownPosition.Y);
        MainMapSelectionBox.Width = 0;
        MainMapSelectionBox.Height = 0;
        
        MainMapSelectionBox.Visibility = Visibility.Visible;
    }
    protected void MainMapMouseUp(object sender, MouseButtonEventArgs e){
        if(!mouseDown) return;
        var dataContext = this.DataContext as GameUIDataContext;

        mouseDown = false;
        GameMapImage.ReleaseMouseCapture();
        
        MainMapSelectionBox.Visibility = Visibility.Collapsed;
        
        Point mouseUpPosition = e.GetPosition(GameMapImage);

        if(mainMap != null){
            int ChunkSize = DrwalCraft.Core.GameMap.ChunkSize;
            int startX = (int)Math.Ceiling(mouseDownPosition.X) / ChunkSize;
            int startY = (int)Math.Ceiling(mouseDownPosition.Y) / ChunkSize;

            startX += mainMap.OffsetLeft;
            startY += mainMap.OffsetTop;
            startX = startX >= DrwalCraft.Core.GameMap.Size? DrwalCraft.Core.GameMap.Size - 1 : startX;
            startY = startY >= DrwalCraft.Core.GameMap.Size? DrwalCraft.Core.GameMap.Size - 1 : startY;


            int endX = (int)Math.Ceiling(mouseUpPosition.X) / ChunkSize;
            int endY = (int)Math.Ceiling(mouseUpPosition.Y) / ChunkSize;

            endX += mainMap.OffsetLeft;
            endY += mainMap.OffsetTop;
            endX = endX >= DrwalCraft.Core.GameMap.Size? DrwalCraft.Core.GameMap.Size - 1 : endX;
            endY = endY >= DrwalCraft.Core.GameMap.Size? DrwalCraft.Core.GameMap.Size - 1 : endY;

            if(endX < startX)
                (startX, endX) = (endX, startX);
            if (endY < startY)
                (startY, endY) = (endY, startY);

            MainMapSelection(e, (startX, startY), (endX, endY), dataContext);
        }
    }
    protected void MainMapMouseMove(object sender, MouseEventArgs e){
        if (mouseDown){
            Point mousePos = e.GetPosition(GameMapImage);

            if (mouseDownPosition.X < mousePos.X){
                Canvas.SetLeft(MainMapSelectionBox, mouseDownPosition.X);
                MainMapSelectionBox.Width = mousePos.X - mouseDownPosition.X;
            }
            else{
                Canvas.SetLeft(MainMapSelectionBox, mousePos.X);
                MainMapSelectionBox.Width = mouseDownPosition.X - mousePos.X;
            }

            if (mouseDownPosition.Y < mousePos.Y){
                Canvas.SetTop(MainMapSelectionBox, mouseDownPosition.Y);
                MainMapSelectionBox.Height = mousePos.Y - mouseDownPosition.Y;
            }
            else{
                Canvas.SetTop(MainMapSelectionBox, mousePos.Y);
                MainMapSelectionBox.Height = mouseDownPosition.Y - mousePos.Y;
            }
        }
    }
    protected void OnProduceClick(object sender, RoutedEventArgs e){
        var button = sender as Button;
        var product = button.DataContext as Type;
        var building = button.Tag as DrwalCraft.Core.Buildings.Building;

        if (building != null && product != null){
            if(building is DrwalCraft.Core.Buildings.Barrack barrack)
                barrack.Produce(product);
        }
}
}