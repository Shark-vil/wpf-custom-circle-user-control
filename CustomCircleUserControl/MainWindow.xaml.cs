using CustomCircleUserControl.UserControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomCircleUserControl
{
    public partial class MainWindow : Window
    {
        private TransformControl _currentTransformControl;

        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            App.AllocConsole();
#endif

            Workingspace.MouseLeftButtonDown += Workingspace_MouseLeftButtonDown;

            TransformControl.a_StartedTransform += (TransformControl transform) =>
            {
                if (_currentTransformControl != null && _currentTransformControl != transform)
                    _currentTransformControl.StopTransform();

                Workingspace.Children.Add(transform);
                _currentTransformControl = transform;
            };

            TransformControl.a_StoppedTransform += (TransformControl transform) =>
            {
                Workingspace.Children.Remove(transform);
                _currentTransformControl = null;
            };
        }

        private void Workingspace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentTransformControl == null)
            {
                var circleControl = new CircleControl();
                circleControl.a_StartEditMode += () => Workingspace.Children.Remove(circleControl);
                circleControl.a_StopEditMode += () => Workingspace.Children.Add(circleControl);

                Canvas.SetLeft(circleControl, Workingspace.ActualWidth / 2 - circleControl.Width / 2);
                Canvas.SetTop(circleControl, Workingspace.ActualHeight / 2 - circleControl.Height / 2);

                circleControl.Edit();
            }
            else
            {
                _currentTransformControl.StopTransform();
            }
        }
    }
}
