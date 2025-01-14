﻿#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider
{
	/// <summary>
	/// A collection of <see cref="IViewProvider"/>s that can be used in sequence to obtain a view for a view model.
	/// </summary>
	public class WrappingViewProvider : List<IViewProvider>, IViewProvider
	{
		#region Delegates / Events

		/// <inheritdoc />
		public event EventHandler<ViewLoadedEventArgs> ViewLoaded
		{
			add
			{
				foreach (var viewProvider in this) viewProvider.ViewLoaded += value;
			}
			remove
			{
				foreach (var viewProvider in this) viewProvider.ViewLoaded -= value;
			}
		}

		#endregion

		#region Constants
		#endregion

		#region Fields
		#endregion

		#region Properties
		#endregion

		#region (De)Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewProviders"></param>
		public WrappingViewProvider(IEnumerable<IViewProvider> viewProviders) : base(viewProviders) { }

		#endregion

		#region Methods
		
		/// <inheritdoc />
		public FrameworkElement GetViewInstance<TClass>(TClass viewModel) where TClass : class
			=> this.GetViewInstance(viewModel, viewAssembly: null);

		/// <inheritdoc />
		public FrameworkElement GetViewInstance<TClass>(TClass viewModel, Assembly viewAssembly) where TClass : class
		{
			if (viewModel is null) return null;

			foreach (var viewProvider in this)
			{
				try
				{
					return viewProvider.GetViewInstance(viewModel, viewAssembly);
				}
				catch (ViewProviderException)
				{
					/* Swallow all exceptions so that all providers are invoked until one finds the view. */
				}
			}

			// If the no view was found, throw an exception.
			if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
			throw new ViewProviderException($"Could not find a matching view for the view model '{viewModel.GetType().FullName}' in any of the available {nameof(IViewProvider)}s.");
		}

		#endregion
	}
}