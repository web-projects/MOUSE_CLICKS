using System.Windows;
using System.Windows.Input;

namespace MouseApplication.MouseHelper
{
    public static class Extensions
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
            "Click",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(UIElement)
            );

        public static void AddClickHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement? element = d as UIElement;

            if (element is { })
            {
                element.MouseLeftButtonDown += new MouseButtonEventHandler(Element_MouseLeftButtonDown);
                element.MouseLeftButtonUp += new MouseButtonEventHandler(Element_MouseLeftButtonUp);
                element.AddHandler(Extensions.ClickEvent, handler);
            }
        }

        public static void RemoveClickHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement? element = d as UIElement;

            if (element is { })
            {
                element.MouseLeftButtonDown -= new MouseButtonEventHandler(Element_MouseLeftButtonDown);
                element.MouseLeftButtonUp -= new MouseButtonEventHandler(Element_MouseLeftButtonUp);
                element.RemoveHandler(Extensions.ClickEvent, handler);
            }
        }

        static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement? uie = sender as UIElement;

            if (uie is { })
            {
                uie.CaptureMouse();
            }
        }
        
        static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement? uie = sender as UIElement;

            if (uie is { } && uie.IsMouseCaptured)
            {
                uie.ReleaseMouseCapture();
                UIElement? element = e.OriginalSource as UIElement;

                if (element is { } && element.InputHitTest(e.GetPosition(element)) is { })
                {
                    element.RaiseEvent(new RoutedEventArgs(Extensions.ClickEvent));
                }
            }
        }
    }
}
