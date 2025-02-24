using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Transactions;

namespace GridForAstar2025
{
    public enum BUTTONTYPE { START, GOAL, WALL, RESET, FINDPATH }


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

        Button resetButton = new Button("ResetBtn", BUTTONTYPE.RESET);
        Button startButton = new Button("StartBtn", BUTTONTYPE.START);
        Button goalButton = new Button("GoalBtn", BUTTONTYPE.GOAL);
        Button wallbutton = new Button("WallBtn", BUTTONTYPE.WALL);
        Button findPathButton = new Button("FindPathBtn", BUTTONTYPE.FINDPATH);
        static public Cell start, goal;
        private Wizard wizard;

        public GameWorld()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            for (int y = 0; y < cellCount; y++)
            {
                for (int x = 0; x < cellCount; x++)
                {
                    Cells.Add(new Point(x, y), new Cell(new Point(x, y), cellSize, cellSize));
                }
            }

            buttons.Add(resetButton);
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
            
            if (!(wizard == null))
            {
                wizard.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            foreach (KeyValuePair<Point, Cell> cell in Cells)
            {
                cell.Value.Draw(_spriteBatch);
            }

            foreach (Button button in buttons)
            {
                button.Draw(_spriteBatch);
            }
            if (!(wizard == null))
            {
            wizard.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        public void OnButtonClick(BUTTONTYPE clicked)
        {

            if (clicked == BUTTONTYPE.RESET)
            {
                foreach (KeyValuePair<Point, Cell> cell in Cells)
                {
                    cell.Value.Reset();
                }
            }
            else if (clicked == BUTTONTYPE.FINDPATH)
            {
                Astar astar = new Astar(Cells);
                var path = astar.FindPath(start.Position, goal.Position);
                foreach (var VARIABLE in path)
                {
                    VARIABLE.spriteColor = Color.Aqua;
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
                wizard = new(clicked, sprites["BunnyIdleF"]);
            }
            else if (CurrentButton == BUTTONTYPE.GOAL)
            {
                if (goal != null)
                {
                    goal.Reset();

                }

                goal = clicked;

                foreach (var item in Cells)
                {
                    item.Value.spriteColor = Color.White;
                }
                Astar();

                
            }
            else if (CurrentButton == BUTTONTYPE.WALL)
            {
                clicked.Sprite = sprites["Wall"];
            }


        }
        public void Astar()
        {
            Astar astar = new Astar(Cells);
            var path = astar.FindPath(start.Position, goal.Position);
            foreach (var VARIABLE in path)
            {
                VARIABLE.spriteColor = Color.Aqua;
            }
            wizard.SetPath(path);
        }
    }
}
