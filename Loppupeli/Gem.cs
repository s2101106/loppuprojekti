using Raylib_CsLo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Loppupeli
{
    internal class Gem
    {
        public bool active;
        SpriteRenderer spriteRenderer;
        public TransformComponent transform;
        public CollisionComponent collision;
        public Gem(Vector2 startPosition, Vector2 direction, float speed, int size, Texture image)
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

    }
}
