using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Move HoldMove = new Move(new Piece((0, 0), (0, 0), new List<Move>(), true, 'U', 0, false, false, false, false), (0, 0), 0, 0);
            AI ai = new AI(new MoveNode(HoldMove, new List<MoveNode>(), 0));
            Board gameboard = new Board();
            while (true)
            {
                gameboard.turn = true;
                Console.WriteLine(gameboard);

                gameboard.GenerateMoves(gameboard.MoveCount == 0 ? HoldMove : gameboard.MoveHistory.Last());
                gameboard.PrintLegalMoves();
                
                Console.WriteLine();
            start:
                string userInput = Console.ReadLine();

                Input move = new Input(userInput, gameboard.turn);

                if (!move.CheckValidInput(move.ReturnInput()))
                {
                    Console.WriteLine("Input invalid. Please write in the format (Piece to Move),(Starting File), End File, End rank. " +
                    "For Example Knight to E6 is input as NE6");
                    goto start;
                }

                Console.WriteLine(move.ReturnInput());

                if (gameboard.MakeMove(move.ReturnInput()))
                {
                    Console.WriteLine(gameboard);
                    gameboard.turn = false;
                    if (gameboard.MoveCount < 1)
                    {
                        gameboard.GenerateMoves(gameboard.MoveHistory.Last());
                        gameboard.PrintLegalMoves();
                        userInput = Console.ReadLine();
                        Input AiMove = new Input(userInput, gameboard.turn);
                        gameboard.MakeMove(AiMove.ReturnInput());
                    }
                    else
                    {
                        ai.FindBestMove(gameboard);
                    } 
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
