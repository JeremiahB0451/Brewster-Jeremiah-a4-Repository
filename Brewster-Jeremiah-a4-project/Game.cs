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
        Enemy[] enemies = new Enemy[100];
        Explosion[] explosions = new Explosion[10];
        float spawnTimer = 3;
        int score = 0;
        Font mainFont;
        public void Setup()
        {
            Window.SetTitle("Space Shooter Game");
            Window.SetSize(600, 800);

            player = new Player();

            //asset loading
            mainFont = Text.LoadFont("Assets/OCRAEXT.TTF", 200);
        }

        public void Update()
        {
            Window.ClearBackground(background);

            //spawn enemies
            spawnTimer -= Time.DeltaTime;
            //try to spawn an enemy when the timer hits 0
            //only reset timer if successful so that it can try again next frame
            if (spawnTimer <= 0) if (SpawnEnemy()) ResetTimer();

            //update all objects (in draw order)

            for (int i = 0; i < enemies.Length; i++) {
                if (enemies[i] == null) continue;
                enemies[i].Update(player, ball);
                if (enemies[i].destroy) { //ball hit the enemy
                    score += enemies[i].addScore;
                    //create explosion
                    for (int j = 0; j < explosions.Length; j++) {
                        if (explosions[j] != null) continue;
                        explosions[j] = new Explosion(enemies[i].position + enemies[i].hitboxSize / 2, enemies[i].addScore, mainFont);
                        break;
                    }
                    enemies[i] = null;
                    continue;
                }
            }

            for (int i = 0; i < explosions.Length; i++) {
                if (explosions[i] == null) continue;
                explosions[i].Update();
                if (explosions[i].existTime <= 0) explosions[i] = null;
            }
            
            if (ball != null) {
                ball.Update();
                if (ball.destroy) ball = null;
            }

            player.Update(ball);
            if (player.createBall) {
                ball = new Ball(player.position, player.size, player.velocity);
                player.createBall = false;
            }

            //display the score (above everthing else)
            Text.Size = 50;
            //drop shadow
            Text.Color = Color.Black;
            Text.Draw($"{score}", 2, 2, mainFont);

            Text.Color = Color.White;
            Text.Draw($"{score}", 0, 0, mainFont);
        }

        //creates a new enemy above the screen
        bool SpawnEnemy() {
            bool successful = false;
            //create an enemy at the first avalable position in enemies
            for (int i = 0; i < enemies.Length; i++) {
                if (enemies[i] != null) continue;
                enemies[i] = new Enemy();
                successful = true;
                //move it to a random x position within the screen at y = -100
                enemies[i].position = new Vector2(Random.Float(Window.Width - enemies[i].hitboxSize.X), -100);
                //delete the enemy if it's colliding with any other enemies
                for (int j = 0; j < enemies.Length; j++) {
                    if (enemies[j] == null || j == i) continue;
                    if (MathF.Abs(enemies[i].position.X - enemies[j].position.X) < enemies[i].hitboxSize.X &&
                        MathF.Abs(enemies[i].position.Y - enemies[j].position.Y) < enemies[i].hitboxSize.Y) {
                        enemies[i] = null;
                        successful = false;
                        break;
                    } else if (MathF.Abs(enemies[i].position.Y - enemies[j].position.Y) < 5) { //align them vertically if they are less than 5 px apart
                        enemies[i].position.Y = enemies[j].position.Y;
                    }
                }
                break;
            }
            return successful;
        }

        //resets the enemy spawn timer complexly
        void ResetTimer() {
            //roll a value to determine the bracket
            float roll = Random.Float();
            if (roll > 0.8f) { //20% chance
                spawnTimer = Random.Float(5, 7);
            } else if (roll > 0.4f) { //40% chance
                spawnTimer = Random.Float(2, 5);
            } else if (roll > 0.15f) { //25% chance
                spawnTimer = Random.Float(0.5f, 2);
            } else { //15% chance
                //double spawn
                spawnTimer = 0;
            }
        }
    }

}
