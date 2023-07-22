using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    private Random rand;
    private int moveCount;

    public MyBot()
    {
        rand = new Random();
        moveCount = -1;
    }

    public Move Think(Board board, Timer timer)
    {   

        moveCount += 1;

        // always play a sensible opener if white
        // probably a waste of tokens
        if (board.IsWhiteToMove && moveCount == 0) {
            Move opener = new Move("e2e4", board);
            return opener;
        }

        Move[] moves = board.GetLegalMoves();
        IEnumerable<Move> query = moves.OrderBy(a => rand.Next());

        Move? bestMove = null;
        var bestScore = 0;
        foreach (var move in query)
        {
            var score = scoreMove(board, move);
            if (score > bestScore)
            {
                bestMove = move;
                bestScore= score;
            }
        }

        System.Console.WriteLine($"best score: {bestScore}");

        return bestMove ?? moves[0];
    }

    public int scoreMove(Board board, Move move)
    {
        board.MakeMove(move);

        var pieceLists = board.GetAllPieceLists();

        var blackScore = 0;
        var whiteScore = 0;
        foreach (var pieceList in pieceLists)
        {
            foreach (var piece in pieceList)
            {
                if (piece.IsWhite)
                {
                    whiteScore += pieceValue(piece) + rankSquare(piece.Square);
                }
                else
                {
                    
                    blackScore += pieceValue(piece) + rankSquare(piece.Square);
                }
            }
        }

        board.UndoMove(move);

        return whiteScore - blackScore;
    }

    // for now just uniformly rank the squares no matter what piece
    public int rankSquare(Square s) {
        var middle = 64 / 2;
        var dist = Math.Abs(s.Index - middle);

        return 64 - dist;
    }

    public int pieceValue(Piece p)
    {
        switch (p.PieceType)
        {
            case PieceType.Pawn:
                return 1;
            case PieceType.Bishop:
                return 3;
            case PieceType.Knight:
                return 3;
            case PieceType.Rook:
                return 5;
            case PieceType.Queen:
                return 9;
            case PieceType.King:
                return 1;
            default:
                return 0;
        }
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}
