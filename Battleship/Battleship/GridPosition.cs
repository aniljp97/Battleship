using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship
{
    public class GridPosition
    {
        public char Row { get; }
        public int Column { get; }

        public GridPosition(char row, int col)
        {
            row = Char.ToUpper(row);

            if(!('A' <= row && row <= 'J'))
            {
                throw new ArgumentOutOfRangeException($"Invalid row: {row}. row of GridPosition must be from 'A' to 'J'.");
            }
            if(!(1 <= col && col <= 10))
            {
                throw new ArgumentOutOfRangeException($"Invalid column: {col}. col of GridPosition must be from 1 to 10.");
            }

            Row = row;
            Column = col;
        }

        public override string ToString()
        {
            return $"{Row}{Column}";
        }
    }
}
