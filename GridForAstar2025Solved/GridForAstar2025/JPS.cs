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

        /// <summary>
        /// Find the index of the cell searched for in the cell array
        /// </summary>
        /// <param name="jpsCells"></param>
        /// <param name="searchCell"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int FindIndex(Cell[] jpsArrayCells, Cell searchCell)
        {
            if(jpsArrayCells is null)
            {
                throw new ArgumentNullException("jpsArrayCells was null");
            }

            int jumpStep = (int)Math.Floor(Math.Sqrt(jpsArrayCells.Length));
            int currentCell = 0;
            int nextCell = jumpStep;

            if (jpsArrayCells.Length != 0)
            {
                while (jpsArrayCells[currentCell-1].CompareTo(searchCell) < 0)
                {
                    currentCell = nextCell;
                    nextCell = jumpStep;

                    if(nextCell >= jpsArrayCells.Length)
                    {
                        nextCell = jpsArrayCells.Length;
                        break;
                    }
                }

                for (int i = currentCell; i < nextCell; i++)
                {
                    if (jpsArrayCells[i].CompareTo(searchCell) == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        
        public List<Cell> FindPath(Point startPoint, Point endPoint)
        {
            openList.Clear();
            closedList.Clear();

            foreach (var cell in jpsCells.Values)
            {
                cell.G = 0;
                cell.H = 0;
                cell.Parent = null;
                cell.spriteColor = Color.White;
            }

            int jumpStep = (int)Math.Floor(Math.Sqrt(jpsCells.Count));
            int nextCell = jumpStep;

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

        private List<Cell> RetracePath(Cell startPoint, Cell endPoint)
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
