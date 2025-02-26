using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridForAstar2025
{
  
    class JPS
    {
        Dictionary<Point, Cell> jpsCells;
        private HashSet<Cell> openList = new HashSet<Cell>();
        private HashSet<Cell> closedList = new HashSet<Cell>();

        public JPS(Dictionary<Point, Cell> jpsCells)
        {
            this.jpsCells = jpsCells;
        }

        private static readonly (int, int)[] Directions =
        { 
            (0,-1), (1,0), (0,1), (-1,0), // Cardinal direction
            (1,-1), (1,1), (-1,1), (-1,-1) // Diagonal direction
        };
        
        public static List<Cell> FindJPSPath(Cell start, Cell goal, int[,] grid)
        {
            List<Cell> openList = new List<Cell> { start };
            HashSet<Cell> closedList = new HashSet<Cell>();

            while(openList.Count > 0)
            {
                var currentCell = openList[0];
                openList.RemoveAt(0);
                closedList.Add(currentCell);

                if(currentCell.Position == goal.Position)
                {
                    return RetracePath(currentCell, goal);
                }

                foreach (var direction in Directions)
                {
                    
                }
            }

        }

        private static Cell Jump(Cell cell,(int,int) direction, int[,] grid, Cell goal )
        {
            int x = cell.Position.X + direction.Item1;
            int y = cell.Position.Y + direction.Item2;

            if (!IsValid(x, y, grid)) 
            { 
                return null; 
            }

            if(x == goal.Position.X && y == goal.Position.Y)
            {
                return new Cell(); // HELP!
            }

        }

        /// <summary>
        /// A method that checks if a given cell (node) in the grid is valid for movement.
        /// It returns a boolean value (true or false) based on whether the cell is 
        /// within the grids boundaries and not an obstacle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        private static bool IsValid(int x, int y, int[,] grid)
        {
            return
                x >= 0 && y >= 0 && // Ensures the coordinates are not negative.
                x < grid.GetLength(0) && x < grid.GetLength(1) && // Ensures the coordinates are within the grid boundaries.
                grid[x, y] == 0; // Checks that the cell is not an obstacle
        }

        /// <summary>
        /// Checks for forced neighbors or turning points
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direction"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        private static bool IsJumpPoint(int x, int y, (int,int) direction, int[,] grid)
        {
            int dx = direction.Item1;
            int dy = direction.Item2;

            //Check for forced neighbors
            if(dx !=0 && dy != 0) // diagonal movement
            {
                if((IsValid(x-dx, y+dy, grid) && !IsValid(x-dx, y, grid)) ||
                    (IsValid(x+dx, y-dy, grid) && !IsValid(x, y-dy, grid)))
                {
                    return true;
                }
            }
            else
            {
                if(dx != 0) // Horizontal movement
                {
                    if((IsValid(x+dx , y+1, grid) && !IsValid(x, y+1, grid)) ||
                        (IsValid(x - dx, y - 1, grid) && !IsValid(x, y - 1, grid)))
                    {
                        return true;
                    }
                }
                else if (dy != 0) // Vertical movement
                {
                    if ((IsValid(x + 1, y + dy, grid) && !IsValid(x + 1, y, grid)) ||
                        (IsValid(x - 1, y - dy, grid) && !IsValid(x -1, y, grid)))
                    {
                        return true;
                    }
                }

            }

            // Check for turning points
            if(dx != 0 && dy != 0)
            {
                return IsValid(x + dx, y, grid) || IsValid(x, y + dy, grid);
            }

            return false;
        }

        public List<Cell> FindPath(Point startPoint, Point endPoint)
        {            
            foreach (var cell in jpsCells.Values)
            {
                cell.G = 0;
                cell.H = 0;
                cell.Parent = null;
                cell.spriteColor = Color.White;
            }

            openList.Add(jpsCells[startPoint]);
            while (openList.Count > 0)
            {
                Cell curCell = openList.First();
                foreach (var t in openList)
                {
                    if (t.F < curCell.F ||
                        t.F == curCell.F && t.H < curCell.H)
                    {
                        curCell = t;
                    }
                }
                openList.Remove(curCell);
                closedList.Add(curCell);

                if (curCell.Position.X == endPoint.X && curCell.Position.Y == endPoint.Y)
                {
                    //path found!
                    return RetracePath(jpsCells[startPoint], jpsCells[endPoint]);
                }

                List<Cell> neighbors = GetNeighbors(curCell);
                foreach (var neighbor in neighbors)
                {
                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }
                    int newMovementCostToNeighbor = curCell.G + GetDistance(curCell.Position, neighbor.Position);
                    if (newMovementCostToNeighbor < neighbor.G || !openList.Contains(neighbor))
                    {
                        neighbor.G = newMovementCostToNeighbor;
                        //calulate h using manhatten principle
                        neighbor.H = ((Math.Abs(neighbor.Position.X - endPoint.X) + Math.Abs(endPoint.Y - neighbor.Position.Y)) * 10);
                        neighbor.Parent = curCell;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private static List<Cell> RetracePath(Cell startPoint, Cell endPoint)
        {
            List<Cell> path = new List<Cell>();
            Cell currentNode = endPoint;

            while (currentNode != startPoint)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Add(startPoint);
            path.Reverse();

            return path;
        }

        private int GetDistance(Point neighborPosition, Point endPoint)
        {
            int dstX = Math.Abs(neighborPosition.X - endPoint.X);
            int dstY = Math.Abs(neighborPosition.Y - endPoint.Y);

            if (dstX > dstY)
            {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }

        private List<Cell> GetNeighbors(Cell curCell)
        {
            List<Cell> neighbors = new List<Cell>(8);
            var wallSprite = GameWorld.Instance.sprites["Wall"];
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    //ignore own position.
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    ////must be inside grid
                    //if (curCell.Position.X+i < 0 && curCell.Position.Y+j<0)
                    //{
                    //    continue;
                    //}

                    Cell curNeighbor;
                    if (jpsCells.TryGetValue(new Point(curCell.Position.X + i, curCell.Position.Y + j), out var cell))
                    {
                        curNeighbor = cell;
                    }
                    else
                    {
                        continue;
                    }

                    //must not be wall
                    if (GameWorld.Instance.sprites["Wall"] == curNeighbor.Sprite)
                    {
                        continue;
                    }
                    //CORNER CASES
                    // could probably have used a variable for cases... Same performance though.
                    switch (i)
                    {
                        //corner cases upper left
                        case -1 when j == 1 && (jpsCells[curCell.Position + new Point(i, 0)].Sprite == wallSprite || jpsCells[curCell.Position + new Point(0, j)].Sprite == wallSprite):
                        //upper right
                        case 1 when j == 1 && (jpsCells[curCell.Position + new Point(i, 0)].Sprite == wallSprite || jpsCells[curCell.Position + new Point(0, j)].Sprite == wallSprite):
                        //lower left
                        case -1 when j == -1 && (jpsCells[curCell.Position + new Point(i, 0)].Sprite == wallSprite || jpsCells[curCell.Position + new Point(0, j)].Sprite == wallSprite):
                        //lower right 
                        case 1 when j == -1 && (jpsCells[curCell.Position + new Point(i, 0)].Sprite == wallSprite || jpsCells[curCell.Position + new Point(0, j)].Sprite == wallSprite):
                            continue;
                        default:
                            neighbors.Add(curNeighbor);
                            break;
                    }
                }
            }

            return neighbors;
        }
    }
}
