using System;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            Game newGame = new Game(Difficulty.Beginner);
            newGame.SetShips();

            while(!newGame.IsGameOver())
            {
                newGame.PlayRound();
            }

            if(newGame.gameState == GameState.PlayerWon)
            {
                Console.WriteLine("\nCongrats! You Won!!! WIN WIN WIN WIN WIN WIN WIN WIN WIN");
            }
            else if(newGame.gameState == GameState.OpponentWon)
            {
                Console.WriteLine("\nToo bad. You Lost. NOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO!");
            }
        }
    }
}
