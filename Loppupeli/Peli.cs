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
        Texture gemImage;
        Texture atlasImage;
        Camera2D camera;
        float cameraX;
        float cameraY;
        Enemy enemy;
        Gem gem;
        int GemCounter;
        Vector2 playerStart;
        Vector2 enemyStart;
        Vector2 gemStart;
        List<Rectangle> mapColliders=new List<Rectangle>();
        List<Rectangle> mapPiikit=new List<Rectangle>();
        float collisionAmount;
        public bool isG;
        public Peli() 
        { 
        
        }
        public void Run()
        {
            Init();
            GameLoop();
        }
        void Init()
        {
            Raylib.InitWindow(window_width, window_height, "LoppyPeli");
            Raylib.SetExitKey(KeyboardKey.KEY_DELETE);
            playerImage = Raylib.LoadTexture("kuvat\\ufoRed.png");
            enemyImage = Raylib.LoadTexture("kuvat\\ufoRed.png");
            gemImage = Raylib.LoadTexture("kuvat\\loppuGemi.png");
            atlasImage = Raylib.LoadTexture("kuvat\\sheet.png");
            //piikkiImage = Raylib.LoadTexture("kuvat\\sheet.png");
            float playerSpeed = 120;
            int playerSize =16;
            isG = false;
            Vector2 playerStart=new Vector2(0, 80);
            Vector2 gemStart = new Vector2(60, 120);
            Vector2 enemyStart = new Vector2(-10, 300 - playerSize);
            player =new Player(playerStart, playerSpeed, playerSize,playerImage,Raylib.RED);
            enemy = new Enemy(enemyStart, new Vector2(1,0),playerSpeed,playerSize, enemyImage);
            gem = new Gem(gemStart, new Vector2(1, 0), playerSpeed, playerSize, gemImage);
            GemCounter = 0;
            camera.target=player.transform.position;
            camera.zoom = 2.0f;
            tiledKartta=MapReader.LoadMapFromFile("kartta1.tmj");
            //tiledPiikit = MapReader.LoadMapFromFile("kartta1.tmj");
            Raylib.SetTargetFPS(30);
        }
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
            /*int map_width = tiledPiikit.layers[0].data.Length;
            int rows = map_width / tiledKartta.width;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < tiledPiikit.width; col++)
                {
                    int tileId = tiledPiikit.layers[0].data[row * tiledPiikit.width + col];
                    int x = col * tiledPiikit.tilewidth;
                    int y = row * tiledPiikit.tileheight;
                    tileId--;
                    DrawPiikki(x, y, tileId);
                }
            }*/
            player.Draw();
            enemy.Draw();
            gem.Draw();
            /*
            //oikee
            Raylib.DrawRectangle(40+12, 120+1, 4, 14, Raylib.BLUE);
            //vasen
            Raylib.DrawRectangle(40, 120+1, 4, 14, Raylib.DARKGREEN);
            //bot
            Raylib.DrawRectangle(40+1,120+12,14,4,Raylib.WHITE);
            //top
            Raylib.DrawRectangle(40+1, 120, 14, 4, Raylib.RED);
            */
            //oikee
            Raylib.DrawRectangle(-30+12, 130+2, 4, 12, Raylib.BLUE);
            //vasen
            Raylib.DrawRectangle(-30, 130+2, 4, 12, Raylib.DARKGREEN);
            //bot
            Raylib.DrawRectangle(-28 +2,130+12,8,8,Raylib.WHITE);
            //top
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
            enemy.Update();
            if (enemy.transform.position.X < enemyStart.X + 40)
            {
                enemy.transform.direction.X *= -1.0f;
            }
            if (enemy.transform.position.X > enemyStart.X - 40)
            {
                enemy.transform.direction.X *= -1.0f;
            }
            Rectangle playerTRec = getTRect(player.transform, player.collision);
            Rectangle playerBRec = getBRect(player.transform, player.collision);
            Rectangle playerRRec = getRRect(player.transform, player.collision);
            Rectangle playerLRec = getLRect(player.transform, player.collision);
            Rectangle playerGRec = getGRect(player.transform, player.collision);
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
            Rectangle playerRec = getRectangle(player.transform,player.collision);
            /*if (mapPiikit.Count >= tiledPiikit.layers[0].data.Length)
            {
                foreach (Rectangle collider in mapPiikit)
                {
                    if (Raylib.CheckCollisionRecs(collider, playerRec))
                    {
                        Console.WriteLine("Kuolit");
                    }
                }
            }*/
            if (gem.active == true)
            {
                Rectangle gemRec = getRectangle(gem.transform, gem.collision);
                if(Raylib.CheckCollisionRecs(gemRec, playerRec))
                {
                    Console.WriteLine("keräsit timun!");
                    GemCounter++;
                    gem.active = false;
                }
            }
            if (enemy.active == true)
            {
                Rectangle enemyRec = getRectangle(enemy.transform, enemy.collision);
                if (Raylib.CheckCollisionRecs(enemyRec, playerRec))
                {
                    Console.WriteLine("kuolit!");
                    enemy.active = false;
                }
            }
        }
        void DrawTile(int x, int y, int tileId)
        {
            int tilesPerRow = 272 / tiledKartta.tilewidth;
            //Console.WriteLine(tilesPerRow.ToString());
            double rowf = Math.Floor((double)tileId / (double)tilesPerRow);

            int row = Convert.ToInt32(rowf);
            int column = tileId % tilesPerRow;

            int u = column * tiledKartta.tilewidth;
            int v = row * tiledKartta.tileheight;
            Vector2 xy= new Vector2(x, y);
            Raylib.DrawTextureRec(atlasImage, new Rectangle(u, v, tiledKartta.tilewidth, tiledKartta.tileheight), xy, Raylib.WHITE);
            if(mapColliders.Count!= tiledKartta.layers[0].data.Length&&tileId!=-1)
            {
                mapColliders.Add(getRect(x,y,16));
            }
        }
        /*void DrawPiikki(int x, int y, int tileId)
        {
            int tilesPerRow = 272 / tiledPiikit.tilewidth;
            //Console.WriteLine(tilesPerRow.ToString());
            double rowf = Math.Floor((double)tileId / (double)tilesPerRow);

            int row = Convert.ToInt32(rowf);
            int column = tileId % tilesPerRow;

            int u = column * tiledPiikit.tilewidth;
            int v = row * tiledPiikit.tileheight;
            Vector2 xy = new Vector2(x, y);
            Raylib.DrawTextureRec(piikitImage, new Rectangle(u, v, tiledPiikit.tilewidth, tiledPiikit.tileheight), xy, Raylib.WHITE);
            if (mapPiikit.Count != tiledPiikit.layers[0].data.Length && tileId != -1)
            {
                mapPiikit.Add(getRect(x, y, 16));
            }
        }*/
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
