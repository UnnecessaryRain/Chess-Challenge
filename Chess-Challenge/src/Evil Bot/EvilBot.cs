using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    public class EvilBot : IChessBot
    {
        // Bot registry
        Func<IChessBot>[] bots = {
            () => new v2_0_0AlphaBeta(),
            () => new v1_0_0LookAhead(),
            () => new SebBot(), // The OG EvilBot
            () => new MyBot(), // The current working bot. Probably don't point at this one.
        };

        IChessBot bot;
        public EvilBot()
        {
            // 0 is latest bot
            int index = 0;
            bot = bots[index]();
        }
        public Move Think(Board board, Timer timer)
        {
            return bot.Think(board, timer);
        }
    }
}
