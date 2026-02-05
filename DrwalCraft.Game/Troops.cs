using Engine;
using Engine.Game;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DrwalCraft.Game.Troops;
public interface ITroop : Engine.Game.IGameObject{
    public void MainAction();
    public void Move();
}

public abstract class Troop : GameObject, ITroop{
    protected int range;
    protected int _speed;
    protected int _actionSpeed;
    protected (int, int)? TravelTarget{get; set;}
    protected int position;
    protected GameObject? AttackTarget{get;set;}
    protected int _moveProgress;
    public abstract void MainAction();
    public abstract void Move();
}

public class Soldier: Troop{
    public Soldier(){
        _speed = 12;
        _actionSpeed = 1;
        TravelTarget = null;
        AttackTarget = null;
        _moveProgress = 0;
        var fullPath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Icons",
            "Tree.png");
        var uri = new Uri(fullPath);
        //var uri = new Uri("../Assets/Icons/Tree.png", UriKind.Relative);
        var sourceImage = new BitmapImage(uri);

        FormatConvertedBitmap convertedBitmap = new FormatConvertedBitmap();
        convertedBitmap.BeginInit();
        convertedBitmap.Source = sourceImage;
        convertedBitmap.DestinationFormat = PixelFormats.Pbgra32;
        convertedBitmap.EndInit();

        int width = convertedBitmap.PixelWidth;
        int height = convertedBitmap.PixelHeight;
        int bytesPerPixel = (convertedBitmap.Format.BitsPerPixel + 7) / 8; // Dla Pbgra32 to 4
        int stride = width * bytesPerPixel; // Długość jednego wiersza w bajtach

        // // 4. Przygotowanie tablicy bajtów
        this.ObjectIcon = new byte[height * stride];

        // // 5. Kopiowanie pikseli z Tree.png do tablicy
        convertedBitmap.CopyPixels(ObjectIcon, stride, 0);
    }
    public override void MainAction()
    {
        //jesli odleglosc od Targetu < range - w jednej lini bez przeszkód
        
    }
    public override void Move(){
        if(_moveProgress == 0){
            (int, int) nextPosition = Engine.Game.GameMap.FindPath(this.Position, (16,16), 8) ?? (-1, -1);
            GameObject? self = Engine.Game.GameMap.Map[this.Position.Item1, this.Position.Item2].GameObject;
            if(nextPosition != (-1, -1)){
                Engine.Game.GameMap.Map[this.Position.Item1, this.Position.Item2].GameObject = null;
                Engine.Game.GameMap.Map[nextPosition.Item1, nextPosition.Item2].GameObject = self;
                this.Position = nextPosition;
                _moveProgress = _speed;
            }
        }
        else{
            _moveProgress --;
        }
    }
}