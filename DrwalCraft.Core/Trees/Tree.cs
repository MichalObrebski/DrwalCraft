using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DrwalCraft.Core;
using DrwalCraft.Core.Animations;

namespace DrwalCraft.Core.Trees;
public class Tree: GameObject{
    public Tree() : base(Players.game, "Tree.png"){
        Name = "Tree";
    }
    public override void MainAction(){
        ExistingObjects.Remove(this);
    }
}