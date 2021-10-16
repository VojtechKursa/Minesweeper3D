namespace Minesweeper3D.Library
{
    /// <summary>
    /// Contains all acceptable states of a <see cref="Cube"/>.
    /// </summary>
    public enum CubeState
    {
        /// <summary>
        /// Specifies a covered (not yet uncovered) cube.
        /// </summary>
        Covered,

        /// <summary>
        /// Specifies an uncovered cube.
        /// </summary>
        Uncovered,

        /// <summary>
        /// Specifies a cube that contains a flag.
        /// </summary>
        Flagged
    }

    /// <summary>
    /// Contains all acceptable states of a <see cref="Game"/>.
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// Specifies a game that hasn't started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Specifies a game that is ongoing.
        /// </summary>
        Ongoing,

        /// <summary>
        /// Specifies a game that ended in a loss for the player (who stepped on a mine)
        /// </summary>
        Lost,

        /// <summary>
        /// Specifies a game that ended in a player's victory.
        /// </summary>
        Won
    }

    /// <summary>
    /// Contains all possible results of a <see cref="Cube"/> uncovering action.
    /// </summary>
    public enum UncoverResult
    {
        /// <summary>
        /// The uncovered <see cref="Cube"/> was mined.
        /// </summary>
        Mine,
        
        /// <summary>
        /// The uncovered <see cref="Cube"/> was clear, didn't contain any mine.
        /// </summary>
        Clear,

        /// <summary>
        /// The uncovering operation was stopped because the <see cref="Cube"/> was <see cref="CubeState.Flagged"/>.
        /// </summary>
        Flag,
        
        /// <summary>
        /// The uncovering operation was stopped because the <see cref="Cube"/> was already <see cref="Uncovered"/>.
        /// </summary>
        Uncovered
    }

    /// <summary>
    /// Contains all possible results of a <see cref="Cube"/> change-flag action.
    /// </summary>
    public enum FlagResult
    {
        /// <summary>
        /// Specifies that the <see cref="Cube"/> was successfully flagged.
        /// </summary>
        Flagged,

        /// <summary>
        /// Specifies that the <see cref="Cube"/> was successuflly unflagged.
        /// </summary>
        Unflagged,

        /// <summary>
        /// Specifies that the <see cref="Cube"/> was already <see cref="CubeState.Uncovered"/> and therefore couldn't be flagged nor unflagged.
        /// </summary>
        Uncovered
    }
}
