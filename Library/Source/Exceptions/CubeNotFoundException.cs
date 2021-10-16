using System;

namespace Minesweeper3D.Library.Exceptions
{
    /// <summary>
    /// An exception that is thrown when the <see cref="Cube"/> that was being searched for in a given <see cref="MineSpace"/> was not found.
    /// </summary>
    public class CubeNotFoundException : Exception
    { }
}
