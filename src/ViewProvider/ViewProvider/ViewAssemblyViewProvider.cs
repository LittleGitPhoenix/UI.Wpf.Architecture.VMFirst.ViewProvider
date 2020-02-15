#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider
{
	/// <summary>
	/// Custom <see cref="DefaultViewProvider"/> that searches discrete view assemblies for views matching view models. This is done with the help of the <see cref="ViewAssemblyProvider"/>.
	/// </summary>
	/// <remarks> The <see cref="DefaultViewProvider.ViewModelNamespaceSuffix"/> and <see cref="DefaultViewProvider.ViewNamespaceSuffix"/> values are not used by this implementation, as the namespace between different assemblies probably won't match. </remarks>
	/// <example>
	/// <para> ViewAssembly\...\Views\MainWindow -> ViewModelAssembly\...\ViewModels\MainWindowModel </para>
	/// <para> ViewAssembly\...\Views\DetailsView -> ViewModelAssembly\...\ViewModels\DetailsViewModel </para>
	/// </example>
	public sealed class ViewAssemblyViewProvider : DefaultViewProvider
	{
		#region Delegates / Events
		#endregion

		#region Constants
		#endregion

		#region Fields
		#endregion

		#region Properties
		#endregion

		#region (De)Constructors

		/// <summary>
		/// Constructor with default values for the multiple view/viewmodel name configurations.
		/// </summary>
		/// <param name="viewModelSetupCallbacks"> A collection of callbacks invoked once the view for a view model has been resolved. </param>
		/// <remarks>
		/// <para> The default values are: </para>
		/// <para> <see cref="DefaultViewProvider.ViewModelNameSuffix"/>: Model </para>
		/// <para> <see cref="DefaultViewProvider.ViewNameSuffix"/>: [EMPTY] </para>
		/// </remarks>
		public ViewAssemblyViewProvider(params Action<object, FrameworkElement>[] viewModelSetupCallbacks)
			: this
			(
				viewModelNameSuffix: "Model",
				viewNameSuffix: "",
				viewModelSetupCallbacks
			)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="viewModelNameSuffix"> The suffix of the view models. </param>
		/// <param name="viewNameSuffix"> The suffix of the views. </param>
		/// <param name="viewModelSetupCallbacks"> A collection of callbacks invoked once the view for a view model has been resolved. </param>
		public ViewAssemblyViewProvider
		(
			string viewModelNameSuffix,
			string viewNameSuffix,
			params Action<object, FrameworkElement>[] viewModelSetupCallbacks
		)
			: base(null, null, viewModelNameSuffix, viewNameSuffix, viewModelSetupCallbacks) { }

		#endregion

		#region Methods

		#region Overrides of DefaultViewProvider
		
		/// <inheritdoc />
		protected internal override string GetViewFullName(Type viewModelType, string viewModelNamespaceSuffix, string viewNamespaceSuffix, string viewModelNameSuffix, string viewNameSuffix)
		{
			//! Fullname matching probably won't work, due to the namespaces not matching between different assemblies.
			//! So return the pure view name which later on is used to find a matching type in the overridden 'GetViewTypes' function.
			return base.GetViewName(viewModelType, viewModelNameSuffix, viewNameSuffix);
		}

		/// <inheritdoc />
		/// <remarks> Delegates its responsibility to <see cref="ViewAssemblyProvider.GetViewAssemblies"/>. </remarks>
		protected internal override Assembly[] GetViewAssemblies(Type viewModelType, Assembly viewAssembly = null)
		{
			return viewAssembly is null ? ViewAssemblyProvider.GetViewAssemblies(viewModelType) : new[] { viewAssembly };
		}

		/// <inheritdoc />
		protected internal override Type[] GetViewTypes(string viewFullName, Assembly[] viewAssemblies)
		{
			var viewName = viewFullName;

			var viewTypes = viewAssemblies
				.SelectMany(assembly => assembly.GetExportedTypes())
				.Where(type => type.Name == viewName)
				.ToArray()
				;

			return viewTypes;
		}

		#endregion

		#endregion
	}
}