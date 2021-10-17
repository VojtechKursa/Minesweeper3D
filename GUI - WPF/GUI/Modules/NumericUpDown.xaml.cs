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
        public int MinValue { get; set; }
        public int MaxValue { get; set; } = 100;

        private int value;
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

        public event EventHandler ValueChanged;

        public NumericUpDown()
        {
            InitializeComponent();

            ValueChanged += NumericUpDown_ValueChanged;
        }

        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            TB_value.Text = value.ToString();
        }

        private void B_down_Click(object sender, RoutedEventArgs e)
        {
            Value--;
        }

        private void B_up_Click(object sender, RoutedEventArgs e)
        {
            Value++;
        }

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
    }
}
