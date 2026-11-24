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
            //calculations
            Vector2 point1;
            Vector2 point2;
            Vector2 point3;
            Vector2 center = new Vector2(position.X + hitboxSize.X / 2, position.Y + hitboxSize.Y / 2);
            float spikeSize = 9;
            float radius = hitboxSize.Y / 2 - spikeSize;
            Vector2 pos1 = new Vector2(position.X + radius + spikeSize, center.Y);
            Vector2 pos2 = new Vector2(position.X + hitboxSize.X - radius - spikeSize, center.Y);

            //draw the shapes
            Draw.SetLineSize(0);
            Draw.SetLineColor(Color.Black);
            Draw.SetFillColor(Color.Red);
            //Draw.Capsule(pos1, pos2, radius);

            //bottom spikes
            for (float i = radius; i < hitboxSize.X - radius; i += spikeSize) {
                point1 = new Vector2(position.X + i, position.Y + hitboxSize.Y - spikeSize - 2); //left point
                point2 = new Vector2(position.X + i + spikeSize, position.Y + hitboxSize.Y - spikeSize - 2); //right point
                point3 = new Vector2(position.X + i + spikeSize / 2, position.Y + hitboxSize.Y); //bottom point
                Draw.Triangle(point1, point2, point3);
            }
            //top spikes
            for (float i = radius; i < hitboxSize.X - radius; i += spikeSize) {
                point1 = new Vector2(position.X + i, position.Y + spikeSize + 2); //left point
                point2 = new Vector2(position.X + i + spikeSize, position.Y + spikeSize + 2); //right point
                point3 = new Vector2(position.X + i + spikeSize / 2, position.Y); //top point
                Draw.Triangle(point1, point2, point3);
            }
            //left and right spikes
            point1 = new Vector2(position.X + spikeSize + 2, center.Y - spikeSize); //top point
            point2 = new Vector2(position.X + spikeSize + 2, center.Y + spikeSize); //bottom point
            point3 = new Vector2(position.X, center.Y); //left point
            Draw.Triangle(point1, point2, point3);
            point1 = new Vector2(position.X + hitboxSize.X - spikeSize - 2, center.Y - spikeSize); //top point
            point2 = new Vector2(position.X + hitboxSize.X - spikeSize - 2, center.Y + spikeSize); //bottom point
            point3 = new Vector2(position.X + hitboxSize.X, center.Y); //right point
            Draw.Triangle(point1, point2, point3);
            //corner spikes
            float offset = 20;
            float ratio = hitboxSize.X / hitboxSize.Y;
            //top left
            point1 = new Vector2(position.X + offset / ratio, center.Y);
            point2 = new Vector2(center.X, position.Y + offset);
            point3 = new Vector2(position.X, position.Y); //corner point
            Draw.Triangle(point1, point2, point3);
            //top right
            point1 = new Vector2(position.X + hitboxSize.X - offset / ratio, center.Y);
            point2 = new Vector2(center.X, position.Y + offset);
            point3 = new Vector2(position.X + hitboxSize.X, position.Y); //corner point
            Draw.Triangle(point1, point2, point3);
            //bottom left
            point1 = new Vector2(position.X + offset / ratio, center.Y);
            point2 = new Vector2(center.X, position.Y + hitboxSize.Y - offset);
            point3 = new Vector2(position.X, position.Y + hitboxSize.Y); //corner point
            Draw.Triangle(point1, point2, point3);
            //bottom right
            point1 = new Vector2(position.X + hitboxSize.X - offset / ratio, center.Y);
            point2 = new Vector2(center.X, position.Y + hitboxSize.Y - offset);
            point3 = new Vector2(position.X + hitboxSize.X, position.Y + hitboxSize.Y); //corner point
            Draw.Triangle(point1, point2, point3);

            //alt -- draw capsule on top 
            Draw.SetLineSize(1);
            Draw.SetLineColor(Color.Black);
            Draw.SetFillColor(Color.Red);
            Draw.Capsule(pos1, pos2, radius);

            ////show hitbox
            //Draw.SetLineSize(1);
            //Draw.SetLineColor(Color.White);
            //Draw.SetFillColor(Color.Clear);
            //Draw.Rectangle(position, hitboxSize);
        }
    }
}
