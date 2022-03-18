using System;
using System.Collections.Generic;

namespace ChessV2
{
    class Program
    {
        static void Main(string[] args)
        {
            AI ai = new AI(new MoveNode(new Move(new Piece((0, 0), (0,0), new List<(int, int)>(), true, 'U', 0, false, false, false, false), (0, 0), 0, 0), new List<MoveNode>()));
            Board gameboard = new Board();
            while (true)
            {
                gameboard.turn = true;
                Console.WriteLine(gameboard);

                gameboard.GenerateMoves();
                gameboard.SelectLegalMoves();

                foreach (Move legalmove in gameboard.LegalMoves)
                {
                    Console.Write($"{legalmove}, ");
                }
                Console.WriteLine();
            start:
                string userInput = Console.ReadLine();

                Input move = new Input(userInput);

                if (!move.CheckValidInput(move.ReturnInput()))
                {
                    Console.WriteLine("Input invalid. Please write in the format (Piece to Move),(Starting File), End File, End rank. " +
                    "For Example Knight to E6 is input as NE6");
                    goto start;
                }

                Console.WriteLine(move.ReturnInput());

                if (gameboard.MakeMove(move.ReturnInput()))
                {
                    gameboard.turn = false;
                    ai.FindBestMove(gameboard);
                }
                else
                {
                    Console.WriteLine("Invalid move, try again.");
                    goto start;
                }
            }
            
        }
    }
}
