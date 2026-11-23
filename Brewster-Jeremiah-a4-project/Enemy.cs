using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MohawkGame2D
{
    public class Enemy
    {
        public Vector2 position;
        float speed = 50;
        public Vector2 hitboxSize = new Vector2(100, 75);
        public int addScore = 0; //score to add when destroyed
        int scorePerHit = 100;
        public bool destroy = false;
        public bool gameOver = false;

        public Enemy()
        {

        }

        public void Update(Player player, Ball ball)
        {
            Motion();
            Colision(player, ball);
            Render();
        }

        //moves the enemy
        void Motion() {
            position.Y += speed * Time.DeltaTime;
        }

        //colision logic against the ball and player
        void Colision(Player player, Ball ball) { 
            //ball colision
            if (ball != null &&
                position.X - ball.size < ball.position.X &&
                position.X + ball.size + hitboxSize.X > ball.position.X &&
                position.Y - ball.size < ball.position.Y &&
                position.Y + ball.size + hitboxSize.Y > ball.position.Y) 
            {
                ball.enemiesHit++;
                addScore += ball.enemiesHit * scorePerHit;
                destroy = true;
                return;
            }

            //player colision
            if (position.X - player.size < player.position.X &&
                position.X + player.size + hitboxSize.X > player.position.X &&
                position.Y - player.size < player.position.Y &&
                position.Y + player.size + hitboxSize.Y > player.position.Y)
            {
                gameOver = true;
                return;
            }

            //bottom of screen
            if (position.Y + hitboxSize.Y >= Window.Height) {
                gameOver = true;
                return;
            }
        }

        //draw the enemy
        void Render() {
            Draw.SetLineSize(0);
            Draw.SetFillColor(Color.Red);
            Draw.Rectangle(position, hitboxSize);
        }
    }
}
