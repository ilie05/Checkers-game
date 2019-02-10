using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SimpleCheckers
{
    public partial class MainForm : Form
    {
        private Board _board;
        private int _selectedId; // id-ul piesei selectate
        private PlayerType _currentPlayer; // om sau calculator
        private Bitmap _boardImage;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                _boardImage = (Bitmap)Image.FromFile("board.png");
            }
            catch
            {
                MessageBox.Show("Nu se poate incarca board.png");
                Environment.Exit(1);
            }

            _board = new Board();
            _currentPlayer = PlayerType.None;
            _selectedId = -1; // nicio piesa selectata

            this.ClientSize = new System.Drawing.Size(1920 , 1080);
            this.pictureBoxBoard.Size = new System.Drawing.Size(800, 800);

            pictureBoxBoard.Refresh();
        }

        private void pictureBoxBoard_Paint(object sender, PaintEventArgs e)
        {
            Bitmap board = new Bitmap(_boardImage);
            e.Graphics.DrawImage(board, 0, 0);

            if (_board == null)
                return;

            int dy = 800 - 100 + 10;
            SolidBrush transparentRed = new SolidBrush(Color.FromArgb(200, 255, 0, 0));
            SolidBrush transparentGreen = new SolidBrush(Color.FromArgb(200, 0, 128, 0));
            SolidBrush transparentYellow = new SolidBrush(Color.FromArgb(200, 255, 255, 0));
            SolidBrush transparentBlack = new SolidBrush(Color.FromArgb(200, 0, 0, 0));

            foreach (Piece p in _board.Pieces)
            {
                SolidBrush brush = transparentRed;
                if (p.Player == PlayerType.Human)
                {
                    if (p.Id == _selectedId)
                        brush = transparentYellow;
                    else
                        brush = transparentGreen;
                }

                e.Graphics.FillEllipse(brush, 10 + p.X * 100, dy - p.Y * 100, 80, 80);

                if (p.Dama) // daca piesa este un dama, trebuie evidentiata
                {
                    brush = transparentBlack;
                    e.Graphics.FillEllipse(brush, (float)(30 + p.X * 100), (float)(dy - p.Y * 100 + 20), 40, 40);
                }
            }
        }

        private void pictureBoxBoard_MouseUp(object sender, MouseEventArgs e)
        {
            if (_currentPlayer != PlayerType.Human)
                return;

            int mouseX = e.X / 100;
            int mouseY = 7 - e.Y / 100;

            if (_selectedId == -1)
            {
                for(int i = 0; i < _board.Pieces.Count; i++)
                {
                    if(_board.Pieces[i].Player == PlayerType.Human && _board.Pieces[i].X == mouseX && _board.Pieces[i].Y == mouseY)
                    {
                        _selectedId = _board.Pieces[i].Id;
                        pictureBoxBoard.Refresh();
                        break;
                    }
                }
            }
            else
            {
                Piece selectedPiece = _board.Pieces[_selectedId];

                if (selectedPiece.X == mouseX && selectedPiece.Y == mouseY)
                {
                    _selectedId = -1;
                    pictureBoxBoard.Refresh();
                }
                else
                {
                    Move m = new Move(_selectedId, mouseX, mouseY);
                    List<Move> allBeatingMoves = _board.BeatingMoves(PlayerType.Human);

                    //if human has a piece to attack, then he must attack it
                    bool exists = false;
                    if (allBeatingMoves.Count > 0)
                    {                        
                        foreach(Move mv in allBeatingMoves)
                        {
                            if (m.NewX == mv.NewX && m.NewY == mv.NewY && m.PieceId == mv.PieceId)
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                            return;
                    }

                    if (selectedPiece.IsValidMove(_board, m))
                    {
                        Board b = _board.MakeMove(m);
                        _selectedId = -1;                        
                        AnimateTransition(_board, b);                        
                        _board = b;
                        pictureBoxBoard.Refresh();
                        _currentPlayer = PlayerType.Computer;

                        CheckFinish();

                        if (_currentPlayer == PlayerType.Computer) // jocul nu s-a terminat
                            ComputerMove();
                    }
                }
            }
        }                

        private void ComputerMove()
        {
            AlphaBetaBoard alphaBetaBoard = new AlphaBetaBoard();
            alphaBetaBoard.board = _board;
            alphaBetaBoard.eval = _board.EvaluationFunction();

            Board nextBoard = Minimax.AlphaBetaPruning(alphaBetaBoard, Double.NegativeInfinity, Double.PositiveInfinity, 5, PlayerType.Computer).board;
            AnimateTransition(_board, nextBoard);
    
            _board = nextBoard;
            pictureBoxBoard.Refresh();

            _currentPlayer = PlayerType.Human;

            CheckFinish();
        }

        private void CheckFinish()
        {
            bool end; PlayerType winner;
            _board.CheckFinish(out end, out winner);

            if (end)
            {
                if (winner == PlayerType.Computer)
                {
                    MessageBox.Show("Calculatorul a castigat!");
                    _currentPlayer = PlayerType.None;
                }
                else if (winner == PlayerType.Human)
                {
                    MessageBox.Show("Ai castigat!");
                    _currentPlayer = PlayerType.None;
                }
            }
        }

        private void AnimateTransition(Board b1, Board b2)
        {
            Bitmap board = new Bitmap(_boardImage);
            int dy = 800 - 100 + 10;
            SolidBrush transparentRed = new SolidBrush(Color.FromArgb(200, 255, 0, 0));
            SolidBrush transparentGreen = new SolidBrush(Color.FromArgb(200, 0, 128, 0));
            SolidBrush transparentBlack = new SolidBrush(Color.FromArgb(200, 0, 0, 0));

            Bitmap final = new Bitmap(800, 800);
            Graphics g = Graphics.FromImage(final);

            int noSteps = 10;

            for (int j = 1; j < noSteps; j++)
            {
                g.DrawImage(board, 0, 0);

                for (int i = 0; i < b1.Pieces.Count; i++)
                {
                    double avx = (j * b2.Pieces[i].X + (noSteps - j) * b1.Pieces[i].X) / (double)noSteps;
                    double avy = (j * b2.Pieces[i].Y + (noSteps - j) * b1.Pieces[i].Y) / (double)noSteps;

                    SolidBrush brush = transparentRed;
                    if (b1.Pieces[i].Player == PlayerType.Human)
                        brush = transparentGreen;

                    g.FillEllipse(brush, (int)(10 + avx * 100), (int)(dy - avy * 100), 80, 80);

                    if (b1.Pieces[i].Dama) // daca piesa este un dama, trebuie evidentiata
                    {
                        brush = transparentBlack;
                        g.FillEllipse(brush, (float)(30 + avx * 100), (float)(dy - avy * 100 + 20), 40, 40);
                    }
                }

                Graphics pbg = pictureBoxBoard.CreateGraphics();
                pbg.DrawImage(final, 0, 0);
            }
        }

        private void jocNouToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            _board = new Board();
            _currentPlayer = PlayerType.Computer;
            ComputerMove();
        }

        private void despreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string copyright =
                "Algoritmul minimax cu retezare alfa-beta\r\n" +
                "(c)2018-2019 Petrasco Ilie\r\n";

            MessageBox.Show(copyright, "Despre jocul Dame simple");
        }

        private void iesireToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}