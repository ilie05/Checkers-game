namespace SimpleCheckers
{
    /// <summary>
    /// Reprezinta mutarea unei singure piese
    /// </summary>
    public class Move
    {
        public int PieceId { get; set; } // id-ul piesei mutate
        public int NewX { get; set; } // noua pozitie X
        public int NewY { get; set; } // noua pozitie Y

        public Move(int pieceId, int newX, int newY)
        {
            PieceId = pieceId;
            NewX = newX;
            NewY = newY;
        }
    }
}