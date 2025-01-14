﻿#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider
{
	/// <summary>
	/// <see cref="IViewProvider"/> that creates views for a given view model instance if both the views and the view models are located within the same assembly.
	/// </summary>
	/// <example>
	/// <para> […].ViewModels.MainWindowModel → […].Views.MainWindow </para>
	/// <para> […].ViewModels.DetailsViewModel → […].Views.DetailsView </para>
	/// </example>
	public class DefaultViewProvider : IViewProvider
	{
		#region Delegates / Events

		/// <inheritdoc />
		public event EventHandler<ViewLoadedEventArgs> ViewLoaded;

		/// <summary> Raises <see cref="ViewLoaded"/> </summary>
		protected void OnViewLoaded<TClass>(TClass viewModel, FrameworkElement view) where TClass : class
		{
			this.ViewLoaded?.Invoke(this, new ViewLoadedEventArgs(viewModel, view));
		}

		#endregion

		#region Constants
		#endregion

		#region Fields

		/// <summary> Caches already resolved views via their view models as key. </summary>
		private readonly ConcurrentDictionary<Type, Type> _viewModelToViewMappings;

		/// <summary> A collection of callbacks invoked once the view for a view model has been resolved. </summary>
		[Obsolete("Instead of view model setup callbacks, use the event based mechanism via 'IViewProvider.ViewLoaded'.")]
		private readonly ICollection<Action<object, FrameworkElement>> _viewModelSetupCallbacks;

		#endregion

		#region Properties

		/// <summary> The part of the view models namespace that will be replaced with <see cref="ViewNamespaceSuffix"/>. </summary>
		protected internal string ViewModelNamespaceSuffix { get; }

		/// <summary> The namespace of the views. </summary>
		protected internal string ViewNamespaceSuffix { get; }

		/// <summary> The part of the view models name that will be replaced with <see cref="ViewNameSuffix"/>. </summary>
		protected internal string ViewModelNameSuffix { get; }

		/// <summary> The suffix of the views. </summary>
		protected internal string ViewNameSuffix { get; }

		#endregion

		#region (De)Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public DefaultViewProvider()
#pragma warning disable 618
			: this
			(
				new Action<object, FrameworkElement>[0]
			) { }
#pragma warning restore 618

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewModelNamespaceSuffix"> The namespace of the view models. </param>
		/// <param name="viewNamespaceSuffix"> The namespace of the views. </param>
		/// <param name="viewModelNameSuffix"> The suffix of the view models. </param>
		/// <param name="viewNameSuffix"> The suffix of the views. </param>
		public DefaultViewProvider
		(
			string viewModelNamespaceSuffix,
			string viewNamespaceSuffix,
			string viewModelNameSuffix,
			string viewNameSuffix
#pragma warning disable 618
		) : this
		(
				viewModelNamespaceSuffix,
				viewNamespaceSuffix,
				viewModelNameSuffix,
				viewNameSuffix,
				new Action<object, FrameworkElement>[0]
			)
		{ }
#pragma warning restore 618

		/// <summary>
		/// Constructor with default values for the multiple view/viewmodel name configurations.
		/// </summary>
		/// <param name="viewModelSetupCallbacks"> A collection of callbacks invoked once the view for a view model has been resolved. </param>
		/// <remarks>
		/// <para> The default values are: </para>
		/// <para> <see cref="ViewModelNamespaceSuffix"/>: ViewModels </para>
		/// <para> <see cref="ViewNamespaceSuffix"/>: Views </para>
		/// <para> <see cref="ViewModelNameSuffix"/>: Model </para>
		/// <para> <see cref="ViewNameSuffix"/>: [EMPTY] </para>
		/// </remarks>
		[Obsolete("Instead of view model setup callbacks, use the event based mechanism via 'IViewProvider.ViewLoaded'.")]
		public DefaultViewProvider(params Action<object, FrameworkElement>[] viewModelSetupCallbacks)
			: this
			(
				viewModelNamespaceSuffix: "ViewModels",
				viewNamespaceSuffix: "Views",
				viewModelNameSuffix: "Model",
				viewNameSuffix: "",
				viewModelSetupCallbacks
			)
		{ }

		/// <summary>
		/// Constructor with default values for the multiple view/viewmodel name configurations.
		/// </summary>
		/// <param name="viewModelSetupCallbacks"> A collection of callbacks invoked once the view for a view model has been resolved. </param>
		/// <remarks>
		/// <para> The default values are: </para>
		/// <para> <see cref="ViewModelNamespaceSuffix"/>: ViewModels </para>
		/// <para> <see cref="ViewNamespaceSuffix"/>: Views </para>
		/// <para> <see cref="ViewModelNameSuffix"/>: Model </para>
		/// <para> <see cref="ViewNameSuffix"/>: [EMPTY] </para>
		/// </remarks>
		[Obsolete("Instead of view model setup callbacks, use the event based mechanism via 'IViewProvider.ViewLoaded'.")]
		public DefaultViewProvider(ICollection<ViewModelSetupCallback> viewModelSetupCallbacks)
			: this
			(
				viewModelNamespaceSuffix: "ViewModels",
				viewNamespaceSuffix: "Views",
				viewModelNameSuffix: "Model",
				viewNameSuffix: "",
				viewModelSetupCallbacks.Select(callback => (Action<object, FrameworkElement>)callback).ToArray()
			)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewModelNamespaceSuffix"> The namespace of the view models. </param>
		/// <param name="viewNamespaceSuffix"> The namespace of the views. </param>
		/// <param name="viewModelNameSuffix"> The suffix of the view models. </param>
		/// <param name="viewNameSuffix"> The suffix of the views. </param>
		/// <param name="viewModelSetupCallbacks"> A collection of callbacks invoked once the view for a view model has been resolved. </param>
		[Obsolete("Instead of view model setup callbacks, use the event based mechanism via 'IViewProvider.ViewLoaded'.")]
		public DefaultViewProvider
		(
			string viewModelNamespaceSuffix,
			string viewNamespaceSuffix,
			string viewModelNameSuffix,
			string viewNameSuffix,
			ICollection<ViewModelSetupCallback> viewModelSetupCallbacks
		)
			: this
			(
				viewModelNamespaceSuffix,
				viewNamespaceSuffix,
				viewModelNameSuffix,
				viewNameSuffix,
				viewModelSetupCallbacks.Select(callback => (Action<object, FrameworkElement>)callback).ToArray()
			)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewModelNamespaceSuffix"> The namespace of the view models. </param>
		/// <param name="viewNamespaceSuffix"> The namespace of the views. </param>
		/// <param name="viewModelNameSuffix"> The suffix of the view models. </param>
		/// <param name="viewNameSuffix"> The suffix of the views. </param>
		/// <param name="viewModelSetupCallbacks"> A collection of callbacks invoked once the view for a view model has been resolved. </param>
		[Obsolete("Instead of view model setup callbacks, use the event based mechanism via 'IViewProvider.ViewLoaded'.")]
		public DefaultViewProvider
		(
			string viewModelNamespaceSuffix,
			string viewNamespaceSuffix,
			string viewModelNameSuffix,
			string viewNameSuffix,
			params Action<object, FrameworkElement>[] viewModelSetupCallbacks
		)
		{
			// Save parameters.
			_viewModelSetupCallbacks = viewModelSetupCallbacks ?? new Action<object, FrameworkElement>[0];
			this.ViewNameSuffix = viewNameSuffix;
			this.ViewNamespaceSuffix = viewNamespaceSuffix;
			this.ViewModelNameSuffix = viewModelNameSuffix;
			this.ViewModelNamespaceSuffix = viewModelNamespaceSuffix;

			// Initialize fields.
			_viewModelToViewMappings = new ConcurrentDictionary<Type, Type>();
		}

		#endregion

		#region Methods

		#region IViewProvider
		
		/// <inheritdoc />
		public virtual FrameworkElement GetViewInstance<TClass>(TClass viewModel) where TClass : class
			=> this.GetViewInstance(viewModel, null);

		/// <inheritdoc />
		public virtual FrameworkElement GetViewInstance<TClass>(TClass viewModel, Assembly viewAssembly) where TClass : class
		{
			if (viewModel is null) return null;

			// Get the type of the view model.
			var viewModelType = viewModel.GetType();

			try
			{
				// Get the view type.
				var viewType = _viewModelToViewMappings.GetOrAdd(viewModelType, type => this.GetViewType(type, viewAssembly));

				// Create an instance from it.
				var view = this.CreateViewInstance(viewType);

				// Always(!) set the data context.
				view.DataContext = viewModel;

				// Further setup the view model.
				this.OnViewLoaded(viewModel, view);
				this.SetupViewModel(viewModel, view);

				// Return it.
				return view;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"{this.GetType().Name.ToUpper()}: {ex.Message}");
				throw;
			}
		}

		private Type GetViewType(Type viewModelType, Assembly viewAssembly)
		{
			// Get the expected full name of the view.
			var viewFullName = this.GetViewFullName(viewModelType, this.ViewModelNamespaceSuffix, this.ViewNamespaceSuffix, this.ViewModelNameSuffix, this.ViewNameSuffix);

			// Get the assemblies that contain a the view for the view model. Theoretically this could be many assemblies, in the end only the first found view will be used.
			var viewAssemblies = this.GetViewAssemblies(viewModelType, viewAssembly);
			if (!viewAssemblies.Any()) throw new ViewProviderException($"Could not find assemblies containing the views for the view model '{viewModelType.FullName}'.");

			// Get the view type.
			var viewTypes = this.GetViewTypes(viewFullName, viewAssemblies);

			// Check if only one view type was found.
			if (viewTypes.Length == 0) throw new ViewProviderException($"Could not find a view named '{viewFullName}' for the view model '{viewModelType.FullName}' in the assemblies '{String.Join(", ", viewAssemblies.Select(assembly => assembly.GetName().Name))}'.");
			if (viewTypes.Length > 1) Trace.WriteLine($"{this.GetType().Name.ToUpper()}: More than one view matches the name '{viewFullName}' for the view model '{viewModelType.FullName}': '{String.Join(", ", viewTypes.Select(type => type.FullName))}'. The first one will be used.");

			// Return the first and view type.
			return viewTypes.First();
		}

		#endregion

		#region Name Building

		/// <summary>
		/// Builds a full view name from the <paramref name="viewModelType"/>.
		/// </summary>
		/// <param name="viewModelType"> The <see cref="Type"/> of the view model. </param>
		/// <param name="viewModelNamespaceSuffix"> The part of the view models namespace that should be replaced with <paramref name="viewNamespaceSuffix"/>. </param>
		/// <param name="viewNamespaceSuffix"> The suffix of the views namespace. </param>
		/// <param name="viewModelNameSuffix"> The part of the view models name that will be replaced with <paramref name="viewNameSuffix"/>. </param>
		/// <param name="viewNameSuffix"> The suffix of the views name. </param>
		/// <returns>
		/// <para> The full name for the view. </para>
		/// <para> Should return <c>Null</c> or an empty string if the full name of the view could not be resolved. </para>
		/// </returns>
		protected internal virtual string GetViewFullName(Type viewModelType, string viewModelNamespaceSuffix, string viewNamespaceSuffix, string viewModelNameSuffix, string viewNameSuffix)
		{
			// Get namespace and name of the view.
			var viewNamespace = this.GetViewNamespace(viewModelType, viewModelNamespaceSuffix, viewNamespaceSuffix);
			var viewName = this.GetViewName(viewModelType, viewModelNameSuffix, viewNameSuffix);

			if (String.IsNullOrWhiteSpace(viewNamespace) || String.IsNullOrWhiteSpace(viewName)) throw new ViewProviderException($"Could not build a view name from the view model '{viewModelType.FullName}'.");

			// Merge them together and return the result.
			var viewFullName = $"{viewNamespace}.{viewName}";
			Debug.WriteLine($"{this.GetType().Name.ToUpper()}: Transformed view model name '{viewModelType.FullName}' into '{viewFullName}'.");
			return viewFullName;
		}

		/// <summary>
		/// Builds the namespace of the view from the <paramref name="viewModelType"/>.
		/// </summary>
		/// <param name="viewModelType"> The <see cref="Type"/> of the view model. </param>
		/// <param name="viewModelNamespaceSuffix"> The part of the view models namespace that should be replaced with <paramref name="viewNamespaceSuffix"/>. </param>
		/// <param name="viewNamespaceSuffix"> The suffix of the views namespace. </param>
		/// <returns>
		/// <para> The expected namespace of the view. </para>
		/// <para> Should return <c>Null</c> or an empty string if the namespace of the view could not be resolved. </para>
		/// </returns>
		protected internal virtual string GetViewNamespace(Type viewModelType, string viewModelNamespaceSuffix, string viewNamespaceSuffix)
		{
			var viewModelNamespace = viewModelType.Namespace;
			if (String.IsNullOrWhiteSpace(viewModelNamespace)) return null;

			// Clean the namespace of the view model if necessary.
			if (!String.IsNullOrWhiteSpace(viewModelNamespaceSuffix))
			{
				var index = viewModelNamespace.LastIndexOf(viewModelNamespaceSuffix, StringComparison.OrdinalIgnoreCase);
				if (index == -1) throw new ViewProviderException($"The view model of type '{viewModelType.Name}' does not reside in the defined namespace '{viewModelNamespaceSuffix}'.");
				viewModelNamespace = viewModelNamespace.Remove(index).TrimEnd('.');
			}

			// Build the namespace of the view.
			var viewNamespace = $"{viewModelNamespace}{(String.IsNullOrWhiteSpace(viewNamespaceSuffix) ? "" : ".")}{viewNamespaceSuffix}";
			return viewNamespace;
		}

		/// <summary>
		/// Builds the view name from the <paramref name="viewModelType"/>.
		/// </summary>
		/// <param name="viewModelType"> The <see cref="Type"/> of the view model. </param>
		/// <param name="viewModelNameSuffix"> The part of the view models name that will be replaced with <paramref name="viewNameSuffix"/>. </param>
		/// <param name="viewNameSuffix"> The suffix of the views name. </param>
		/// <returns>
		/// <para> The expected name for the view. </para>
		/// <para> Should return <c>Null</c> or an empty string if the name of the view could not be resolved. </para>
		/// </returns>
		protected internal virtual string GetViewName(Type viewModelType, string viewModelNameSuffix, string viewNameSuffix)
		{
			// Clean the name of the view model if necessary.
			var viewModelName = viewModelType.Name;
			if (!String.IsNullOrWhiteSpace(viewModelNameSuffix))
			{
				var index = viewModelName.LastIndexOf(viewModelNameSuffix, StringComparison.Ordinal);
				if (index == -1) throw new ViewProviderException($"The view model of type '{viewModelType.Name}' does not end with the defined suffix '{viewModelNameSuffix}'.");
				viewModelName = viewModelName.Remove(index);
			}

			// Build the name of the view.
			var viewName = $"{viewModelName}{viewNameSuffix}";
			return viewName;
		}

		#endregion

		#region Type Lookup

		/// <summary>
		/// Gets assemblies that contain views.
		/// </summary>
		/// <param name="viewModelType"> The <see cref="Type"/> of the view model. </param>
		/// <param name="viewAssembly"> Optional <see cref="Assembly"/> that contains the view. Default is <c>Null</c>. Is set in case the assembly is already known. </param>
		/// <returns>
		/// <para> A collection of <see cref="Assembly"/>s that contain views. </para>
		/// <para> Should return <c>Null</c> or an empty collection if no view assemblies could be resolved. </para>
		/// </returns>
		protected internal virtual Assembly[] GetViewAssemblies(Type viewModelType, Assembly viewAssembly = null)
		{
			return new[] { viewAssembly ?? viewModelType.Assembly };
		}

		/// <summary>
		/// Gets the view type from the <paramref name="viewAssemblies"/> matching the <paramref name="viewFullName"/>.
		/// </summary>
		/// <param name="viewFullName"> The full name of the view. </param>
		/// <param name="viewAssemblies"> The <see cref="Assembly"/>s containing views. </param>
		/// <returns>
		/// <para> All matching view types. </para>
		/// <para> Should return <c>Null</c> or an empty collection if no view types could be resolved. </para>
		/// </returns>
		protected internal virtual Type[] GetViewTypes(string viewFullName, Assembly[] viewAssemblies)
		{
			var viewTypes = viewAssemblies
				.Select(assembly => assembly.GetType(viewFullName))
				.Where(type => type != null)
				.ToArray()
				;

			return viewTypes;
		}

		#endregion

		#region Creation

		/// <summary>
		/// Creates an instance of the <paramref name="viewType"/>.
		/// </summary>
		/// <param name="viewType"> The <see cref="Type"/> of the view of which an instance should be created. </param>
		/// <returns>
		/// <para> A new instance of the <paramref name="viewType"/>. </para>
		/// <para> Should return <c>Null</c> if no view instance could be created. </para>
		/// </returns>
		protected internal virtual FrameworkElement CreateViewInstance(Type viewType)
		{
			//return this.ProviderConfiguration.ViewCreationCallback.Invoke(viewType);
			if (viewType.IsAbstract) throw new ViewProviderException($"The view '{viewType.FullName}' is abstract and cannot be instantiated.");
			if (viewType.IsValueType) throw new ViewProviderException($"The view '{viewType.FullName}' is a value type which is not supported.");
			if (!typeof(FrameworkElement).IsAssignableFrom(viewType)) throw new ViewProviderException($"The view '{viewType.FullName}' is not a {nameof(FrameworkElement)}.");

			Func<FrameworkElement> factory = () => Activator.CreateInstance(viewType) as FrameworkElement;
			if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
			{
				return factory.Invoke();
			}
			else
			{
				// First try to use the dispatcher.
				var dispatcher = Application.Current is null ? Dispatcher.CurrentDispatcher : Application.Current.Dispatcher;
				if (dispatcher != null)
				{
					try
					{
						return dispatcher?.Invoke(() => factory.Invoke(), DispatcherPriority.Normal);
					}
					catch (TargetInvocationException) { /* ignore */ }
				}

				// Try to promote the current thread to STA.
				var success = Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
				if (success)
				{
					return factory.Invoke();
				}
				else
				{
					throw new ViewProviderException($"The view '{viewType.FullName}' has to be created from a {ApartmentState.STA} thread and promoting the current one failed.");
				}
			}
		}

		#endregion

		#region Modification

		/// <summary>
		/// Allows further modification of the <paramref name="viewModel"/> like property injection or adding callbacks to some events of the <paramref name="view"/>.
		/// </summary>
		/// <typeparam name="TClass"> The <see cref="Type"/> of the <paramref name="viewModel"/>. </typeparam>
		/// <param name="viewModel"> The view model. </param>
		/// <param name="view"> The view as <see cref="FrameworkElement"/>. </param>
		/// <remarks>
		/// <para> • The <paramref name="view"/>s <see cref="FrameworkElement.DataContext"/> is already set to the <paramref name="viewModel"/> by now. </para>
		/// <para> • Either override this to implement custom / advanced binding or simply supply custom callbacks with a signature of <see cref="Action{T1, T2}"/> where <c>T1</c> is <see cref="object"/> and <c>T2</c> is <see cref="FrameworkElement"/> that will be invoked from this method. </para>
		/// </remarks>
		[Obsolete("Instead of view model setup callbacks, use the event based mechanism via 'IViewProvider.ViewLoaded'.")]
		protected virtual void SetupViewModel<TClass>(TClass viewModel, FrameworkElement view) where TClass : class
		{
			foreach (var callbacks in _viewModelSetupCallbacks)
			{
				callbacks.Invoke(viewModel, view);
			}
		}

		#endregion

		#endregion
	}
}