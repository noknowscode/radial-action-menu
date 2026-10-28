using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

namespace RadialActionMenuDemo.Controls;

public sealed partial class RadialActionMenu : UserControl
{
    private const double TriggerSize = 56;
    private const double ItemSize = 48;
    private const double DefaultRadius = 90;
    private const double DefaultMargin = 24;
    private const int MaxItems = 5;
    private const int StaggerDelayMs = 50;
    private const int OpenDurationMs = 500;
    private const int CloseDurationMs = 300;

    private readonly Border[] _menuItemBorders;
    private readonly CompositeTransform[] _menuItemTransforms;
    private readonly Button[] _menuItemButtons;
    private readonly FontIcon[] _menuItemIcons;

    private bool _isOpen;
    private Storyboard? _currentStoryboard;

    public RadialActionMenu()
    {
        InitializeComponent();

        _menuItemBorders = new[] { MenuItem0, MenuItem1, MenuItem2, MenuItem3, MenuItem4 };
        _menuItemTransforms = new[] { MenuItem0Transform, MenuItem1Transform, MenuItem2Transform, MenuItem3Transform, MenuItem4Transform };
        _menuItemButtons = new[] { MenuItem0Button, MenuItem1Button, MenuItem2Button, MenuItem3Button, MenuItem4Button };
        _menuItemIcons = new[] { MenuItem0Icon, MenuItem1Icon, MenuItem2Icon, MenuItem3Icon, MenuItem4Icon };

        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items),
        typeof(ObservableCollection<RadialMenuItemData>),
        typeof(RadialActionMenu),
        new PropertyMetadata(null, OnItemsChanged));

    public ObservableCollection<RadialMenuItemData> Items
    {
        get => (ObservableCollection<RadialMenuItemData>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        nameof(Position),
        typeof(MenuPosition),
        typeof(RadialActionMenu),
        new PropertyMetadata(MenuPosition.BottomRight, OnPositionChanged));

    public MenuPosition Position
    {
        get => (MenuPosition)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen),
        typeof(bool),
        typeof(RadialActionMenu),
        new PropertyMetadata(false, OnIsOpenChanged));

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public static readonly DependencyProperty AccentColorProperty = DependencyProperty.Register(
        nameof(AccentColor),
        typeof(Brush),
        typeof(RadialActionMenu),
        new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 31, 31, 31)), OnAccentColorChanged));

    public Brush AccentColor
    {
        get => (Brush)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public static readonly DependencyProperty IconColorProperty = DependencyProperty.Register(
        nameof(IconColor),
        typeof(Brush),
        typeof(RadialActionMenu),
        new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 31, 31, 31)), OnIconColorChanged));

    public Brush IconColor
    {
        get => (Brush)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
        nameof(Radius),
        typeof(double),
        typeof(RadialActionMenu),
        new PropertyMetadata(DefaultRadius));

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public event EventHandler? Opening;
    public event EventHandler? Opened;
    public event EventHandler? Closing;
    public event EventHandler? Closed;
    public event EventHandler<RadialMenuItemData>? ItemSelected;

    public void Open()
    {
        if (!_isOpen)
        {
            IsOpen = true;
        }
    }

    public void Close()
    {
        if (_isOpen)
        {
            IsOpen = false;
        }
    }

    public void Toggle()
    {
        IsOpen = !IsOpen;
    }

    private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RadialActionMenu menu)
        {
            menu.UpdateMenuItems();
        }
    }

    private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RadialActionMenu menu)
        {
            menu.UpdateLayout();
        }
    }

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RadialActionMenu menu)
        {
            var isOpen = (bool)e.NewValue;
            if (isOpen)
            {
                menu.PlayOpenAnimation();
            }
            else
            {
                menu.PlayCloseAnimation();
            }
        }
    }

    private static void OnAccentColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RadialActionMenu menu && e.NewValue is Brush brush)
        {
            menu.TriggerBorder.Background = brush;
        }
    }

    private static void OnIconColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RadialActionMenu menu && e.NewValue is Brush brush)
        {
            foreach (var icon in menu._menuItemIcons)
            {
                icon.Foreground = brush;
            }
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateLayout();
        UpdateMenuItems();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateLayout();
    }

    private void OnTriggerClick(object sender, RoutedEventArgs e)
    {
        Toggle();
    }

    private void OnMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            var index = Array.IndexOf(_menuItemButtons, button);
            if (index >= 0 && Items != null && index < Items.Count)
            {
                var item = Items[index];
                ItemSelected?.Invoke(this, item);

                if (item.Command?.CanExecute(item.CommandParameter) == true)
                {
                    item.Command.Execute(item.CommandParameter);
                }

                Close();
            }
        }
    }

    private new void UpdateLayout()
    {
        if (ActualWidth <= 0 || ActualHeight <= 0) return;

        var triggerX = Position switch
        {
            MenuPosition.BottomRight or MenuPosition.TopRight => ActualWidth - TriggerSize - DefaultMargin,
            MenuPosition.BottomLeft or MenuPosition.TopLeft => DefaultMargin,
            MenuPosition.Center => (ActualWidth - TriggerSize) / 2,
            _ => ActualWidth - TriggerSize - DefaultMargin
        };

        var triggerY = Position switch
        {
            MenuPosition.BottomRight or MenuPosition.BottomLeft => ActualHeight - TriggerSize - DefaultMargin,
            MenuPosition.Center => (ActualHeight - TriggerSize) / 2,
            MenuPosition.TopRight or MenuPosition.TopLeft => DefaultMargin,
            _ => ActualHeight - TriggerSize - DefaultMargin
        };

        Canvas.SetLeft(TriggerBorder, triggerX);
        Canvas.SetTop(TriggerBorder, triggerY);

        Canvas.SetLeft(PulseRing, triggerX);
        Canvas.SetTop(PulseRing, triggerY);

        var centerX = triggerX + (TriggerSize - ItemSize) / 2;
        var centerY = triggerY + (TriggerSize - ItemSize) / 2;

        foreach (var border in _menuItemBorders)
        {
            Canvas.SetLeft(border, centerX);
            Canvas.SetTop(border, centerY);
        }
    }

    private void UpdateMenuItems()
    {
        if (Items == null) return;

        var count = Math.Min(Items.Count, MaxItems);

        for (int i = 0; i < MaxItems; i++)
        {
            var isVisible = i < count;
            _menuItemBorders[i].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;

            if (isVisible && i < Items.Count)
            {
                var item = Items[i];

                if (!string.IsNullOrEmpty(item.Glyph))
                {
                    _menuItemIcons[i].Glyph = item.Glyph;
                }

                if (!string.IsNullOrEmpty(item.Label))
                {
                    AutomationProperties.SetName(_menuItemButtons[i], item.Label);
                }

                _menuItemButtons[i].IsEnabled = item.IsEnabled;
            }
        }
    }

    private (double startAngle, double endAngle) GetArcAngles()
    {
        return Position switch
        {
            MenuPosition.BottomRight => (180, 270),
            MenuPosition.BottomLeft => (270, 360),
            MenuPosition.TopRight => (90, 180),
            MenuPosition.TopLeft => (0, 90),
            MenuPosition.Center => (225, 315),
            _ => (180, 270)
        };
    }

    private Point GetItemOffset(int index, int totalItems)
    {
        if (totalItems <= 1) return new Point(0, -Radius);

        var (startAngle, endAngle) = GetArcAngles();
        var arcSpan = endAngle - startAngle;
        var angleStep = arcSpan / (totalItems - 1);
        var angle = startAngle + (index * angleStep);
        var radians = angle * Math.PI / 180;

        return new Point(
            Math.Cos(radians) * Radius,
            Math.Sin(radians) * Radius
        );
    }

    private void PlayOpenAnimation()
    {
        _currentStoryboard?.Stop();
        _isOpen = true;
        Opening?.Invoke(this, EventArgs.Empty);

        var storyboard = new Storyboard();
        var itemCount = Items != null ? Math.Min(Items.Count, MaxItems) : 0;

        var pulseScaleX = new DoubleAnimation
        {
            From = 1,
            To = 2.5,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(pulseScaleX, PulseRingScale);
        Storyboard.SetTargetProperty(pulseScaleX, "ScaleX");
        storyboard.Children.Add(pulseScaleX);

        var pulseScaleY = new DoubleAnimation
        {
            From = 1,
            To = 2.5,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(pulseScaleY, PulseRingScale);
        Storyboard.SetTargetProperty(pulseScaleY, "ScaleY");
        storyboard.Children.Add(pulseScaleY);

        var pulseOpacity = new DoubleAnimation
        {
            From = 0.8,
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(pulseOpacity, PulseRing);
        Storyboard.SetTargetProperty(pulseOpacity, "Opacity");
        storyboard.Children.Add(pulseOpacity);

        var backdropAnimation = new DoubleAnimation
        {
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(250)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(backdropAnimation, Backdrop);
        Storyboard.SetTargetProperty(backdropAnimation, "Opacity");
        storyboard.Children.Add(backdropAnimation);

        var triggerRotation = new DoubleAnimation
        {
            To = 135,
            Duration = new Duration(TimeSpan.FromMilliseconds(350)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };
        Storyboard.SetTarget(triggerRotation, PlusIconRotation);
        Storyboard.SetTargetProperty(triggerRotation, "Angle");
        storyboard.Children.Add(triggerRotation);

        var triggerScaleDown = new DoubleAnimation
        {
            To = 0.9,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(triggerScaleDown, TriggerTransform);
        Storyboard.SetTargetProperty(triggerScaleDown, "ScaleX");
        storyboard.Children.Add(triggerScaleDown);

        var triggerScaleDownY = new DoubleAnimation
        {
            To = 0.9,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(triggerScaleDownY, TriggerTransform);
        Storyboard.SetTargetProperty(triggerScaleDownY, "ScaleY");
        storyboard.Children.Add(triggerScaleDownY);

        for (int i = 0; i < itemCount; i++)
        {
            var offset = GetItemOffset(i, itemCount);
            var delay = TimeSpan.FromMilliseconds(i * StaggerDelayMs);
            var duration = TimeSpan.FromMilliseconds(OpenDurationMs);

            _menuItemBorders[i].IsHitTestVisible = true;

            var opacityAnim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(opacityAnim, _menuItemBorders[i]);
            Storyboard.SetTargetProperty(opacityAnim, "Opacity");
            storyboard.Children.Add(opacityAnim);

            var translateXAnim = new DoubleAnimation
            {
                From = 0,
                To = offset.X,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 5 }
            };
            Storyboard.SetTarget(translateXAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(translateXAnim, "TranslateX");
            storyboard.Children.Add(translateXAnim);

            var translateYAnim = new DoubleAnimation
            {
                From = 0,
                To = offset.Y,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 5 }
            };
            Storyboard.SetTarget(translateYAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(translateYAnim, "TranslateY");
            storyboard.Children.Add(translateYAnim);

            var scaleXAnim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 6 }
            };
            Storyboard.SetTarget(scaleXAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(scaleXAnim, "ScaleX");
            storyboard.Children.Add(scaleXAnim);

            var scaleYAnim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 6 }
            };
            Storyboard.SetTarget(scaleYAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(scaleYAnim, "ScaleY");
            storyboard.Children.Add(scaleYAnim);

            var rotationAnim = new DoubleAnimation
            {
                From = -180,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(400)),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(rotationAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(rotationAnim, "Rotation");
            storyboard.Children.Add(rotationAnim);
        }

        storyboard.Completed += (s, e) =>
        {
            Backdrop.IsHitTestVisible = false;
            Opened?.Invoke(this, EventArgs.Empty);
        };

        _currentStoryboard = storyboard;
        storyboard.Begin();
    }

    private void PlayCloseAnimation()
    {
        _currentStoryboard?.Stop();
        _isOpen = false;
        Closing?.Invoke(this, EventArgs.Empty);

        var storyboard = new Storyboard();
        var itemCount = Items != null ? Math.Min(Items.Count, MaxItems) : 0;

        var backdropAnimation = new DoubleAnimation
        {
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(backdropAnimation, Backdrop);
        Storyboard.SetTargetProperty(backdropAnimation, "Opacity");
        storyboard.Children.Add(backdropAnimation);

        var triggerRotation = new DoubleAnimation
        {
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };
        Storyboard.SetTarget(triggerRotation, PlusIconRotation);
        Storyboard.SetTargetProperty(triggerRotation, "Angle");
        storyboard.Children.Add(triggerRotation);

        var triggerScaleUp = new DoubleAnimation
        {
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(triggerScaleUp, TriggerTransform);
        Storyboard.SetTargetProperty(triggerScaleUp, "ScaleX");
        storyboard.Children.Add(triggerScaleUp);

        var triggerScaleUpY = new DoubleAnimation
        {
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(triggerScaleUpY, TriggerTransform);
        Storyboard.SetTargetProperty(triggerScaleUpY, "ScaleY");
        storyboard.Children.Add(triggerScaleUpY);

        for (int i = 0; i < itemCount; i++)
        {
            var reverseIndex = itemCount - 1 - i;
            var delay = TimeSpan.FromMilliseconds(reverseIndex * 30);
            var duration = TimeSpan.FromMilliseconds(CloseDurationMs);

            _menuItemBorders[i].IsHitTestVisible = false;

            var opacityAnim = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(opacityAnim, _menuItemBorders[i]);
            Storyboard.SetTargetProperty(opacityAnim, "Opacity");
            storyboard.Children.Add(opacityAnim);

            var translateXAnim = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(translateXAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(translateXAnim, "TranslateX");
            storyboard.Children.Add(translateXAnim);

            var translateYAnim = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(translateYAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(translateYAnim, "TranslateY");
            storyboard.Children.Add(translateYAnim);

            var scaleXAnim = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(scaleXAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(scaleXAnim, "ScaleX");
            storyboard.Children.Add(scaleXAnim);

            var scaleYAnim = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(duration),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(scaleYAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(scaleYAnim, "ScaleY");
            storyboard.Children.Add(scaleYAnim);

            var rotationAnim = new DoubleAnimation
            {
                To = -180,
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                BeginTime = delay,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(rotationAnim, _menuItemTransforms[i]);
            Storyboard.SetTargetProperty(rotationAnim, "Rotation");
            storyboard.Children.Add(rotationAnim);
        }

        storyboard.Completed += (s, e) =>
        {
            Backdrop.IsHitTestVisible = false;
            Closed?.Invoke(this, EventArgs.Empty);
        };

        _currentStoryboard = storyboard;
        storyboard.Begin();
    }
}
