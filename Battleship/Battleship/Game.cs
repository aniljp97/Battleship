using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Battleship
{
    public enum GameState
    {
        PlayerWon, // means game is over
        OpponentWon, // means game is over
        Unfinished
    }

    public class Game
    {
        Board playerBoard;
        CPU opponent;
        Board opponentBoard;
        public GameState gameState = GameState.Unfinished;

        public Game(Difficulty difficulty)
        {
            playerBoard = new Board();
            opponentBoard = new Board();
            opponent = new CPU(opponentBoard, difficulty);
        }

        public void SetShips()
        {
            Ship carrier = new Ship(5, SquareState.Carrier);
            Ship battleship = new Ship(4, SquareState.Battleship);
            Ship destoryer = new Ship(3, SquareState.Destroyer);
            Ship submarine = new Ship(3, SquareState.Submarine);
            Ship patrolBoat = new Ship(2, SquareState.PatrolBoat);

            SetPlayerShip(carrier);
            SetPlayerShip(battleship);
            SetPlayerShip(destoryer);
            SetPlayerShip(submarine);
            SetPlayerShip(patrolBoat);

            opponent.SetShips(carrier, battleship, destoryer, submarine, patrolBoat);
        }

        public void PlayRound()
        {
            Console.Clear();

            // View and Info of target grid
            string carrierStr = opponentBoard.IsShipDestroyed(SquareState.Carrier) ? SquareState.Carrier.ToString() : "";
            string battleshipStr = opponentBoard.IsShipDestroyed(SquareState.Battleship) ? $"{SquareState.Battleship}" : "";
            string destroyerStr = opponentBoard.IsShipDestroyed(SquareState.Destroyer) ? $"{SquareState.Destroyer}" : "";
            string submarineStr = opponentBoard.IsShipDestroyed(SquareState.Submarine) ? $"{SquareState.Submarine}" : "";
            string patrolBoatStr = opponentBoard.IsShipDestroyed(SquareState.PatrolBoat) ? $"{SquareState.PatrolBoat}" : "";
            Console.WriteLine($"Enemy Field   -   Ships you've sunk: {carrierStr}, {battleshipStr}, {destroyerStr}, {submarineStr}, {patrolBoatStr}");
            opponentBoard.PrintBoardOpponentView();
            Console.WriteLine();

            // View and info of own grid
            carrierStr = playerBoard.IsShipDestroyed(SquareState.Carrier) ? SquareState.Carrier.ToString() : "";
            battleshipStr = playerBoard.IsShipDestroyed(SquareState.Battleship) ? $"{SquareState.Battleship}" : "";
            destroyerStr = playerBoard.IsShipDestroyed(SquareState.Destroyer) ? $"{SquareState.Destroyer}" : "";
            submarineStr = playerBoard.IsShipDestroyed(SquareState.Submarine) ? $"{SquareState.Submarine}" : "";
            patrolBoatStr = playerBoard.IsShipDestroyed(SquareState.PatrolBoat) ? $"{SquareState.PatrolBoat}" : "";
            Console.WriteLine($"Your Field   -   Your sunken Ships: {carrierStr}, {battleshipStr}, {destroyerStr}, {submarineStr}, {patrolBoatStr}");
            playerBoard.PrintBoardPlayerView();

            // User inputting move
            Console.Write("Input a position (ex.'E4') on the enemy field to shoot at and press ENTER to fire: ");
        UserMove:
            var playerMovePosition = Console.ReadLine();
            playerMovePosition = playerMovePosition.ToUpper();

            if (!(playerMovePosition.Length == 2 && IsValidBoardRow(playerMovePosition[0]) && IsValidBoardColumn(playerMovePosition[1] - '0')) && !(playerMovePosition.Length == 3 && IsValidBoardRow(playerMovePosition[0]) && playerMovePosition[1] == '1' && playerMovePosition[2] == '0'))
            {
                Console.Write("Invalid input. Must be a character from A to J, followed by a number from 1 to 10. Example: F10. Try Again: ");
                goto UserMove;
            }

            char playerMoveRow = playerMovePosition[0];
            int playerMoveColumn = Int16.Parse(playerMovePosition[1..]);
            string moveResult = opponentBoard.ShootSquare(new GridPosition(playerMoveRow, playerMoveColumn));
            Console.Write($"Your shot was a {moveResult}!");
            switch(moveResult)
            {
                case "MISS":
                    break;
                case "HIT":
                    Console.Write(" Go Again!: ");
                    goto UserMove;
                case "Invalid Move":
                    Console.Write(" Make sure you move is not a square you have already shot and try again: ");
                    goto UserMove;
            }
            Thread.Sleep(1000);
            if (opponentBoard.IsAllShipsDestroyed()) // Check if user won
            {
                gameState = GameState.PlayerWon;
                return;
            }

            // CPU move
            Console.WriteLine();
            OpponentMove:
            Console.WriteLine("\nOpponent moving...");
            Thread.Sleep(500);
            moveResult = opponent.MakeMove(playerBoard);
            Console.Write($"Opponent shot was a {moveResult}!");
            switch (moveResult)
            {
                case "MISS":
                    break;
                case "HIT":
                    Console.Write(" They get to go again.");
                    goto OpponentMove;
                case "Invalid Move":
                    Console.Write(" Man this CPU sucks.");
                    goto UserMove;
            }
            if (playerBoard.IsAllShipsDestroyed()) // Check if CPU won
            {
                gameState = GameState.OpponentWon;
                return;
            }


            Console.WriteLine("\n\nPress ENTER to continue and update boards...");
            WaitForEnterKey();
        }

        public bool IsGameOver()
        {
            if(gameState == GameState.PlayerWon || gameState == GameState.OpponentWon)
            {
                return true;
            }

            return false;
        }

        private void SetPlayerShip(Ship ship)
        {
            List<GridPosition> shipSpaces = playerBoard.GetFirstAvaibleSpace(new GridPosition('A', 1), ship.length);
            playerBoard.SetSpaces(shipSpaces, ship.squareState);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\nSet your {ship.squareState} Ship denoted by the '{(char)ship.squareState}' character... (Press ENTER to confirm ship position)\n");
                playerBoard.PrintBoardPlayerView();
                Console.WriteLine("Controls:\nA = move left; W = move up; S = move down; D = move right; Q = flip orientation; ");
                ConsoleKeyInfo keyPress = Console.ReadKey();
                switch (keyPress.Key)
                {
                    case ConsoleKey.A: // move left
                        if (shipSpaces.First().Column - 1 >= 1)
                        {
                            var newShipSpaces = shipSpaces.Select(s => new GridPosition(s.Row, s.Column - 1)).ToList();
                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if (newShipSpaces.Last().Column - 1 < 1)
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition(s.Row, s.Column - 1)).ToList();
                            }
                        }
                        break;
                    case ConsoleKey.W: // move up
                        if (shipSpaces.First().Row - 1 >= 'A')
                        {
                            var newShipSpaces = shipSpaces.Select(s => new GridPosition((char)(s.Row - 1), s.Column)).ToList();
                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if (newShipSpaces.First().Row - 1 < 'A')
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition((char)(s.Row - 1), s.Column)).ToList();
                            }
                        }
                        break;
                    case ConsoleKey.S: // move down
                        if (shipSpaces.Last().Row + 1 <= 'J')
                        {
                            var newShipSpaces = shipSpaces.Select(s => new GridPosition((char)(s.Row + 1), s.Column)).ToList();
                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if (newShipSpaces.Last().Row + 1 > 'J')
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition((char)(s.Row + 1), s.Column)).ToList();
                            }
                        }
                        break;
                    case ConsoleKey.D: // move right
                        if (shipSpaces.Last().Column + 1 <= 10)
                        {
                            var newShipSpaces = shipSpaces.Select(s => new GridPosition(s.Row, s.Column + 1)).ToList();
                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if (newShipSpaces.Last().Column + 1 > 10)
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition(s.Row, s.Column + 1)).ToList();
                            }
                        }
                        break;
                    case ConsoleKey.Q: // switch ship direction
                        // First find if vertical or horizontal
                        if (shipSpaces.Select(s => s.Row).Distinct().Count() == 1) // if all keys (representing row is all the same), it is horizontal
                        {
                            var newShipSpaces = new List<GridPosition> { shipSpaces.First() };
                            int newColumn = shipSpaces.First().Column;
                            for (int newRow = 1; newRow < shipSpaces.Count(); newRow++)
                            {
                                if (newShipSpaces.Last().Row + 1 > 'J')
                                {
                                    newShipSpaces = newShipSpaces.Select(s => new GridPosition((char)(s.Row - 1), s.Column)).ToList();
                                    newShipSpaces.Add(new GridPosition('J', newColumn));
                                }
                                else
                                {
                                    newShipSpaces.Add(new GridPosition((char)(newShipSpaces.Last().Row + 1), newColumn));
                                }
                            }

                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if(newShipSpaces.Last().Row + 1 > 'J')
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition((char)(s.Row + 1), s.Column)).ToList();
                            }

                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if(newShipSpaces.First().Row - 1 < 'A')
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition((char)(s.Row - 1), s.Column)).ToList();
                            }
                        }
                        else // it is vertical
                        {
                            var newShipSpaces = new List<GridPosition> { shipSpaces.First() };
                            char newRow = shipSpaces.First().Row;
                            for (int newCol = 1; newCol < shipSpaces.Count(); newCol++)
                            {
                                if (newShipSpaces.Last().Column + 1 > 10)
                                {
                                    newShipSpaces = newShipSpaces.Select(s => new GridPosition(s.Row, s.Column - 1)).ToList();
                                    newShipSpaces.Add(new GridPosition(newRow, 10));
                                }
                                else
                                {
                                    newShipSpaces.Add(new GridPosition(newRow, newShipSpaces.Last().Column + 1));
                                }
                            }

                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if(newShipSpaces.Last().Column + 1 > 10)
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition(s.Row, s.Column + 1)).ToList();
                            }

                            while (true)
                            {
                                if (playerBoard.IsAllSpacesStateMatch(newShipSpaces, SquareState.Blank, ship.squareState))
                                {
                                    playerBoard.SetSpaces(shipSpaces, SquareState.Blank);
                                    playerBoard.SetSpaces(newShipSpaces, ship.squareState);
                                    shipSpaces = newShipSpaces;
                                    break;
                                }
                                if(newShipSpaces.Last().Column - 1 < 1)
                                {
                                    break;
                                }

                                newShipSpaces = newShipSpaces.Select(s => new GridPosition(s.Row, s.Column - 1)).ToList();
                            }
                        }
                        break;
                    case ConsoleKey.Enter: // next ship or finish
                        return;
                }

            }
        }

        private bool IsValidBoardRow(char c)
        {
            return 'A' <= c && c <= 'J';
        }

        private bool IsValidBoardColumn(int c)
        {
            return 1 <= c && c <= 10;
        }

        private void WaitForEnterKey()
        {
            ConsoleKey key = ConsoleKey.S;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey().Key;
            }
        }
    }
}
