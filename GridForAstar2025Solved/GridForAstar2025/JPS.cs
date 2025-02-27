using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
                      
        
        public List<Cell> FindJPSPath(Point startPoint, Point endPoint)
        {
            openList.Clear();
            closedList.Clear();
                        
            if(!jpsCells.ContainsKey(startPoint) || !jpsCells.ContainsKey(endPoint))
            {
                Debug.WriteLine("Invalid start or end point");
                return null;
            }
            
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

                List<Cell> neighbors = GetJumpNeighbors(curCell, endPoint);
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
                        neighbor.H = GetDistance(neighbor.Position, endPoint);
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

                if (currentNode == null)
                {
                    Debug.WriteLine($"Reached a null parent during path retracing. Path might be invalid.");
                }
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

        private List<Cell> GetJumpNeighbors(Cell curCell, Point endPoint)
        {
            List<Cell> neighbors = new List<Cell>();
            Point[] directions = new Point[]
            {
                new Point(0,-1), new Point(1,0), new Point(0,1), new Point(-1,0), // Cardinal direction
                new Point(1,-1), new Point(1,1), new Point(-1,1), new Point(-1,-1) // Diagonal direction
            };

            foreach (var direction in directions)
            {
                Cell jumpPoint = Jump(curCell.Position, direction, endPoint);
                if(jumpPoint != null && !neighbors.Contains(jumpPoint))
                {
                    neighbors.Add(jumpPoint);
                }
            }
            Debug.WriteLine($"Found {neighbors.Count} jump point for {curCell.Position}");
            return neighbors;
        }

        private Cell Jump(Point current, Point direction, Point endPoint)
        {
            Point next = new Point(current.X + direction.X, current.Y + direction.Y);
            
            // Check if next is a valid key in the dictionary
            if (!jpsCells.ContainsKey(next) || IsBlocked(next))
            {
                Debug.WriteLine($"Key not found at: {next}");               
                return null;
                
            }
            

            if(next == endPoint)
            {
                return jpsCells[next];
            }


            // Check for forced neighbors
            if(direction.X != 0 && direction.Y != 0) // diagonal movement 
            {
                if ((jpsCells.ContainsKey(new Point(next.X - direction.X, next.Y)) && IsBlocked(new Point(next.X - direction.X, next.Y)))                    
                    ||
                    (jpsCells.ContainsKey(new Point(next.X, next.Y - direction.Y)) && IsBlocked(new Point(next.X, next.Y - direction.Y))))
                    
                {
                    Debug.WriteLine($"Jumping from {current} in direction {direction} to {next}");
                    return jpsCells[next];
                }
            }
            else
            {
                if(direction.X != 0) // horizontal movement
                {
                    if ((jpsCells.ContainsKey(new Point(next.X, next.Y + 1)) && IsBlocked(new Point(next.X, next.Y + 1)))
                        ||
                        (jpsCells.ContainsKey(new Point(next.X, next.Y - 1)) && IsBlocked(new Point(next.X, next.Y - 1))))                        
                    {
                        Debug.WriteLine($"Jumping from {current} in direction {direction} to {next}");
                        return jpsCells[next];
                    }
                }
                else if(direction.Y != 0) // vertical movement
                {
                    if ((jpsCells.ContainsKey(new Point(next.X + 1, next.Y)) && IsBlocked(new Point(next.X + 1, next.Y))) 
                        ||
                        (jpsCells.ContainsKey(new Point(next.X - 1, next.Y)) && IsBlocked(new Point(next.X - 1, next.Y))))
                    {
                        Debug.WriteLine($"Jumping from {current} in direction {direction} to {next}");
                        return jpsCells[next];
                    }
                }
            }
            return Jump(next, direction, endPoint);
        }
        private bool IsBlocked(Point point)
        {
            return jpsCells.ContainsKey(point) &&
                        (jpsCells[point].Sprite == GameWorld.Instance.sprites["Wall"] ||
                        jpsCells[point].Sprite == GameWorld.Instance.sprites["Tree"] ||
                        jpsCells[point].Sprite == GameWorld.Instance.sprites["Mushroom"]);
        }
    }
}
