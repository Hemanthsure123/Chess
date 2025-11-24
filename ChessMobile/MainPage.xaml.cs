using ChessLogic;
using ChessLogic.Moves;

namespace ChessMobile
{
    public partial class MainPage : ContentPage
    {
        // 1. Keep your existing variables
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Microsoft.Maui.Controls.Shapes.Rectangle[,] highlights = new Microsoft.Maui.Controls.Shapes.Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

        private GameState gameState;
        private Position selectedPos = null;

        public MainPage()
        {
            InitializeComponent();
            InitializeBoard();

            // Initialize Logic
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
        }

        // 2. Adapted Initialization
        // In WPF you added children manually. In MAUI, we do the same but add Tap Gestures here.
        private void InitializeBoard()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    // Create Image for the Piece
                    Image image = new Image();
                    pieceImages[r, c] = image;
                    // Add to Grid (row, col)
                    BoardGrid.Add(image, c, r);

                    // Create Rectangle for Highlight
                    var highlight = new Microsoft.Maui.Controls.Shapes.Rectangle
                    {
                        Fill = Colors.Transparent,
                        Opacity = 0.5
                    };
                    highlights[r, c] = highlight;
                    BoardGrid.Add(highlight, c, r);

                    // 3. NEW: Handle Touch Input
                    // Instead of calculating X/Y pixels like in WPF, we attach a listener to the square directly.
                    var tapGesture = new TapGestureRecognizer();
                    int capturedRow = r;
                    int capturedCol = c;
                    tapGesture.Tapped += (s, e) => OnSquareTapped(capturedRow, capturedCol);

                    // Attach gesture to the highlight rectangle (it sits on top)
                    highlight.GestureRecognizers.Add(tapGesture);
                }
            }
        }

        // 4. Adapted DrawBoard (Simpler Image Loading)
        private void DrawBoard(Board board)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = board[r, c];
                    // MAUI loads directly from filename string
                    pieceImages[r, c].Source = GetImageSource(piece);
                }
            }
        }

        // Helper to get image filename string
        private string GetImageSource(Piece piece)
        {
            if (piece == null) return null;

            // Mapping logic from your Images.cs
            string color = piece.Color == Player.White ? "W" : "B";
            string type = piece.Type.ToString();
            return $"{type}{color}.png"; // e.g., "PawnW.png"
        }

        // 5. Logic Flow (Copied from your MainWindow.cs)
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
            // Mobile doesn't have a mouse cursor, so we skip SetCursor logic
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
                highlights[to.Row, to.Column].Fill = Colors.Green; // Simple green highlight
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