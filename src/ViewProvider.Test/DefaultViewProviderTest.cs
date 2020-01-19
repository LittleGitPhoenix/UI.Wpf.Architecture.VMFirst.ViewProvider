using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using Moq;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider;

namespace Architecture.VMFirst.ViewProvider.Test
{
	[TestFixture]
	public class DefaultViewProviderTest
	{
		#region GetViewInstance

		[Test]
		public void GetViewInstance_From_Non_STA_Throws()
		{
			var model = new ViewModels.SomeViewModel();
			IViewProvider viewProvider = new DefaultViewProvider();
			Assert.Throws<ViewProviderException>(() => viewProvider.GetViewInstance(model));
		}

		[Test]
		[Apartment(ApartmentState.STA)]
		public void GetViewInstance_Succeeds()
		{
			var model = new ViewModels.SomeViewModel();
			var targetViewType = typeof(Views.SomeView);
			IViewProvider viewProvider = new DefaultViewProvider();
			var view = viewProvider.GetViewInstance(model);
			Assert.AreEqual(targetViewType, view.GetType());
		}

		#endregion

		#region GetViewAssemblies

		[Test]
		public void GetViewAssemblies_Succeeds()
		{
			var viewProvider = new DefaultViewProvider();
			var viewAssemblies = viewProvider.GetViewAssemblies(typeof(SomeViewModel));
			
			Assert.AreEqual(1, viewAssemblies.Length);
			Assert.AreEqual(Assembly.GetExecutingAssembly(), viewAssemblies.First());
		}

		#endregion

		#region GetViewTypes

		[Test]
		public void GetViewTypes_Succeeds()
		{
			var viewProvider = new DefaultViewProvider();
			var targetViewType = typeof(SomeView);
			var viewFullName = $"{targetViewType.Namespace}.{targetViewType.Name}";
			var viewTypes = viewProvider.GetViewTypes(viewFullName, new[] { Assembly.GetExecutingAssembly() });

			Assert.That( viewTypes.Length, Is.EqualTo(1));
			Assert.That(viewTypes.First(), Is.EqualTo(typeof(SomeView)));
		}

		[Test]
		public void GetViewTypes_Fails()
		{
			var viewProvider = new DefaultViewProvider();
			var viewFullName = Guid.NewGuid().ToString();
			var viewTypes = viewProvider.GetViewTypes(viewFullName, new[] { Assembly.GetExecutingAssembly() });
			Assert.That(viewTypes, Is.Empty);
		}

		#endregion

		#region View Name

		[Test]
		public void GetViewName_With_Default_Values_Succeeds()
		{
			var viewProvider = new DefaultViewProvider();
			var viewModelType = typeof(SomeViewModel);

			var viewName = viewProvider.GetViewName(viewModelType, viewProvider.ViewModelNameSuffix, viewProvider.ViewNameSuffix);

			Assert.That(viewName, Is.EqualTo(typeof(SomeView).Name));
		}

		[Test]
		public void GetViewName_With_Custom_Values_Succeeds()
		{
			var viewModelType = typeof(SomeViewModel);
			var viewProvider = new DefaultViewProvider
			(
				viewModelNamespaceSuffix: "",
				viewNamespaceSuffix: "",
				viewModelNameSuffix: "del",
				viewNameSuffix: "hawk"
			);

			var viewName = viewProvider.GetViewName(viewModelType, viewProvider.ViewModelNameSuffix, viewProvider.ViewNameSuffix);
			Assert.That(viewName, Is.EqualTo("SomeViewMohawk"));
		}

		[Test]
		public void GetViewName_Without_ViewModelNameSuffix_Succeeds()
		{
			var viewModelType = typeof(SomeViewModel);
			var viewProvider = new DefaultViewProvider
			(
				viewModelNamespaceSuffix: "",
				viewNamespaceSuffix: "",
				viewModelNameSuffix: "",
				viewNameSuffix: ""
			);

			var viewName = viewProvider.GetViewName(viewModelType, viewProvider.ViewModelNameSuffix, viewProvider.ViewNameSuffix);
			Assert.That(viewName, Is.EqualTo(viewModelType.Name));
		}

		[Test]
		public void GetViewName_Throws()
		{
			var viewModelType = typeof(SomeViewModel);
			var viewProvider = new DefaultViewProvider
			(
				viewModelNamespaceSuffix: "",
				viewNamespaceSuffix: "",
				viewModelNameSuffix: Guid.NewGuid().ToString(), // This is not the name you are looking for.
				viewNameSuffix: ""
			);

			// The defined view model suffix does not match.
			Assert.Throws<ViewProviderException>(() => viewProvider.GetViewName(viewModelType, viewProvider.ViewModelNameSuffix, viewProvider.ViewNameSuffix));
		}

		#endregion

		#region View Namespace

		[Test]
		public void GetViewNamespace_With_Default_NamespaceSuffix_Succeeds()
		{
			var viewModelType = typeof(ViewModels.SomeViewModel);
			var targetViewNamespace = typeof(Views.SomeView).Namespace;
			var viewProvider = new DefaultViewProvider();

			var viewNamespace = viewProvider.GetViewNamespace(viewModelType, viewProvider.ViewModelNamespaceSuffix, viewProvider.ViewNamespaceSuffix);
			Assert.That(viewNamespace, Is.EqualTo(targetViewNamespace));
		}

		[Test]
		public void GetViewNamespace_With_Custom_NamespaceSuffix_Succeeds()
		{
			var viewModelType = typeof(VM.SomeViewModel);
			var targetViewNamespace = typeof(V.SomeView).Namespace;
			var viewProvider = new DefaultViewProvider
			(
				viewModelNamespaceSuffix: "VM",
				viewNamespaceSuffix: "V",
				viewModelNameSuffix: "",
				viewNameSuffix: ""
			);

			var viewNamespace = viewProvider.GetViewNamespace(viewModelType, viewProvider.ViewModelNamespaceSuffix, viewProvider.ViewNamespaceSuffix);
			Assert.That(viewNamespace, Is.EqualTo(targetViewNamespace));
		}

		[Test]
		public void GetViewNamespace_Without_NamespaceSuffix_Succeeds()
		{
			var viewModelType = typeof(SomeViewModel);
			var targetViewNamespace = viewModelType.Namespace;
			var viewProvider = new DefaultViewProvider
			(
				viewModelNamespaceSuffix: String.Empty,
				viewNamespaceSuffix: String.Empty,
				viewModelNameSuffix: "",
				viewNameSuffix: ""
			);

			var viewNamespace = viewProvider.GetViewNamespace(viewModelType, viewProvider.ViewModelNamespaceSuffix, viewProvider.ViewNamespaceSuffix);
			Assert.AreEqual(targetViewNamespace, viewNamespace);
		}

		[Test]
		public void GetViewNamespace_Throws()
		{
			var viewModelType = typeof(SomeViewModel);
			var viewProvider = new DefaultViewProvider
			(
				viewModelNamespaceSuffix: Guid.NewGuid().ToString(), // This is not the namespace you are looking for.
				viewNamespaceSuffix: "",
				viewModelNameSuffix: "", 
				viewNameSuffix: ""
			);

			// The defined namespace of the view models does not match.
			Assert.Throws<ViewProviderException>(() => viewProvider.GetViewNamespace(viewModelType, viewProvider.ViewModelNamespaceSuffix, viewProvider.ViewNamespaceSuffix));
		}

		#endregion

		#region View FullName

		[Test]
		public void GetViewFullName_No_Name_Throws()
		{
			var mockedViewProvider = new Mock<DefaultViewProvider>(MockBehavior.Strict);
			mockedViewProvider
				.Setup(provider => provider.GetViewName(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Guid.NewGuid().ToString())
				;
			mockedViewProvider
				.Setup(provider => provider.GetViewNamespace(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(String.Empty)
				;
			mockedViewProvider
				.Setup(provider => provider.GetViewFullName(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.CallBase()
				;

			var viewProvider = mockedViewProvider.Object;
			var viewModelType = typeof(SomeViewModel);
			Assert.Throws<ViewProviderException>(() => viewProvider.GetViewFullName(viewModelType, viewProvider.ViewModelNamespaceSuffix, viewProvider.ViewNamespaceSuffix, viewProvider.ViewModelNameSuffix, viewProvider.ViewNameSuffix));
		}

		[Test]
		public void GetViewFullName_No_Namespace_Throws()
		{
			var mockedViewProvider = new Mock<DefaultViewProvider>(MockBehavior.Strict);
			mockedViewProvider
				.Setup(provider => provider.GetViewName(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(String.Empty)
				;
			mockedViewProvider
				.Setup(provider => provider.GetViewNamespace(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Guid.NewGuid().ToString())
				;
			mockedViewProvider
				.Setup(provider => provider.GetViewFullName(It.IsAny<Type>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.CallBase()
				;
			
			var viewProvider = mockedViewProvider.Object;
			var viewModelType = typeof(SomeViewModel);
			Assert.Throws<ViewProviderException>(() => viewProvider.GetViewFullName(viewModelType, viewProvider.ViewModelNamespaceSuffix, viewProvider.ViewNamespaceSuffix, viewProvider.ViewModelNameSuffix, viewProvider.ViewNameSuffix));
		}

		#endregion
	}
}