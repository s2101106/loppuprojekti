using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_CsLo;
using static System.Formats.Asn1.AsnWriter;
using TurboMapReader;
using System.Numerics;

namespace Loppupeli
{
    internal class Peli
    {
        const int window_width = 640;
        const int window_height = 420;
        public TiledMap tiledKartta;
        Player player;
        Texture playerImage;
        Texture enemyImage;
        Texture atlasImage;
        Camera2D camera;
        float cameraX;
        float cameraY;
        Enemy enemy;
        Vector2 playerStart;
        Vector2 enemyStart;
        List<Rectangle> mapColliders=new List<Rectangle>();
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
            playerImage = Raylib.LoadTexture("kuvat\\ufoRed.png");
            enemyImage = Raylib.LoadTexture("kuvat\\ufoRed.png");
            atlasImage = Raylib.LoadTexture("kuvat\\sheet.png");
            float playerSpeed = 120;
            int playerSize =16;
            Vector2 playerStart=new Vector2(40, 120);
            Vector2 enemyStart = new Vector2(-10, 300 - playerSize);
            player =new Player(playerStart, playerSpeed, playerSize,playerImage,Raylib.RED);
            enemy = new Enemy(enemyStart, new Vector2(1,0),playerSpeed,playerSize, enemyImage);
            camera.target=player.transform.position;
            camera.zoom = 1.0f;
            tiledKartta=MapReader.LoadMapFromFile("kartta1.tmj");
            tiledKartta.PrintToConsole();
            Raylib.SetTargetFPS(30);
        }
        void GameLoop()
        {
            while(Raylib.WindowShouldClose()==false)
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Raylib.BLACK);
                UpdateGame();
                DrawGame();
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
            player.Draw();
            enemy.Draw();
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
            Raylib.DrawCircle(window_width / 2, window_height / 2, 20, Raylib.MAROON);
            Raylib.DrawText(Raylib.TextFormat($"Score:500"), 550, 400, 20, Raylib.RED);
        }
        void UpdateGame()
        {
            player.Update();
            enemy.Update();
            cameraX = player.transform.position.X - 100;
            cameraY= player.transform.position.Y - 100;
            camera.target=new Vector2(cameraX,cameraY);
            if (enemy.transform.position.X > enemyStart.X + 100)
            {
                enemy.transform.direction.X *= -1.0f;
            }
            if (enemy.transform.position.X < enemyStart.X - 100)
            {
                enemy.transform.direction.X *= -1.0f;
            }
            Rectangle playerTRec = getTRect(player.transform, player.collision);
            Rectangle playerBRec = getBRect(player.transform, player.collision);
            Rectangle playerRRec = getRRect(player.transform, player.collision);
            Rectangle playerLRec = getLRect(player.transform, player.collision);
            if (mapColliders.Count >= tiledKartta.layers[0].data.Length)
            {
                foreach (Rectangle collider in mapColliders)
                {
                    if (Raylib.CheckCollisionRecs(collider, playerLRec))
                    {
                        Console.WriteLine("OSUIT liikkuessa vasemmalle!");
                        player.transform.position.X+=0.5f;
                        Console.WriteLine(mapColliders.Count.ToString());
                    }
                    if (Raylib.CheckCollisionRecs(collider, playerRRec))
                    {
                        Console.WriteLine("OSUIT liikkuessa oikealle!");
                        player.transform.position.X-=0.5f;
                        Console.WriteLine(mapColliders.Count.ToString());
                    }
                    if (Raylib.CheckCollisionRecs(collider, playerTRec))
                    {
                        Console.WriteLine("OSUIT liikkuessa ylös!");
                        player.transform.position.Y+=0.5f;
                        Console.WriteLine(mapColliders.Count.ToString());
                    }
                    if (Raylib.CheckCollisionRecs(collider, playerBRec))
                    {
                        Console.WriteLine("OSUIT liikkuessa alas!");
                        player.transform.position.Y-=0.5f;
                        Console.WriteLine(mapColliders.Count.ToString());
                    }

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
        Rectangle getRectangle(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X,
                t.position.Y, c.size.X, c.size.Y);
            return r;
        }
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
        Rectangle getBRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r=new Rectangle(t.position.X+1,t.position.Y+12,c.size.X-2,c.size.Y-12);
            return r;
        }
        Rectangle getTRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X+1, t.position.Y, c.size.X-2, c.size.Y - 12);
            return r;
        }
        Rectangle getRRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X+12, t.position.Y+1, c.size.X-12, c.size.Y-2);
            return r;
        }
        Rectangle getLRect(TransformComponent t, CollisionComponent c)
        {
            Rectangle r = new Rectangle(t.position.X, t.position.Y+1, c.size.X-12, c.size.Y-2);
            return r;
        }
        Rectangle getRect(int x, int y, int size)
        {
            Rectangle r = new Rectangle(x,y, size, size);
            return r;
        }
    }
}
