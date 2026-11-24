using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MohawkGame2D
{
    public class Explosion
    {
        Vector2 position;
        public float existTime = 3;
        float colorChangeTime = 0f;
        int score;
        string drawText = "";
        Color textColor;
        Sound explosionSound;
        Font font;


        public Explosion(Vector2 position, int score, Font font)
        {
            explosionSound = Audio.LoadSound("Assets/Explosion.wav");
            Audio.SetVolume(explosionSound, 0.8f);

            Audio.Play(explosionSound);

            this.position = position;
            this.score = score;
            this.font = font;
        }

        public void Update()
        {
            Logic();
            Render();
        }

        void Logic() {
            existTime -= Time.DeltaTime;
            colorChangeTime -= Time.DeltaTime;

            //draw the text differently depending on score
            switch (score)
            {
                case 0:
                    //dont show score
                    Text.Size = 0;
                    textColor = Color.Clear;
                    drawText = "";
                    return;
                case 100:
                case 200:
                    Text.Size = 20;
                    textColor = Color.White;
                    drawText = $"{score}";
                    return;
                case 300:
                case 400:
                    Text.Size = 30;
                    textColor = new Color(90, 180, 240);
                    drawText = $"{score}";
                    return;
                case 500:
                    Text.Size = 50;
                    //change colour every 0.5 secconds
                    if (colorChangeTime <= 0) {
                        textColor = Random.Color(new Color(0, 0, 0), new Color(255, 255, 255), true);
                        colorChangeTime = 0.3f;
                    }
                    drawText = $"{score}";
                    return;
            }
        }

        void Render() {
            //explosion
            //fill
            Draw.SetLineSize(0);
            Draw.SetFillColor(new Color(255, 0, 0));
            Draw.Rectangle(position.X - 30, position.Y - 21, 66, 55);

            Draw.SetLineSize(12);
            Draw.SetLineColor(new Color(255, 192, 0));
            //inner line
            float[] xCoords = [position.X + 00, position.X + 10, position.X + 40, position.X + 20, position.X + 50, position.X + 35, position.X + 37, position.X + 15, position.X + 05, position.X - 15, position.X - 30, position.X - 20, position.X - 50, position.X - 30, position.X - 30, position.X - 10, position.X + 00];
            float[] yCoords = [position.Y - 35, position.Y - 15, position.Y - 25, position.Y + 00, position.Y + 10, position.Y + 20, position.Y + 35, position.Y + 20, position.Y + 40, position.Y + 25, position.Y + 35, position.Y + 10, position.Y + 00, position.Y - 05, position.Y - 20, position.Y - 10, position.Y - 35];
            Draw.PolyLine(xCoords, yCoords);
            //outline; scaled positions
            for (int i = 0; i < xCoords.Length; i++) {
                float dist = xCoords[i] - position.X;
                xCoords[i] = position.X + dist * 1.3f;
            }
            for (int i = 0; i < yCoords.Length; i++) {
                float dist = yCoords[i] - position.Y;
                yCoords[i] = position.Y + dist * 1.3f;
            }
            Draw.SetLineSize(12);
            Draw.SetLineColor(new Color(255, 255, 0));
            Draw.PolyLine(xCoords, yCoords);

            //draw the text
            //drop shadow
            Text.Color = Color.Black;
            Text.Draw(drawText, position.X + 2, position.Y + 2);
            Text.Color = textColor;
            Text.Draw(drawText, position);

            //Draw.SetLineSize(1);
            //Draw.SetLineColor(Color.White);
            //Draw.SetFillColor(Color.Clear);
            //Draw.Rectangle(position.X - 50, position.Y - 35, 100, 70);
        }
    }
}
