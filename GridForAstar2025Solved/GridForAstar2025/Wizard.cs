using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GridForAstar2025
{
    internal class Wizard
    {
        public Vector2 pos;
        public Vector2 dest;

        private Texture2D sprite;
        private float speed = 1;

        private int current;
        public bool MoveDone = true;
        public List<Cell> Path { get; private set; }



        public Wizard(Cell pos, Texture2D sprite)
        {
            this.pos = new Vector2(pos.Position.X, pos.Position.Y);
            this.sprite = sprite;
        }


        public void SetPath(List<Cell> path)
        {
            if (path is null) return;
            if (path.Count < 1) return;

            Path = path;
            current = 0;
            dest = new Vector2(Path[current].Position.X, Path[current].Position.Y);
            MoveDone = false;
        }

        
        private bool AtDestination()
        {
            if ((dest - pos).Length() < 5)
            {
                pos = dest;

                if (current < Path.Count - 1)
                {
                    current++;
                    dest = new Vector2(Path[current].Position.X, Path[current].Position.Y);
                }
                else
                {
                    MoveDone = true;
                }
                return true;
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (MoveDone) return;

            var direction = dest - pos;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            
                pos += (direction) * gameTime.ElapsedGameTime.Seconds;
                if (AtDestination())
                {
                    if (pos == new Vector2(Path.Last().Position.X, Path.Last().Position.Y))
                    {
                        GameWorld.start.Reset();
                        GameWorld.start = Path.Last();
                    }
                }
            
            
        }

        //public void Update(GameTime gameTime)
        //{
        //    if (MoveDone) return;

        //    var direction = dest - pos;
        //    if (direction != Vector2.Zero) direction.Normalize();

        //    var distance = gameTime.ElapsedGameTime.Microseconds * speed;
        //    int iterations = (int)Math.Ceiling(distance / 5);
        //    distance /= iterations;

        //    for (int i = 0; i < iterations; i++)
        //    {
        //        pos = direction * distance;
        //        if (AtDestination()) return;
        //    }
        //}

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle((int)(pos.X * 100), (int)(pos.Y * 100), 100, 100), new Rectangle(0, 0, 32, 32), Color.White);
        }
    }
}
