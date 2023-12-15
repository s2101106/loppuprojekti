using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_CsLo;
namespace Loppupeli
{
    internal class Player
    {
        public TransformComponent transform { get; private set; }
        public bool active = false;
        public bool isGround;
        SpriteRenderer spriteRenderer;
        public CollisionComponent collision;
        public int gravity = 2;

        public float direction = 0.0f;

        public Player(Vector2 startPos, float speed, int size, Texture image, Color color)
        {
            transform = new TransformComponent(startPos, new Vector2(0, 0), speed);
            active = true;
            collision = new CollisionComponent(new Vector2(size, size));
            spriteRenderer = new SpriteRenderer(image, color, transform, collision);
        }
        public void Update()
        {
            float deltaTime = Raylib.GetFrameTime();
            if (isGround==false)
            {
                transform.position.Y += gravity;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_A) && active == true)
            {
                direction = -1.0f;
                transform.position.X += direction * transform.speed * deltaTime;
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D) && active == true)
            {
                direction = 1.0f;
                transform.position.X += direction * transform.speed * deltaTime;
            }
            if(Raylib.IsKeyDown(KeyboardKey.KEY_SPACE) && active == true&& isGround==true)
            {
                Console.WriteLine("hyppäsit");
                transform.position.Y -= (transform.speed*10) * deltaTime;
            }
            else
            {
                direction = 0.0f;
            }
        }
        public void Draw()
        {
            if (active)
            {
                Raylib.DrawRectangleV(transform.position, collision.size, Raylib.SKYBLUE);
                spriteRenderer.Draw();
            }
        }
    }
}
