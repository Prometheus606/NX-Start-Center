using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace NXStartCenter;

public partial class MainWindow : Window
{
    private bool _isMinimizing;
    private bool _isRestoring;

    private readonly int _animationDuration = 75;
    private readonly double _resizeFactor = 0.18;
    private readonly double _targetOpacity = 0.15;
    private readonly double _taskbarOffset = 8;

    private double _normalLeft;
    private double _normalTop;
    private double _normalWidth;
    private double _normalHeight;
    private double _oldMinWidth;
    private double _oldMinHeight;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        StateChanged += (_, _) =>
        {
            if (WindowState == WindowState.Normal && !_isMinimizing && !_isRestoring)
            {
                RestoreWithAnimation();
            }
        };
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }
        else
        {
            DragMove();
        }
    }

    //private void Minimize_Click(object sender, RoutedEventArgs e)
    //{
    //    WindowState = WindowState.Minimized;
    //}

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        if (_isMinimizing || _isRestoring)
            return;

        _isMinimizing = true;

        _normalLeft = Left;
        _normalTop = Top;
        _normalWidth = Width;
        _normalHeight = Height;

        _oldMinWidth = MinWidth;
        _oldMinHeight = MinHeight;

        MinWidth = 0;
        MinHeight = 0;

        var duration = TimeSpan.FromMilliseconds(_animationDuration);
        var ease = new QuarticEase { EasingMode = EasingMode.EaseOut };

        var targetWidth = _normalWidth * _resizeFactor;
        var targetHeight = _normalHeight * _resizeFactor;
        var targetLeft = _normalLeft + (_normalWidth - targetWidth) / 2;
        var targetTop = SystemParameters.WorkArea.Bottom - targetHeight - _taskbarOffset;

        var storyboard = new Storyboard();

        AddAnimation(storyboard, this, Window.LeftProperty, _normalLeft, targetLeft, duration, ease);
        AddAnimation(storyboard, this, Window.TopProperty, _normalTop, targetTop, duration, ease);
        AddAnimation(storyboard, this, Window.WidthProperty, _normalWidth, targetWidth, duration, ease);
        AddAnimation(storyboard, this, Window.HeightProperty, _normalHeight, targetHeight, duration, ease);
        AddAnimation(storyboard, this, Window.OpacityProperty, 1, _targetOpacity, duration, ease);

        storyboard.Completed += (_, _) =>
        {
            WindowState = WindowState.Minimized;
            _isMinimizing = false;
        };

        storyboard.Begin();
    }

    private static void AddAnimation(
        Storyboard storyboard,
        DependencyObject target,
        DependencyProperty property,
        double from,
        double to,
        TimeSpan duration,
        IEasingFunction ease)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = duration,
            EasingFunction = ease
        };

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(property));
        storyboard.Children.Add(animation);
    }

    private void RestoreWithAnimation()
    {
        _isRestoring = true;

        var duration = TimeSpan.FromMilliseconds(_animationDuration);
        var ease = new QuarticEase { EasingMode = EasingMode.EaseOut };

        var startWidth = _normalWidth * _resizeFactor;
        var startHeight = _normalHeight * _resizeFactor;
        var startLeft = _normalLeft + (_normalWidth - startWidth) / 2;
        var startTop = SystemParameters.WorkArea.Bottom - startHeight - _taskbarOffset;

        MinWidth = 0;
        MinHeight = 0;

        Left = startLeft;
        Top = startTop;
        Width = startWidth;
        Height = startHeight;
        Opacity = _targetOpacity;

        var storyboard = new Storyboard();

        AddAnimation(storyboard, this, Window.LeftProperty, startLeft, _normalLeft, duration, ease);
        AddAnimation(storyboard, this, Window.TopProperty, startTop, _normalTop, duration, ease);
        AddAnimation(storyboard, this, Window.WidthProperty, startWidth, _normalWidth, duration, ease);
        AddAnimation(storyboard, this, Window.HeightProperty, startHeight, _normalHeight, duration, ease);
        AddAnimation(storyboard, this, Window.OpacityProperty, _targetOpacity, 1, duration, ease);

        storyboard.Completed += (_, _) =>
        {
            BeginAnimation(Window.LeftProperty, null);
            BeginAnimation(Window.TopProperty, null);
            BeginAnimation(Window.WidthProperty, null);
            BeginAnimation(Window.HeightProperty, null);
            BeginAnimation(Window.OpacityProperty, null);

            MinWidth = _oldMinWidth;
            MinHeight = _oldMinHeight;

            Opacity = 1;

            _isRestoring = false;
        };

        storyboard.Begin();
    }
}