// https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/Microsoft.Toolkit.Uwp.UI.Controls.Input/RadialGauge/RadialGaugeAutomationPeer.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Windows.Foundation;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Exposes <see cref="RadialGauge"/> to Microsoft UI Automation.
    /// </summary>
    public class RadialGauge2AutomationPeer :
        RangeBaseAutomationPeer,
        IRangeValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGauge2AutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner element to create for.</param>
        public RadialGauge2AutomationPeer(RadialGauge2 owner)
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public new bool IsReadOnly => !((RadialGauge2)Owner).IsInteractive;

        /// <inheritdoc/>
        public new double LargeChange => ((RadialGauge2)Owner).StepSize;

        /// <inheritdoc/>
        public new double Maximum => ((RadialGauge2)Owner).Maximum;

        /// <inheritdoc/>
        public new double Minimum => ((RadialGauge2)Owner).Minimum;

        /// <inheritdoc/>
        public new double SmallChange => ((RadialGauge2)Owner).StepSize;

        /// <inheritdoc/>
        public new double Value => ((RadialGauge2)Owner).Value;

        /// <inheritdoc/>
        public new void SetValue(double value)
        {
            ((RadialGauge2)Owner).Value = value;
        }

        /// <inheritdoc/>
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            return null;
        }

        /// <inheritdoc/>
        protected override string GetNameCore()
        {
            var gauge = (RadialGauge2)Owner;
            return "radial gauge. " + (string.IsNullOrWhiteSpace(gauge.Unit) ? "no unit specified, " : "unit " + gauge.Unit + ", ") + Value;
        }

        /// <inheritdoc/>
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.RangeValue)
            {
                // Expose RangeValue properties.
                return this;
            }

            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc/>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <summary>
        /// Raises the property changed event for this AutomationPeer for the provided identifier.
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        public void RaiseValueChangedEvent(double oldValue, double newValue)
        {
            RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, PropertyValue.CreateDouble(oldValue), PropertyValue.CreateDouble(newValue));
        }
    }
}