﻿// https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp.UI.Controls.Input/RadialGauge/RadialGauge.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.System;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// A Modern UI Radial Gauge using XAML and Composition API.
    /// The scale of the gauge is a clockwise arc that sweeps from MinAngle (default lower left, at -150°) to MaxAngle (default lower right, at +150°).
    /// </summary>
    //// All calculations are for a 200x200 square. The viewbox will do the rest.
    [TemplatePart(Name = ContainerPartName, Type = typeof(Grid))]
    [TemplatePart(Name = ScalePartName, Type = typeof(Path))]
    [TemplatePart(Name = TrailPartName, Type = typeof(Path))]
    [TemplatePart(Name = ValueTextPartName, Type = typeof(TextBlock))]
    public class RadialGauge2 : RangeBase
    {
        /// <summary>
        /// Identifies the optional StepSize property.
        /// </summary>
        public static readonly DependencyProperty StepSizeProperty =
            DependencyProperty.Register(nameof(StepSize), typeof(double), typeof(RadialGauge2), new PropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="IsInteractive"/> property.
        /// </summary>
        public static readonly DependencyProperty IsInteractiveProperty =
            DependencyProperty.Register(nameof(IsInteractive), typeof(bool), typeof(RadialGauge2), new PropertyMetadata(false, OnInteractivityChanged));

        /// <summary>
        /// Identifies the ScaleWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleWidthProperty =
            DependencyProperty.Register(nameof(ScaleWidth), typeof(double), typeof(RadialGauge2), new PropertyMetadata(16.0, OnScaleChanged));

        /// <summary>
        /// Identifies the NeedleBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleBrushProperty =
            DependencyProperty.Register(nameof(NeedleBrush), typeof(SolidColorBrush), typeof(RadialGauge2), new PropertyMetadata(null, OnFaceChanged));

        /// <summary>
        /// Identifies the Unit dependency property.
        /// </summary>
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(RadialGauge2), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the TrailBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty TrailBrushProperty =
            DependencyProperty.Register(nameof(TrailBrush), typeof(Brush), typeof(RadialGauge2), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ScaleBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleBrushProperty =
            DependencyProperty.Register(nameof(ScaleBrush), typeof(Brush), typeof(RadialGauge2), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ScaleTickBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleTickBrushProperty =
            DependencyProperty.Register(nameof(ScaleTickBrush), typeof(Brush), typeof(RadialGauge2), new PropertyMetadata(null, OnFaceChanged));

        /// <summary>
        /// Identifies the TickBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty TickBrushProperty =
            DependencyProperty.Register(nameof(TickBrush), typeof(SolidColorBrush), typeof(RadialGauge2), new PropertyMetadata(null, OnFaceChanged));

        /// <summary>
        /// Identifies the ValueStringFormat dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueStringFormatProperty =
            DependencyProperty.Register(nameof(ValueStringFormat), typeof(string), typeof(RadialGauge2), new PropertyMetadata("N0", (s, e) => OnValueChanged(s)));

        /// <summary>
        /// Identifies the TickSpacing dependency property.
        /// </summary>
        public static readonly DependencyProperty TickSpacingProperty =
        DependencyProperty.Register(nameof(TickSpacing), typeof(int), typeof(RadialGauge2), new PropertyMetadata(10, OnFaceChanged));

        /// <summary>
        /// Identifies the NeedleLength dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleLengthProperty =
            DependencyProperty.Register(nameof(NeedleLength), typeof(double), typeof(RadialGauge2), new PropertyMetadata(100d, OnFaceChanged));

        /// <summary>
        /// Identifies the NeedleWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty NeedleWidthProperty =
            DependencyProperty.Register(nameof(NeedleWidth), typeof(double), typeof(RadialGauge2), new PropertyMetadata(5d, OnFaceChanged));

        /// <summary>
        /// Identifies the ScalePadding dependency property.
        /// </summary>
        public static readonly DependencyProperty ScalePaddingProperty =
            DependencyProperty.Register(nameof(ScalePadding), typeof(double), typeof(RadialGauge2), new PropertyMetadata(20d, OnFaceChanged));

        /// <summary>
        /// Identifies the ScaleTickWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ScaleTickWidthProperty =
            DependencyProperty.Register(nameof(ScaleTickWidth), typeof(double), typeof(RadialGauge2), new PropertyMetadata(2d, OnFaceChanged));

        /// <summary>
        /// Identifies the TickWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty TickWidthProperty =
            DependencyProperty.Register(nameof(TickWidth), typeof(double), typeof(RadialGauge2), new PropertyMetadata(5d, OnFaceChanged));

        /// <summary>
        /// Identifies the TickLength dependency property.
        /// </summary>
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register(nameof(TickLength), typeof(double), typeof(RadialGauge2), new PropertyMetadata(10d, OnFaceChanged));

        /// <summary>
        /// Identifies the MinAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty MinAngleProperty =
            DependencyProperty.Register(nameof(MinAngle), typeof(int), typeof(RadialGauge2), new PropertyMetadata(-150, OnScaleChanged));

        /// <summary>
        /// Identifies the MaxAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxAngleProperty =
            DependencyProperty.Register(nameof(MaxAngle), typeof(int), typeof(RadialGauge2), new PropertyMetadata(150, OnScaleChanged));

        /// <summary>
        /// Identifies the ValueAngle dependency property.
        /// </summary>
        protected static readonly DependencyProperty ValueAngleProperty =
            DependencyProperty.Register(nameof(ValueAngle), typeof(double), typeof(RadialGauge2), new PropertyMetadata(null));

        // Template Parts.
        private const string ContainerPartName = "PART_Container";
        private const string ScalePartName = "PART_Scale";
        private const string TrailPartName = "PART_Trail";
        private const string ValueTextPartName = "PART_ValueText";

        // For convenience.
        private const double Degrees2Radians = Math.PI / 180;

        private double _normalizedMinAngle;
        private double _normalizedMaxAngle;

        private Compositor _compositor;
        private ContainerVisual _root;
        private CompositionSpriteShape _needle;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGauge2"/> class.
        /// Create a default radial gauge control.
        /// </summary>
        public RadialGauge2()
        {
            DefaultStyleKey = typeof(RadialGauge2);

            SmallChange = 1;
            LargeChange = 10;
            Unloaded += RadialGauge_Unloaded;
        }

        private void RadialGauge_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unregister event handlers.
            KeyboardAccelerators.Clear();
            PointerReleased -= RadialGauge_PointerReleased;
            Unloaded -= RadialGauge_Unloaded;
        }

        /// <summary>
        /// Gets or sets the rounding interval for the Value.
        /// </summary>
        public double StepSize
        {
            get => (double)GetValue(StepSizeProperty);
            set => SetValue(StepSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control accepts setting its value through interaction.
        /// </summary>
        public bool IsInteractive
        {
            get => (bool)GetValue(IsInteractiveProperty);
            set => SetValue(IsInteractiveProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the scale, in percentage of the gauge radius.
        /// </summary>
        public double ScaleWidth
        {
            get => (double)GetValue(ScaleWidthProperty);
            set => SetValue(ScaleWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the displayed unit measure.
        /// </summary>
        public string Unit
        {
            get => (string)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        /// <summary>
        /// Gets or sets the needle brush.
        /// </summary>
        public SolidColorBrush NeedleBrush
        {
            get => (SolidColorBrush)GetValue(NeedleBrushProperty);
            set => SetValue(NeedleBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the trail brush.
        /// </summary>
        public Brush TrailBrush
        {
            get => (Brush)GetValue(TrailBrushProperty);
            set => SetValue(TrailBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the scale brush.
        /// </summary>
        public Brush ScaleBrush
        {
            get => (Brush)GetValue(ScaleBrushProperty);
            set => SetValue(ScaleBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the scale tick brush.
        /// </summary>
        public SolidColorBrush ScaleTickBrush
        {
            get => (SolidColorBrush)GetValue(ScaleTickBrushProperty);
            set => SetValue(ScaleTickBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the outer tick brush.
        /// </summary>
        public SolidColorBrush TickBrush
        {
            get => (SolidColorBrush)GetValue(TickBrushProperty);
            set => SetValue(TickBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the value string format.
        /// </summary>
        public string ValueStringFormat
        {
            get => (string)GetValue(ValueStringFormatProperty);
            set => SetValue(ValueStringFormatProperty, value);
        }

        /// <summary>
        /// Gets or sets the tick spacing, in units. Values of zero or less will be ignored when drawing.
        /// </summary>
        public int TickSpacing
        {
            get => (int)GetValue(TickSpacingProperty);
            set => SetValue(TickSpacingProperty, value);
        }

        /// <summary>
        /// Gets or sets the needle length, in percentage of the gauge radius.
        /// </summary>
        public double NeedleLength
        {
            get => (double)GetValue(NeedleLengthProperty);
            set => SetValue(NeedleLengthProperty, value);
        }

        /// <summary>
        /// Gets or sets the needle width, in percentage of the gauge radius.
        /// </summary>
        public double NeedleWidth
        {
            get => (double)GetValue(NeedleWidthProperty);
            set => SetValue(NeedleWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the distance of the scale from the outside of the control, in percentage of the gauge radius.
        /// </summary>
        public double ScalePadding
        {
            get => (double)GetValue(ScalePaddingProperty);
            set => SetValue(ScalePaddingProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the scale ticks, in percentage of the gauge radius.
        /// </summary>
        public double ScaleTickWidth
        {
            get => (double)GetValue(ScaleTickWidthProperty);
            set => SetValue(ScaleTickWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the length of the ticks, in percentage of the gauge radius.
        /// </summary>
        public double TickLength
        {
            get => (double)GetValue(TickLengthProperty);
            set => SetValue(TickLengthProperty, value);
        }

        /// <summary>
        /// Gets or sets the width of the ticks, in percentage of the gauge radius.
        /// </summary>
        public double TickWidth
        {
            get => (double)GetValue(TickWidthProperty);
            set => SetValue(TickWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets the start angle of the scale, which corresponds with the Minimum value, in degrees.
        /// </summary>
        /// <remarks>Changing MinAngle may require retemplating the control.</remarks>
        public int MinAngle
        {
            get => (int)GetValue(MinAngleProperty);
            set => SetValue(MinAngleProperty, value);
        }

        /// <summary>
        /// Gets or sets the end angle of the scale, which corresponds with the Maximum value, in degrees.
        /// </summary>
        /// <remarks>Changing MaxAngle may require retemplating the control.</remarks>
        public int MaxAngle
        {
            get => (int)GetValue(MaxAngleProperty);
            set => SetValue(MaxAngleProperty, value);
        }

        /// <summary>
        /// Gets or sets the current angle of the needle (between MinAngle and MaxAngle). Setting the angle will update the Value.
        /// </summary>
        protected double ValueAngle
        {
            get => (double)GetValue(ValueAngleProperty);
            set => SetValue(ValueAngleProperty, value);
        }

        /// <summary>
        /// Gets the normalized minimum angle.
        /// </summary>
        /// <value>The minimum angle in the range from -180 to 180.</value>
        protected double NormalizedMinAngle => _normalizedMinAngle;

        /// <summary>
        /// Gets the normalized maximum angle.
        /// </summary>
        /// <value>The maximum angle, in the range from -180 to 540.</value>
        protected double NormalizedMaxAngle => _normalizedMaxAngle;

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadialGauge2AutomationPeer(this);
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // Register event handlers.
            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.Control,
                VirtualKey.Left,
                (ka, kaea) =>
                {
                    Value = Math.Max(Minimum, Value - LargeChange);
                    kaea.Handled = true;
                });
            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.None,
                VirtualKey.Left,
                (ka, kaea) =>
                {
                    Value = Math.Max(Minimum, Value - SmallChange);
                    kaea.Handled = true;
                });
            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.Control,
                VirtualKey.Down,
                (ka, kaea) =>
                {
                    Value = Math.Max(Minimum, Value - LargeChange);
                    kaea.Handled = true;
                });

            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.None,
                VirtualKey.Down,
                (ka, kaea) =>
                {
                    Value = Math.Max(Minimum, Value - SmallChange);
                    kaea.Handled = true;
                });
            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.Control,
                VirtualKey.Right,
                (ka, kaea) =>
                {
                    Value = Math.Min(Maximum, Value + LargeChange);
                    kaea.Handled = true;
                });
            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.None,
                VirtualKey.Right,
                (ka, kaea) =>
                {
                    Value = Math.Min(Maximum, Value + SmallChange);
                    kaea.Handled = true;
                });
            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.Control,
                VirtualKey.Up,
                (ka, kaea) =>
                {
                    Value = Math.Min(Maximum, Value + LargeChange);
                    kaea.Handled = true;
                });
            this.AddKeyboardAccelerator(
                VirtualKeyModifiers.None,
                VirtualKey.Up,
                (ka, kaea) =>
                {
                    Value = Math.Min(Maximum, Value + SmallChange);
                    kaea.Handled = true;
                });
            PointerReleased += RadialGauge_PointerReleased;

            // Apply color scheme.
            OnScaleChanged(this);

            base.OnApplyTemplate();
        }

        /// <inheritdoc/>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            OnScaleChanged(this);
        }

        /// <inheritdoc/>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            OnScaleChanged(this);
        }

        /// <inheritdoc/>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            OnValueChanged(this);
            base.OnValueChanged(oldValue, newValue);
            if (AutomationPeer.ListenerExists(AutomationEvents.LiveRegionChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(this) as RadialGauge2AutomationPeer;
                peer?.RaiseValueChangedEvent(oldValue, newValue);
            }
        }

        private static void OnValueChanged(DependencyObject d)
        {
            RadialGauge2 radialGauge = (RadialGauge2)d;
            if (!double.IsNaN(radialGauge.Value))
            {
                if (radialGauge.StepSize != 0)
                {
                    radialGauge.Value = radialGauge.RoundToMultiple(radialGauge.Value, radialGauge.StepSize);
                }

                var middleOfScale = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                var valueText = radialGauge.GetTemplateChild(ValueTextPartName) as TextBlock;
                radialGauge.ValueAngle = radialGauge.ValueToAngle(radialGauge.Value);

                // Needle
                if (radialGauge._needle != null)
                {
                    radialGauge._needle.RotationAngleInDegrees = (float)radialGauge.ValueAngle;
                }

                // Trail
                var trail = radialGauge.GetTemplateChild(TrailPartName) as Path;
                if (trail != null)
                {
                    if (radialGauge.ValueAngle > radialGauge.NormalizedMinAngle)
                    {
                        trail.Visibility = Visibility.Visible;

                        if (radialGauge.ValueAngle - radialGauge.NormalizedMinAngle == 360)
                        {
                            // Draw full circle.
                            var eg = new EllipseGeometry();
                            eg.Center = new Point(100, 100);
                            eg.RadiusX = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                            eg.RadiusY = eg.RadiusX;
                            trail.Data = eg;
                        }
                        else
                        {
                            // Draw arc.
                            var pg = new PathGeometry();
                            var pf = new PathFigure();
                            pf.IsClosed = false;
                            pf.StartPoint = radialGauge.ScalePoint(radialGauge.NormalizedMinAngle, middleOfScale);
                            var seg = new ArcSegment();
                            seg.SweepDirection = SweepDirection.Clockwise;
                            seg.IsLargeArc = radialGauge.ValueAngle > (180 + radialGauge.NormalizedMinAngle);
                            seg.Size = new Size(middleOfScale, middleOfScale);
                            seg.Point = radialGauge.ScalePoint(Math.Min(radialGauge.ValueAngle, radialGauge.NormalizedMaxAngle), middleOfScale);  // On overflow, stop trail at MaxAngle.
                            pf.Segments.Add(seg);
                            pg.Figures.Add(pf);
                            trail.Data = pg;
                        }
                    }
                    else
                    {
                        trail.Visibility = Visibility.Collapsed;
                    }
                }

                // Value Text
                if (valueText != null)
                {
                    valueText.Text = radialGauge.Value.ToString(radialGauge.ValueStringFormat);
                }
            }
        }

        private static void OnInteractivityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadialGauge2 radialGauge = (RadialGauge2)d;

            if (radialGauge.IsInteractive)
            {
                radialGauge.Tapped += radialGauge.RadialGauge_Tapped;
                radialGauge.ManipulationDelta += radialGauge.RadialGauge_ManipulationDelta;
                radialGauge.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            }
            else
            {
                radialGauge.Tapped -= radialGauge.RadialGauge_Tapped;
                radialGauge.ManipulationDelta -= radialGauge.RadialGauge_ManipulationDelta;
                radialGauge.ManipulationMode = ManipulationModes.None;
            }
        }

        private static void OnScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnScaleChanged(d);
        }

        private static void OnScaleChanged(DependencyObject d)
        {
            RadialGauge2 radialGauge = (RadialGauge2)d;

            radialGauge.UpdateNormalizedAngles();

            var scale = radialGauge.GetTemplateChild(ScalePartName) as Path;
            if (scale != null)
            {
                if (radialGauge.NormalizedMaxAngle - radialGauge.NormalizedMinAngle == 360)
                {
                    // Draw full circle.
                    var eg = new EllipseGeometry();
                    eg.Center = new Point(100, 100);
                    eg.RadiusX = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                    eg.RadiusY = eg.RadiusX;
                    scale.Data = eg;
                }
                else
                {
                    // Draw arc.
                    var pg = new PathGeometry();
                    var pf = new PathFigure();
                    pf.IsClosed = false;
                    var middleOfScale = 100 - radialGauge.ScalePadding - (radialGauge.ScaleWidth / 2);
                    pf.StartPoint = radialGauge.ScalePoint(radialGauge.NormalizedMinAngle, middleOfScale);
                    var seg = new ArcSegment();
                    seg.SweepDirection = SweepDirection.Clockwise;
                    seg.IsLargeArc = radialGauge.NormalizedMaxAngle > (radialGauge.NormalizedMinAngle + 180);
                    seg.Size = new Size(middleOfScale, middleOfScale);
                    seg.Point = radialGauge.ScalePoint(radialGauge.NormalizedMaxAngle, middleOfScale);
                    pf.Segments.Add(seg);
                    pg.Figures.Add(pf);
                    scale.Data = pg;
                }

                if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode)
                {
                    OnFaceChanged(radialGauge);
                }
            }
        }

        private static void OnFaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelpers.IsRunningInLegacyDesignerMode)
            {
                OnFaceChanged(d);
            }
        }

        private static void OnFaceChanged(DependencyObject d)
        {
            RadialGauge2 radialGauge = (RadialGauge2)d;

            var container = radialGauge.GetTemplateChild(ContainerPartName) as Grid;
            if (container == null || DesignTimeHelpers.IsRunningInLegacyDesignerMode)
            {
                // Bad template.
                return;
            }

            radialGauge._root = container.GetVisual();
            radialGauge._root.Children.RemoveAll();
            radialGauge._compositor = radialGauge._root.Compositor;

            if (radialGauge.TickSpacing > 0)
            {
                // Ticks.
                var ticks = radialGauge._compositor.CreateShapeVisual();
                ticks.Size = new Vector2((float)radialGauge.Height, (float)radialGauge.Width);
                ticks.BorderMode = CompositionBorderMode.Soft;
                ticks.Opacity = (float)radialGauge.TickBrush.Opacity;

                var roundedTickRectangle = radialGauge._compositor.CreateRoundedRectangleGeometry();
                roundedTickRectangle.Size = new Vector2((float)radialGauge.TickWidth, (float)radialGauge.TickLength);
                roundedTickRectangle.CornerRadius = new Vector2((float)radialGauge.TickWidth / 2, (float)radialGauge.TickWidth / 2);

                var tssFillBrush = radialGauge._compositor.CreateColorBrush(radialGauge.TickBrush.Color);
                var tssOffset = new Vector2(100 - ((float)radialGauge.TickWidth / 2), 0.0f);
                var tssCenterPoint = new Vector2((float)radialGauge.TickWidth / 2, 100.0f);

                for (double i = radialGauge.Minimum; i <= radialGauge.Maximum; i += radialGauge.TickSpacing)
                {
                    var tickSpriteShape = radialGauge._compositor.CreateSpriteShape(roundedTickRectangle);
                    tickSpriteShape.FillBrush = tssFillBrush;
                    tickSpriteShape.Offset = tssOffset;
                    tickSpriteShape.CenterPoint = tssCenterPoint;
                    tickSpriteShape.RotationAngleInDegrees = (float)radialGauge.ValueToAngle(i);

                    ticks.Shapes.Add(tickSpriteShape);
                }

                radialGauge._root.Children.InsertAtTop(ticks);

                // Scale Ticks.
                SpriteVisual scaleTick;
                var stSize = new Vector2((float)radialGauge.ScaleTickWidth, (float)radialGauge.ScaleWidth);
                var stBrush = radialGauge._compositor.CreateColorBrush(radialGauge.ScaleTickBrush.Color);
                var stOffset = new Vector3(100 - ((float)radialGauge.ScaleTickWidth / 2), (float)radialGauge.ScalePadding, 0);
                var stCenterPoint = new Vector3((float)radialGauge.ScaleTickWidth / 2, 100 - (float)radialGauge.ScalePadding, 0);

                for (double i = radialGauge.Minimum; i <= radialGauge.Maximum; i += radialGauge.TickSpacing)
                {
                    scaleTick = radialGauge._compositor.CreateSpriteVisual();
                    scaleTick.Size = stSize;
                    scaleTick.Brush = stBrush;
                    scaleTick.Offset = stOffset;
                    scaleTick.CenterPoint = stCenterPoint;
                    scaleTick.Opacity = (float)radialGauge.ScaleTickBrush.Opacity;
                    scaleTick.RotationAngleInDegrees = (float)radialGauge.ValueToAngle(i);
                    radialGauge._root.Children.InsertAtTop(scaleTick);
                }
            }

            // Needle.
            var shapeVisual = radialGauge._compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2((float)radialGauge.Height, (float)radialGauge.Width);
            shapeVisual.BorderMode = CompositionBorderMode.Soft;
            shapeVisual.Opacity = (float)radialGauge.NeedleBrush.Opacity;

            var roundedNeedleRectangle = radialGauge._compositor.CreateRoundedRectangleGeometry();
            roundedNeedleRectangle.Size = new Vector2((float)radialGauge.NeedleWidth, (float)radialGauge.NeedleLength);
            roundedNeedleRectangle.CornerRadius = new Vector2((float)radialGauge.NeedleWidth / 2, (float)radialGauge.NeedleWidth / 2);

            radialGauge._needle = radialGauge._compositor.CreateSpriteShape(roundedNeedleRectangle);
            radialGauge._needle.FillBrush = radialGauge._compositor.CreateColorBrush(radialGauge.NeedleBrush.Color);
            radialGauge._needle.CenterPoint = new Vector2((float)radialGauge.NeedleWidth / 2, (float)radialGauge.NeedleLength);
            radialGauge._needle.Offset = new Vector2(100 - ((float)radialGauge.NeedleWidth / 2), 100 - (float)radialGauge.NeedleLength);

            shapeVisual.Shapes.Add(radialGauge._needle);

            radialGauge._root.Children.InsertAtTop(shapeVisual);

            OnValueChanged(radialGauge);
        }

        private void RadialGauge_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            SetGaugeValueFromPoint(e.Position);
        }

        private void RadialGauge_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SetGaugeValueFromPoint(e.GetPosition(this));
        }

        private void RadialGauge_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (IsInteractive)
            {
                e.Handled = true;
            }
        }

        private void UpdateNormalizedAngles()
        {
            var result = Mod(MinAngle, 360);

            if (result >= 180)
            {
                result = result - 360;
            }

            _normalizedMinAngle = result;

            result = Mod(MaxAngle, 360);

            if (result < 180)
            {
                result = result + 360;
            }

            if (result > NormalizedMinAngle + 360)
            {
                result = result - 360;
            }

            _normalizedMaxAngle = result;
        }

        private void SetGaugeValueFromPoint(Point p)
        {
            var pt = new Point(p.X - (ActualWidth / 2), -p.Y + (ActualHeight / 2));

            var angle = Math.Atan2(pt.X, pt.Y) / Degrees2Radians;
            var divider = Mod(NormalizedMaxAngle - NormalizedMinAngle, 360);
            if (divider == 0)
            {
                divider = 360;
            }

            var value = Minimum + ((Maximum - Minimum) * Mod(angle - NormalizedMinAngle, 360) / divider);
            if (value < Minimum || value > Maximum)
            {
                // Ignore positions outside the scale angle.
                return;
            }

            Value = RoundToMultiple(value, StepSize);
        }

        private Point ScalePoint(double angle, double middleOfScale)
        {
            return new Point(100 + (Math.Sin(Degrees2Radians * angle) * middleOfScale), 100 - (Math.Cos(Degrees2Radians * angle) * middleOfScale));
        }

        private double ValueToAngle(double value)
        {
            // Off-scale on the left.
            if (value < Minimum)
            {
                return MinAngle;
            }

            // Off-scale on the right.
            if (value > Maximum)
            {
                return MaxAngle;
            }

            return ((value - Minimum) / (Maximum - Minimum) * (NormalizedMaxAngle - NormalizedMinAngle)) + NormalizedMinAngle;
        }

        private double Mod(double number, double divider)
        {
            var result = number % divider;
            result = result < 0 ? result + divider : result;
            return result;
        }

        private double RoundToMultiple(double number, double multiple)
        {
            double modulo = number % multiple;
            if (double.IsNaN(modulo))
            {
                return number;
            }

            if ((multiple - modulo) <= modulo)
            {
                modulo = multiple - modulo;
            }
            else
            {
                modulo *= -1;
            }

            return number + modulo;
        }
    }
}