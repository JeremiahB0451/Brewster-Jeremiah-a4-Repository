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
        Ball ball;

        public void Setup()
        {
            Window.SetTitle("Space Shooter Game");
            Window.SetSize(600, 800);

            player = new Player();
        }

        public void Update()
        {
            Window.ClearBackground(background);

            if (ball != null) {
                ball.Update();
                if (ball.destroy) ball = null;
            }

            player.Update(ball);
            if (player.createBall) {
                ball = new Ball(player.position, player.size, player.velocity);
                player.createBall = false;
            }
        }
    }

}
