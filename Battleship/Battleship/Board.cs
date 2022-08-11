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
        public string ShootSquare(char row, int column)
        {
            try
            {
                SquareState squareState = GetSquare(row, column);
                switch (squareState)
                {
                    case SquareState.Blank:
                        SetSquare(row, column, SquareState.Miss);
                        return "Miss";
                    case SquareState.Carrier:
                        SetSquare(row, column, SquareState.Hit);
                        if(IsShipDestroyed(SquareState.Carrier))
                        {
                            return $"HIT and sunk their {SquareState.Carrier}";
                        }
                        return "HIT";
                    case SquareState.Battleship:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.Battleship))
                        {
                            return $"HIT and sunk their {SquareState.Battleship}";
                        }
                        return "HIT";
                    case SquareState.Destroyer:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.Destroyer))
                        {
                            return $"HIT and sunk their {SquareState.Destroyer}";
                        }
                        return "HIT";
                    case SquareState.Submarine:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.Submarine))
                        {
                            return $"HIT and sunk their {SquareState.Submarine}";
                        }
                        return "HIT";
                    case SquareState.PatrolBoat:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.PatrolBoat))
                        {
                            return $"HIT and sunk their {SquareState.PatrolBoat}";
                        }
                        return "HIT";
                    case SquareState.Hit:
                    case SquareState.Miss:
                        return "Invalid Move";
                }
            }
            catch(Exception)
            {
                return "Invalid Move";
            }

            return "Invalid Move";
        }

        public string ShootRandomSquare()
        {
            try
            {
                Start:
                int randPos = RandomGenerator.Next(100);

                char row = (char)((randPos / 10) + 'A');
                int column = randPos - (randPos / 10 * 10) + 1;

                SquareState squareState = GetSquare(row, column);
                switch (squareState)
                {
                    case SquareState.Blank:
                        SetSquare(row, column, SquareState.Miss);
                        return $"Miss at {row}{column}";
                    case SquareState.Carrier:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.Carrier))
                        {
                            return $"HIT at {row}{column} and sunk your {SquareState.Carrier}";
                        }
                        return $"HIT at {row}{column}";
                    case SquareState.Battleship:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.Battleship))
                        {
                            return $"HIT at {row}{column} and sunk your {SquareState.Battleship}";
                        }
                        return $"HIT at {row}{column}";
                    case SquareState.Destroyer:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.Destroyer))
                        {
                            return $"HIT at {row}{column} and sunk your {SquareState.Destroyer}";
                        }
                        return $"HIT at {row}{column}";
                    case SquareState.Submarine:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.Submarine))
                        {
                            return $"HIT at {row}{column} and sunk your {SquareState.Submarine}";
                        }
                        return $"HIT at {row}{column}";
                    case SquareState.PatrolBoat:
                        SetSquare(row, column, SquareState.Hit);
                        if (IsShipDestroyed(SquareState.PatrolBoat))
                        {
                            return $"HIT at {row}{column} and sunk your {SquareState.PatrolBoat}";
                        }
                        return $"HIT at {row}{column}";
                    case SquareState.Hit:
                    case SquareState.Miss:
                        goto Start;
                }
            }
            catch (Exception)
            {
                return "Invalid Move";
            }

            return "Invalid Move";
        }

        // starting from the argument defined row and column, search left to right and top to bottom through grid,
        // checking that the size fits from the square and to the right horizontal, then down vertically
        public List<KeyValuePair<char,int>> GetFirstAvaibleSpace(char startRow, int startCol, int size, bool checkHorizontalFirst = true)
        {
            for (char row = startRow; row <= 'J'; row++)
            {
                for (int col = startCol; col < 10; col++)
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
                            List<KeyValuePair<char, int>> res = new List<KeyValuePair<char, int>>();
                            for(int i = col; i < size + col; i++)
                            {
                                res.Add(new KeyValuePair<char, int>(row, i));
                            }
                            return res;
                        }

                        if (!IsSquareBlank(row, spaceCol))
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
                            List<KeyValuePair<char, int>> res = new List<KeyValuePair<char, int>>();
                            for (char i = row; i < size + row; i++)
                            {
                                res.Add(new KeyValuePair<char, int>(i, col));
                            }
                            return res;
                        }

                        if (!IsSquareBlank(spaceRow, col))
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
            if (shipState != SquareState.Carrier && shipState != SquareState.Battleship && shipState != SquareState.Destroyer && shipState != SquareState.Submarine && shipState != SquareState.PatrolBoat)
            {
                throw new ArgumentException($"shipState arg must be a ship (Carrier, Battleship, Destroyer, Submarine, or PatrolBoat)");
            }

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    if (_grid[row, col] == shipState)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsAllShipsDestroyed()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    SquareState shipState = _grid[row, col];
                    if (shipState == SquareState.Carrier || shipState == SquareState.Battleship || shipState == SquareState.Destroyer || shipState == SquareState.Submarine || shipState == SquareState.PatrolBoat)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsSpacesStateMatch(List<KeyValuePair<char, int>> spaces, params SquareState[] states)
        {
            try
            {
                foreach (KeyValuePair<char, int> position in spaces)
                {
                    bool hasMatch = false;
                    foreach (SquareState state in states)
                    {
                        if (GetSquare(position.Key, position.Value) == state)
                        {
                            hasMatch = true;
                        }
                    }

                    if(!hasMatch)
                    {
                        return false;
                    }
                }
            }
            catch(ArgumentException)
            {
                return false;
            }

            return true;
        }

        public void SetSpaces(List<KeyValuePair<char, int>> spaces, SquareState state)
        {
            foreach (KeyValuePair<char, int> position in spaces)
            {
                SetSquare(position.Key, position.Value, state);
            }
        }
            
        public void SetShipRandomly(Ship ship)
        {
            List<KeyValuePair<char, int>> shipSpaces = null;
            int randDirec = RandomGenerator.Next(2);

            while (shipSpaces == null)
            {
                int randPos = RandomGenerator.Next(100);

                char randRow = (char)((randPos / 10) + 'A');
                int randCol = randPos - (randPos / 10 * 10) + 1;

                shipSpaces = GetFirstAvaibleSpace(randRow, randCol, ship.length, randDirec == 1 ? true : false);
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
                for(int col = 0; col < 10; col++)
                {
                    SquareState squareState = _grid[row - 'A', col];
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
                for (int col = 0; col < 10; col++)
                {
                    SquareState squareState = _grid[row - 'A', col];
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


        private void SetSquare(char row, int column, SquareState state)
        {
            row = Char.ToUpper(row);

            if ('A' > row || row > 'J')
            {
                throw new ArgumentException($"Row can only be 'A' to 'J'. Invalid: {row}");
            }

            if (1 > column && column > 10)
            {
                throw new ArgumentException($"Column can only be 1 to 10. Invalid: {column}");
            }

            _grid[row - 'A', column - 1] = state;
        }

        private SquareState GetSquare(char row, int column)
        {
            row = Char.ToUpper(row);

            if ('A' > row || row > 'J')
            {
                throw new ArgumentException($"Row can only be 'A' to 'J'. Invalid: {row}");
            }

            if (1 > column || column > 10)
            {
                throw new ArgumentException($"Column can only be 1 to 10. Invalid: {column}");
            }

            return _grid[row - 'A', column - 1];
        }

        private bool IsSquareBlank(char row, int column)
        {
            try
            {
                if (GetSquare(row, column) == SquareState.Blank)
                {
                    return true;
                }
            }
            catch (Exception) { }

            return false;
        }
    }
}
