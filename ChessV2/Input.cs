using System;
namespace ChessV2
{
    public class Input
    {
        private const string files = "ABCDEFGH";
        //private readonly int[] ranks = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private const string pieces = "PRNBQK";
        private bool isPiece = false;
        private bool isStartFile = false;
        private bool isStartRank = false;
        private char piece;
        private int startFile;
        private int startRank;
        private (int, int) move;
        //private string Input;

        public Input(string input, bool turn)
        {
            if (input.ToUpper() == "O-O")
            {
                piece = 'K';
                startFile = 5;
                isPiece = true;
                isStartFile = true;
                move = (7, turn ? 1 : 8);

            }
            if (input.ToUpper() == "O-O-O")
            {
                piece = 'K';
                startFile = 5;
                isPiece = true;
                isStartFile = true;
                
                move = (3, turn ? 1 : 8);
            }
            if (isPiece == false)
            {
                if (input.Length == 2)
                {

                    move = (GetFile(input[0]), GetRank(input[1]));
                }
                if (input.Length == 3)
                {
                    move = (GetFile(input[1]), GetRank(input[2]));
                    piece = CheckPieceValid(input[0]);
                    isPiece = true;
                }
                if (input.Length == 4)
                {
                    move = (GetFile(input[2]), GetRank(input[3]));
                    piece = CheckPieceValid(input[0]);
                    if (!int.TryParse(input[1].ToString(), out startRank))
                    {
                        Console.WriteLine(startRank);
                        isStartRank = true;
                    }
                    else
                    {
                        startFile = GetFile(input[1]);
                        isStartFile = true;
                    }
                    isPiece = true;
                    
                }
            }
        }

        private int GetFile(char file)
        {
            int fileIndex;
            try
            {
                fileIndex = files.IndexOf(char.ToUpper(file));
            }
            catch (ArgumentOutOfRangeException)
            {
                fileIndex = -1;
            }
            return fileIndex + 1;
        }
        private int GetRank(char rank)
        {
            try
            {
                return Convert.ToInt32(char.GetNumericValue(rank));
            }
            catch (ArgumentException)
            {
                return 0;
            }

        }

        private char CheckPieceValid(char piece)
        {
            if (!pieces.Contains(piece))
            {
                piece = 'X';
            }
            return piece;
        }

        public (char, (int, int), int, int) ReturnInput()
        {
            if (isPiece)
            {
                if (isStartFile)
                {
                    return (piece, move, startFile, 0);
                }
                if (isStartRank)
                {
                    return (piece, move, 0, startRank);
                }
                else
                {
                    return (piece, move, 0, 0);
                }
            }
            return ('P', move, 0, 0);
        }

        public bool CheckValidInput((char, (int, int), int, int) input)
        {
            if (input.Item1 == 'X')
            {
                return false;
            }
            if (input.Item2.Item1 == 0 || input.Item2.Item2 == 0)
            {
                return false;
            }
            return true;
        }
    }
}
