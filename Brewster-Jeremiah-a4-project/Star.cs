using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MohawkGame2D
{
    public class Star
    {
        public Vector2 position = new Vector2(Random.Float(Window.Width), -10);
        float speed = Random.Float(5, 50);
        float radius = Random.Float(1, 5);
        Color baseColor = new Color(255, 255, Random.Integer(192, 255), Random.Integer(0, 255));
        Color color;
        bool twinkle = false;
        float twinkleChance = 1f / 20f;
        bool decreacing = true;
        float twinkleAmount = 1;

        public Star(Vector2 position, bool posOveride)
        {
            if (posOveride) this.position = position;
            color = baseColor;
        }

        public void Update()
        {
            Logic();
            Render();
        }

        //logic and motion
        void Logic() {
            //motion
            position.Y += speed * Time.DeltaTime;

            //twinkle
            if (!twinkle) { //chance to starnt twinkling
                float roll = Random.Float();
                twinkle = roll < twinkleChance * Time.DeltaTime;
            } else {
                //change intencity over time
                color.A = (int)MathF.Round(baseColor.A * twinkleAmount);
                if (decreacing) {
                    twinkleAmount -= 3 * Time.DeltaTime;
                    if (twinkleAmount <= 0) decreacing = false;
                } else {
                    twinkleAmount += Time.DeltaTime;
                    if (twinkleAmount >= 1) {
                        decreacing = true;
                        twinkle = false;
                        twinkleAmount = 1;
                        color.A = baseColor.A;
                    }
                }
            }
        }

        void Render() {
            Draw.SetLineSize(0);
            Draw.SetFillColor(color);
            Draw.Circle(position, radius);
        }
    }
}
