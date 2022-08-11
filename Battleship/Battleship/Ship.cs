using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship
{
    public class Ship
    {
        public int length;
        public SquareState squareState;

        public Ship(int length, SquareState squareState)
        {
            this.length = length;
            this.squareState = squareState;
        }
    }
}
