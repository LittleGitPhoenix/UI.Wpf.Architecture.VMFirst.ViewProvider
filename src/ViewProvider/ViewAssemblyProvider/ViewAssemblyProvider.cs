#region LICENSE NOTICE
//! This file is subject to the terms and conditions defined in file 'LICENSE.md', which is part of this source code package.
#endregion


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider
{
	/// <summary>
	/// Searches and continuously updates all loaded assemblies for instances of <see cref="IViewHolder"/> implementations and stores the so provided <see cref="IViewHolder.ViewModelAssemblies"/>.
	/// </summary>
	/// <remarks> This class is used by <see cref="ViewAssemblyViewProvider"/> to resolve the assemblies containing views for view models. </remarks>
	internal static class ViewAssemblyProvider
	{
		#region Delegates / Events
		#endregion

		#region Constants
		#endregion

		#region Fields

		/// <summary>
		/// Mapping of view model assemblies to view assemblies.
		/// </summary>
		private static readonly List<AssemblyMapping> AssemblyMappings;

		private static readonly object AssemblyMappingsLock;

		#endregion

		#region Properties
		#endregion

		#region (De)Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		static ViewAssemblyProvider()
		{
			// Initialize fields.
			ViewAssemblyProvider.AssemblyMappings = new List<AssemblyMapping>();
			ViewAssemblyProvider.AssemblyMappingsLock = new object();

			// Start watching loaded assemblies for IViewHolders.
			ViewAssemblyProvider.UpdatedLoadedAssemblies();
			AppDomain.CurrentDomain.AssemblyLoad += ViewAssemblyProvider.AssemblyLoaded;
		}

		#endregion

		#region Methods

		private static void UpdatedLoadedAssemblies()
		{
			AppDomain.CurrentDomain
				.GetAssemblies()
				.ToList()
				.ForEach(ViewAssemblyProvider.TryAddNewAssembly)
				;
		}

		private static void AssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
			var loadedAssembly = args.LoadedAssembly;
			ViewAssemblyProvider.TryAddNewAssembly(loadedAssembly);
		}

		private static void TryAddNewAssembly(Assembly newAssembly)
		{
			if (newAssembly.IsDynamic) return;
			if (newAssembly.GetName().Name.StartsWith("Microsoft.VisualStudio.")) return; // Hopefully no one names their assemblies this way. Removing this may lead to a 'FileNotFoundException' for 'Microsoft.VisualStudio.Text.Data' in design mode when opening a dialog within the loaded event of a view.

			// Check if the assembly contains a IViewHolder.
			var viewHolders = newAssembly
				.GetExportedTypes()
				.Where(type => !type.IsAbstract && !type.IsValueType && typeof(IViewHolder).IsAssignableFrom(type))
				.Select(viewHolderType => viewHolderType.GetConstructor(Type.EmptyTypes)) // Get the default constructor.
				.Where(viewHolderConstructor => viewHolderConstructor != null)
				.Select(viewHolderConstructor => (IViewHolder) viewHolderConstructor.Invoke(null))
				.ToArray()
				;

			if (!viewHolders.Any()) return;

			lock (ViewAssemblyProvider.AssemblyMappingsLock)
			{
				foreach (var viewHolder in viewHolders)
				{
					foreach (var viewModelAssembly in viewHolder.ViewModelAssemblies)
					{
						Debug.WriteLine($"{nameof(ViewAssemblyProvider).ToUpper()}: Added new view model to view mapping: {viewModelAssembly.GetName().Name} -> {newAssembly.GetName().Name}");
						ViewAssemblyProvider.AssemblyMappings.Add(new AssemblyMapping(viewModelAssembly, newAssembly));
					}
				}
			}
		}

		/// <summary>
		/// Gets all available <see cref="Assembly"/>s that have mappings for the <paramref name="viewModelType"/>.
		/// </summary>
		/// <param name="viewModelType"> The <see cref="Type"/> of the view model to look for. </param>
		/// <returns> A collection of <see cref="Assembly"/>s that have registered mappings for the <paramref name="viewModelType"/>. </returns>
		public static Assembly[] GetViewAssemblies(Type viewModelType)
		{
			lock (ViewAssemblyProvider.AssemblyMappingsLock)
			{
				var viewModelAssembly = viewModelType.Assembly;
				var viewAssemblies = ViewAssemblyProvider.AssemblyMappings
					.Where(mapping => mapping.ViewModelAssembly == viewModelAssembly)
					.Select(mapping => mapping.ViewAssembly)
					.ToArray()
					;
				return viewAssemblies;
			}
		}

		#endregion

		#region Nested Types

		private class AssemblyMapping
		{
			internal Assembly ViewModelAssembly { get; }
			internal Assembly ViewAssembly { get; }

			public AssemblyMapping(Assembly viewModelAssembly, Assembly viewAssembly)
			{
				this.ViewModelAssembly = viewModelAssembly;
				this.ViewAssembly = viewAssembly;
			}
		}

		#endregion
	}
}