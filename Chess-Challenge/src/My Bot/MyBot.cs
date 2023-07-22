using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10 };

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();

        Move? bestMove = null;
        var bestScore = 0;
        foreach (var move in moves)
        {
            if (MoveIsCheckmate(board, move))
            {
                return move;
            }

            var score = scoreMove(board, move);
            if (score > bestScore)
            {
                bestMove = move;
                bestScore = score;
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
    public int rankSquare(Square s)
    {
        var middle = 64 / 2;
        var dist = Math.Abs(s.Index - middle);
        System.Console.WriteLine($"dist {dist}");
        return 64 - dist;
    }

    public int pieceValue(Piece p)
    {
        return pieceValues[(int)p.PieceType];
    }

    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}
