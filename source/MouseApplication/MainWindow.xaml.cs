using MouseApplication.MouseHelper;
using MouseApplication.Properties;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace MouseApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ------ WIN32 ---
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT pPoint);

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public Int32 X;
            public Int32 Y;
        };
        #endregion --- WIN32 ---

        private System.Timers.Timer? timer;
        private const int intervalTimerMs = 5000;
        private const int intervalTimerDelayMs = intervalTimerMs / 3;

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnForm1_Closing;
        }

        public void OnForm1_Load(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.MainForm_HasSetDefaults)
            {
                var location = Settings.Default.MainForm_Location;
                Application.Current.MainWindow.Left = location.X;
                Application.Current.MainWindow.Top = location.Y;
                var size = Settings.Default.MainForm_Size;
                this.Height = size.Height;
                this.Width = size.Width;
            }

            Extensions.AddClickHandler(Button2, DoMouseClick);

            timer = new System.Timers.Timer(intervalTimerMs);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_ElapsedEventHandler);
            timer.Enabled = true;
            timer.AutoReset = true;
            Debug.WriteLine("Mouse Click: enabled");
        }

        private void OnForm1_Closing(object? sender, CancelEventArgs e)
        {
            Settings.Default.MainForm_Location = new System.Drawing.Point((int)Application.Current.MainWindow.Left, (int)Application.Current.MainWindow.Top);
            Settings.Default.MainForm_Size = new System.Drawing.Size((int)this.Width, (int)this.Height);
            Settings.Default.MainForm_HasSetDefaults = true;

            Settings.Default.Save();

            Extensions.RemoveClickHandler(Button2, DoMouseClick);
        }

        public void Timer_ElapsedEventHandler(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    MouseDevice md = InputManager.Current.PrimaryMouseDevice;
                    MouseButtonEventArgs mouseEvent = new MouseButtonEventArgs(md, 0, MouseButton.Left);
                    Button1_Click(sender, mouseEvent);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Timer_ElapsedEventHandler - Exception={ex.Message}");
            }
        }

        private static Point GetMousePosition()
        {
            var w32Mouse = new POINT();
            GetCursorPos(out w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private static bool SetMousePosition(Point w32Mouse)
        {
            return SetCursorPos((int)w32Mouse.X, (int)w32Mouse.Y);
        }

        private void Button1_Click(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine($"Mouse Interval={timer?.Interval}");

            try
            {
                //Point locationFromWindow = Button2.TranslatePoint(new Point(0, 0), this);
                //Point locationFromScreen = Button2.PointToScreen(locationFromWindow);
                //Debug.WriteLine($"Button2 Position: X={locationFromScreen.X}, Y={locationFromScreen.Y}");

                this.Dispatcher.Invoke(() =>
                {
                    Point cursorPosition = GetMousePosition();
                    Debug.WriteLine($"Cursor Position: X={cursorPosition.X}, Y={cursorPosition.Y}");

                    SetMousePositionToElement(Button2);

                    //Button2_Click(sender, null);
                    //Button2DoSomeThing_Click(new object(), MouseClickEvent());
                    Button2Control_KeyDown(new object(),
                        new KeyEventArgs(Keyboard.PrimaryDevice,
                            PresentationSource.FromVisual(Button2),
                            0,
                            Key.Enter));
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Button1_Click - Exception={ex.Message}");
            }
        }

        private void Button2_Click(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine($"Mouse Clicked.");

            this.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(intervalTimerDelayMs);

                SetMousePositionToElement(Button3);
            });
        }

        private void Button3_Click(object sender, MouseButtonEventArgs e)
        {
            if (timer?.Enabled ?? false)
            {
                timer.Enabled = false;
                Debug.WriteLine("Mouse Click: disabled");
            }
        }

        public static MouseButtonEventArgs MouseClickEvent()
        {
            MouseDevice md = InputManager.Current.PrimaryMouseDevice;
            MouseButtonEventArgs mouseEvent = new MouseButtonEventArgs(md, 0, MouseButton.Left);
            return mouseEvent;
        }

        private void Button2DoSomeThing_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Button2DoSomeThing_Click: routed event");
        }

        private void DoMouseClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DoMouseClick: routed event via keyboard simulation");

            this.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(intervalTimerDelayMs);

                SetMousePositionToElement(Button3);
            });
        }

        private void Button2Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoMouseClick(sender, e);
            }
        }

        private void SetMousePositionToElement(System.Windows.Controls.Control control)
        {
            Point controlCurrentPosition = control.TransformToAncestor(this)
                .Transform(new Point(0, 0));

            Debug.WriteLine($"Control Relative Position: X={controlCurrentPosition.X}, Y={controlCurrentPosition.Y}");

            Point controlCenter = new Point(control.ActualWidth / 2, control.ActualHeight / 2);
            Point controlLocation =
                new Point(controlCurrentPosition.X + controlCenter.X,
                          controlCurrentPosition.Y + controlCenter.Y);

            Point controlPosition = PointToScreen(controlLocation);
            Debug.WriteLine($"Control Absolute Position: X={controlPosition.X}, Y={controlPosition.Y}");

            SetMousePosition(controlPosition);
        }
    }
}