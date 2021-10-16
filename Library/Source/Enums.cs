namespace Minesweeper_3D_Library
{
    /// <summary>
    /// Contains all acceptable states of a <see cref="Cube"/>.
    /// </summary>
    public enum CubeState { Covered, Uncovered, Flagged }

    /// <summary>
    /// Contains all acceptable states of a <see cref="Game"/>.
    /// </summary>
    public enum GameStatus { NotStarted, Ongoing, Lost, Won }

    /// <summary>
    /// Contains all possible results of a cube uncovering action.
    /// </summary>
    public enum UncoveringResult { Mine, Clear, Flag }
}
