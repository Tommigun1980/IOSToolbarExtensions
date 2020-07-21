# IOSToolbarExtensions
*iOS toolbar extensions for Xamarin.Forms*

NuGet package available at https://www.nuget.org/packages/IOSToolbarExtensions/

## Intro

![IOSToolbarExtensions](/docs/IOSToolbarExtensions.png)

This package adds the following two extensions for Xamarin.Forms projects targeting iOS:
* Left-hand primary toolbar items
* Android:esque secondary toolbar item menu bar

## Setup

Add the following line to your Xamarin.iOS project's AssemblyInfo.cs file:

```c#
[assembly: ExportRenderer(typeof(ContentPage), typeof(IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer), Priority = short.MaxValue)]
```

## Left-hand Primary Toolbar Items

Xamarin.Forms doesn't support rendering primary toolbar items on the left side on iOS. IOSToolbarExtensions adds support for this.

### Usage (XAML)

Import the IOSToolbarExtensions assembly:

```c#
xmlns:iostoolbarextensions="clr-namespace:IOSToolbarExtensions;assembly=iOSToolbarExtensions"
```

And use `iostoolbarextensions:LeftHandToolbarItem` instead of `ToolbarItem` for any primary item. LeftHandToolbarItems will be rendered on the left side of the navigation bar on iOS, and will render as normal ToolbarItems on Android. LeftHandToolbarItem inherits from ToolbarItem so all the regular properties are available.

Example:

```xaml
<ContentPage.ToolbarItems>
  <iostoolbarextensions:LeftHandToolbarItem
    Text="Cancel"
    Order="Primary"
    Priority="0" />
</ContentPage.ToolbarItems>
```

### Configuration

To turn off left-hand toolbar item rendering, call

```c#
IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer.EnablePrimaryLeftHandToolbarItemRendering = false;
```

in your Xamarin.iOS project.

## Android:esque Secondary Toolbar Item Menu Bar

Xamarin.Forms renders secondary toolbar items on iOS in a way that is quite unusable. IOSToolbarExtensions remedies this by rendering any secondary toolbar items in a way very similar to how Android does it natively.

### Usage (XAML)

There are no special steps to this as the custom renderer will draw any Secondary toolbar items in a nicer menu on iOS without any additional setup.

For posterity, here is an example of a Secondary toolbar item:

```xaml
  <ContentPage.ToolbarItems>
      <ToolbarItem
          Text="Log Out"
          Order="Secondary"
          Priority="0" />
  </ContentPage.ToolbarItems>
```

### Configuration

To turn off Android:esque secondary toolbar item rendering, call

```xaml
IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer.EnableSecondaryToolbarRendering = false;
```

in your Xamarin.iOS project.

If need be, you can customize rendering of the menu by setting properties of your choice in the

```c#
IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer.SecondaryToolbarUserSettings
```

object in your Xamarin.iOS project.

The editable properties are

```c#
public string Icon { get; set; } = "more.png";
public float RowHeight { get; set; } = -1;
public float ColumnWidth { get; set; } = 200.0f;
public float ShadowOpacity { get; set; } = 0.7f;
public float ShadowRadius { get; set; } = 4.0f;
public float ShadowOffsetX { get; set; } = 0.0f;
public float ShadowOffsetY { get; set; } = 0.0f;
public UIFont Font { get; set; } = UIFont.PreferredCallout;
```

## Acknowledgements

The Android:esque secondary toolbar menu item bar was inspired by https://github.com/AmitManchanda/iOSSecondaryToolbarMenubar, by https://github.com/AmitManchanda.
The secondary toolbar menu default icon graphic is from https://github.com/AmitManchanda/iOSSecondaryToolbarMenubar.
