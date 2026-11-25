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
        Star[] stars = new Star[25];
        Explosion playerExplosion;
        float spawnTimer = 3;

        //chances for different spawn timings
        static float[] startSpawnRates = [0.4f, 0.2f, 0.03f];
        static float[] spawnRates1 = [0.8f, 0.4f, 0.15f];
        static float[] spawnRates2 = [0.95f, 0.85f, 0.25f];
        float[] curentSpawnRates;
        //when to swap to different rates
        float[] spawnChangeTimes = [10, 25, 30, 45, 55, 65, 80, 100];
        int spawnChangeTimesIndex = 0;
        float timePlaying = 0;
        int score = 0;
        int highScore = 0;
        bool playing = true;
        Font mainFont;
        Music bgMisic;
        public void Setup()
        {
            Window.SetTitle("Space Shooter Game");
            Window.SetSize(600, 800);

            player = new Player();

            curentSpawnRates = startSpawnRates;

            //create initial star layout
            for (int i = 0; i < stars.Length; i++) {
                stars[i] = new Star(Random.Vector2(Window.Width, Window.Height), true);
            }

            //asset loading
            mainFont = Text.LoadFont("Assets/OCRAEXT.TTF", 200);
            bgMisic = Audio.LoadMusic("Assets/SpaceMusic.ogg");
            Audio.SetVolume(bgMisic, 1.6f);
            Audio.Play(bgMisic);
        }

        public void Update()
        {
            Window.ClearBackground(background);

            

            //draw stars in both states
            for (int i = 0; i < stars.Length; i++) {
                stars[i].Update();
                if (stars[i].position.Y > Window.Height + 10) stars[i] = new Star(new Vector2(0,0), false);
            } 

            if (playerExplosion != null) { //ensures the player explosion despawns
                playerExplosion.Update();
                if (playerExplosion.existTime <= 0) playerExplosion = null;
            }

            if (playing) GameLoop();
            else LoseScreen();
        }
        
        //displays 'game over' and highscore
        //resets game when key pressed
        void LoseScreen() {
            //stop music
            Audio.Stop(bgMisic);

            //detete all game objects
            player = null;
            ball = null;
            enemies = new Enemy[100];
            explosions = new Explosion[10];

            //display score and text
            Text.Color = Color.White;
            Text.Size = 100;
            Text.Draw("GAME OVER", 40, 200, mainFont);
            Text.Size = 50;
            Text.Draw($"\t\t\t\tScore: {score}" +
                $"\n" +
                $"\n\t\tHighScore: {highScore}" +
                $"\n" +
                $"\n\tPress Space to\n   try again", 60, 350, mainFont);

            //restart game when spacebar pressed
            if (Input.IsKeyboardKeyPressed(KeyboardKey.Space)) { 
                playing = true;
                spawnTimer = 3;
                score = 0;
                player = new Player();
                curentSpawnRates = startSpawnRates;
                spawnChangeTimesIndex = 0;
                timePlaying = 0;
                Audio.Play(bgMisic);
            }
        }

        //all game logic and drawing
        void GameLoop()
        {
            timePlaying += Time.DeltaTime;
            //change spawn rates
            if (spawnChangeTimesIndex < spawnChangeTimes.Length) { //stop swapping once end of list is reached
                if (timePlaying >= spawnChangeTimes[spawnChangeTimesIndex]) {
                    spawnChangeTimesIndex++;
                    //swap back and forth
                    if (curentSpawnRates != spawnRates1) curentSpawnRates = spawnRates1;
                    else curentSpawnRates = spawnRates2;
                }
            }

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
                } else if (enemies[i].gameOver) { //player lost
                    playing = false;
                    playerExplosion = new Explosion(player.position, 0, mainFont);
                    if (score > highScore) highScore = score;
                    break;
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
            if (roll > curentSpawnRates[0]) { 
                spawnTimer = Random.Float(5, 7);
            } else if (roll > curentSpawnRates[1]) { 
                spawnTimer = Random.Float(2, 5);
            } else if (roll > curentSpawnRates[2]) { 
                spawnTimer = Random.Float(0.5f, 2);
            } else { 
                //double spawn
                spawnTimer = 0;
            }
        }
    }

}
