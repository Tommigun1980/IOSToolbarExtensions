// Right-hand toolbar inspired by https://github.com/AmitManchanda/iOSSecondaryToolbarMenubar

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using IOSToolbarExtensions.iOS.Renderers;

[assembly: ExportRenderer(typeof(ContentPage), typeof(IOSToolbarExtensionsContentPageRenderer), Priority = short.MaxValue)]
namespace IOSToolbarExtensions.iOS.Renderers
{
	public class IOSToolbarExtensionsContentPageRenderer : PageRenderer
	{
		// left hand primary item buttons
		public static bool EnablePrimaryLeftHandToolbarItemRendering { get; set; } = true;

		private ToolbarItem[] primaryToolbarItems;

		// right hand secondary item toolbar
		public class SecondaryToolbarSettings
		{
			public string Icon { get; set; } = "more.png";

			public float RowHeight { get; set; } = -1;

			public float ColumnWidth { get; set; } = 200.0f;

			//public float CornerRadius { get; set; } = 5.0f;

			public float ShadowOpacity { get; set; } = 0.7f;

			public float ShadowRadius { get; set; } = 4.0f;

			public float ShadowOffsetX { get; set; } = 0.0f;

			public float ShadowOffsetY { get; set; } = 0.0f;

			public UIFont Font { get; set; } = UIFont.PreferredCallout;

			public float GetRowHeight()
			{
				return SecondaryToolbarUserSettings.RowHeight > 0 ?
					SecondaryToolbarUserSettings.RowHeight :
					(float)(this.Font.Ascender - this.Font.Descender) * 2;
			}
		}
		public static bool EnableSecondaryToolbarRendering { get; set; } = true;
		public static SecondaryToolbarSettings SecondaryToolbarUserSettings { get; set; } = new SecondaryToolbarSettings();

		private bool isDropDownMenuActive;
		private ToolbarItem[] secondaryToolbarItems;
		private UIView transparentCloseDropDownMenuView;
		private UITapGestureRecognizer transparentCloseDropDownMenuViewTapRecognizer;
		private UITableView toolbarTableView;

		private bool hasInited;

		public override void ViewWillAppear(bool animated)
		{
			if (!this.hasInited && this.NavigationController != null)
			{
				this.hasInited = true;

				var contentPage = (ContentPage)this.Element;

				this.secondaryToolbarItems = contentPage.ToolbarItems.Where(i => i.Order == ToolbarItemOrder.Secondary).OrderBy(b => b.Priority).ToArray();
				this.SetupRightHandSecondaryItemToolbar();

				this.primaryToolbarItems = contentPage.ToolbarItems.Where(i => i.Order != ToolbarItemOrder.Secondary).OrderBy(b => b.Priority).ToArray();
				this.SetupLeftHandPrimaryToolbarItems();
			}

			base.ViewWillAppear(animated);
		}

		private void SetupRightHandSecondaryItemToolbar()
		{
			if (EnableSecondaryToolbarRendering)
			{
				if (this.secondaryToolbarItems.Any())
				{
					var contentPage = (ContentPage)this.Element;

					foreach (var toolbarItem in this.secondaryToolbarItems)
						contentPage.ToolbarItems.Remove(toolbarItem);

					contentPage.ToolbarItems.Add(new ToolbarItem()
					{
						Order = ToolbarItemOrder.Primary,
						Priority = int.MaxValue,
						IconImageSource = SecondaryToolbarUserSettings.Icon,
						Command = new Command(this.ToggleDropDownMenu)
					});
				}
			}
		}

		private void SetupLeftHandPrimaryToolbarItems()
		{
			if (EnablePrimaryLeftHandToolbarItemRendering)
			{
				if (this.primaryToolbarItems.Any(t => t is LeftHandToolbarItem))
				{
					var rightBarButtonItems = this.NavigationController.TopViewController.NavigationItem.RightBarButtonItems;

					var leftButtons = new List<UIBarButtonItem>();
					var rightButtons = new List<UIBarButtonItem>();
					for (var i = 0; i < this.primaryToolbarItems.Length; i++)
					{
						var index = this.primaryToolbarItems.Length - 1 - i;
						System.Diagnostics.Debug.Assert(index >= 0 && index < rightBarButtonItems.Length, $"OOB. Was: {index}. Length: {rightBarButtonItems.Length}");
						var uiButton = rightBarButtonItems[index];

						var toolbarItemData = this.primaryToolbarItems[i];
						if (toolbarItemData is LeftHandToolbarItem)
							leftButtons.Add(uiButton);
						else
							rightButtons.Insert(0, uiButton); // right-hand side items are drawn in inverted order
					}

					this.NavigationController.TopViewController.NavigationItem.LeftBarButtonItems = leftButtons.ToArray();
					this.NavigationController.TopViewController.NavigationItem.RightBarButtonItems = rightButtons.ToArray();
				}
			}
		}

		public override void ViewDidDisappear(bool animated)
		{
			this.CloseDropDownMenu();

			base.ViewDidDisappear(animated);
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			if (this.isDropDownMenuActive)
				this.toolbarTableView.Frame = this.GetPositionForDropDownMenu();
		}

		private void OpenDropDownMenu()
		{
			if (!this.isDropDownMenuActive && secondaryToolbarItems.Length > 0)
			{
				System.Diagnostics.Debug.Assert(this.View.Subviews != null && View.Subviews.Length > 0 && View.Bounds != null);
				this.isDropDownMenuActive = true;

				this.transparentCloseDropDownMenuView = new UIView(new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height))
				{
					BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0)
				};
				this.transparentCloseDropDownMenuViewTapRecognizer = new UITapGestureRecognizer(this.CloseDropDownMenu);
				this.transparentCloseDropDownMenuView.AddGestureRecognizer(transparentCloseDropDownMenuViewTapRecognizer);
				this.Add(this.transparentCloseDropDownMenuView);

				this.toolbarTableView = new UITableView(this.GetPositionForDropDownMenu())
				{
					Source = new TableSource(this.secondaryToolbarItems, this.CloseDropDownMenu),
					ClipsToBounds = true,
					ScrollEnabled = false,
					//BackgroundColor = UserSettings.MenuBackgroundColor.ToUIColor()
				};
				//this.toolbarTableView.Layer.CornerRadius = RightToolbarMenuUserSettings.CornerRadius;

				this.toolbarTableView.Layer.MasksToBounds = false;
				//this.toolbarTableView.Layer.ShadowColor = UserSettings.ShadowColor.ToCGColor();
				this.toolbarTableView.Layer.ShadowOpacity = SecondaryToolbarUserSettings.ShadowOpacity;
				this.toolbarTableView.Layer.ShadowRadius = SecondaryToolbarUserSettings.ShadowRadius;
				this.toolbarTableView.Layer.ShadowOffset = new SizeF(SecondaryToolbarUserSettings.ShadowOffsetX, SecondaryToolbarUserSettings.ShadowOffsetY);

				this.Add(this.toolbarTableView);
			}
		}

		private void CloseDropDownMenu()
		{
			if (this.isDropDownMenuActive)
			{
				this.isDropDownMenuActive = false;

				this.transparentCloseDropDownMenuView.RemoveGestureRecognizer(transparentCloseDropDownMenuViewTapRecognizer);
				this.transparentCloseDropDownMenuViewTapRecognizer.Dispose();
				this.transparentCloseDropDownMenuViewTapRecognizer = null;

				this.toolbarTableView.RemoveFromSuperview();
				this.toolbarTableView.Dispose();
				this.toolbarTableView = null;

				this.transparentCloseDropDownMenuView.RemoveFromSuperview();
				this.transparentCloseDropDownMenuView.Dispose();
				this.transparentCloseDropDownMenuView = null;
			}
		}

		private void ToggleDropDownMenu()
		{
			if (!this.isDropDownMenuActive)
				this.OpenDropDownMenu();
			else
				this.CloseDropDownMenu();
		}

		private RectangleF GetPositionForDropDownMenu()
		{
			return new RectangleF(
				(float)this.View.Bounds.Width - SecondaryToolbarUserSettings.ColumnWidth,
				0,
				SecondaryToolbarUserSettings.ColumnWidth,
				this.secondaryToolbarItems.Length * SecondaryToolbarUserSettings.GetRowHeight());
		}
	}

	internal class TableSource : UITableViewSource
	{
		private ToolbarItem[] toolbarItems;
		private Action itemSelected;

		private const string CellIdentifier = "RightToolbarTableCell";

		public TableSource(ToolbarItem[] toolbarItems, Action itemSelected)
		{
			this.toolbarItems = toolbarItems;
			this.itemSelected = itemSelected;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.toolbarItems.Length;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return IOSToolbarExtensionsContentPageRenderer.SecondaryToolbarUserSettings.GetRowHeight();
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var toolbarItem = this.toolbarItems[indexPath.Row];

			UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
			if (cell == null)
			{
				cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
			}
			// TODO: is this needed?
			/*else
            {
				if (cell.ImageView.Image != null)
				{
					cell.ImageView.Image.Dispose();
					cell.ImageView.Image = null;
				}
            }*/

			UIImage image = null;
			if (toolbarItem.IconImageSource != null)
			{
				var imageSourceHandler = GetImageSourceHandler(toolbarItem.IconImageSource);
				if (imageSourceHandler != null)
				{
					var imageLoadTask = imageSourceHandler.LoadImageAsync(toolbarItem.IconImageSource);
					if (imageLoadTask.Status == System.Threading.Tasks.TaskStatus.Created)
						imageLoadTask.RunSynchronously();
					image = imageLoadTask.IsCompletedSuccessfully ? imageLoadTask.Result : null;
				}
			}
			cell.ImageView.Image = image;

			cell.TextLabel.Font = IOSToolbarExtensionsContentPageRenderer.SecondaryToolbarUserSettings.Font;
			cell.TextLabel.Text = toolbarItem.Text;

			cell.TextLabel.Enabled = toolbarItem.IsEnabled;
			cell.UserInteractionEnabled = toolbarItem.IsEnabled;
			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var toolbarItem = this.toolbarItems[indexPath.Row];

			if (toolbarItem.Command != null && toolbarItem.Command.CanExecute(toolbarItem.CommandParameter))
				toolbarItem.Command.Execute(toolbarItem.CommandParameter);

			var onClickedMethodInfo = toolbarItem.GetType().GetMethod("OnClicked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			onClickedMethodInfo.Invoke(toolbarItem, new object[0]);

			tableView.DeselectRow(indexPath, true);

			this.itemSelected?.Invoke();
		}

		private static IImageSourceHandler GetImageSourceHandler(ImageSource source)
		{
			if (source is UriImageSource)
				return new ImageLoaderSourceHandler();
			if (source is FileImageSource)
				return new FileImageSourceHandler();
			if (source is StreamImageSource)
				return new StreamImagesourceHandler();
			return null;
		}
	}
}
