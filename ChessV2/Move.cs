using System;
namespace ChessV2
{
    public struct Move
    {
        
        public Piece P
        {
            get; set;
        }

        public (int, int) Coords
        {
            get; set;
        }

        public int StartFile
        {
            get; set;
        }

        public int StartRank
        {
            get; set;
        }

        public bool Castle
        {
            get; set;
        }

        public bool Capture
        {
            get; set;
        }

        public Move(Piece p, (int, int) coords, int startfile, int startrank, bool capture=false, bool castle = false)
        {
            P = p;
            Coords = coords;
            StartFile = startfile;
            StartRank = startrank;
            Capture = capture;
            Castle = castle;
        }

        public override string ToString()
        {
            string files = "ABCDEFGH";
            string str = $"{P}{files[Coords.Item1 - 1]}{Coords.Item2}";
            if (Castle)
            {
                str = Coords.Item1==7 ? "O-O" : "O-O-O";
            }
            return str;
        }

        public bool EqualsInput((char, (int, int), int, int) userInput)
        {
            if (userInput.Item1 != P.CharRep)
            {
                return false;
            }
            if (userInput.Item2 != Coords)
            {
                return false;
            }
            if (userInput.Item3 != 0 && userInput.Item3 != StartFile)
            {
                return false;
            }
            if (userInput.Item4 != 0 && userInput.Item4 != StartRank)
            {
                return false;
            }
            return true;
        }
    }
}
