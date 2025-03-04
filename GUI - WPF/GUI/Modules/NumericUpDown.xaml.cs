using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper3D.WPF.GUI.Modules
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        #region Variables and Properties

        /// <summary>
        /// Gets or sets the minimum value that is acceptable by this <see cref="NumericUpDown"/>. (Default is 0)
        /// </summary>
        public int MinValue { get; set; }
        /// <summary>
        /// Gets or sets the maximum value that is acceptable by this <see cref="NumericUpDown"/>. (Default is 100)
        /// </summary>
        public int MaxValue { get; set; } = 100;

        /// <summary>
        /// Gets or sets the current value in this <see cref="NumericUpDown"/>.
        /// </summary>
        public int Value
        {
            get => value;
            set
            {
                if (value >= MinValue && value <= MaxValue)
                {
                    if (this.value != value)
                    {
                        this.value = value;
                        ValueChanged.Invoke(this, new EventArgs());
                    }
                }
            }
        }
        private int value;

        #endregion

        #region Events

        /// <summary>
        /// An event that's triggered when the <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        #endregion

        #region Constructors

        public NumericUpDown()
        {
            InitializeComponent();

            ValueChanged += NumericUpDown_ValueChanged;
        }

        #endregion

        #region Event handlers

        private void NumericUpDown_ValueChanged(object sender, EventArgs e) => TB_value.Text = value.ToString();

        private void B_down_Click(object sender, RoutedEventArgs e) => Value--;

        private void B_up_Click(object sender, RoutedEventArgs e) => Value++;

        private void TB_value_TextInput(object sender, TextCompositionEventArgs e)
        {
            bool isNumber = false;
            int number = 0;

            try
            {
                number = Convert.ToInt32(TB_value.Text);
                isNumber = true;
            }
            catch
            { }

            if (isNumber)
            {
                if (number >= MinValue && number <= MaxValue)
                {
                    if (Value != number)
                        Value = number;
                }
                else
                    TB_value.Text = Value.ToString();
            }
            else
                TB_value.Text = Value.ToString();
        }

        #endregion
    }
}
