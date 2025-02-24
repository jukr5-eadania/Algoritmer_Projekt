using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using GridForAstar2025;

class Button
{
    private Texture2D sprite;

    private string spriteName;

    private BUTTONTYPE button;

    public Rectangle Rectangle { get; set; }

    public Button(string spriteName, BUTTONTYPE button, string buttonName = "")
    {
        this.button = button;
        this.spriteName = spriteName;
    }

    public void Update()
    {
        if (Rectangle.Contains(new Point(Mouse.GetState().X, Mouse.GetState().Y)) && Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            OnClick();
        }

    }

    public void LoadContent(Point position)
    {
        sprite = GameWorld.Instance.sprites[spriteName];

        Rectangle = new Rectangle(position.X, position.Y, sprite.Width, sprite.Height);

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(sprite, Rectangle, Color.White); 
    }

    public void OnClick()
    {
        GameWorld.Instance.OnButtonClick(button);


    }
}

