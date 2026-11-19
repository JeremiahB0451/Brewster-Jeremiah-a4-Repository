// Include the namespaces (code libraries) you need below.
using System;
using System.Numerics;

// The namespace your code is in.
namespace MohawkGame2D
{
    public class Game
    {
        Color background = new Color(0, 0, 40);
        Player player;

        public void Setup()
        {
            Window.SetTitle("Space Shooter Game");
            Window.SetSize(600, 800);

            player = new Player();
        }

        public void Update()
        {
            Window.ClearBackground(background);

            player.Update();
        }
    }

}
