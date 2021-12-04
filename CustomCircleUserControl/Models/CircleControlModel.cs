using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomCircleUserControl.Models
{
    public class CircleControlModel : INotifyPropertyChanged
    {
        private string _text;
        private double _textNumber;
        private string _color;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            { 
                _text = value;
                OnPropertyChanged();
            }
        }

        public double TextNumber
        {
            get
            {
                if (double.IsNaN(_textNumber) || _textNumber < 0) _textNumber = 0;
                return _textNumber;
            }
            set
            {
                _textNumber = value;
                Text = Convert.ToString(_textNumber);
                OnPropertyChanged();
            }
        }

        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
