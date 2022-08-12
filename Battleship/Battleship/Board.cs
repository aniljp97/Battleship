using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battleship
{
    public enum SquareState
    {
        Blank = ' ', // nothing
        Hit = 'X', // hit of a ship
        Miss = 'O', // an attempted hit on a blank square

        // Ships (a hit takes precedence)
        Carrier = 'C',
        Battleship = 'B',
        Destroyer = 'D',
        Submarine = 'S',
        PatrolBoat = 'P',
    }

    public class Board
    {
        private readonly string ColumnHeader = "  | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10\n";
        Random RandomGenerator = new Random();


        private SquareState[,] _grid = new SquareState[10, 10];

        public Board()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    _grid[row, col] = SquareState.Blank;
                }
            }
        }

        // Returns string describing the success of the shot
        public string ShootSquare(GridPosition square)
        {
            SquareState squareState = GetSquareState(square);
            switch (squareState)
            {
                case SquareState.Blank:
                    SetSquareState(square, SquareState.Miss);
                    return "Miss";
                case SquareState.Carrier:
                    SetSquareState(square, SquareState.Hit);
                    if(IsShipDestroyed(SquareState.Carrier))
                    {
                        return $"HIT and sunk the {SquareState.Carrier}";
                    }
                    return "HIT";
                case SquareState.Battleship:
                    SetSquareState(square, SquareState.Hit);
                    if (IsShipDestroyed(SquareState.Battleship))
                    {
                        return $"HIT and sunk the {SquareState.Battleship}";
                    }
                    return "HIT";
                case SquareState.Destroyer:
                    SetSquareState(square, SquareState.Hit);
                    if (IsShipDestroyed(SquareState.Destroyer))
                    {
                        return $"HIT and sunk the {SquareState.Destroyer}";
                    }
                    return "HIT";
                case SquareState.Submarine:
                    SetSquareState(square, SquareState.Hit);
                    if (IsShipDestroyed(SquareState.Submarine))
                    {
                        return $"HIT and sunk the {SquareState.Submarine}";
                    }
                    return "HIT";
                case SquareState.PatrolBoat:
                    SetSquareState(square, SquareState.Hit);
                    if (IsShipDestroyed(SquareState.PatrolBoat))
                    {
                        return $"HIT and sunk the {SquareState.PatrolBoat}";
                    }
                    return "HIT";
                case SquareState.Hit:
                case SquareState.Miss:
                    return "Invalid Move";
            }

            return "Invalid Move";
        }

        // starting from the argument defined row and column, search left to right and top to bottom through grid,
        // checking that the size fits from the square and to the right horizontal, then down vertically
        public List<GridPosition> GetFirstAvaibleSpace(GridPosition startPosition, int size, bool checkHorizontalFirst = true)
        {
            for (char row = startPosition.Row; row <= 'J'; row++)
            {
                for (int col = startPosition.Column; col <= 10; col++)
                {
                    if(!checkHorizontalFirst)
                    {
                        goto VerticalCheck;
                    }
                    HorizontalCheck:
                    if(col + size >= 10)
                    {
                        if (checkHorizontalFirst)
                        {
                            goto VerticalCheck;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    // check if the squares from the current square and the squares right of the size all have blank states
                    for (int spaceCol = col; ; spaceCol++)
                    {
                        if (spaceCol == size + col)
                        {
                            List<GridPosition> res = new List<GridPosition>();
                            for(int i = col; i < size + col; i++)
                            {
                                res.Add(new GridPosition(row, i));
                            }
                            return res;
                        }

                        if (GetSquareState(new GridPosition(row, spaceCol)) != SquareState.Blank)
                        {
                            break;
                        }
                    }
                    if (!checkHorizontalFirst)
                    {
                        continue;
                    }

                    VerticalCheck:
                    if (row + size > 'J')
                    {
                        if (checkHorizontalFirst)
                        {
                            continue;
                        }
                        else
                        {
                            goto HorizontalCheck;
                        }
                    }
                    // check if the squares from the current square and the squares down of the size all have blank states
                    for (char spaceRow = row; ; spaceRow++)
                    {
                        if (spaceRow == size + row)
                        {
                            List<GridPosition> res = new List<GridPosition>();
                            for (char i = row; i < size + row; i++)
                            {
                                res.Add(new GridPosition(i, col));
                            }
                            return res;
                        }

                        if (GetSquareState(new GridPosition(spaceRow, col)) != SquareState.Blank)
                        {
                            break;
                        }
                    }

                    if (!checkHorizontalFirst)
                    {
                        goto HorizontalCheck;
                    }
                }
            }

            return null;
        }

        public bool IsShipDestroyed(SquareState shipState)
        {
            for (char row = 'A'; row <= 'J'; row++)
            {
                for (int col = 1; col <= 10; col++)
                {
                    if (GetSquareState(new GridPosition(row, col)) == shipState)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsAllShipsDestroyed()
        {
            for (char row = 'A'; row <= 'J'; row++)
            {
                for (int col = 1; col <= 10; col++)
                {
                    SquareState shipState = GetSquareState(new GridPosition(row, col));
                    if (shipState == SquareState.Carrier || shipState == SquareState.Battleship || shipState == SquareState.Destroyer || shipState == SquareState.Submarine || shipState == SquareState.PatrolBoat)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsAllSpacesStateMatch(List<GridPosition> spaces, params SquareState[] states)
        {
            foreach (GridPosition position in spaces)
            {
                bool hasMatch = false;
                foreach (SquareState state in states)
                {
                    if (GetSquareState(position) == state)
                    {
                        hasMatch = true;
                    }
                }

                if(!hasMatch)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetSpaces(List<GridPosition> spaces, SquareState state)
        {
            foreach (GridPosition position in spaces)
            {
                SetSquareState(position, state);
            }
        }
            
        public void SetShipRandomly(Ship ship)
        {
            List<GridPosition> shipSpaces = null;
            int randDirec = RandomGenerator.Next(2);

            while (shipSpaces == null)
            {
                int randPos = RandomGenerator.Next(100);

                char randRow = (char)((randPos / 10) + 'A');
                int randCol = randPos - (randPos / 10 * 10) + 1;

                shipSpaces = GetFirstAvaibleSpace(new GridPosition(randRow, randCol), ship.length, randDirec == 1 ? true : false);
            }
            SetSpaces(shipSpaces, ship.squareState);
        }

        public void PrintBoardPlayerView()
        {
            StringBuilder board = new StringBuilder(ColumnHeader);
            for(char row = 'A'; row <= 'J'; row++)
            {
                board.AppendLine("--+---+---+---+---+---+---+---+---+---+---");
                board.Append($" {row}");
                for(int col = 1; col <= 10; col++)
                {
                    SquareState squareState = GetSquareState(new GridPosition(row, col));
                    switch(squareState)
                    {
                        case SquareState.Carrier:
                        case SquareState.Battleship:
                        case SquareState.Destroyer:
                        case SquareState.Submarine:
                        case SquareState.PatrolBoat:
                        case SquareState.Hit:
                            board.Append($"| {(char)squareState} ");
                            break;
                        case SquareState.Miss:
                        case SquareState.Blank:
                            board.Append($"|   ");
                            break;
                    }
                }

                board.AppendLine();
            }

            Console.WriteLine(board);
        }

        public void PrintBoardOpponentView()
        {
            StringBuilder board = new StringBuilder(ColumnHeader);
            for (char row = 'A'; row <= 'J'; row++)
            {
                board.AppendLine("--+---+---+---+---+---+---+---+---+---+---");
                board.Append($" {row}");
                for (int col = 1; col <= 10; col++)
                {
                    SquareState squareState = GetSquareState(new GridPosition(row, col));
                    switch (squareState)
                    {
                        case SquareState.Miss:
                        case SquareState.Hit:
                            board.Append($"| {(char)squareState} ");
                            break;
                        case SquareState.Carrier:
                        case SquareState.Battleship:
                        case SquareState.Destroyer:
                        case SquareState.Submarine:
                        case SquareState.PatrolBoat:
                        case SquareState.Blank:
                            board.Append($"|   ");
                            break;
                    }
                }

                board.AppendLine();
            }

            Console.WriteLine(board);
        }


        private void SetSquareState(GridPosition square, SquareState state)
        {
            char row = Char.ToUpper(square.Row);
            _grid[square.Row - 'A', square.Column - 1] = state;
        }

        private SquareState GetSquareState(GridPosition square)
        {
            char row = Char.ToUpper(square.Row);
            return _grid[row - 'A', square.Column - 1];
        }
    }
}
