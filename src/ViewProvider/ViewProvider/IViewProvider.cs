#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Reflection;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider
{
	/// <summary>
	/// Interface for view providers.
	/// </summary>
	public interface IViewProvider
	{
		/// <summary>
		/// Builds a new view instance for the <paramref name="viewModel"/> and sets its <see cref="FrameworkElement.DataContext"/> to the <paramref name="viewModel"/>.
		/// </summary>
		/// <typeparam name="TClass"> The <see cref="Type"/> of the  <paramref name="viewModel"/>. </typeparam>
		/// <param name="viewModel"> The view model class. </param>
		/// <returns> A new instance of the view. Should return <c>Null</c> if no view could be resolved. </returns>
		FrameworkElement GetViewInstance<TClass>(TClass viewModel) where TClass : class;

		/// <summary>
		/// Builds a new view instance for the <paramref name="viewModel"/> and sets its <see cref="FrameworkElement.DataContext"/> to the <paramref name="viewModel"/>.
		/// </summary>
		/// <typeparam name="TClass"> The <see cref="Type"/> of the  <paramref name="viewModel"/>. </typeparam>
		/// <param name="viewModel"> The view model class. </param>
		/// <param name="viewAssembly"> The <see cref="Assembly"/> where the view is located in. </param>
		/// <returns>
		/// <para> A new instance of the view. </para>
		/// <para> Should return <c>Null</c> if no view could be resolved. </para>
		/// </returns>
		FrameworkElement GetViewInstance<TClass>(TClass viewModel, Assembly viewAssembly) where TClass : class;
	}
}