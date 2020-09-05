using System;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Represents constants in the code
        private const string DEFAULTRESULTS = "0";
        private const string NEGATIVERESULTS = "-";
        private const string DECIMALRESULTS = ".";
        private const int MAXIMUMRESULTSLENGTH = 15;
        // Calculator class to handle calculations
        private readonly CalculatorFunctions _calc;
        // Variable to determine if the Results box can be written too from scratch
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
            if (Results.Text.Length + 1 > MAXIMUMRESULTSLENGTH) return;
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
            CurrentEquation.Text = "";
            Results.Text = DEFAULTRESULTS;
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
        /// Logic for when back button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BackspaceClick(object sender, RoutedEventArgs e)
        {
            if (Results.Text.Equals(DEFAULTRESULTS)) return;
            Results.Text = Results.Text.Remove(Results.Text.Length - 1, 1);

            if (string.IsNullOrEmpty(Results.Text))
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
        
        /// <summary>
        /// Logic for when percentage button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PercentClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToSingle(Results.Text) * 0.01f;
            _calc.PercentValue(value);
            Results.Text = _calc.CurrentResult.ToString();
            CurrentEquation.Text = _calc.CurrentFuncQueue;
            _textBoxWritable = false;
        }
        
        /// <summary>
        /// Logic for when square root button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SqrtClick(object sender, RoutedEventArgs e)
        {
            if (Results.Text.Contains(NEGATIVERESULTS))
            {
                Results.Text = "Invalid Input";
                _calc.ClearCurrentOperations();
                _textBoxWritable = false;
            }
            else
            {
                _calc.SqrtValue(Convert.ToSingle(Results.Text));
                Results.Text = _calc.CurrentResult.ToString();
                CurrentEquation.Text = _calc.CurrentFuncQueue;
                _textBoxWritable = false;
            }
        }

        /// <summary>
        /// Logic for when square button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SqClick(object sender, RoutedEventArgs e)
        {
            var value = Convert.ToSingle(Results.Text);
            _calc.SquareValue(value);
            Results.Text = _calc.CurrentResult.ToString();
            CurrentEquation.Text = _calc.CurrentFuncQueue;
            _textBoxWritable = false;
        }

        /// <summary>
        /// Logic for when inverse button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InverseClick(object sender, RoutedEventArgs e)
        {
            var rValue = Convert.ToSingle(Results.Text);

            if (rValue == 0.0f)
            {
                Results.Text = "Cannot Divide By Zero";
                _calc.ClearCurrentOperations();
                _textBoxWritable = false;

            }
            else
            {
                _calc.InverseValue(rValue);
                Results.Text = _calc.CurrentResult.ToString();
                CurrentEquation.Text = _calc.CurrentFuncQueue;
                _textBoxWritable = false;
            }
        }

        /// <summary>
        /// Logic for when a basic operation button is pressed (e.g. + - / *)
        /// </summary>
        /// <param name="sender">Contains the parameter for the state of the calculator</param>
        /// <param name="e"></param>
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
            }
            else
            {
                _calc.OperatePreviousResults(value, state);
                Results.Text = _calc.CurrentResult.ToString();
                CurrentEquation.Text = _calc.CurrentFuncQueue;
                _textBoxWritable = false;
            }
        }

        /// <summary>
        /// Logic for when a decimal number is required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DecimalClick(object sender, RoutedEventArgs e)
        {
            var currentResults = Results.Text;

            if(currentResults.Contains(DECIMALRESULTS)) return;

            Results.Text += DECIMALRESULTS;
        }

        /// <summary>
        /// Logic for when the end of an equation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EqualClick(object sender, RoutedEventArgs e)
        {
            var nextValue = Convert.ToSingle(Results.Text);

            if(_calc.State == CalculatorFunctions.CalcState.none) return;
            if (_calc.State == CalculatorFunctions.CalcState.div && nextValue == 0.0f)
                Results.Text = "Cannot Divide By Zero";
            else
            {
                var eqResult = _calc.Equal(nextValue);
                Results.Text = eqResult.ToString();
                CurrentEquation.Text = _calc.CurrentFuncQueue;
                HistoryView.Items.Add(new ResultsWrapper(_calc.CurrentFuncQueue + eqResult.ToString()));
            }
            _calc.ClearCurrentOperations();
            _textBoxWritable = false;
        }
    }

    /// <summary>
    /// Wrapper class for strings in order for simple use of ListView
    /// </summary>
    public class ResultsWrapper
    {
        public ResultsWrapper(string s)
        {
            Results = s;
        }
        public string Results { get; }
    }

    /// <summary>
    /// Representing a calculator and its functionality
    /// </summary>
    public class CalculatorFunctions
    {
        // Used as the current state of the calculator
        public CalcState State { get; private set; }
        // Used to hold the current results
        public float CurrentResult { get; private set; }
        // Used as the current function being used
        public string CurrentFuncQueue { get; private set; }
        // Current string housing current operations on a single value
        private string CurrentVar;

        public CalculatorFunctions()
        {
            CurrentResult = 0.0f;
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
                    CurrentFuncQueue += CurrentVar;
                    CurrentVar = "";
                    AddSign(newState);
                    State = newState;
                   break;
                case CalcState.sub:
                    SubValue(value);
                    CurrentFuncQueue += CurrentVar;
                    CurrentVar = "";
                    AddSign(newState);
                    State = newState;
                    break;
                case CalcState.multi:
                    MultiplyValue(value);
                    CurrentFuncQueue += CurrentVar;
                    CurrentVar = "";
                    AddSign(newState);
                    State = newState;
                    break;
                case CalcState.div:
                    DivideValue(value);
                    CurrentFuncQueue += CurrentVar;
                    CurrentVar = "";
                    AddSign(newState);
                    State = newState;
                    break;
                case CalcState.none:
                    switch(newState)
                    {
                        case CalcState.add:
                            AddValue(value);
                            CurrentFuncQueue += CurrentVar + " + ";
                            CurrentVar = "";
                            break;
                        case CalcState.sub:
                            SubValue(value);
                            CurrentFuncQueue += CurrentVar + " - ";
                            CurrentVar = "";
                            break;
                        case CalcState.multi:
                            MultiplyValue(value);
                            CurrentFuncQueue += CurrentVar + " \u2715 ";
                            CurrentVar = "";
                            break;
                        case CalcState.div:
                            DivideValue(value);
                            CurrentFuncQueue += CurrentVar + " \u00F7 ";
                            CurrentVar = "";
                            break;
                        case CalcState.outlier:
                            CurrentResult = value;
                            CurrentFuncQueue += CurrentVar;
                            State = newState;
                            break;
                    }      
                    break;
                case CalcState.outlier:
                    State = newState;
                    if (newState != CalcState.outlier && newState != CalcState.none)
                    {
                        CurrentFuncQueue += CurrentVar;
                        AddSign(State);
                        CurrentVar = "";
                    }
                    else
                    {
                        CurrentResult = value;
                        CurrentFuncQueue = CurrentVar;
                    }
                    break;
            }
        }

        /// <summary>
        /// Appends a sign to the end of the current function queue.
        /// </summary>
        /// <param name="newState">Enum for the new state of the calculator</param>
        private void AddSign(CalcState newState)
        {
            switch(newState)
            {
                case CalcState.add:
                    CurrentFuncQueue += " + ";
                    break;
                case CalcState.sub:
                    CurrentFuncQueue += " - ";
                    break;
                case CalcState.multi:
                    CurrentFuncQueue += " \u2715 ";
                    break;
                case CalcState.div:
                    CurrentFuncQueue += " \u00F7 ";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Clears all current operations in the queue
        /// </summary>
        public void ClearCurrentOperations()
        {
            CurrentResult = 0.0f;
            State = CalcState.none;
            CurrentFuncQueue = "";
            CurrentVar = "";
        }

        /// <summary>
        /// Takes the inverse of the value then performs the previously called operation on it
        /// </summary>
        /// <param name="value">Value to apply inverse</param>
        public void InverseValue(float value)
        {
            var inverseValue = 1 / value;
            // Need to see if we are using value or applying non operations to a value
            // in order to properly write equation
            if (State == CalcState.outlier)
                CurrentVar = "1/(" + CurrentVar + ")";
            else
                CurrentVar = "1/(" + value + ")";
            OperatePreviousResults(inverseValue, CalcState.outlier);
        }
        
        /// <summary>
        /// Takes the percentage of the previous operations and does the operation needed
        /// </summary>
        /// <param name="value"></param>
        public void PercentValue(float value)
        {
            var percentage = CurrentResult * value;
            CurrentVar = percentage.ToString();
            OperatePreviousResults(percentage, CalcState.outlier);
        }

        /// <summary>
        /// Takes the square root of the value passed in
        /// </summary>
        /// <param name="value"></param>
        public void SqrtValue(float value)
        {
            var sqrtValue = Convert.ToSingle(Math.Sqrt(value));
            // Need to see if we are using value or applying non operations to a value
            // in order to properly write equation
            if(State == CalcState.outlier)
                CurrentVar = "sqrt(" + CurrentVar + ")";
            else
                CurrentVar = "sqrt(" + value + ")";
            OperatePreviousResults(sqrtValue, CalcState.outlier);
        }
        
        /// <summary>
        /// Takes the square of the value passed in
        /// </summary>
        /// <param name="value"></param>
        public void SquareValue(float value)
        {
            var sqValue = Convert.ToSingle(Math.Pow(value, 2));
            if (State == CalcState.outlier)
                CurrentVar = "sqr(" + CurrentVar + ")";
            else
                CurrentVar = "sqr(" + value + ")";
            OperatePreviousResults(sqValue, CalcState.outlier);
        }

        /// <summary>
        /// Divides the previous operation's results with the value passed in
        /// </summary>
        /// <param name="value"></param>
        public void DivideValue(float value)
        {
            if (State == CalcState.none)
                CurrentResult = value;
            else
                CurrentResult /= value;

            if (string.IsNullOrEmpty(CurrentVar))
                CurrentVar = value.ToString();
            State = CalcState.div;
        }

        /// <summary>
        /// Multiply previous operation's results with the value passed in
        /// </summary>
        /// <param name="value"></param>
        public void MultiplyValue(float value)
        {
            if (State == CalcState.none)
                CurrentResult = value;
            else
                CurrentResult *= value;

            if (string.IsNullOrEmpty(CurrentVar))
                CurrentVar = value.ToString();
            State = CalcState.multi;
        }

        /// <summary>
        /// Subtracts the previous operation's results with the value passed in
        /// </summary>
        /// <param name="value"></param>
        public void SubValue(float value)
        {
            if (State == CalcState.none)
                CurrentResult = value;

            else
                CurrentResult -= value;

            if (string.IsNullOrEmpty(CurrentVar))
                CurrentVar = value.ToString();
            State = CalcState.sub;
        }

        /// <summary>
        /// Adds the previous operation's results with the value passed in
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(float value)
        {
            CurrentResult += value;
            if(string.IsNullOrEmpty(CurrentVar))
                CurrentVar = value.ToString();
            State = CalcState.add;
        }

        /// <summary>
        /// Operates on the previous results with a new value based on the last 
        /// state of the calculator
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public float Equal(float value) 
        {
            
            OperatePreviousResults(value, CalcState.none);
            CurrentFuncQueue += " = ";

            return CurrentResult;
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
