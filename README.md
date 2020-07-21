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

This feature is turned on by default. To turn off left-hand toolbar item rendering, call the following in your Xamarin.iOS project:

```c#
IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer.EnablePrimaryLeftHandToolbarItemRendering = false;
```

## Android:esque Secondary Toolbar Item Menu Bar

Xamarin.Forms renders secondary toolbar items on iOS in a way that is quite unusable. IOSToolbarExtensions remedies this by rendering any secondary toolbar items in a way very similar to how Android does it natively. Native Android's secondary item drawing is unaffected by this.

### Usage (XAML)

There are no special steps to this as the custom renderer will draw any Secondary toolbar items in a nicer menu on iOS without any additional setup.

For posterity, here is an example of a Secondary toolbar item with text and no icon:

```xaml
  <ContentPage.ToolbarItems>
      <ToolbarItem
          Text="Log Out"
          Order="Secondary"
          Priority="0" />
  </ContentPage.ToolbarItems>
```

### Configuration

This feature is turned on by default. To turn off Android:esque secondary toolbar item rendering, call the following in your Xamarin.iOS project:

```xaml
IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer.EnableSecondaryToolbarRendering = false;
```

If need be, you can customize rendering of the menu by assigning properties of your choice in your Xamarin.iOS project to the following object:

```c#
IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer.SecondaryToolbarUserSettings
```

The editable properties are:

```c#
public string Icon { get; set; } = "more.png"; // this represents the icon for the menu, and defaults to an embedded resource with three dots. the value is assigned to a ToolbarItem's IconImageSource, so it can be a resource name or a url.
public float RowHeight { get; set; } = -1; // if <= 0, height will be calculated as font height * 2, else height will be assigned to this value.
public float ColumnWidth { get; set; } = 200.0f;
public float ShadowOpacity { get; set; } = 0.7f;
public float ShadowRadius { get; set; } = 4.0f;
public float ShadowOffsetX { get; set; } = 0.0f;
public float ShadowOffsetY { get; set; } = 0.0f;
public UIFont Font { get; set; } = UIFont.PreferredCallout;
```

Example - change the toolbar's icon to a downloaded image:

```c#
IOSToolbarExtensions.iOS.Renderers.IOSToolbarExtensionsContentPageRenderer.SecondaryToolbarUserSettings.Icon = "https://someurl.com/someimage.png";
```

## Acknowledgements

The Android:esque secondary toolbar menu item bar was inspired by https://github.com/AmitManchanda/iOSSecondaryToolbarMenubar, by https://github.com/AmitManchanda.
The secondary toolbar menu default icon graphic is from https://github.com/AmitManchanda/iOSSecondaryToolbarMenubar.
