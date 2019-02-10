using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCheckers
{
    /// <summary>
    /// Reprezinta o configuratie a jocului (o tabla de joc) la un moment dat
    /// </summary>
    public class Board
    {
        public int Size { get; set; } // dimensiunea tablei de joc
        public List<Piece> Pieces { get; set; } // lista de piese, atat ale omului cat si ale calculatorului
        public int piecesHuman;
        public int piecesComputer;

        public Board()
        {
            Size = 8;
            piecesHuman = 12;
            piecesComputer = 12;
            Pieces = new List<Piece>(24);            
            int k = 0;
            for (int i = 0; i < Size; i++)
            {
                for(int j = Size-1; j >= Size-3; j--)
                {
                    if((i+j)%2 == 1)
                    {
                        Pieces.Add(new Piece(i, j, k++, false, PlayerType.Computer));
                    }
                }

                for (int j = 0; j < 3; j++)
                {
                    if ((i + j) % 2 == 1)
                    {
                        Pieces.Add(new Piece(i, j, k++, false, PlayerType.Human));
                    }
                }
            }                
        }

        public Board(Board b)
        {
            Size = b.Size;
            Pieces = new List<Piece>(24);
            piecesComputer = b.piecesComputer;
            piecesHuman = b.piecesHuman;

            foreach (Piece p in b.Pieces)
                Pieces.Add(new Piece(p.X, p.Y, p.Id, p.Dama, p.Player));
        }

        /// <summary>
        /// Calculeaza functia de evaluare statica pentru configuratia (tabla) curenta
        /// </summary>
        public double EvaluationFunction()
        {
            double simpleConst = 5.0, damaConst = 7.5;
            double evalValueCumputer = 0, evalValueHuman = 0;

            foreach (Piece p in Pieces)
            {
                if (p.Player == PlayerType.Human && p.X != -1 && p.Y != -1)
                    //simple piece; not dama
                    if (!p.Dama)
                        evalValueHuman += simpleConst;
                    //dama piece
                    else
                        evalValueHuman += damaConst;
                    //almost dama for simple piece
                    if (p.Y == 1 && !p.Dama)
                        evalValueHuman += 12.5;
                    //move to almost dama
                    if (p.Y == 2 && !p.Dama)
                        evalValueHuman += 10;
                    //is in danger
                    if (isDangerous(p))
                        evalValueHuman -= 30;
                else if (p.Player == PlayerType.Computer && p.X != -1 && p.Y != -1)
                {
                    //simple piece; not dama
                    if (!p.Dama)
                        evalValueCumputer += simpleConst;
                    //dama piece
                    else
                        evalValueCumputer += damaConst;

                    //almost dama for simple piece
                    if (p.Y == 1 && !p.Dama)
                        evalValueCumputer += 12.5;

                    //move to almost dama
                    if (p.Y == 2 && !p.Dama)
                        evalValueCumputer += 10;

                    //is in danger
                    if (isDangerous(p))
                        evalValueCumputer -= 30;
                }
            }

            return evalValueCumputer - evalValueHuman;
        }

        /// <summary>
        /// Creaza o noua configuratie aplicand mutarea primita ca parametru in configuratia curenta
        /// </summary>
        public Board MakeMove(Move move)
        {
            Board nextBoard = new Board(this); // copy

            Piece piece = nextBoard.Pieces[move.PieceId]; // current Piece

            
            //testam daca este o mutare de atac si stergem piesa atacata in caz ca DA
            if ((move.NewX == piece.X - 2) && (move.NewY == piece.Y - 2))
            {
                foreach(Piece p in nextBoard.Pieces)
                {
                    if (p.X == piece.X - 1 && p.Y == piece.Y - 1)
                    {
                        p.X = -1;
                        p.Y = -1;
                        if (piece.Player == PlayerType.Human)
                            nextBoard.piecesComputer--;
                        else
                            nextBoard.piecesHuman--;
                    }
                }
            }
            else if ((move.NewX == piece.X + 2) && (move.NewY == piece.Y - 2)) 
            {
                foreach (Piece p in nextBoard.Pieces)
                {
                    if (p.X == piece.X + 1 && p.Y == piece.Y - 1)
                    {
                        p.X = -1;
                        p.Y = -1;
                        if (piece.Player == PlayerType.Human)
                            nextBoard.piecesComputer--;
                        else
                            nextBoard.piecesHuman--;
                    }
                }
            }
            else if ((move.NewX == piece.X - 2) && (move.NewY == piece.Y + 2)) 
            {
                foreach (Piece p in nextBoard.Pieces)
                {
                    if (p.X == piece.X - 1 && p.Y == piece.Y + 1)
                    {
                        p.X = -1;
                        p.Y = -1;
                        if (piece.Player == PlayerType.Human)
                            nextBoard.piecesComputer--;
                        else
                            nextBoard.piecesHuman--;
                    }
                }
            }
            else if ((move.NewX == piece.X + 2) && (move.NewY == piece.Y + 2)) 
            {
                foreach (Piece p in nextBoard.Pieces)
                {
                    if (p.X == piece.X + 1 && p.Y == piece.Y + 1)
                    {
                        p.X = -1;
                        p.Y = -1;
                        if (piece.Player == PlayerType.Human)
                            nextBoard.piecesComputer--;
                        else
                            nextBoard.piecesHuman--;
                    }
                }
            }

            nextBoard.Pieces[move.PieceId].X = move.NewX;
            nextBoard.Pieces[move.PieceId].Y = move.NewY;

            //give to piece the 'dama' status
            if(nextBoard.Pieces[move.PieceId].Player == PlayerType.Human && move.NewY == 7)
            {
                nextBoard.Pieces[move.PieceId].Dama = true;
                this.Pieces[move.PieceId].Dama = true;
            }
            if (nextBoard.Pieces[move.PieceId].Player == PlayerType.Computer && move.NewY == 0)
            {
                nextBoard.Pieces[move.PieceId].Dama = true;
                this.Pieces[move.PieceId].Dama = true;
            }
            return nextBoard;
        }
        

        /// <summary>
        /// Verifica daca configuratia curenta este castigatoare
        /// </summary>
        /// <param name="finished">Este true daca cineva a castigat si false altfel</param>
        /// <param name="winner">Cine a castigat: omul sau calculatorul</param>
        public void CheckFinish(out bool finished, out PlayerType winner)
        {
            if (this.piecesHuman == 0)
            {
                finished = true;
                winner = PlayerType.Computer;
                return;
            }

            if (this.piecesComputer == 0)
            {
                finished = true;
                winner = PlayerType.Human;
                return;
            }

            finished = false;
            winner = PlayerType.None;
        }

        /// <summary>
        /// returneaza o lista de posibile mutari de atac pentru toate piesele unui anumit jucator
        /// </summary>
        public List<Move> BeatingMoves(PlayerType player)
        {
            List<Move> moves = new List<Move>(0);

            for(int i = 0; i< this.Pieces.Count; i++)
            {
                if(this.Pieces[i].Player == player && this.Pieces[i].X != -1 && this.Pieces[i].Y != -1)
                {                    
                    moves.AddRange(this.Pieces[i].beatPiece(this));
                }                
            }
            return moves;
        }

        /// <summary>
        /// testeaza daca mutarea pune piesa sub bataie
        /// </summary>
        /// 
        private bool isDangerous(Piece p)
        {
            if (p.Player == PlayerType.Computer)
            {
                //check first diagonal
                if (Piece.existsEnemyPiece(this, p.X - 1, p.Y + 1, PlayerType.Human) && !Piece.existsEnemyPiece(this, p.X + 1, p.Y - 1, PlayerType.Human))
                    return true;
                if (!Piece.existsEnemyPiece(this, p.X - 1, p.Y + 1, PlayerType.Human) && Piece.existsEnemyPiece(this, p.X + 1, p.Y - 1, PlayerType.Human))
                    return true;

                //check second diagonal
                if (!Piece.existsEnemyPiece(this, p.X + 1, p.Y + 1, PlayerType.Human) && Piece.existsEnemyPiece(this, p.X - 1, p.Y - 1, PlayerType.Human))
                    return true;
                if (Piece.existsEnemyPiece(this, p.X + 1, p.Y + 1, PlayerType.Human) && !Piece.existsEnemyPiece(this, p.X - 1, p.Y - 1, PlayerType.Human))
                    return true;
            }
            else
            {
                //check first diagonal
                if (Piece.existsEnemyPiece(this, p.X - 1, p.Y + 1, PlayerType.Computer) && !Piece.existsEnemyPiece(this, p.X + 1, p.Y - 1, PlayerType.Computer))
                    return true;
                if (!Piece.existsEnemyPiece(this, p.X - 1, p.Y + 1, PlayerType.Computer) && Piece.existsEnemyPiece(this, p.X + 1, p.Y - 1, PlayerType.Computer))
                    return true;

                //check second diagonal
                if (!Piece.existsEnemyPiece(this, p.X + 1, p.Y + 1, PlayerType.Computer) && Piece.existsEnemyPiece(this, p.X - 1, p.Y - 1, PlayerType.Computer))
                    return true;
                if (Piece.existsEnemyPiece(this, p.X + 1, p.Y + 1, PlayerType.Computer) && !Piece.existsEnemyPiece(this, p.X - 1, p.Y - 1, PlayerType.Computer))
                    return true;
            }

            return false;

        }
    }
}