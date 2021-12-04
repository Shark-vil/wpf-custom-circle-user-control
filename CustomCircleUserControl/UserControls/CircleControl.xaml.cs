using CustomCircleUserControl.Interfaces;
using CustomCircleUserControl.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using static CustomCircleUserControl.UserControls.TransformControl;

namespace CustomCircleUserControl.UserControls
{
    public partial class CircleControl : UserControl, ITransformElement
    {
        public CircleControlModel Context;
        public double Left
        {
            get { return GetCanvasLeft(); }
            set { Canvas.SetLeft(this, value); }
        }
        public double Top
        {
            get { return GetCanvasTop(); }
            set { Canvas.SetTop(this, value); }
        }

        public Action a_StartEditMode;
        public Action a_StopEditMode;

        private static double _startNumber = 1;
        private static CircleControl _pastCircleControl;

        public CircleControl()
        {
            InitializeComponent();
            InitializeDataContext();

            if (_pastCircleControl != null)
            {
                Width = double.IsNaN(_pastCircleControl.Width) ? Width : _pastCircleControl.Width;
                Height = double.IsNaN(_pastCircleControl.Height) ? Height : _pastCircleControl.Height;
            }

            Circle.DragStarted += DragStartedEditMode;
            _startNumber++;
            _pastCircleControl = this;
        }

        public void StartEditMode() => a_StartEditMode?.Invoke();

        public void StopEditMode() => a_StopEditMode?.Invoke();

        public double GetLeft() => Left;

        public double GetTop() => Top;

        public double GetWidth() => Width;

        public double GetHeight() => Height;

        public UIElement GetElement() => this;

        public void SetLeft(double left) => Left = left;

        public void SetTop(double top) => Top = top;

        public void SetWidth(double width) => Width = width;

        public void SetHeight(double height) => Height = height;

        public void Edit()
        {
            var transformControl = new TransformControl();
            transformControl.SetTransform(this);
            transformControl.a_Completed += (TransformModel transformModel) =>
            {
                if (transformModel.StartHorizontalOffset < transformModel.HorizontalOffset)
                    Context.TextNumber -= transformModel.HorizontalOffset;
                else
                    Context.TextNumber += transformModel.HorizontalOffset;
            };
        }

        private void InitializeDataContext()
        {
            Context = new CircleControlModel();
            Context.Color = Colors.LightGreen.ToString();
            Context.TextNumber = _startNumber;
            DataContext = Context;
        }

        private void DragStartedEditMode(object sender, DragStartedEventArgs e) => Edit();

        private double GetCanvasLeft()
        {
            double left = Canvas.GetLeft(this);
            if (double.IsNaN(left)) left = 0;
            return left;
        }

        private double GetCanvasTop()
        {
            double top = Canvas.GetTop(this);
            if (double.IsNaN(top)) top = 0;
            return top;
        }
    }
}
