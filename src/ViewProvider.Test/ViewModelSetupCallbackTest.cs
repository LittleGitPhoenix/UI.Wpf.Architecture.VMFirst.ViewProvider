using System;
using System.Windows;
using NUnit.Framework;
using Phoenix.UI.Wpf.Architecture.VMFirst.ViewProvider;

namespace Architecture.VMFirst.ViewProvider.Test
{
	[TestFixture]
	public class ViewModelSetupCallbackTest
	{
		[Test]
		public void Check_Action_Is_Converted_To_ViewModelSetupCallback()
		{
			// Arrange
			Action<object, FrameworkElement> callback = (o, element) => { };

			// Act
			var viewModelSetupCallback = (ViewModelSetupCallback) callback;
			
			// Assert
			Assert.NotNull(viewModelSetupCallback);
		}

		[Test]
		public void Check_ViewModelSetupCallback_Is_Converted_To_Action()
		{
			// Arrange
			Action<object, FrameworkElement> callback = (o, element) => { };

			// Act
			var convertedCallback = (Action<object, FrameworkElement>) ((ViewModelSetupCallback) callback);

			// Assert
			Assert.AreSame(callback, convertedCallback);
		}
	}
}