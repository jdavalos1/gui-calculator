using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DEFAULTRESULTS = "0";
        private const string NEGATIVERESULTS = "-";
        private readonly CalculatorFunctions _calc;
        private bool _textBoxWritable = true;

        public MainWindow()
        {
            _calc = new CalculatorFunctions();
            InitializeComponent();
        }

        /// <summary>
        /// Logic for when numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NumbClick(object sender, RoutedEventArgs e)
        {
            var buttonClick = (sender as Button);

            if (!_textBoxWritable || Results.Text.ToString().Equals(DEFAULTRESULTS))
                Results.Text = buttonClick.Content.ToString();
            else
                Results.Text += buttonClick.Content.ToString();
            _textBoxWritable = true;
        }

        /// <summary>
        /// Logic for when the clear button for the current number is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClearClick(object sender, RoutedEventArgs e)
        {
            Results.Text = DEFAULTRESULTS;
        }

        /// <summary>
        /// Logic for when the negative/positive button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NegativeClick(object sender, RoutedEventArgs e)
        {
            if(!Results.Text.Equals(DEFAULTRESULTS))
            {
                Results.Text = Results.Text.Contains(NEGATIVERESULTS) ? Results.Text.Remove(0, 1) : Results.Text.Insert(0, NEGATIVERESULTS);
            }
        }
        
        public void SqrtClick(object sender, RoutedEventArgs e)
        {
            if (Results.Text.Contains(NEGATIVERESULTS))
                Results.Text = "Invalid Input";
            
        }

        public void DivideClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToSingle(Results.Text);
            _calc.DivideValue(value);
            _textBoxWritable = false;
        }

        public void MultClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToSingle(Results.Text);
            _calc.MultiplyValue(value);
            _textBoxWritable = false;
        }

        public void SubClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToSingle(Results.Text);
            _calc.SubValue(value);
            _textBoxWritable = false;
        }
        public void AddClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToSingle(Results.Text);
            _calc.AddValue(value);
            _textBoxWritable = false;
        }

        public void EqualClick(object sender, RoutedEventArgs e)
        {
            var nextValue = Convert.ToSingle(Results.Text);

            if(_calc._state == CalculatorFunctions.CalcState.div )
            {
                Results.Text = "Cannot Divide By Zero";
            }

            Results.Text = _calc.Equal(nextValue).ToString();
            _textBoxWritable = false;
        }
    }

    public class CalculatorFunctions
    {
        private readonly List<Tuple<float, string>> _memory;
        public CalcState _state { get; private set; }
        public float CurrentResult { get; private set; }
        public string CurrentFuncQueue { get; private set; }

        public CalculatorFunctions()
        {
            CurrentResult = 0.0f;
            _memory = new List<Tuple<float, string>>();
            _state = 0;
        }

        public void SqrtValue(float value)
        {
            CurrentResult = Convert.ToSingle(Math.Sqrt(value));
            
        }
        public void DivideValue(float value)
        {
            if (_state == CalcState.none)
                CurrentResult = value;
            else
                CurrentResult /= value;
            _state = CalcState.div;
            CurrentFuncQueue += value.ToString() + " \u00F7 ";
        }

        public void MultiplyValue(float value)
        {
            if (_state == CalcState.none)
                CurrentResult = value;
            else
                CurrentResult *= value;
            _state = CalcState.multi;
            CurrentFuncQueue += value.ToString() + " \u00D7 ";
        }

        public void SubValue(float value)
        {
            if (_state == CalcState.non)
                CurrentResult = value;

            else
                CurrentResult -= value;

            _state = CalcState.sub;
            CurrentFuncQueue += value.ToString() + " - ";
        }

        public void AddValue(float value)
        {
            CurrentResult += value;
            _state = CalcState.add;
            CurrentFuncQueue += value.ToString() + " + ";
        }

        public float Equal(float value)
        {
            switch(_state)
            {
                case CalcState.add:
                    CurrentResult += value;
                    break;
                case CalcState.sub:
                    CurrentResult -= value;
                    break;
                case CalcState.multi:
                    CurrentResult *= value;
                    break;
                case CalcState.div:
                    CurrentResult /= value;
                    break;
                case CalcState.none:
                    break;
            }

            CurrentFuncQueue += value.ToString() + " = " + CurrentResult;

            _memory.Add(new Tuple<float, string>(CurrentResult, CurrentFuncQueue));
            CurrentFuncQueue = "";
            CurrentResult = 0.0f;
            _state = CalcState.none;
            return _memory.Last().Item1;
        }
        
        public string CurrentFunction()
        {
            return _memory.Last().Item2;
        }

        /// <summary>
        /// Perform on an operation with outliers such as inverse, sqr, sqrt, and %
        /// </summary>
        /// <param name="value"></param>
        private void outlierOperation(float value)
        {
            if (_state == CalcState.none)
            {
                CurrentResult = value;
                return;
            }
            switch (_state)
            {
                case CalcState.add:
                    CurrentResult += value;
                    break;
                case CalcState.sub:
                    CurrentResult -= value;
                    break;
                case CalcState.multi:
                    CurrentResult *= value;
                    break;
                case CalcState.div:
                    CurrentResult /= value;
                    break;
            }
            _state = CalcState.none;
        }

        /// <summary>
        /// Used to determine the state of the calculator when equal is pressed
        /// </summary>
        public enum CalcState
        {
            none,
            add,
            sub,
            div,
            multi,
        }
    }

}
