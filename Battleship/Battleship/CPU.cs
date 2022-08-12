using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship
{
    public enum Difficulty
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public class CPU
    {
        Board board;
        Difficulty smarts;

        GridPosition prevMove = null;
        bool prevMoveHitAndNotDestroyedShip = false;

        Random RandomGenerator = new Random();

        public CPU(Board board, Difficulty smarts)
        {
            this.board = board;
            this.smarts = smarts;
        }

        public void SetShips(params Ship[] ships)
        {
            foreach (var ship in ships)
            {
                board.SetShipRandomly(ship);
            }
        }

        public string MakeMove(Board enemyBoard)
        {
            if(smarts == Difficulty.Beginner)
            {
            Retry:
                string shotResult = enemyBoard.ShootSquare(GetRandomPosition());
                if (shotResult == "Invalid Move")
                    goto Retry;

                return shotResult;
            }
            else if(smarts == Difficulty.Intermediate)
            {
                GridPosition shotPosition;
                string shotResult;

                // if it is the first move, shot random and move on
                if (prevMove == null)
                {
                    shotPosition = GetRandomPosition();
                    Console.Write(shotPosition.ToString());
                    shotResult = enemyBoard.ShootSquare(shotPosition);
                }
                else // Not the first move
                {
                    // if the previous move was a hit and the hit didn't destroy a ship, shoot at a valid adjacent space
                    if (prevMoveHitAndNotDestroyedShip)
                    {
                        // to appease compiler of use of unassigned variables
                        shotPosition = GetRandomPosition();
                        shotResult = "Invalid Move";

                        List<GridPosition> shotsToTry = new List<GridPosition>();
                        if (prevMove.Column + 1 <= 10)
                        {
                            shotsToTry.Add(new GridPosition(prevMove.Row, prevMove.Column + 1)); // one to the right
                        }
                        if (prevMove.Row + 1 <= 'J')
                        {
                            shotsToTry.Add(new GridPosition((char)(prevMove.Row + 1), prevMove.Column)); // one below
                        }
                        if (prevMove.Column - 1 >= 1)
                        {
                            shotsToTry.Add(new GridPosition(prevMove.Row, prevMove.Column - 1)); // one to the left
                        }
                        if (prevMove.Row - 1 >= 'A')
                        {
                            shotsToTry.Add(new GridPosition((char)(prevMove.Row - 1), prevMove.Column)); // one up
                        }

                        foreach(var shot in shotsToTry)
                        {
                            shotResult = enemyBoard.ShootSquare(shot);
                            if(shotResult == "MISS")
                            {
                                shotPosition = prevMove;
                                Console.Write(shotPosition.ToString() + " ");
                                break;
                            }
                            else if(shotResult == "Invalid Move")
                            {
                                continue;
                            }
                            else
                            {
                                shotPosition = shot;
                                Console.Write(shotPosition.ToString() + " ");
                                break;
                            }
                        }
                    }
                    // if not, shoot randomly
                    else
                    {
                    Retry:
                        shotPosition = GetRandomPosition();
                        shotResult = enemyBoard.ShootSquare(shotPosition);
                        if (shotResult == "Invalid Move")
                            goto Retry;

                        Console.Write(shotPosition.ToString() + " ");
                    }
                }

                prevMove = shotPosition;
                if (shotResult == "HIT")
                {
                    prevMoveHitAndNotDestroyedShip = true;
                }
                else
                {
                    prevMoveHitAndNotDestroyedShip = false;
                }
                
                return shotResult;
            }
            else if(smarts == Difficulty.Advanced)
            {
                // TODO
                // Check for previous hits that are not a part of a sunken ship and shoot based on that
                // after a HIT, account for ships left and do not choose the adjacent spaces that has the most space in that direction
                // Also after a HIT, only try long ends if 2 or more hits align
                // If randomly choosing, choose spot that eliminate the most open spaces of the length of the smallest ship alive

            }

            throw new ArgumentException($"Process of move making not implemented for CPU of {smarts} difficulty.");
        }

        private GridPosition GetRandomPosition()
        {
            int randPos = RandomGenerator.Next(100);

            char row = (char)((randPos / 10) + 'A');
            int column = randPos - (randPos / 10 * 10) + 1;

            return new GridPosition(row, column);
        }
    }
}
