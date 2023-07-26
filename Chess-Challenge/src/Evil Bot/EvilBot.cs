using ChessChallenge.API;
using System;

namespace ChessChallenge.Example
{
    public class EvilBot : IChessBot
    {
        // Bot registry
        Func<IChessBot>[] bots = {
            () => new Gen1Lookahead(),
            () => new SebBot(), // The OG EvilBot
            () => new MyBot(), // The current working bot
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
