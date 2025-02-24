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
        private float speed = 5;

        private int current;
        public bool MoveDone = true;
        public bool readyToMove = true;
        public List<Cell> Path { get; private set; }



        public Wizard(Cell pos, Texture2D sprite)
        {
            this.pos = new Vector2(pos.Position.X, pos.Position.Y);
            this.sprite = sprite;
        }


        public void SetPath(List<Cell> path)
        {
            readyToMove = false;
            if (path is null) return;
            if (path.Count < 1) return;

            Path = path;
            current = 0;
            dest = new Vector2(Path[current].Position.X, Path[current].Position.Y);
            MoveDone = false;
        }

        
        private bool AtDestination()
        {
            if ((dest - pos).Length() < 0.1f)
            {
                pos = dest;

                if (current < Path.Count - 1)
                {
                    GameWorld.start = Path[current];
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


            pos += (direction * speed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (AtDestination())
                {
                    if (pos == new Vector2(Path.Last().Position.X, Path.Last().Position.Y))
                    {
                        GameWorld.start = Path.Last();
                        readyToMove = true;

                    }
                }
            
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, new Rectangle((int)(pos.X * 100), (int)(pos.Y * 100), 100, 100), new Rectangle(0, 0, 32, 32), Color.White);
        }
    }
}
