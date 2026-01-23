# Radial Action Menu

## Problem Statement

Create a reusable, animated floating action button (FAB) that expands into a radial menu of action items. The component should feel playful and modern with spring physics animations, suitable for showcasing Uno Platform's animation and custom control capabilities.

**Target user**: Developers building mobile-first or touch-friendly applications who need an engaging, space-efficient action menu.

## Design Brief

See [design-brief.md](design-brief.md)

**Key characteristics**:
- Playful geometric with glassmorphism aesthetic
- Light/airy theme with vibrant coral accent
- Items spring outward in arc with rotation and bounce physics
- Position-aware (adjusts arc based on corner placement)

## Architecture

See [architecture.md](architecture.md)

**Key decisions**:
- UserControl with Canvas for absolute positioning
- CompositeTransform for GPU-accelerated animations
- BackEase for spring-like overshoot effect
- Staggered animations (40ms per item)

## Validation Notes

- Platform: Android only
- AcrylicBrush blur enabled
- Shadow Z translation: 10
- Icons: Segoe Fluent Icons
- Demo actions: Share, Edit, Delete, Favorite
- Container: Self-positioning (fills parent)

## Prompts

**Initial prompt** (to kick off implementation):
> Implement the Radial Action Menu component following the design brief and architecture documents.

**Full prompt log**: See [prompts.md](prompts.md)

**Total prompts**: 6

**Time spent**: TBD

## Technical Recap

**Platform targets**:
- [ ] WebAssembly
- [ ] Windows
- [ ] iOS
- [x] Android
- [ ] macOS
- [ ] Linux

**Architecture pattern**:
- [ ] MVUX
- [ ] MVVM
- [x] Other: Code-behind with DependencyProperties

**Key Uno Platform features**:
- Storyboard animations with DoubleAnimation
- CompositeTransform (TranslateX/Y, ScaleX/Y, Rotation)
- BackEase for spring-like physics
- Canvas for absolute positioning
- ThemeShadow for elevation

**Uno Toolkit components**:
- SafeArea.Insets
- Material theme resources

**Third-party integrations**:
- None

## Twitter Caption

*Ready-to-post caption for video recording:*

Built a playful Radial Action Menu with Uno Platform! Spring physics, staggered animations, and glassmorphism - all in C#/XAML running natively on Android. The FAB expands into a beautiful arc with bounce effects. Love how smooth these animations feel!

#UnoPlatform #dotnet #XAML #Android #CrossPlatform #UIAnimation

## Assets

- [Reference image](assets/) - Original design reference
- [Source code](src/RadialActionMenuDemo/) - Complete Uno Platform project

## Lessons Learned

- BackEase provides good spring-like overshoot without complex physics calculations
- Canvas with CompositeTransform is effective for radial layouts with animations
- Staggered BeginTime creates engaging cascading effects
- Self-positioning controls simplify consumer usage
