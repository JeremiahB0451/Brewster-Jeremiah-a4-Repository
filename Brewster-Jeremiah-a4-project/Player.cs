using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MohawkGame2D
{
    public class Player
    {
        public Vector2 position = new Vector2(300, 700);
        public Vector2 velocity = new Vector2(0,0);
        Vector2 acceleration = new Vector2(0, 0);
        public float size = 40;
        public bool createBall = false;

        public Player()
        {

        }

        public void Update(Ball ball)
        {
            PInput(ball);
            Motion();
            Colision();
            Render();
        }

        //processes the player's input
        void PInput(Ball ball) {
            acceleration = new Vector2(0, 0);

            //player movement with arrow keys based on acceleration
            if (Input.IsKeyboardKeyDown(KeyboardKey.Left)) acceleration.X = -1;
            if (Input.IsKeyboardKeyDown(KeyboardKey.Right)) acceleration.X = 1;
            if (Input.IsKeyboardKeyDown(KeyboardKey.Up)) acceleration.Y = -1;
            if (Input.IsKeyboardKeyDown(KeyboardKey.Down)) acceleration.Y = 1;
            if (acceleration.Length() != 0) acceleration = Vector2.Normalize(acceleration) * 100;

            //shoot ball when spacebar pressed; only if the ball doesn't exist
            if (Input.IsKeyboardKeyPressed(KeyboardKey.Space) && ball == null) createBall = true;
        }

        //calculates the motion and updates the position
        void Motion() {
            velocity += acceleration * Time.DeltaTime;
            position += velocity * Time.DeltaTime;
        }

        //handles wall colisions
        void Colision() {
            if (velocity.Length() == 0) return; //safety
            Vector2 direction = Vector2.Normalize(velocity);
            float dampening = 0.5f;
            if (position.X + size > Window.Width) { //right wall
                //snap
                Vector2 perpindicular = -Vector2.UnitX;
                float distancePast = position.X + size - Window.Width;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.X *= -1;
                //reduce speed
                velocity *= dampening;
            }
            if (position.X - size < 0) { //left wall
                //snap
                Vector2 perpindicular = Vector2.UnitX;
                float distancePast = position.X - size;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.X *= -1;
                //reduce speed
                velocity *= dampening;
            }
            if (position.Y + size > Window.Height) { //bottom wall
                //snap
                Vector2 perpindicular = -Vector2.UnitY;
                float distancePast = position.Y + size - Window.Height;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.Y *= -1;
                //reduce speed
                velocity *= dampening;
            }
            if (position.Y - size < 0) { //top wall
                //snap
                Vector2 perpindicular = Vector2.UnitY;
                float distancePast = position.Y - size;
                float distanceSnap = distancePast / Vector2.Dot(direction, perpindicular);
                position -= direction * MathF.Abs(distanceSnap);
                //refect
                velocity.Y *= -1;
                //reduce speed
                velocity *= dampening;
            }
        }

        //draws the player ship
        void Render() {
            //find the positions of the points of the ship
            //points in direction of motion
            Vector2 direction;
            Vector2 P3Direction;
            Vector2 P4Direction;
            if (velocity.Length() != 0) direction = Vector2.Normalize(velocity);
            else direction = -Vector2.UnitY; //safety

            float angle = 130 * MathF.PI / 180; //in degrees

            Vector2 point1 = position + direction * size; //front
            Vector2 point2 = position - direction * size*0.2f; //back indent

            //https://en.wikipedia.org/wiki/Rotation_matrix
            //rotate the direction
            P3Direction.X = direction.X * MathF.Cos(angle) - direction.Y * MathF.Sin(angle);
            P3Direction.Y = direction.X * MathF.Sin(angle) + direction.Y * MathF.Cos(angle);
            P4Direction.X = direction.X * MathF.Cos(MathF.PI - angle) - direction.Y * MathF.Sin(MathF.PI - angle);
            P4Direction.Y = direction.X * MathF.Sin(MathF.PI - angle) + direction.Y * MathF.Cos(MathF.PI - angle);

            Vector2 point3 = position + P3Direction * size; //back points
            Vector2 point4 = position - P4Direction * size;

            //draw the ship from 2 triangles
            Draw.SetLineSize(0);
            Draw.SetFillColor(Color.Gray);
            Draw.Triangle(point1, point2, point3);
            Draw.Triangle(point1, point2, point4);

            //draw a shield bubble around the ship
            Draw.SetLineSize(5);
            Draw.SetLineColor(0, 0, 255, 128);
            Draw.SetFillColor(0, 0, 255, 0);
            Draw.Circle(position, size);
        }
    }
}
