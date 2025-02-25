
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using GridForAstar2025;

public class Cell
{
    public Color spriteColor = Color.White;

    private Color edgeColor = Color.Black;

    public Texture2D Sprite { get; set; }

    public Point Position { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }

    private Rectangle topLine;

    private Rectangle bottomLine;

    private Rectangle rightLine;

    private Rectangle leftLine;

    private Rectangle background;
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H;
    public Cell Parent;
    public Cell(Point position, int width, int height)
    {
        this.Position = position;

        this.Width = width;
        this.Height = height;
    }

    public void LoadContent()
    {

        Sprite = GameWorld.Instance.sprites["Pixel"];

        topLine = new Rectangle(Position.X * Width, Position.Y * Height, Width, 1);

        bottomLine = new Rectangle(Position.X * Width, (Position.Y * Height) + Height, Width, 1);

        rightLine = new Rectangle((Position.X * Width) + Width, Position.Y * Height, 1, Height);

        leftLine = new Rectangle(Position.X * Width, Position.Y * Height, 1, Height);

        background = new Rectangle(Position.X * Width, Position.Y * Height, Width, Height);
    }

    

    public void Draw(SpriteBatch spriteBatch)
    {

        spriteBatch.Draw(Sprite, background, spriteColor);
        spriteBatch.Draw(GameWorld.Instance.sprites["Pixel"], topLine, edgeColor);
        spriteBatch.Draw(GameWorld.Instance.sprites["Pixel"], bottomLine, edgeColor);
        spriteBatch.Draw(GameWorld.Instance.sprites["Pixel"], rightLine, edgeColor);
        spriteBatch.Draw(GameWorld.Instance.sprites["Pixel"], leftLine, edgeColor);
        //string cellString = $"{Position.X.ToString()},{Position.Y.ToString()}\n F:{F} \n h:{H}\n g: {G}";
        //spriteBatch.DrawString(GameWorld.Instance.SpriteFont, cellString, new Vector2(topLine.X, topLine.Y), Color.Black);



    }

    public void Reset()
    {
        Sprite = GameWorld.Instance.sprites["Pixel"];
        spriteColor = Color.White;
        G = 0;
        H = 0;
    }


}

