# Radial Action Menu - Architecture & Component Mapping

## App Architecture Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                        Host Page/Container                       │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │                                                           │  │
│  │                     Page Content                          │  │
│  │                                                           │  │
│  │                                                           │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                  │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │              RadialActionMenu (UserControl)               │  │
│  │  ┌─────────────────────────────────────────────────────┐  │  │
│  │  │  RadialBackdrop (Border with opacity)               │  │  │
│  │  │  - Semi-transparent overlay                         │  │  │
│  │  │  - Does NOT dismiss on tap                          │  │  │
│  │  └─────────────────────────────────────────────────────┘  │  │
│  │                                                           │  │
│  │  ┌─────────────────────────────────────────────────────┐  │  │
│  │  │  MenuContainer (Canvas - absolute positioning)      │  │  │
│  │  │                                                     │  │  │
│  │  │     ○ Item3    Arc layout calculated               │  │  │
│  │  │    ╱           at runtime based on                 │  │  │
│  │  │   ○ Item2      position property                   │  │  │
│  │  │  ╱                                                  │  │  │
│  │  │ ○ Item1                                            │  │  │
│  │  │  ╲                                                  │  │  │
│  │  │   ◉ TriggerButton                                  │  │  │
│  │  │                                                     │  │  │
│  │  └─────────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Component Hierarchy

```
RadialActionMenu (UserControl)
├── Grid (Root - fills container)
│   ├── Border (Backdrop - fills grid, visibility bound to IsOpen)
│   │   └── Background: Semi-transparent black
│   │
│   └── Canvas (MenuContainer - for absolute positioning)
│       ├── RadialMenuItem[0] (Border + Button)
│       │   ├── RenderTransform: CompositeTransform
│       │   └── Content: FontIcon/SymbolIcon
│       │
│       ├── RadialMenuItem[1]
│       ├── RadialMenuItem[2]
│       ├── RadialMenuItem[3] (optional, max 4)
│       │
│       └── RadialTriggerButton (Button with custom template)
│           ├── RenderTransform: RotateTransform
│           └── Content: FontIcon (+ / ×)
```

---

## Uno Platform Component Mapping

### Core Controls

| Design Element | Uno/WinUI Control | Notes |
|----------------|-------------------|-------|
| RadialActionMenu | `UserControl` | Main container, manages state and animations |
| Backdrop | `Border` | Full-screen overlay with `Background` opacity animation |
| Menu Container | `Canvas` | Enables absolute positioning via `Canvas.Left`/`Canvas.Top` |
| Trigger Button | `Button` | Custom `ControlTemplate` with circular shape |
| Menu Item | `Button` in `Border` | Circular button with glassmorphism styling |
| Icons | `FontIcon` or `SymbolIcon` | Using Segoe Fluent Icons or custom font |

### Transforms & Animation

| Animation | Uno/WinUI API | Property |
|-----------|---------------|----------|
| Trigger Rotation | `RotateTransform` | `Angle` (0° → 135°) |
| Item Position | `CompositeTransform` | `TranslateX`, `TranslateY` |
| Item Scale | `CompositeTransform` | `ScaleX`, `ScaleY` (0 → 1) |
| Item Rotation | `CompositeTransform` | `Rotation` (-180° → 0°) |
| Backdrop Opacity | `DoubleAnimation` | `Opacity` (0 → 0.4) |

### Animation Implementation

```
Storyboard (OpenMenuStoryboard)
├── DoubleAnimation → TriggerButton.RotateTransform.Angle
│   Duration: 300ms, EasingFunction: CubicEase (EaseOut)
│
├── DoubleAnimation → Backdrop.Opacity
│   Duration: 200ms, EasingFunction: CubicEase (EaseOut)
│
├── DoubleAnimationUsingKeyFrames → Item1.CompositeTransform.TranslateX
│   └── SplineDoubleKeyFrame (spring-like via KeySpline)
│       KeyTime: 400ms, KeySpline: "0.34,1.56,0.64,1"
│
├── DoubleAnimationUsingKeyFrames → Item1.CompositeTransform.TranslateY
├── DoubleAnimationUsingKeyFrames → Item1.CompositeTransform.ScaleX
├── DoubleAnimationUsingKeyFrames → Item1.CompositeTransform.ScaleY
├── DoubleAnimationUsingKeyFrames → Item1.CompositeTransform.Rotation
│
├── (Repeat for Item2 with BeginTime offset: 40ms)
├── (Repeat for Item3 with BeginTime offset: 80ms)
└── (Repeat for Item4 with BeginTime offset: 120ms, if present)
```

---

## State Management

### Visual States

```xaml
<VisualStateManager.VisualStateGroups>
    <VisualStateGroup x:Name="OpenCloseStates">
        <VisualState x:Name="Closed">
            <!-- Default state - items hidden at center -->
        </VisualState>
        <VisualState x:Name="Open">
            <VisualState.Storyboard>
                <!-- OpenMenuStoryboard -->
            </VisualState.Storyboard>
        </VisualState>
    </VisualStateGroup>
</VisualStateManager.VisualStateGroups>
```

### Dependency Properties

```csharp
public partial class RadialActionMenu : UserControl
{
    // Items collection (max 4 enforced in setter)
    public static readonly DependencyProperty ItemsProperty;

    // Corner position
    public static readonly DependencyProperty PositionProperty;

    // Open/closed state
    public static readonly DependencyProperty IsOpenProperty;

    // Accent color for trigger and hover states
    public static readonly DependencyProperty AccentColorProperty;

    // Icon color (unified)
    public static readonly DependencyProperty IconColorProperty;

    // Distance from center to items
    public static readonly DependencyProperty RadiusProperty;

    // Arc span in degrees
    public static readonly DependencyProperty ArcSpanProperty;

    // Accessibility: skip animations
    public static readonly DependencyProperty UseReducedMotionProperty;
}
```

### Data Model

```csharp
public class RadialMenuItemData
{
    public IconElement Icon { get; set; }
    public string Label { get; set; }  // For accessibility (AutomationProperties.Name)
    public ICommand Command { get; set; }
    public object CommandParameter { get; set; }
    public bool IsEnabled { get; set; } = true;
}
```

---

## Position Calculation

Arc angles based on `Position` property:

```csharp
private (double startAngle, double endAngle) GetArcAngles()
{
    return Position switch
    {
        MenuPosition.BottomRight => (180, 270),  // Up and Left
        MenuPosition.BottomLeft => (270, 360),   // Up and Right
        MenuPosition.TopRight => (90, 180),      // Down and Left
        MenuPosition.TopLeft => (0, 90),         // Down and Right
        _ => (180, 270)
    };
}

private Point GetItemPosition(int index, int totalItems)
{
    var (startAngle, endAngle) = GetArcAngles();
    var arcSpan = endAngle - startAngle;
    var angleStep = arcSpan / (totalItems - 1);  // Distribute evenly
    var angle = startAngle + (index * angleStep);
    var radians = angle * Math.PI / 180;

    return new Point(
        Math.Cos(radians) * Radius,
        Math.Sin(radians) * Radius
    );
}
```

---

## Styling Approach

### Glassmorphism Effect for Menu Items

```xaml
<Style x:Key="RadialMenuItemStyle" TargetType="Border">
    <Setter Property="Width" Value="48"/>
    <Setter Property="Height" Value="48"/>
    <Setter Property="CornerRadius" Value="24"/>
    <Setter Property="Background">
        <Setter.Value>
            <AcrylicBrush TintColor="White"
                          TintOpacity="0.8"
                          AlwaysUseFallback="False"/>
        </Setter.Value>
    </Setter>
    <Setter Property="BorderBrush" Value="#20FFFFFF"/>
    <Setter Property="BorderThickness" Value="1"/>
</Style>
```

**Note**: `AcrylicBrush` with `AlwaysUseFallback="False"` enables backdrop blur on supported platforms. Fallback to solid white on unsupported platforms.

### Trigger Button Style

```xaml
<Style x:Key="RadialTriggerButtonStyle" TargetType="Button">
    <Setter Property="Width" Value="56"/>
    <Setter Property="Height" Value="56"/>
    <Setter Property="CornerRadius" Value="28"/>
    <Setter Property="Background" Value="{Binding AccentColor}"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Grid>
                    <Ellipse Fill="{TemplateBinding Background}">
                        <Ellipse.Shadow>
                            <ThemeShadow/>
                        </Ellipse.Shadow>
                    </Ellipse>
                    <FontIcon x:Name="TriggerIcon"
                              Glyph="&#xE710;"
                              Foreground="White"
                              FontSize="24"
                              RenderTransformOrigin="0.5,0.5">
                        <FontIcon.RenderTransform>
                            <RotateTransform x:Name="IconRotation"/>
                        </FontIcon.RenderTransform>
                    </FontIcon>
                </Grid>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

---

## File Structure

```
RadialActionMenu/
├── RadialActionMenu.xaml           # Main UserControl XAML
├── RadialActionMenu.xaml.cs        # Code-behind with logic
├── RadialMenuItemData.cs           # Data model for items
├── MenuPosition.cs                 # Enum for positions
└── Themes/
    └── RadialActionMenuStyles.xaml # Resource dictionary with styles
```

---

## Animation Timing Summary

| Animation | Duration | Easing | Stagger |
|-----------|----------|--------|---------|
| Trigger rotation | 300ms | CubicEase (EaseOut) | - |
| Backdrop fade in | 200ms | CubicEase (EaseOut) | - |
| Item spring out | 400ms | KeySpline "0.34,1.56,0.64,1" | 40ms per item |
| Item spring in (close) | 250ms | CubicEase (EaseIn) | 30ms reverse |
| Hover scale | 150ms | CubicEase (EaseOut) | - |
| Press scale | 100ms | CubicEase (EaseIn) | - |

---

## Accessibility Implementation

```xaml
<!-- On RadialActionMenu root -->
<UserControl AutomationProperties.Name="Actions menu">

<!-- On Trigger Button -->
<Button AutomationProperties.Name="{Binding IsOpen,
        Converter={StaticResource OpenCloseToLabelConverter}}"
        AutomationProperties.HelpText="Tap to show available actions">

<!-- On each Menu Item -->
<Button AutomationProperties.Name="{Binding Label}"
        AutomationProperties.LiveSetting="Polite">
```

### Reduced Motion Support

```csharp
private void ApplyAnimations()
{
    if (UseReducedMotion || !UISettings.AnimationsEnabled)
    {
        // Instant state change, no storyboard
        SetItemPositionsImmediate();
        return;
    }

    // Play full animation storyboard
    OpenMenuStoryboard.Begin();
}
```

---

## Platform Considerations

| Feature | Support | Notes |
|---------|---------|-------|
| CompositeTransform animations | All platforms | GPU-accelerated |
| AcrylicBrush (backdrop) | WASM, Skia, Windows | Fallback to solid on Android/iOS unless explicitly enabled |
| ThemeShadow | All platforms | Uno Platform cross-platform shadow |
| Storyboard/DoubleAnimation | All platforms | Full support |
| SplineDoubleKeyFrame | All platforms | For spring-like easing |
| FontIcon | All platforms | Use Segoe Fluent Icons |

---

## Resolved Questions

| Question | Decision |
|----------|----------|
| Platform target | Android only |
| AcrylicBrush | Enable blur on Android (`AlwaysUseFallback="False"`) |
| Shadow depth | Z translation = 10 |
| Icon set | Segoe Fluent Icons |
| Demo actions | Share, Edit, Delete, Favorite (4 items) |
| Container behavior | Self-positioning (Option A) - control fills parent, positions itself based on `Position` property |

## Usage Example

```xaml
<Grid>
    <PageContent />
    <local:RadialActionMenu Position="BottomRight" />
</Grid>
```
