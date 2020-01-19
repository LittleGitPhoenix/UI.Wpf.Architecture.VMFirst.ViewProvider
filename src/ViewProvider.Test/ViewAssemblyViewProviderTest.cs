using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Architecture.VMFirst.ViewProvider.Test.ViewAssembly;
using Architecture.VMFirst.ViewProvider.Test.ViewModelAssembly;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider;

namespace Architecture.VMFirst.ViewProvider.Test
{
	[TestFixture]
	public class ViewAssemblyViewProviderTest
	{
		#region GetViewInstance

		[Test]
		public void GetViewInstance_From_Non_STA_Throws()
		{
			var viewProvider = new ViewAssemblyViewProvider();
			Assert.Throws<ViewProviderException>(() => viewProvider.CreateViewInstance(typeof(SomeView)));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void GetViewInstance_Succeeds()
		{
			var model = new ViewModel();
			var targetViewType = typeof(View);
			var viewProvider = new ViewAssemblyViewProvider();
			var view = viewProvider.GetViewInstance(model);
			Assert.That(view, Is.TypeOf(targetViewType));
		}

		#endregion
	}
}