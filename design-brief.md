# Radial Action Menu - Design Brief

## Overview

A floating action button (FAB) that expands into a radial menu of action items. Items spring outward in an arc with rotation and bounce physics, creating a playful, engaging interaction pattern.

---

## Visual Design

### Tone & Aesthetic
- **Style**: Playful geometric with glassmorphism
- **Mood**: Light, airy, modern, delightful
- **Differentiator**: Spring physics with rotation and bounce on expansion

### Reference Analysis
The provided reference shows:
- Circular action buttons arranged in a radial pattern
- White/frosted glass backgrounds with subtle shadows
- Purple/violet icon colors
- A primary trigger button with coral/orange accent and close (×) icon
- Dark blurred background suggesting overlay/modal behavior

---

## Theme

### Mode
- Primary: Light theme implementation
- Background: Subtle warm gradient or neutral surface
- Glassmorphism: Frosted glass effect on menu items

### Elevation
- Menu items appear elevated above content
- Backdrop blur/dim when menu is open
- Layered shadows for depth perception

---

## Color Palette

### Primary Colors
| Role | Color | Usage |
|------|-------|-------|
| Accent (Trigger) | `#F97316` (Coral/Orange) | Main FAB background, active states |
| Accent Ring | `#FBBF24` (Amber) | Focus ring on trigger button |
| Icon Default | `#7C3AED` (Violet) | Icons in menu items |

### Surface Colors
| Role | Color | Usage |
|------|-------|-------|
| Item Background | `#FFFFFF` @ 80% opacity | Menu item circles |
| Item Background Hover | `#FFFFFF` @ 95% opacity | Hover state |
| Backdrop | `#000000` @ 40% opacity | Background overlay when open |

### Semantic Colors
| Role | Color | Usage |
|------|-------|-------|
| Close Icon | `#FFFFFF` | × icon on trigger when open |
| Shadow | `#000000` @ 15% opacity | Drop shadows |

---

## Typography

### Tooltips (if implemented)
| Element | Font | Size | Weight | Color |
|---------|------|------|--------|-------|
| Tooltip Label | System/Segoe UI | 12sp | Medium (500) | `#1F2937` |

*Note: Primary interaction is icon-based; tooltips are optional accessibility enhancement.*

---

## Spacing & Dimensions

### Trigger Button (FAB)
| Property | Value |
|----------|-------|
| Diameter | 56dp |
| Icon Size | 24dp |
| Touch Target | 56dp minimum |

### Menu Items
| Property | Value |
|----------|-------|
| Diameter | 48dp |
| Icon Size | 20dp |
| Touch Target | 48dp minimum |

### Layout
| Property | Value |
|----------|-------|
| Radial Distance | 80-100dp from center |
| Arc Span | 90° (quarter circle) |
| Item Spacing | Evenly distributed within arc |
| Margin from Edge | 16-24dp |

### Focus Ring
| Property | Value |
|----------|-------|
| Width | 3dp |
| Offset | 4dp from button edge |
| Color | Amber (`#FBBF24`) |

---

## Layout

### Positioning
- **Default Position**: Bottom-right corner of container
- **Configurable**: Bottom-left, top-right, top-left
- **Arc Direction**: Expands away from nearest corner

### Arc Angles by Position
| Position | Start Angle | End Angle | Direction |
|----------|-------------|-----------|-----------|
| Bottom-Right | 180° | 270° | Up and Left |
| Bottom-Left | 270° | 360° | Up and Right |
| Top-Right | 90° | 180° | Down and Left |
| Top-Left | 0° | 90° | Down and Right |

### Responsive Behavior
- Maintains fixed dimensions across screen sizes
- Position adjusts to stay within safe area
- On very small screens, consider reducing radial distance

---

## Components

### 1. RadialActionMenu (Container)
- Manages open/closed state
- Handles backdrop
- Positions trigger and menu items
- Orchestrates animations

### 2. RadialTriggerButton
- Primary FAB that toggles menu
- Displays + icon when closed, × when open
- Rotates during state transition
- Has coral/orange background

### 3. RadialMenuItem
- Individual action button
- Circular with glassmorphism effect
- Contains single icon
- Supports custom accent color per item

### 4. RadialBackdrop
- Semi-transparent overlay
- Blur effect on background content
- Dismisses menu on tap

### 5. RadialTooltip (Optional)
- Label that appears on hover/long-press
- Positioned adjacent to menu item
- Arrow pointing to item

---

## States

### Trigger Button States
| State | Visual |
|-------|--------|
| Default (Closed) | Coral background, + icon, elevation shadow |
| Hover | Slight scale up (1.05), deeper shadow |
| Pressed | Scale down (0.95), reduced shadow |
| Open | Rotated 135°, × icon visible |
| Focused | Amber ring visible |
| Disabled | 50% opacity, no interactions |

### Menu Item States
| State | Visual |
|-------|--------|
| Default | White glass background, violet icon |
| Hover | Scale up (1.1), increased opacity, glow shadow |
| Pressed | Scale down (0.95), accent color tint |
| Focused | Accent color ring |
| Disabled | 50% opacity |

### Menu States
| State | Visual |
|-------|--------|
| Closed | Items at center (scale 0), invisible |
| Opening | Items spring outward with stagger |
| Open | Items at final positions |
| Closing | Items collapse inward (reverse stagger) |

---

## Behaviors

### Opening Sequence
1. User taps trigger button
2. Trigger rotates 135° (+ becomes ×)
3. Backdrop fades in with blur
4. Menu items spring outward:
   - Staggered delay: 30-50ms per item
   - Each item starts at center (scale 0, rotated -180°)
   - Springs to final position with overshoot bounce
   - Rotation animates from -180° to 0°

### Closing Sequence
1. User taps trigger or selects item (backdrop tap does NOT close)
2. Menu items collapse inward:
   - Reverse stagger (last item first)
   - Faster than opening (snappier feel)
   - Scale to 0, rotate back
3. Backdrop fades out
4. Trigger rotates back to 0°

### Item Selection
1. User taps menu item
2. Item shows pressed state briefly
3. Action callback fires
4. Menu closes automatically
5. Optional: Ripple effect on selection

### Gesture Support
| Gesture | Action |
|---------|--------|
| Tap trigger | Toggle menu open/closed |
| Tap item | Select action, close menu |
| Tap backdrop | No action (does not close) |
| Escape key | Close menu |

---

## Accessibility

### Touch Targets
- All interactive elements minimum 44×44dp (iOS) / 48×48dp (Android)
- Adequate spacing between items to prevent mis-taps

### Screen Reader Support
- Trigger: "Actions menu, collapsed" / "Actions menu, expanded"
- Items: "[Action name] button"
- Backdrop: "Close menu"

### Keyboard Navigation
- Tab to focus trigger
- Enter/Space to toggle menu
- Arrow keys to navigate items when open
- Enter/Space to select focused item
- Escape to close

### Focus Management
- Focus trapped within menu when open
- Focus returns to trigger on close
- Visible focus indicators on all interactive elements

### Motion Sensitivity
- Respect `prefers-reduced-motion`
- Reduced motion: Instant show/hide, no spring physics
- Maintain functionality without animation

### Color Contrast
- Icons on white: Minimum 3:1 contrast ratio
- White on coral: Minimum 4.5:1 contrast ratio
- Focus rings clearly visible

---

## Micro-interactions

### Trigger Button
| Interaction | Animation |
|-------------|-----------|
| Hover | Scale to 1.05 over 150ms ease-out |
| Press | Scale to 0.95 over 100ms ease-in |
| Toggle | Rotate 135° over 300ms spring curve |
| Icon swap | Cross-fade over 200ms |

### Menu Items
| Interaction | Animation |
|-------------|-----------|
| Spring out | 400ms spring curve (0.34, 1.56, 0.64, 1) with overshoot |
| Rotation | -180° to 0° over 400ms synchronized with position |
| Stagger | 40ms delay between each item |
| Hover | Scale to 1.1 over 150ms, shadow glow |
| Press | Scale to 0.95 over 100ms |
| Collapse | 250ms ease-in, reverse stagger |

### Backdrop
| Interaction | Animation |
|-------------|-----------|
| Fade in | 200ms ease-out |
| Blur | 200ms transition to 8dp blur |
| Fade out | 150ms ease-in |

### Spring Physics Parameters
```
Tension: 180
Friction: 12
Mass: 1
Overshoot: ~15% of travel distance
```

Equivalent cubic-bezier: `cubic-bezier(0.34, 1.56, 0.64, 1)`

---

## Component API (Conceptual)

```
RadialActionMenu
├── Properties
│   ├── Items: Collection of RadialMenuItemData (max 4)
│   ├── Position: BottomRight | BottomLeft | TopRight | TopLeft
│   ├── AccentColor: Color (trigger button and unified item accent)
│   ├── IconColor: Color (all item icons)
│   ├── Radius: double (distance from center)
│   ├── ArcSpan: double (degrees, default 90)
│   ├── IsOpen: bool
│   └── UseReducedMotion: bool
├── Events
│   ├── Opening
│   ├── Opened
│   ├── Closing
│   ├── Closed
│   └── ItemSelected(item)
└── Methods
    ├── Open()
    ├── Close()
    └── Toggle()

RadialMenuItemData
├── Icon: IconSource
├── Label: string (for accessibility only)
├── Command: ICommand
└── IsEnabled: bool
```

**Constraints**:
- Maximum 4 items enforced
- Minimum 3 items recommended for visual balance
- Unified color scheme (no per-item accent colors)

---

## Visual Specifications Summary

```
┌─────────────────────────────────────────┐
│                                         │
│                                         │
│                    ○ Item 3             │
│                   ╱                     │
│                  ╱                      │
│             ○ Item 2                    │
│            ╱                            │
│           ╱                             │
│      ○ Item 1                           │
│       ╲                                 │
│        ╲  80-100dp                      │
│         ╲                               │
│          ◉ Trigger (56dp)               │
│            ↑                            │
│         16-24dp margin                  │
└─────────────────────────────────────────┘

Item Layout (Bottom-Right position):
- Arc spans 180° to 270°
- Items distributed evenly
- Trigger remains stationary
```

---

## Resolved Questions

| Question | Decision |
|----------|----------|
| Item Count | 3-4 items (enforced max) |
| Overflow | Not supported - enforce max of 4 |
| Per-item Colors | Unified color scheme |
| Tooltips | Icon-only, no tooltips |
| Close Behavior | Re-tap FAB to close (no backdrop dismiss) |
| Nesting | Flat only, no sub-menus |
| Platform Variations | None - consistent across all platforms |
