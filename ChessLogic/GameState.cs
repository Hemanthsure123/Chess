using ChessLogic.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }

        public Result Result { get; private set; } = null;  



        public GameState(Player Player, Board board)
        {
            CurrentPlayer = Player;
            Board = board;
        }

        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if(Board.IsEmpty(pos) || Board[pos].Color !=CurrentPlayer)
            {
                return Enumerable.Empty<Move>();
            }

            Piece piece=Board[pos];
            IEnumerable<Move> moveCanditates= piece.GetMoves(pos, Board);
            return moveCanditates.Where(m => m.IsLegal(Board));

        }


        public void MakeMove(Move move)
        {
            move.Execute(Board);
            CurrentPlayer = CurrentPlayer.Opponent();
        }


        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos =>
            {
                Piece piece = Board[pos];
                return piece.GetMoves(pos, Board);
            });
            return moveCandidates.Where(m => m.IsLegal(Board));
        }


        private void CheckForGameOver()
        {
            if(!AllLegalMovesFor(CurrentPlayer).Any())
            {
                if(Board.IsInCheck(CurrentPlayer))
                {
                    Result = Result.Win(CurrentPlayer.Opponent());
                }
                else
                {
                    Result = Result.Draw(EndReason.Stalemate);
                }
            });
        }
         




    }
}
