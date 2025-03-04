using System;

namespace Minesweeper3D.Library.Exceptions
{
    /// <summary>
    /// An exception that is thrown when a program tries to interact with a <see cref="Game"/> that's already over (either in <see cref="GameStatus.Lost"/> or <see cref="GameStatus.Won"/>).
    /// </summary>
    public class GameOverException : Exception
    { }
}
