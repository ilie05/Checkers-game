using System;
using System.Collections.Generic;

namespace SimpleCheckers
{
    public enum PlayerType { None, Computer, Human };

    public struct Point
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// Reprezinta o piesa de joc
    /// </summary>
    public class Piece
    {
        public int Id { get; set; } // identificatorul piesei
        public int X { get; set; } // pozitia X pe tabla de joc
        public int Y { get; set; } // pozitia Y pe tabla de joc
        public bool Dama { get; set; } // daca s-a ajuns in capatul tablei cu piesa sau nu
        public PlayerType Player { get; set; } // carui tip de jucator apartine piesa (om sau calculator)

        public Piece(int x, int y, int id, bool dama, PlayerType player)
        {
            Dama = dama;
            X = x;
            Y = y;
            Id = id;
            Player = player;
        }

        public Piece(Piece p)
        {
            Id = p.Id;
            X = p.X;
            Y = p.Y;
            Dama = p.Dama;
            Player = p.Player;
        }

        public Piece() { }

        /// <summary>
        /// Returneaza lista tuturor mutarilor permise pentru piesa curenta (this) 
        /// in configuratia (tabla de joc) primita ca parametru
        /// </summary>
        public List<Move> ValidMoves(Board currentBoard)
        {
            //there are just 4 posible direction to move
            List<Move> moves = new List<Move>(0);
            for(int x = this.X - 2; x <= this.X + 2; x++)
            {
                for(int y = this.Y -2; y <= this.Y + 2; y++)
                {
                    Move m = new Move(this.Id, x, y);
                    if (IsValidMove(currentBoard, m))            
                        moves.Add(m);
                }
            }
            
            return moves;
        }

        /// <summary>
        /// Testeaza daca o mutare este valida intr-o anumita configuratie
        /// </summary>
        public bool IsValidMove(Board currentBoard, Move move)
        {
            // out of the table
            if (move.NewX > 7 || move.NewX < 0 || move.NewY > 7 || move.NewY < 0)
                return false;

            //muta peste alta piesa sau nu a mutat nici o piesa
            foreach (Piece p in currentBoard.Pieces)               
                if (move.NewX == p.X && move.NewY == p.Y)
                    return false;
            
            if(this.Player == PlayerType.Human)
            {
                //move one position
                if ((((move.NewX == this.X - 1) && (move.NewY == this.Y - 1)) || ((move.NewX == this.X + 1) && (move.NewY == this.Y - 1))) && this.Dama)                  
                    return true;
                else if (((move.NewX == this.X - 1) && (move.NewY == this.Y + 1)) || ((move.NewX == this.X + 1) && (move.NewY == this.Y + 1)))
                    return true;

                // attack a computer piece
                else if (((move.NewX == this.X - 2) && (move.NewY == this.Y - 2)) && existsEnemyPiece(currentBoard, this.X - 1, this.Y - 1, PlayerType.Computer))
                    return true;
                else if (((move.NewX == this.X + 2) && (move.NewY == this.Y - 2)) && existsEnemyPiece(currentBoard, this.X + 1, this.Y - 1, PlayerType.Computer))
                    return true;
                else if (((move.NewX == this.X - 2) && (move.NewY == this.Y + 2)) && existsEnemyPiece(currentBoard, this.X - 1, this.Y + 1, PlayerType.Computer))
                    return true;
                else if (((move.NewX == this.X + 2) && (move.NewY == this.Y + 2)) && existsEnemyPiece(currentBoard, this.X + 1, this.Y + 1, PlayerType.Computer))
                    return true;
                else
                    return false;                                
            }
            else if (this.Player == PlayerType.Computer)
            {
                //move one position
                if (((move.NewX == this.X - 1) && (move.NewY == this.Y - 1)) || ((move.NewX == this.X + 1) && (move.NewY == this.Y - 1)))
                    return true;
                else if ((((move.NewX == this.X - 1) && (move.NewY == this.Y + 1)) || ((move.NewX == this.X + 1) && (move.NewY == this.Y + 1))) && this.Dama)
                    return true;

                // attack a human piece 
                else if (((move.NewX == this.X - 2) && (move.NewY == this.Y - 2)) && existsEnemyPiece(currentBoard, this.X - 1, this.Y - 1, PlayerType.Human))
                    return true;
                else if (((move.NewX == this.X + 2) && (move.NewY == this.Y - 2)) && existsEnemyPiece(currentBoard, this.X + 1, this.Y - 1, PlayerType.Human))
                    return true;
                else if (((move.NewX == this.X - 2) && (move.NewY == this.Y + 2)) && existsEnemyPiece(currentBoard, this.X - 1, this.Y + 1, PlayerType.Human))
                    return true;                
                else if (((move.NewX == this.X + 2) && (move.NewY == this.Y + 2)) && existsEnemyPiece(currentBoard, this.X + 1, this.Y + 1, PlayerType.Human))
                    return true;
                else
                    return false;
            }
            //n-ar trebui sa ajunga aici
            return true;
        }

        /// <summary>
        /// Returneaza o lista de 'moves' pentru piesa curenta, cu care ar ataca o piesa inamica  
        /// </summary>
        public List<Move> beatPiece(Board currentBoard /*ref List<Point> beatPiecesCoordinates*/) {

            //there are just 4 posible direction to beat a piece
            List<Move> moves = new List<Move>(0);

            if (this.Player == PlayerType.Computer)
            {
                Move m = new Move(this.Id, this.X - 2, this.Y - 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X - 1, this.Y - 1, PlayerType.Human))
                    moves.Add(m);

                m = new Move(this.Id, this.X + 2, this.Y - 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X + 1, this.Y - 1, PlayerType.Human))
                    moves.Add(m);
                          
                m = new Move(this.Id, this.X + 2, this.Y + 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X + 1, this.Y + 1, PlayerType.Human))
                    moves.Add(m);

                m = new Move(this.Id, this.X - 2, this.Y + 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X - 1, this.Y + 1, PlayerType.Human))
                    moves.Add(m);
                
            }
            else if(this.Player == PlayerType.Human)
            {
                Move m = new Move(this.Id, this.X - 2, this.Y - 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X - 1, this.Y - 1, PlayerType.Computer))
                    moves.Add(m);

                m = new Move(this.Id, this.X + 2, this.Y - 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X + 1, this.Y - 1, PlayerType.Computer))
                    moves.Add(m);

                m = new Move(this.Id, this.X + 2, this.Y + 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X + 1, this.Y + 1, PlayerType.Computer))
                    moves.Add(m);

                m = new Move(this.Id, this.X - 2, this.Y + 2);
                if (IsValidMove(currentBoard, m) && existsEnemyPiece(currentBoard, this.X - 1, this.Y + 1, PlayerType.Computer))
                    moves.Add(m);
            }

            return moves;
        }

        /// <summary>
        /// testeaza daca exista o piesa inamica pe o anumita pozitie
        /// </summary>
        public static bool existsEnemyPiece(Board currentBoard, int posX, int posY, PlayerType player)
        {
            foreach(Piece p in currentBoard.Pieces)
                if(p.Player == player && posX == p.X && posY == p.Y)
                    return true;

            return false;
        }        
    }

}