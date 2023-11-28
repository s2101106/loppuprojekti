using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Loppupeli
{
    internal class Enemy
    {
        public TransformComponent transform;
        public CollisionComponent collision;
        public bool active;
        SpriteRenderer spriteRenderer;

        public Enemy(Vector2 startPosition, Vector2 direction, float speed, int size, Texture image)
        {

            transform = new TransformComponent(startPosition, direction, speed);
            collision = new CollisionComponent(new Vector2(size, size));
            spriteRenderer = new SpriteRenderer(image, Raylib.RED, transform, collision);
            active = true;
        }
        internal void Draw()
        {
            if (active)
            {
                Raylib.DrawRectangleV(transform.position, collision.size, Raylib.DARKBROWN);
                spriteRenderer.Draw();
            }
        }

        public void Update()
        {
            if (active)
            {
                float deltaTime = Raylib.GetFrameTime();
                transform.position += transform.direction * transform.speed * deltaTime;
            }
        }
    }
}
