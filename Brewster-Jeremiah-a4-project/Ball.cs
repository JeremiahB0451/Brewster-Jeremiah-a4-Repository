using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MohawkGame2D
{
    public class Ball
    {
        public Vector2 position;
        Vector2 velocity;
        public float size = 15;
        int wallsHit = 0;
        int wallLimit = 3;
        int upgradeAt = 1;
        bool upgraded = false;
        public int enemiesHit = 0;
        int enenmyLimit = 1;
        int enemyLimitUpgrade = 5;
        public bool destroy = false;
        Vector2[] trailPositions;
        float[] trailSizes;
        int trailLength = 40;
        Sound bounce;

        //colours
        Color ballColor;
        Color trailColor;
        Color baseColor = new Color(205, 127, 50);
        Color upgradedColor = new Color(128, 240, 240);
        Color targetColor;

        public Ball(Vector2 position, float playerRaduis, Vector2 velocity)
        {
            this.velocity = velocity * 2f; //scaled speed
            //make sure the ball moves
            if (velocity.Length() == 0) this.velocity = new Vector2(0, -10);
            this.position = position + Vector2.Normalize(this.velocity) * playerRaduis; //tip of player ship

            //initilize the trailPositions to the starting position and trailSizes to 0
            trailPositions = new Vector2[trailLength];
            trailSizes = new float[trailLength];
            for (int i = 0; i < trailLength; i++) {
                trailPositions[i] = this.position;
                trailSizes[i] = 0;
            }

            //initilize colours
            ballColor = baseColor;
            trailColor = new Color(baseColor.R, baseColor.G, baseColor.B, 128);

            bounce = Audio.LoadSound("Assets/WallBounce.wav");
            Audio.SetVolume(bounce, 0.5f);
        }

        public void Update()
        {
            Motion();
            Colision();
            Logic();
            Render();
        }

        //calculates the motion and updates the position
        void Motion()
        {
            position += velocity * Time.DeltaTime;
        }

        //handles wall colisions
        void Colision() {
            Vector2 direction = Vector2.Normalize(velocity);
            if (position.X + size > Window.Width) { //right wall
                //snap
                Vector2 perpindicular = -Vector2.UnitX;
                float distancePast = position.X + size - Window.Width;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.X *= -1;
                wallsHit++;
                Audio.Play(bounce);
            }
            if (position.X - size < 0) { //left wall
                //snap
                Vector2 perpindicular = Vector2.UnitX;
                float distancePast = position.X - size;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.X *= -1;
                wallsHit++;
                Audio.Play(bounce);
            }
            if (position.Y + size > Window.Height) { //bottom wall
                //snap
                Vector2 perpindicular = -Vector2.UnitY;
                float distancePast = position.Y + size - Window.Height;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.Y *= -1;
                wallsHit++;
                Audio.Play(bounce);
            }
            if (position.Y - size < 0) { //top wall
                //snap
                Vector2 perpindicular = Vector2.UnitY;
                float distancePast = position.Y - size;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.Y *= -1;
                wallsHit++;
                Audio.Play(bounce);
            }
        }

        //handles ball-specific logic
        void Logic()
        {
            //destroy ball if it has hit too many walls
            if (wallsHit >= wallLimit) {
                Audio.Stop(bounce);
                destroy = true;
                return;
            }
            //'upgrade' ball after it hits enough walls
            if (wallsHit >= upgradeAt && !upgraded) {
                enenmyLimit = enemyLimitUpgrade;
                velocity *= 1.5f;
                ballColor = upgradedColor;
                targetColor = Random.Color();
                upgraded = true;
            }
            //destroy ball after hitting too many enemies
            if (enemiesHit >= enenmyLimit)
            {
                destroy = true;
                return;
            }

            //trail logic
            //decreace the size of each 'active' part
            float shrinkAmount = size / trailLength;
            for (int i = 0; i < trailLength; i++) {
                if (trailSizes[i] > 0) trailSizes[i] -= shrinkAmount;
            }
            //find the first 'inactive' (size <= 0) part and make it active: set to current position
            for (int i = 0; i < trailLength; i++) {
                if (trailSizes[i] <= 0) {
                    trailSizes[i] = size;
                    trailPositions[i] = position;
                    break;
                }
            }

            //make the trail change color when upgraded
            if (upgraded) {
                //bring the color closer to target color
                trailColor.R = (int)MathF.Round(float.Lerp(trailColor.R, targetColor.R, 0.10f));
                trailColor.G = (int)MathF.Round(float.Lerp(trailColor.G, targetColor.G, 0.10f));
                trailColor.B = (int)MathF.Round(float.Lerp(trailColor.B, targetColor.B, 0.10f));
                //once close enough, get a new target
                if (MathF.Abs(trailColor.R - targetColor.R) < 10 &&
                    MathF.Abs(trailColor.G - targetColor.G) < 10 &&
                    MathF.Abs(trailColor.B - targetColor.B) < 10) {
                    targetColor = Random.Color();
                }
            }
        }

        //draws the ball
        void Render()
        {
            //trail
            Draw.SetLineSize(0);
            Draw.SetFillColor(trailColor);
            for (int i = 0; i < trailLength; i++) {
                Draw.Circle(trailPositions[i], trailSizes[i]);
            }

            //ball
            Draw.SetLineSize(5);
            Draw.SetLineColor(128, 0, 0);
            Draw.SetFillColor(ballColor);
            Draw.Circle(position, size);
        }
    }
}
