﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace GridForAstar2025
{
    public enum BUTTONTYPE { START, GOAL, WALL, FINDPATH }


    public class GameWorld : Game
    {

        private static GameWorld instance;

        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        private GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;

        public SpriteFont SpriteFont { get; private set; }

        /// <summary>
        /// Grid
        /// </summary>
        private int cellCount = 10;

        private int cellSize = 100;

        /// <summary>
        /// Collections
        /// </summary>
        public Dictionary<Point, Cell> Cells { get; private set; } = new Dictionary<Point, Cell>();

        public Dictionary<string, Texture2D> sprites { get; private set; } = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Buttons
        /// </summary>
        private List<Button> buttons = new List<Button>();
        public BUTTONTYPE CurrentButton { get; set; }

        Button startButton = new Button("StartBtn", BUTTONTYPE.START);
        Button goalButton = new Button("GoalBtn", BUTTONTYPE.GOAL);
        Button wallbutton = new Button("WallBtn", BUTTONTYPE.WALL);
        Button findPathButton = new Button("FindPathBtn", BUTTONTYPE.FINDPATH);
        private List<Cell> goals = new List<Cell>();
        private Cell start, stormTowerKey, iceTowerKey, stormTower, iceTower, Portal;
        HashSet<Point> usedCells = new HashSet<Point>();

        private Random rnd = new Random();

        public GameWorld()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            for (int y = 0; y < cellCount; y++)
            {
                for (int x = 0; x < cellCount; x++)
                {
                    Cells.Add(new Point(x, y), new Cell(new Point(x, y), cellSize, cellSize));
                }
            }

            buttons.Add(startButton);
            buttons.Add(goalButton);
            buttons.Add(wallbutton);
            buttons.Add(findPathButton);

            _graphics.PreferredBackBufferWidth = cellCount * cellSize + 200;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = cellCount * cellSize + 1;   // set this value to the desired height of your window
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Loads the content of our spritefont
            SpriteFont = Content.Load<SpriteFont>("MyFont");

            DirectoryInfo d = new DirectoryInfo(@"..\..\..\Content");


            FileInfo[] Files = d.GetFiles("*.png");



            foreach (FileInfo file in Files)
            {
                int i = file.Name.IndexOf('.');
                string name = file.Name.Remove(i);
                sprites.Add(name, Content.Load<Texture2D>(name));
            }

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (KeyValuePair<Point, Cell> cell in Cells)
            {
                cell.Value.LoadContent();
            }

            int[,] wallPattern = new int[,]
            {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            };

            int wOffsetX = 4, wOffsetY = 1;
            for (int y = 0; y < wallPattern.GetLength(0); y++)
            {
                for (int x = 0; x < wallPattern.GetLength(1); x++)
                {
                    if (wallPattern[y, x] == 1 && Cells.TryGetValue(new Point(x + wOffsetX, y + wOffsetY), out Cell wallCell))
                    {
                        wallCell.Sprite = sprites["Wall"];
                        usedCells.Add(wallCell.Position);
                    }
                }
            }

            int[,] forestPattern = new int[,]
            {
                {1, 1, 1, 1, 1},
                {0, 0, 0, 0, 0},
                {1, 1, 1, 1, 1},
            };

            int fOffsetX = 2, fOffsetY = 7;
            for (int y = 0; y < forestPattern.GetLength(0); y++)
            {
                for (int x = 0; x < forestPattern.GetLength(1); x++)
                {
                    if (forestPattern[y, x] == 1 && Cells.TryGetValue(new Point(x + fOffsetX, y + fOffsetY), out Cell forestCell))
                    {
                        forestCell.Sprite = sprites["Wall"];
                        usedCells.Add(forestCell.Position);
                    }
                }
            }

            if (Cells.TryGetValue(new Point(1, 8), out Cell startCell))
            {
                start = startCell;
                start.Sprite = sprites["Mario"];
                usedCells.Add(start.Position);
            }
            if (Cells.TryGetValue(GetRandomUnusedCell(), out Cell goalCell1))
            {
                stormTowerKey = goalCell1;
                stormTowerKey.Sprite = sprites["Peach"];
                goals.Add(stormTowerKey);
            }
            if (Cells.TryGetValue(GetRandomUnusedCell(), out Cell goalCell2))
            {
                iceTowerKey = goalCell2;
                iceTowerKey.Sprite = sprites["Peach"];
                goals.Add(iceTowerKey);
            }
            if (Cells.TryGetValue(new Point(2, 4), out Cell goalCell3))
            {
                stormTower = goalCell3;
                stormTower.Sprite = sprites["Peach"];
                goals.Add(stormTower);
                usedCells.Add(stormTower.Position);
            }
            if (Cells.TryGetValue(new Point(8, 7), out Cell goalCell4))
            {
                iceTower = goalCell4;
                iceTower.Sprite = sprites["Peach"];
                goals.Add(iceTower);
                usedCells.Add(iceTower.Position);
            }
            if (Cells.TryGetValue(new Point(0, 8), out Cell goalCell5))
            {
                Portal = goalCell5;
                Portal.Sprite = sprites["Peach"];
                goals.Add(Portal);
                usedCells.Add(Portal.Position);
            }

            PlaceButtons();
            // TODO: use this.Content to load your game content here
        }

        private void PlaceButtons()
        {
            buttons[0].LoadContent(new Point(cellCount * cellSize + 10, 0));

            for (int i = 1; i < buttons.Count; i++)
            {
                buttons[i].LoadContent(new Point(buttons[i - 1].Rectangle.X, buttons[i - 1].Rectangle.Y + buttons[i - 1].Rectangle.Height + 5));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            foreach (KeyValuePair<Point, Cell> cell in Cells)
            {
                cell.Value.Update();
            }
            foreach (Button btn in buttons)
            {
                btn.Update();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            foreach (KeyValuePair<Point, Cell> cell in Cells)
            {
                cell.Value.Draw(_spriteBatch);
            }

            foreach (Button button in buttons)
            {
                button.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        public void OnButtonClick(BUTTONTYPE clicked)
        {
            if (clicked == BUTTONTYPE.FINDPATH)
            {
                Astar astar = new Astar(Cells);
                foreach (var goal in goals)
                {
                    var path = astar.FindPath(start.Position, goal.Position);
                    foreach (var VARIABLE in path)
                    {
                        VARIABLE.spriteColor = Color.Aqua;
                    }
                    start = goal;
                }
            }
            else
            {
                CurrentButton = clicked;
            }
        }

        public void OnCellClick(Cell clicked)
        {

            if (CurrentButton == BUTTONTYPE.START)
            {
                if (start != null)
                {
                    start.Reset();

                }
                start = clicked;
                clicked.Sprite = sprites["Mario"];
            }
            //else if (CurrentButton == BUTTONTYPE.GOAL)
            //{
            //    if (goal != null)
            //    {
            //        goal.Reset();

            //    }

            //    goal = clicked;

            //    clicked.Sprite = sprites["Peach"];
            //}
            else if (CurrentButton == BUTTONTYPE.WALL)
            {
                clicked.Sprite = sprites["Wall"];
            }


        }

        public Point GetRandomUnusedCell()
        {
            Point point;

            do
            {
                point = new Point(rnd.Next(0, 10), rnd.Next(0, 10));
            } while (usedCells.Contains(point));
            usedCells.Add(point);
            return point;
        }
    }
}
