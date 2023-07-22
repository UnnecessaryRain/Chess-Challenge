using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10 };
    bool endgame = false;

    // todo: encode into bit array of either 8 or 4 bit values
    // we only need 2 ^ 7 for all values in this table. we can compress the array into a 8 64bit ints
    // less if we use less resolution. e.g. 4 bits for 0 - 16 instead of -50 to 50
    // this would mean 4 uint64 to store a pieces table, rather than 64 ints
    // see: https://github.com/UnnecessaryRain/Chess-Challenge/blob/util/compress-tables.py
    int[,] bonuses = {
        {
            // king end game
            -50,-40,-30,-20,-20,-30,-40,-50,
            -30,-20,-10,  0,  0,-10,-20,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-30,  0,  0,  0,  0,-30,-30,
            -50,-30,-30,-30,-30,-30,-30,-50
        },
        // pawn
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
            5,  5, 10, 25, 25, 10,  5,  5,
            0,  0,  0, 20, 20,  0,  0,  0,
            5, -5,-10,  0,  0,-10, -5,  5,
            5, 10, 10,-20,-20, 10, 10,  5,
            0,  0,  0,  0,  0,  0,  0,  0
        },
        // knight
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50,
        },
        {
            // bishop
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
        },
        // rook
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            5, 10, 10, 10, 10, 10, 10,  5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            0,  0,  0,  5,  5,  0,  0,  0
        },
        // queen
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -5,  0,  5,  5,  5,  5,  0, -5,
            0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        },
        // king
        {
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
            20, 20,  0,  0,  0,  0, 20, 20,
            20, 30, 10,  0,  0, 10, 30, 20
        },

    };

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();

        Move? bestMove = null;
        var bestScore = int.MinValue;
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

    int scoreMove(Board board, Move move)
    {
        board.MakeMove(move);

        var pieceLists = board.GetAllPieceLists();

        var blackScore = 0;
        var whiteScore = 0;

        var blackPieces = 0;
        var whitePieces = 0;

        foreach (var pieceList in pieceLists)
        {
            foreach (var piece in pieceList)
            {
                if (piece.IsWhite)
                {
                    whiteScore += 100 * pieceValue(piece) + rankPosition(piece);

                    if (!piece.IsKing && !piece.IsPawn)
                    {
                        whitePieces += 1;
                    }
                }
                else
                {
                    blackScore += 100 * pieceValue(piece) + rankPosition(piece);
                    if (!piece.IsKing && !piece.IsPawn)
                    {
                        blackPieces += 1;
                    }
                }
            }
        }

        board.UndoMove(move);

        if (whitePieces <= 3 || blackPieces <= 3)
        {
            endgame = true;
        }

        if (board.IsWhiteToMove)
        {
            return whiteScore - blackScore;
        }
        else
        {
            return blackScore - whiteScore;
        }
    }

    int rankPosition(Piece p)
    {
        if (p.IsKing && endgame)
        {
            return bonuses[0, p.Square.Index];
        }
        return bonuses[(int)p.PieceType, p.Square.Index];
    }

    int pieceValue(Piece p)
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
