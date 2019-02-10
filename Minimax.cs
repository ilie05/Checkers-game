using System;
using System.Collections.Generic;

namespace SimpleCheckers
{
    /// <summary>
    /// Implementeaza algoritmul de cautare a mutarii optime
    /// </summary>
    public class Minimax
    {
        private static Random _rand = new Random();

        public static AlphaBetaBoard AlphaBetaPruning(AlphaBetaBoard currentNode, double alfa, double beta, int depth, PlayerType player)
        {
            bool finished;
            PlayerType winner;
            currentNode.board.CheckFinish(out finished,out winner);

            if(finished || depth == 0)
                return currentNode;

            AlphaBetaBoard returnBoard = new AlphaBetaBoard();
            Piece p;
            if(player == PlayerType.Computer)
            {
                //daca sunt miscari obligatorii pentru a ataca o piesa
                List<Board> childBoards = new List<Board>(0);
                returnBoard.eval = Double.MinValue;

                List<Move> mandatoryMoves= currentNode.board.BeatingMoves(PlayerType.Computer);                            

                //daca nu sunt piese ce trebuie atacate
                if (mandatoryMoves.Count == 0)
                {
                    for (int i = 0; i < currentNode.board.Pieces.Count; i++)
                    {
                        p = currentNode.board.Pieces[i];
                        if (p.Player == PlayerType.Computer && p.X != -1 && p.Y != -1)
                        {
                            mandatoryMoves = p.ValidMoves(currentNode.board);
                            foreach (Move m in mandatoryMoves)
                                childBoards.Add(currentNode.board.MakeMove(m));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < mandatoryMoves.Count; i++)
                        childBoards.Add(currentNode.board.MakeMove(mandatoryMoves[i]));
                }

                for(int i = 0; i< childBoards.Count; i++)
                {
                    AlphaBetaBoard childAlpha = new AlphaBetaBoard();
                    childAlpha.board = childBoards[i];
                    childAlpha.eval = childBoards[i].EvaluationFunction();
                    AlphaBetaBoard newBoad = AlphaBetaPruning(childAlpha, alfa, beta, depth - 1, PlayerType.Human);

                    if (newBoad.eval > returnBoard.eval) // consideram valoarea maxima a functiei de evaluare
                    {
                        returnBoard = newBoad;
                        returnBoard.board = childBoards[i]; // aici se pastreaza copilul care duce catre cea mai buna frunza
                    }
                    if (returnBoard.eval > alfa) // alfa = max(alfa, v)
                        alfa = returnBoard.eval;

                    if (beta <= alfa)
                        break; // retezarea beta
                }                
                return returnBoard;
            }
            else
            {
                returnBoard.eval = Double.MaxValue;

                //daca sunt miscari obligatorii pentru a ataca o piesa
                List<Move> mandatoryMoves = currentNode.board.BeatingMoves(player);
                List<Board> childBoards = new List<Board>(0);

                //daca nu sunt piese ce trebuie atacate
                if (mandatoryMoves.Count == 0)
                {
                    for (int i = 0; i < currentNode.board.Pieces.Count; i++)
                    {
                        p = currentNode.board.Pieces[i];
                        if (p.Player == PlayerType.Human && p.X != -1 && p.Y != -1)
                        {
                            mandatoryMoves = p.ValidMoves(currentNode.board);
                            foreach (Move m in mandatoryMoves)
                                childBoards.Add(currentNode.board.MakeMove(m));
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < mandatoryMoves.Count; i++)
                        childBoards.Add(currentNode.board.MakeMove(mandatoryMoves[i]));
                }

                for (int i = 0; i < childBoards.Count; i++)
                {
                    AlphaBetaBoard childAlpha = new AlphaBetaBoard();
                    childAlpha.board = childBoards[i];
                    childAlpha.eval = childBoards[i].EvaluationFunction();
                    AlphaBetaBoard newBoad = AlphaBetaPruning(childAlpha, alfa, beta, depth - 1, PlayerType.Human);

                    if (newBoad.eval < returnBoard.eval) // consideram valoarea maxima a functiei de evaluare
                    {
                        returnBoard = newBoad;
                        returnBoard.board = childBoards[i]; // aici se pastreaza copilul care duce catre cea mai buna frunza
                    }
                    if (returnBoard.eval < alfa) // alfa = max(alfa, v)
                        alfa = returnBoard.eval;

                    if (beta <= alfa)
                        break; // retezarea beta
                }
                return returnBoard;
            }
        }
    }
}
