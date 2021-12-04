using System.Windows;

namespace CustomCircleUserControl.Interfaces
{
    public interface ITransformElement
    {
        public double GetLeft();
        public double GetTop();
        public double GetWidth();
        public double GetHeight();
        public void SetLeft(double left);
        public void SetTop(double top);
        public void SetWidth(double width);
        public void SetHeight(double height);
        public void StartEditMode();
        public void StopEditMode();
        public UIElement GetElement();
    }
}
