using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Shapes;
using ChessLogic;
using ChessLogic.Moves;

// Explicitly map types to avoid conflicts
using GameState = ChessLogic.GameState;
using Image = Microsoft.Maui.Controls.Image;

namespace ChessMobile
{
    public partial class MainPage : ContentPage
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Microsoft.Maui.Controls.Shapes.Rectangle[,] highlights = new Microsoft.Maui.Controls.Shapes.Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

        private GameState gameState;
        private Position selectedPos = null;

        public MainPage()
        {
            InitializeComponent();
            InitializeBoard();

            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
        }

        private void InitializeBoard()
        {
            BoardGrid.Children.Clear();

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    // 1. Piece Image
                    Image image = new Image
                    {
                        Aspect = Aspect.AspectFit,
                        InputTransparent = true // Clicks go through to the highlight/grid
                    };
                    pieceImages[r, c] = image;
                    BoardGrid.Add(image, c, r);

                    // 2. Highlight Rectangle & Tap Area
                    var highlight = new Microsoft.Maui.Controls.Shapes.Rectangle
                    {
                        Fill = Colors.Transparent,
                        Opacity = 0.5
                    };
                    highlights[r, c] = highlight;
                    BoardGrid.Add(highlight, c, r);

                    // 3. Tap Gesture
                    var tapGesture = new TapGestureRecognizer();
                    int capturedRow = r;
                    int capturedCol = c;
                    tapGesture.Tapped += (s, e) => OnSquareTapped(capturedRow, capturedCol);

                    highlight.GestureRecognizers.Add(tapGesture);
                }
            }
        }

        private void DrawBoard(Board board)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = board[r, c];
                    pieceImages[r, c].Source = GetImageSource(piece);
                }
            }
        }

        private string GetImageSource(Piece piece)
        {
            if (piece == null) return null;

            // Generates filenames like "pawnw.png"
            string color = piece.Color == Player.White ? "w" : "b";
            string type = piece.Type.ToString().ToLower();

            return $"{type}{color}.png";
        }

        private void OnSquareTapped(int row, int col)
        {
            Position pos = new Position(row, col);

            if (selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPositionSelected(pos);
            }
        }

        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);
            if (moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighlights();
            }
        }

        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighlights();

            if (moveCache.TryGetValue(pos, out Move move))
            {
                HandleMove(move);
            }
        }

        private void HandleMove(Move move)
        {
            gameState.MakeMove(move);
            DrawBoard(gameState.Board);
        }

        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();
            foreach (Move move in moves)
            {
                moveCache[move.ToPos] = move;
            }
        }

        private void ShowHighlights()
        {
            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Colors.LightGreen;
            }
        }

        private void HideHighlights()
        {
            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Colors.Transparent;
            }
        }
    }
}