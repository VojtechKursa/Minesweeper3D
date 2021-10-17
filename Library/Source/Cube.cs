namespace Minesweeper3D.Library
{
    /// <summary>
    /// Represents the base unit of the Minesweeper game. (Equivalent to the separate squares/boxes in 2D version)
    /// </summary>
    public class Cube
    {
        #region Variables and Properties

        /// <summary>
        /// Gets or sets the <see cref="CubeState"/> of the <see cref="Cube"/>.
        /// </summary>
        public CubeState State { get; set; }

        /// <summary>
        /// Gets (or sets) a value indicating whether there is a mine in the <see cref="Cube"/>.
        /// </summary>
        /// <returns>True if there is a mine on the <see cref="Cube"/>, otherwise false.</returns>
        public bool HasMine { get; protected set; }

        /// <summary>
        /// Gets (or sets) the amount of mines surrounding the <see cref="Cube"/>.
        /// </summary>
        public byte SurroundingMines { get; protected set; }

        /// <summary>
        /// Gets (or sets) the position of the <see cref="Cube"/> as an array of <see cref="int"/> in [x,y,z] format.
        /// </summary>
        public int[] Position { get; protected set; } = null;

        /// <summary>
        /// Gets the <see cref="MineSpace"/> this <see cref="Cube"/> is a part of.
        /// </summary>
        protected MineSpace Parent { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="Cube"/>.
        /// </summary>
        /// <param name="mineSpace">The parent <see cref="MineSpace"/> of this <see cref="Cube"/>. (The <see cref="MineSpace"/> this <see cref="Cube"/> will be a part of).</param>
        /// <param name="hasMine">A value indicating whether this <see cref="Cube"/> is supposed to have a mine.</param>
        public Cube(MineSpace mineSpace, bool hasMine)
        {
            Parent = mineSpace;
            HasMine = hasMine;
            State = CubeState.Covered;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Cube"/>.<br />
        /// This constructor accepts an additional value (<i>position</i>).<br />
        /// <see cref="Cube"/>s created using this constructor don't have to find out their position themselves during <see cref="Init"/>. This saves some processing power needed for that operation.
        /// </summary>
        /// <param name="mineSpace">The parent <see cref="MineSpace"/> of this <see cref="Cube"/>. (The <see cref="MineSpace"/> this <see cref="Cube"/> will be a part of).</param>
        /// <param name="hasMine">A value indicating whether this <see cref="Cube"/> is supposed to have a mine.</param>
        /// <param name="position">The <see cref="Position"/> value of the newly created <see cref="Cube"/>.</param>
        public Cube(MineSpace mineSpace, bool hasMine, int[] position) : this(mineSpace, hasMine)
        {
            Position = position;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the <see cref="Cube"/> by setting it's <see cref="Position"/> value (if it isn't already set) and <see cref="SurroundingMines"/> value.<br />
        /// This initialization should be done after the filling of the parent <see cref="MineSpace"/> is complete.<br />
        /// These values are calculated here so they are immediatelly available at any time, saving the processing power required to find out these parameters each time they are needed.
        /// </summary>
        public void Init()
        {
            if (Position == null)
                Position = Parent.GetCubePosition(this);

            SurroundingMines = Parent.GetSurroundingMines(this);
        }

        #endregion
    }
}
