using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_CsLo;
using static System.Formats.Asn1.AsnWriter;
using TurboMapReader;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace Loppupeli
{
    internal class Peli
    {
        enum GameState
        {
            Start,
            Play,
            Pause,
            Options,
            DevMenu
        }
        GameState state;
        const int window_width = 640;
        const int window_height = 420;
        public TiledMap tiledKartta;
        public TiledMap tiledPiikit;
        Player player;
        Texture playerImage;
        Texture enemyImage;
        Texture gemImage1;
        Texture gemImage2;
        Texture gemImage3;
        Texture atlasImage;
        Camera2D camera;
        float cameraX;
        float cameraY;
        Enemy enemy;
        Gem gem1;
        Gem gem2;
        Gem gem3;
        int GemCounter;
        List<Gem>Gems = new List<Gem>();
        Vector2 playerStart;
        Vector2 enemyStart;
        Vector2 gemStart1;
        Vector2 gemStart2;
        Vector2 gemStart3;
        List<Rectangle> mapColliders=new List<Rectangle>();
        List<Rectangle> mapPiikit=new List<Rectangle>();
        float collisionAmount;
        public bool isG;
        public int Lc;
        public int Rc;
        public Peli() 
        { 
        
        }
        public void Run()
        {
            Init();
            GameLoop();
        }
        //Alustetaan peli
        void Init()
        {
            Raylib.InitWindow(window_width, window_height, "LoppyPeli");
            Raylib.SetExitKey(KeyboardKey.KEY_DELETE);
            playerImage = Raylib.LoadTexture("kuvat\\ufoRed.png");
            enemyImage = Raylib.LoadTexture("kuvat\\ufoRed.png");
            gemImage1 = Raylib.LoadTexture("kuvat\\loppuGemi.png");
            gemImage2 = Raylib.LoadTexture("kuvat\\loppuGemi.png");
            gemImage3 = Raylib.LoadTexture("kuvat\\loppuGemi.png");
            atlasImage = Raylib.LoadTexture("kuvat\\sheet.png");
            float playerSpeed = 120;
            int playerSize =16;
            isG = false;
            Vector2 playerStart=new Vector2(20, 250);
            Vector2 gemStart1 = new Vector2(60, 120);
            Vector2 gemStart2 = new Vector2(410, 10);
            Vector2 gemStart3 = new Vector2(460, 90);
            Vector2 enemyStart = new Vector2(160, 80 - playerSize);
            Lc = 140;
            Rc = 320;
            player =new Player(playerStart, playerSpeed, playerSize,playerImage,Raylib.RED);
            enemy = new Enemy(enemyStart, new Vector2(1,0),playerSpeed,playerSize, enemyImage);
            gem1 = new Gem(gemStart1, new Vector2(1, 0), playerSpeed, playerSize, gemImage1);
            gem2 = new Gem(gemStart2, new Vector2(1, 0), playerSpeed, playerSize, gemImage2);
            gem3 = new Gem(gemStart3, new Vector2(1, 0), playerSpeed, playerSize, gemImage3);
            Gems.Add(gem1);
            Gems.Add(gem2);
            Gems.Add(gem3);
            GemCounter = 0;
            camera.target=player.transform.position;
            camera.zoom = 2.0f;
            tiledKartta=MapReader.LoadMapFromFile("karttav1.tmj");
            Raylib.SetTargetFPS(30);
        }
        //Vaihdellaan pelin näkymiä tilan mukaan
        void GameLoop()
        {
            while(Raylib.WindowShouldClose()==false)
            {
                Raylib.BeginDrawing();
                switch (state)
                {
                    case GameState.Play:
                        Raylib.ClearBackground(Raylib.BLACK);
                        UpdateGame();
                        DrawGame();
                        break;
                    case GameState.Start:
                        Raylib.ClearBackground(Raylib.BLACK);
                        MainMenu();
                        break;
                    case GameState.Pause:
                        Raylib.ClearBackground(Raylib.BLACK);
                        PauseMenu();
                        break;
                    case GameState.DevMenu:
                        Raylib.ClearBackground(Raylib.BLACK);
                        DevMenu();
                        break;
                    case GameState.Options:
                        Raylib.ClearBackground(Raylib.BLACK);
                        OptionsMenu();
                        break;
                }
                Raylib.EndDrawing();

            }
            
        }
        void DrawGame()
        {
            Raylib.BeginMode2D(camera);
            //Piirretään kartanpalaset
            int map_width = tiledKartta.layers[0].data.Length;
            int rows = map_width / tiledKartta.width;
            for (int row=0; row<rows; row++)
            {
                for (int col=0; col<tiledKartta.width; col++) 
                {
                    int tileId = tiledKartta.layers[0].data[row * tiledKartta.width + col];
                    int x = col * tiledKartta.tilewidth;
                    int y=row * tiledKartta.tileheight;
                    tileId--;
                    DrawTile(x, y, tileId);
                }
            }

            player.Draw();
            enemy.Draw();
            gem1.Draw();
            gem2.Draw();
            gem3.Draw();

            Raylib.DrawRectangle(-30 +2, 130, 12, 4, Raylib.RED);
            Raylib.DrawCircle(window_width / 2, window_height / 2, 20, Raylib.MAROON);
            Raylib.DrawText(Raylib.TextFormat($"Score:{GemCounter}"), player.transform.position.X+110, player.transform.position.Y+85, 20, Raylib.RED);
        }
        void UpdateGame()
        {
            if (Raylib.IsKeyDown(KeyboardKey.KEY_ESCAPE))
            {
                state = GameState.Pause;
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_P))
            {
                state = GameState.DevMenu;
            }
            player.Update();
            cameraX = player.transform.position.X - 100;
            cameraY= player.transform.position.Y - 100;
            camera.target=new Vector2(cameraX,cameraY);
            //Näillä if-lausekkeilla laitetaan enemy liikkumaan edestakaisin
            if (enemy.transform.position.X >= Rc)
            {
                enemy.transform.direction.X *= -1.0f;
            }
            if (enemy.transform.position.X <= Lc)
            {
                enemy.transform.direction.X *= -1.0f;
            }

            enemy.Update();
            Rectangle playerRec = getRectangle(player.transform,player.collision);
            Rectangle playerGRec = getGRect(player.transform, player.collision);
            //Tässä tarkastetaAN osuuko pelaaja piikkeihin
            if (mapPiikit.Count >= 2)
            {
                foreach (Rectangle collider in mapPiikit)
                {
                    if (Raylib.CheckCollisionRecs(collider, playerRec))
                    {
                        player.active = false;

                    }
                    if (Raylib.CheckCollisionRecs(collider, playerGRec))
                    {
                        player.active=false;
                    }
                }
            }
            Rectangle playerTRec = getTRect(player.transform, player.collision);
            Rectangle playerBRec = getBRect(player.transform, player.collision);
            Rectangle playerRRec = getRRect(player.transform, player.collision);
            Rectangle playerLRec = getLRect(player.transform, player.collision);
            //Tässä tarkastetaan osuuko pelaaja kartanpalasiin ja mistä suunnasta
            if (mapColliders.Count >= tiledKartta.layers[0].data.Length)
            {
                foreach (Rectangle collider in mapColliders)
                {
                    if (Raylib.CheckCollisionRecs(collider, playerGRec))
                    {
                        isG = true;
                        break;
                    }
                    if (Raylib.CheckCollisionRecs(collider, playerGRec)==false)
                    {
                        isG=false;
                    }
                    if (Raylib.CheckCollisionRecs(collider, playerLRec))
                    {
                        collisionAmount= (collider.x + 16)-player.transform.position.X;
                        player.transform.position.X+=collisionAmount;
                    }
                    else if (Raylib.CheckCollisionRecs(collider, playerRRec))
                    {
                        collisionAmount = (collider.x - 16) - player.transform.position.X;
                        player.transform.position.X += collisionAmount;
                    }
                    else if (Raylib.CheckCollisionRecs(collider, playerTRec))
                    {
                        collisionAmount = (collider.y + 16) - player.transform.position.Y;
                        player.transform.position.Y += collisionAmount;
                    }
                    else if (Raylib.CheckCollisionRecs(collider, playerBRec))
                    {
                        collisionAmount = (collider.y - 16) - player.transform.position.Y;
                        player.transform.position.Y += collisionAmount;
                    }

                }

                        player.isGround = isG;
            }
            //Osuuko pelaaja kerättäviin esineisiin
            foreach(Gem gemi in Gems)
            {
                if (gemi.active == true)
                {
                    Rectangle gemRec = getRectangle(gemi.transform, gemi.collision);
                    if(Raylib.CheckCollisionRecs(gemRec, playerRec))
                    {
                        GemCounter++;
                        gemi.active = false;
                    }
                }

            }
            //osuuko pelaaja vastustajaan
            if (enemy.active == true)
            {
                Rectangle enemyRec = getRectangle(enemy.transform, enemy.collision);
                if (Raylib.CheckCollisionRecs(enemyRec, playerRec))
                {
                    player.active = false;
                    enemy.active = false;
                }
            }
        }
        //Piirtää kartanpalasen ja lisää sen colliderin listaan jonka avulla tarkastetaan törmäyksiä.
        void DrawTile(int x, int y, int tileId)
        {
            int tilesPerRow = 272 / tiledKartta.tilewidth;

            double rowf = Math.Floor((double)tileId / (double)tilesPerRow);

            int row = Convert.ToInt32(rowf);
            int column = tileId % tilesPerRow;

            int u = column * tiledKartta.tilewidth;
            int v = row * tiledKartta.tileheight;
            Vector2 xy= new Vector2(x, y);
            Raylib.DrawTextureRec(atlasImage, new Rectangle(u, v, tiledKartta.tilewidth, tiledKartta.tileheight), xy, Raylib.WHITE);
            if (mapColliders.Count != tiledKartta.layers[0].data.Length && tileId == 0)
            {
                mapPiikit.Add(getRect(x, y, 16));
            }
            if(mapColliders.Count!= tiledKartta.layers[0].data.Length&&tileId!=-1)
            {
                mapColliders.Add(getRect(x,y,16));
            }
        }
        //Tästä kerätään kaikkien collider kuutiot, Pelaajaa varten kerätään pelaajan joka puolelta collider jotta se ei mene kartan läpi.
        Rectangle getRectangle(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X,
                t.position.Y, c.size.X, c.size.Y);
            return r;
        }
                    
        Rectangle getBRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r=new Rectangle(t.position.X+2,t.position.Y+12,c.size.X-4,c.size.Y-12);
            return r;
        }
        Rectangle getGRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X , t.position.Y + 14, c.size.X - 8, c.size.Y - 12);
            return r;
        }
        Rectangle getTRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X+2, t.position.Y, c.size.X-4, c.size.Y - 12);
            return r;
        }
        Rectangle getRRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X+12, t.position.Y+2, c.size.X-12, c.size.Y-4);
            return r;
        }
        Rectangle getLRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X, t.position.Y+2, c.size.X-12, c.size.Y-4);
            return r;
        }
        Rectangle getRect(int x, int y, int size)
        {
            Rectangle r = new Rectangle(x,y, size, size);
            return r;
        }
        //Piirretään menuja
        void MainMenu()
        {
            Raylib.DrawText("Jumper",window_width/2-75, 30, 50, Raylib.RED);
            if (RayGui.GuiButton(new Rectangle(window_width/2-50, 200, 100, 50), "Play"))
            {
                state = GameState.Play;
            }
        }
        void PauseMenu()
        {
            Raylib.DrawText("Paused", window_width / 2 - 75, 30, 50, Raylib.RED);
            if (RayGui.GuiButton(new Rectangle(window_width / 2 - 50, 200, 100, 50), "Play"))
            {
                state = GameState.Play;
            }
            if (RayGui.GuiButton(new Rectangle(window_width / 2 - 50, 300, 100, 50), "Options"))
            {
                state = GameState.Options;
            }
        }
        void DevMenu()
        {
            Raylib.DrawText("Developer tools", window_width / 2 - 185, 30, 50, Raylib.RED);
            if (RayGui.GuiButton(new Rectangle(window_width / 2 - 50, 200, 100, 50), "Play"))
            {
                state = GameState.Play;
            }
        }
        void OptionsMenu()
        {
            Raylib.DrawText("Options", window_width / 2 - 85, 30, 50, Raylib.RED);
            if (RayGui.GuiButton(new Rectangle(window_width / 2 - 50, 200, 100, 50), "Play"))
            {
                state = GameState.Play;
            }
        }
    }
}
