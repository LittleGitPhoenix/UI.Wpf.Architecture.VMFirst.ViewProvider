#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider
{
	/// <summary>
	/// Helper class for wrapping and converting view model setup callbacks.
	/// </summary>
	/// <remarks> The main purpose of this class is to wrap callback actions so they can registered with a IOC.  </remarks>
	public class ViewModelSetupCallback
	{
		private readonly Action<object, FrameworkElement> _setupCallback;

		private ViewModelSetupCallback(Action<object, FrameworkElement> setupCallback)
		{
			_setupCallback = setupCallback;
		}

		/// <summary>
		/// Converts the <paramref name="setupCallback"/> into an instance of <see cref="ViewModelSetupCallback"/>.
		/// </summary>
		/// <param name="setupCallback"> The setup callback with a signature of <see cref="Action{T1, T2}"/> where <c>T1</c> is <see cref="object"/> and <c>T2</c> is <see cref="FrameworkElement"/>. </param>
		public static explicit operator ViewModelSetupCallback(Action<object, FrameworkElement> setupCallback)
		{
			return new ViewModelSetupCallback(setupCallback);
		}

		/// <summary>
		/// Converts the <paramref name="viewModelSetupCallback"/> back into an <see cref="Action{T1, T2}"/> where <c>T1</c> is <see cref="object"/> and <c>T2</c> is <see cref="FrameworkElement"/>.
		/// </summary>
		/// <param name="viewModelSetupCallback"> THe callback action.</param>
		public static implicit operator Action<object, FrameworkElement>(ViewModelSetupCallback viewModelSetupCallback)
		{
			return viewModelSetupCallback._setupCallback;
		}
	}
}