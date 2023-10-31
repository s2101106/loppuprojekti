using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_CsLo;
using static System.Formats.Asn1.AsnWriter;

namespace Loppupeli
{
    internal class Peli
    {
        const int window_width = 640;
        const int window_height = 420;
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
            Raylib.SetTargetFPS(30);
        }
        void GameLoop()
        {
            while(Raylib.WindowShouldClose()==false)
            {
                DrawGame();
            }
            
        }
        void DrawGame()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLUE);
            Raylib.DrawCircle(window_width / 2, window_height / 2, 20, Raylib.MAROON);
            Raylib.DrawText(Raylib.TextFormat($"Score:500"), 550, 400, 20, Raylib.RED);
            Raylib.EndDrawing();
        }
    }
}
