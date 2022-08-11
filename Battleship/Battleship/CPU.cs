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
                return enemyBoard.ShootRandomSquare();
            }
            else if(smarts == Difficulty.Intermediate)
            {
                // TODO
            }
            else if(smarts == Difficulty.Advanced)
            {

            }

            throw new ArgumentException($"Process of move making not implemented for CPU of {smarts} difficulty.");
        }
    }
}
