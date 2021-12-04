using CustomCircleUserControl.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace CustomCircleUserControl.UserControls
{
    [ContentProperty(nameof(Children))]
    public partial class TransformControl : UserControl
    {
        public class TransformModel
        {
            public double StartHorizontalOffset;
            public double StartVerticalOffset;
            public double HorizontalOffset;
            public double VerticalOffset;
        }

        public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(
            nameof(Children),
            typeof(UIElementCollection),
            typeof(TransformControl),
            new PropertyMetadata());

        public UIElementCollection Children
        {
            get { return (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty); }
            private set { SetValue(ChildrenProperty, value); }
        }

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

        public static Action<TransformControl> a_StartedTransform;
        public static Action<TransformControl> a_StoppedTransform;
        public Action<TransformModel> a_Completed;

        private enum SizePointType
        {
            RightUp = 0,
            RightDown = 1,
            LeftUp = 2,
            LeftDown = 3,
        }

        private const double _minSize = 50;
        private Vector _currentPosition;
        private Vector _currentSize;
        private double _horizontalOffset;
        private double _verticalOffset;
        private double _horizontalOffsetStart;
        private double _verticalOffsetStart;
        private ITransformElement _currentTransformElement;
        private SizePointType _currentSizeType;

        public TransformControl()
        {
            InitializeComponent();

            Children = ChildrenBlock.Children;

            RegisterResizeControllersEvents();
            RegisterPivoteEvents();
        }

        public void SetTransform(ITransformElement transformElement)
        {
            Width = transformElement.GetWidth() + 30;
            Height = transformElement.GetHeight() + 30;
            Canvas.SetLeft(this, transformElement.GetLeft() - 15);
            Canvas.SetTop(this, transformElement.GetTop() - 15);
            _currentTransformElement = transformElement;
            _currentTransformElement.StartEditMode();
            Children.Add(transformElement.GetElement());
            a_StartedTransform?.Invoke(this);
        }

        public void StopTransform()
        {
            Children.Remove(_currentTransformElement.GetElement());
            _currentTransformElement.SetWidth(Width - 30);
            _currentTransformElement.SetHeight(Height - 30);
            _currentTransformElement.SetLeft(Left + 15);
            _currentTransformElement.SetTop(Top + 15);
            _currentTransformElement.StopEditMode();
            a_StoppedTransform?.Invoke(this);
        }

        private void RegisterPivoteEvents()
        {
            Pivot.DragDelta += Circle_DragDelta;
        }

        private void RegisterResizeControllersEvents()
        {
            RightUpPivot.DragStarted += (object sender, DragStartedEventArgs e) =>
            {
                _currentSizeType = SizePointType.RightUp;
                DragStarted(sender, e);
            };
            RightUpPivot.DragDelta += DragDelta;
            RightUpPivot.DragCompleted += DragCompleted;

            RightDownPivot.DragStarted += (object sender, DragStartedEventArgs e) =>
            {
                _currentSizeType = SizePointType.RightDown;
                DragStarted(sender, e);
            };
            RightDownPivot.DragDelta += DragDelta;
            RightDownPivot.DragCompleted += DragCompleted;

            LeftUpPivot.DragStarted += (object sender, DragStartedEventArgs e) =>
            {
                _currentSizeType = SizePointType.LeftUp;
                DragStarted(sender, e);
            };
            LeftUpPivot.DragDelta += DragDelta;
            LeftUpPivot.DragCompleted += DragCompleted;

            LeftDownPivot.DragStarted += (object sender, DragStartedEventArgs e) =>
            {
                _currentSizeType = SizePointType.LeftDown;
                DragStarted(sender, e);
            };
            LeftDownPivot.DragDelta += DragDelta;
            LeftDownPivot.DragCompleted += DragCompleted;
        }

        private Point GetMouse(object sender) => Mouse.GetPosition(Window.GetWindow((DependencyObject)sender));

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

        private void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var transformModel = new TransformModel();
            transformModel.StartHorizontalOffset = _horizontalOffsetStart;
            transformModel.StartVerticalOffset = _verticalOffsetStart;
            transformModel.HorizontalOffset = _horizontalOffset;
            transformModel.VerticalOffset = _verticalOffset;
            a_Completed?.Invoke(transformModel);
        }

        private void DragStarted(object sender, DragStartedEventArgs e)
        {
            Point mousePoint = GetMouse(sender);
            _horizontalOffsetStart = mousePoint.X;
            _verticalOffsetStart = mousePoint.Y;
            _currentPosition = new Vector(Left, Top);
            _currentSize = new Vector(Width, Height);
        }

        private void Circle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left += e.HorizontalChange;
            Top += e.VerticalChange;
        }

        private void DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point mousePoint = GetMouse(sender);

            if (_currentSizeType == SizePointType.LeftUp || _currentSizeType == SizePointType.LeftDown)
            {
                _horizontalOffset = _horizontalOffsetStart - mousePoint.X;
                _verticalOffset = _verticalOffsetStart - mousePoint.Y;
            }
            else
            {
                _horizontalOffset = mousePoint.X - _horizontalOffsetStart;
                _verticalOffset = mousePoint.Y - _verticalOffsetStart;
            }

            Resize();
        }

        private void Resize()
        {
            double currentWidth = _currentSize.X;
            double currentHeight = _currentSize.Y;

            if (_currentSizeType == SizePointType.RightUp)
            {
                Left = _currentPosition.X;
                Top = _currentPosition.Y - _horizontalOffset;
            }
            else if (_currentSizeType == SizePointType.RightDown)
            {
                Left = _currentPosition.X;
                Top = _currentPosition.Y;
            }
            else if (_currentSizeType == SizePointType.LeftUp)
            {
                Left = _currentPosition.X - _horizontalOffset;
                Top = _currentPosition.Y - _horizontalOffset;
            }
            else if (_currentSizeType == SizePointType.LeftDown)
            {
                Left = _currentPosition.X - _horizontalOffset;
                Top = _currentPosition.Y;
            }

            double newWidth = currentWidth + _horizontalOffset;
            double newHeight = currentHeight + _horizontalOffset;

            Width = newWidth > _minSize ? newWidth : _minSize;
            Height = newHeight > _minSize ? newHeight : _minSize;
        }
    }
}
