using System.Collections.Generic;
using System.Reflection;
using Architecture.VMFirst.ViewProvider.Test.ViewModelAssembly;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider;

namespace Architecture.VMFirst.ViewProvider.Test.ViewAssembly
{
	public class ViewHolder : IViewHolder
	{
		/// <inheritdoc />
		public ICollection<Assembly> ViewModelAssemblies { get; } = new List<Assembly>() { typeof(ViewModel).Assembly };
	}
}