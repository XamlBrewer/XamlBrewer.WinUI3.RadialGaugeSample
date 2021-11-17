using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.System;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    public static class UIElementExtensions
    {
        public static void AddKeyboardAccelerator(this UIElement element,
          VirtualKeyModifiers keyModifiers,
          VirtualKey key,
          TypedEventHandler<KeyboardAccelerator, KeyboardAcceleratorInvokedEventArgs> handler)
        {
            var accelerator =
              new KeyboardAccelerator()
              {
                  Modifiers = keyModifiers,
                  Key = key
              };
            accelerator.Invoked += handler;
            element.KeyboardAccelerators.Add(accelerator);
        }
    }
}
