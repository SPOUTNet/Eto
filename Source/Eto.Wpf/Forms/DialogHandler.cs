using System;
using Eto.Forms;
using sw = System.Windows;
using swc = System.Windows.Controls;
using Eto.Wpf.Forms.Controls;
using System.Threading.Tasks;
using Eto.Drawing;

namespace Eto.Wpf.Forms
{
	public class DialogHandler : WpfWindow<sw.Window, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		Button defaultButton;
		Button abortButton;
		Rectangle? parentWindowBounds;

		public DialogHandler()
		{
			Control = new sw.Window();
			Control.ShowInTaskbar = false;
			Resizable = false;
			Minimizable = false;
			Maximizable = false;
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public void ShowModal(Control parent)
		{
			if (parent != null && !LocationSet)
			{
				var parentWindow = parent.ParentWindow;
				if (parentWindow != null)
				{
					var handler = (IWpfWindow)parentWindow.Handler;
					handler.SetOwnerFor(Control);
					// CenterOwner does not work in certain cases (e.g. with autosizing)
					Control.WindowStartupLocation = sw.WindowStartupLocation.Manual;
					Control.SourceInitialized += HandleSourceInitialized;
					parentWindowBounds = parentWindow.Bounds;
				}
			}
			Control.ShowDialog();
			WpfFrameworkElementHelper.ShouldCaptureMouse = false;
		}

		public Task ShowModalAsync(Control parent)
		{
			var tcs = new TaskCompletionSource<bool>();
			Application.Instance.AsyncInvoke(() =>
			{
				ShowModal(parent);
				tcs.SetResult(true);
			});
			return tcs.Task;
		}

		void HandleSourceInitialized(object sender, EventArgs e)
		{
			if (parentWindowBounds != null)
			{
				var bounds = parentWindowBounds.Value;
				Control.Left = bounds.Left + (bounds.Width - Control.ActualWidth) / 2;
				Control.Top = bounds.Top + (bounds.Height - Control.ActualHeight) / 2;
				parentWindowBounds = null;
			}
			Control.SourceInitialized -= HandleSourceInitialized;
		}

		public Button DefaultButton
		{
			get { return defaultButton; }
			set
			{
				if (defaultButton != null)
				{
					var handler = (ButtonHandler)defaultButton.Handler;
					handler.Control.IsDefault = false;
				}
				defaultButton = value;
				if (defaultButton != null)
				{
					var handler = (ButtonHandler)defaultButton.Handler;
					handler.Control.IsDefault = true;
				}
			}
		}

		public Button AbortButton
		{
			get { return abortButton; }
			set
			{
				if (abortButton != null)
				{
					var handler = (ButtonHandler)abortButton.Handler;
					handler.Control.IsCancel = false;
				}
				abortButton = value;
				if (abortButton != null)
				{
					var handler = (ButtonHandler)abortButton.Handler;
					handler.Control.IsCancel = true;
				}
			}
		}
	}
}
