#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider
{
	/// <summary>
	/// Event argument when a view for a view model has been loaded.
	/// </summary>
	public class ViewLoadedEventArgs : EventArgs
	{
		/// <summary> The view model the <see cref="View"/> is bound to. </summary>
		public object ViewModel { get; }

		/// <summary> The <see cref="FrameworkElement"/> that is displayed. </summary>
		public FrameworkElement View { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewModel"> <see cref="ViewModel"/> </param>
		/// <param name="view"> <see cref="View"/> </param>
		public ViewLoadedEventArgs(object viewModel, FrameworkElement view)
		{
			this.ViewModel = viewModel;
			this.View = view;
		}
	}
}