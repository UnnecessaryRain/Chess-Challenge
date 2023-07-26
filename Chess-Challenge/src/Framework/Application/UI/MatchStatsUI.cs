using Raylib_cs;
using System.Numerics;
using System;

namespace ChessChallenge.Application
{
    public static class MatchStatsUI
    {
        public static void DrawMatchStats(ChallengeController controller)
        {
            if (controller.PlayerWhite.IsBot && controller.PlayerBlack.IsBot)
            {
                int nameFontSize = UIHelper.ScaleInt(40);
                int regularFontSize = UIHelper.ScaleInt(35);
                int headerFontSize = UIHelper.ScaleInt(45);
                Color col = new(180, 180, 180, 255);
                Vector2 startPos = UIHelper.Scale(new Vector2(1500, 250));
                float spacingY = UIHelper.Scale(35);

                DrawNextText($"Game {controller.CurrGameNumber} of {controller.TotalGameCount}", headerFontSize, Color.WHITE);
                startPos.Y += spacingY * 2;

                DrawStats(controller.BotStatsA);
                startPos.Y += spacingY * 2;
                DrawStats(controller.BotStatsB);
           

                void DrawStats(ChallengeController.BotMatchStats stats)
                {
                    float tournamentScore = stats.NumWins + ((float)stats.NumDraws / 2f);
                    int totalPossibleScore = stats.NumWins + stats.NumDraws + stats.NumLosses;
                    float winPercentage = 100f * tournamentScore / totalPossibleScore;

                    Color statusCol = Color.YELLOW;
                    if (winPercentage < 40) {
                        statusCol = Color.RED;
                    } else if (winPercentage > 60) {
                        statusCol = Color.GREEN;
                    }

                    DrawNextText(stats.BotName + ":", nameFontSize, Color.WHITE);
                    DrawNextText($"Tournament Score: {tournamentScore} / {totalPossibleScore} ({(int)winPercentage}%)", regularFontSize, statusCol);
                    DrawNextText($"Score: W = {stats.NumWins} | D = {stats.NumDraws} | L = {stats.NumLosses}", regularFontSize, col);
                    DrawNextText($"Num Timeouts: {stats.NumTimeouts}", regularFontSize, col);
                    DrawNextText($"Num Illegal Moves: {stats.NumIllegalMoves}", regularFontSize, col);
                }
           
                void DrawNextText(string text, int fontSize, Color col)
                {
                    UIHelper.DrawText(text, startPos, fontSize, 1, col);
                    startPos.Y += spacingY;
                }
            }
        }
    }
}
