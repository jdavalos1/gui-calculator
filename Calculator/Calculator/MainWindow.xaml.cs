using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Transactions;
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
        /// Clear all current results and operations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClearEverythingClick(object sender, RoutedEventArgs e)
        {
            _calc.ClearCurrentOperations();
            // Would need to clear out a potential text box with current equation
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
            if(Convert.ToSingle(Results.Text) != 0.0f)
            {
                Results.Text = Results.Text.Contains(NEGATIVERESULTS) ? Results.Text.Remove(0, 1) : Results.Text.Insert(0, NEGATIVERESULTS);
            }
        }
        
        public void PercentClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToSingle(Results.Text);
            _calc.PercentValue(value);
            Results.Text = _calc.CurrentResult.ToString();
            _textBoxWritable = false;
        }
        
        public void SqrtClick(object sender, RoutedEventArgs e)
        {
            if (Results.Text.Contains(NEGATIVERESULTS))
                Results.Text = "Invalid Input";
            else
            {
                _calc.SqrtValue(Convert.ToSingle(Results.Text));
                Results.Text = _calc.CurrentResult.ToString();
                _textBoxWritable = false;
            }
        }

        public void BasicOprations(object sender, RoutedEventArgs e)
        {
            var state = (CalculatorFunctions.CalcState)(sender as Button).CommandParameter;
            var value = Convert.ToSingle(Results.Text);

            // Check if we are dividing by zero when new button is pressed
            if (_calc.State == CalculatorFunctions.CalcState.div && value == 0.0f)
            {
                Results.Text = "Cannot Divide By Zero";
                _calc.ClearCurrentOperations();
                _textBoxWritable = false;
                return;
            }

            _calc.OperatePreviousResults(value, state);
            Results.Text = _calc.CurrentResult.ToString();
            _textBoxWritable = false;
        }

        public void EqualClick(object sender, RoutedEventArgs e)
        {
            var nextValue = Convert.ToSingle(Results.Text);

            if (_calc.State == CalculatorFunctions.CalcState.div && nextValue == 0.0f)
            {
                Results.Text = "Cannot Divide By Zero";
                _calc.ClearCurrentOperations();
                _textBoxWritable = false;
                return;
            }

            Results.Text = _calc.Equal(nextValue).ToString();
            _textBoxWritable = false;
        }
        
    }

    public class CalculatorFunctions
    {
        private readonly List<Tuple<float, string>> _memory;
        public CalcState State { get; private set; }
        public float CurrentResult { get; private set; }
        public string CurrentFuncQueue { get; private set; }

        public CalculatorFunctions()
        {
            CurrentResult = 0.0f;
            _memory = new List<Tuple<float, string>>();
            State = 0;
        }
       
        /// <summary>
        /// Operate on the previous result with the new value inputted.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="newState"></param>
        public void OperatePreviousResults(float value, CalcState newState)
        {
            // If there was a previous operation then complete that first
            // e.g.  5 x --> 5 x 4 + we need to first do 5 x 4 then prepare
            // for the new state
            // if there was no previous state then we need to prepare the start
            // of the new state
            switch(State)
            {
                case CalcState.add:
                    AddValue(value);
                    State = newState;
                    break;
                case CalcState.sub:
                    SubValue(value);
                    State = newState;
                    break;
                case CalcState.multi:
                    MultiplyValue(value);
                    State = newState;
                    break;
                case CalcState.div:
                    DivideValue(value);
                    State = newState;
                    break;
                case CalcState.none:
                    switch(newState)
                    {
                        case CalcState.add:
                            AddValue(value);
                            break;
                        case CalcState.sub:
                            SubValue(value);
                            break;
                        case CalcState.multi:
                            MultiplyValue(value);
                            break;
                        case CalcState.div:
                            DivideValue(value);
                            break;
                        case CalcState.outlier:
                            CurrentResult = value;
                            State = newState;
                            break;
                    }      
                    break;
                case CalcState.outlier:
                    State = newState;
                    break;
            }
        }

        public void ClearCurrentOperations()
        {
            CurrentResult = 0.0f;
            State = CalcState.none;
            CurrentFuncQueue = "";
        }

        public void PercentValue(float value)
        {
            var percentage = CurrentResult * value;
            OutlierOperation(percentage);
        }

        public void SqrtValue(float value)
        {
            var sqrtValue = Convert.ToSingle(Math.Sqrt(value));
            OperatePreviousResults(sqrtValue, CalcState.outlier);
        }
        
        public void SquareValue(float value)
        {
            var sqValue = Convert.ToSingle(Math.Pow(value, 2));
            OutlierOperation(sqValue);
        }

        public void DivideValue(float value)
        {
            if (State == CalcState.none)
                CurrentResult = value;
            else
                CurrentResult /= value;
            State = CalcState.div;
            CurrentFuncQueue += value.ToString() + " \u00F7 ";
        }

        public void MultiplyValue(float value)
        {
            if (State == CalcState.none)
                CurrentResult = value;
            else
                CurrentResult *= value;
            State = CalcState.multi;
            CurrentFuncQueue += value.ToString() + " \u00D7 ";
        }

        public void SubValue(float value)
        {
            if (State == CalcState.none)
                CurrentResult = value;

            else
                CurrentResult -= value;

            State = CalcState.sub;
            CurrentFuncQueue += value.ToString() + " - ";
        }

        public void AddValue(float value)
        {
            CurrentResult += value;
            State = CalcState.add;
            CurrentFuncQueue += value.ToString() + " + ";
        }

        public float Equal(float value) 
        {
            OperatePreviousResults(value, CalcState.none);
            switch (State)
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
            State = CalcState.none;
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
        private void OutlierOperation(float value)
        {
            switch (State)
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
                    CurrentResult = value;
                    break;
            }
            State = CalcState.none;
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
            outlier
        }
    }

}
