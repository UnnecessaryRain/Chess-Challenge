using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10 };
    bool endgame = false;

    ulong[] bonuses4BitEncoded = new ulong[]{
        // end game king
        3495870780490875728, 3712011551724954929, 3690818012361479699, 232002722662791173, 
        // pawn
        4901400205211402052, 4928665618871680836, 4928682111292473156, 4922527062430515012,
        // knight
        4662022712826569280, 4802750193488012866, 2624117187294292516, 300418803178482180,
        // bishop
        5811321330933802320, 6173590641860848213, 6182034569037654613, 385725581320215045,
        // rook
        11912109320270051925, 6148914691236560725, 6148914691236560725, 6486596357414301525,
        // queen
        10432297153236223632, 7407576949158890598, 7408368403499437158, 679040375191464969,
        // mid game king
        11357329583054717699, 13807285710405832977, 18147629851284148497, 15697706967737839664,
    };
    int[][] bonuses = new int[7][];

    int[] decode(int row)
    {
        int[] decoded = new int[64];
        for (int index = 0; index < 64; index += 4)
        {
            for (int offset = 0; offset < 4; offset++)
            {
                decoded[index + offset] = (int)((bonuses4BitEncoded[4 * row + offset] & (0xFUL << index)) >> index);
            }
        }

        return decoded;
    }

    public MyBot()
    {
        for (int i = 0; i < 7; i++)
        {
            bonuses[i] = decode(i);
        }
    }

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
                    whiteScore += 100 * pieceValue(piece) + 3 * rankPosition(piece);

                    if (!piece.IsKing && !piece.IsPawn)
                    {
                        whitePieces += 1;
                    }
                }
                else
                {
                    blackScore += 100 * pieceValue(piece) + 3 * rankPosition(piece);
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
            return bonuses[0][p.Square.Index];
        }
        return bonuses[(int)p.PieceType][p.Square.Index];
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
