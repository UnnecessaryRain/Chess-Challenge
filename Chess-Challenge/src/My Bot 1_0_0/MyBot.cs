using ChessChallenge.API;
using System;
using System.Linq;
using System.Collections.Generic;

// MyBot Gen 1.0.0

public class MyBot1_0_0 : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 1, 3, 3, 5, 9, 10 };
    bool endgame = false;

    // encoded bonus tables into 4 bits each. basically a hex digit a square
    ulong[] bonuses4BitEncoded = {
        3679493948365249331, 3698354910480559923, 3702348336692815923, 3679759771450551091,
        8324507638868382896, 8616961243766044211, 8464402909655370870, 8536705626098334261,
        8473989754189755749, 8694967273287121481, 5173134702987290256, 7388325181603460980,
        9611370395500739034, 8468302649965992571, 3991708199661706410, 5189894196857322940,
        5076230116626220019, 3784562331441488326, 2695551116230764234, 610086452657121240,
        12049722898641358096, 6879907284540823885, 15828030931128108700, 12799320452428683447,
        4785078954081792, 282578821623040, 281475268729856, 281475269655040,
        11004222269478774916, 10779853804781800327, 6456644324026062922, 5362553090199083784,
        5883060895017956964, 3842300325061347153, 8157825962269506357, 3965485853230777350,
        6072354615139658957, 2901786392607235531, 11040369202050022847, 533702091712601245,
        5855652187812918181, 1306750580650650009, 3679855179141196665, 5761018906897562,
        5959888243263190896, 8473444396761344948, 7388624318532209559, 3997660986565438327,
    };
    int[][] bonuses = new int[12][];

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

    public MyBot1_0_0()
    {
        for (int i = 0; i < 12; i++)
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

        if (board.IsInCheckmate())
        {
            board.UndoMove(move);
            return int.MaxValue;
        }

        var pieceLists = board.GetAllPieceLists();
        int[] scores = { 0, 0 };
        int[] pieceCounts = { 0, 0 };

        foreach (var pieceList in pieceLists)
        {
            foreach (var piece in pieceList)
            {
                var colorIndex = piece.IsWhite ? 0 : 1;
                scores[colorIndex] += 100 * pieceValues[(int)piece.PieceType] + 3 * rankPosition(piece);
                if (!piece.IsKing && !piece.IsPawn)
                {
                    pieceCounts[colorIndex]++;
                }
            }
        }

        board.UndoMove(move);

        if (pieceCounts[0] <= 3 || pieceCounts[1] <= 3)
        {
            endgame = true;
        }

        var moveIndex = board.IsWhiteToMove ? 0 : 1;
        return scores[moveIndex] - scores[1 - moveIndex];
    }

    int rankPosition(Piece p)
    {
        var endGameIndex = endgame ? 6 : 0;
        return bonuses[endGameIndex + (int)p.PieceType - 1][p.Square.Index];
    }
}
