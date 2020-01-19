# Phoenix.UI.Wpf.ViewProvider

| .NET Framework | .NET Standard | .NET Core |
| :-: | :-: | :-: |
| :heavy_check_mark: 4.5 | :heavy_minus_sign: | :heavy_check_mark: 3.1 |

This project is a view model to view resolver to be used in a **View Model First** approach for **WPF** applications.

___

# General Information

In contrast to the classic **View first** approach, where a view knows how to instantiate its view model, used by many popular **MVVM** framework likes **Prism** there is an alternative in the form of **View Model First**. This pattern lets you instantiate the view models manually and their views will be automatically attached. Coming from other UI technologies like **WinForms** this may feel more familiar. Typical frameworks using view model first are [Caliburn Micro](<https://caliburnmicro.com/>) or [Stylet](<https://github.com/canton7/Stylet>).

In a **View Model First** approach the views are however not magically attached to their view models. This must be handled by something. Spotlight on this ***ViewProvider***.

# Concept

When manually creating a new view model the ***ViewProvider*** has the task to find a matching view for it and bind them together. Normally identifying a view is based naming conventions. A real simple example: If the ***ViewProvider*** has to lookup a view for a *DetailViewModel* then it would replace *ViewModel* with *View* and then search for a type with that name *DefaultView*. Keep in mind, that such naming conventions may not only apply to the name of the class, but also to its namespace.

# View Providers

There are different implementations to the ***IViewProvider*** interface. The [***DefaultViewProvider***](#DefaultViewProvider) is the most commonly used and normally it shouldn't be necesarry to create a completly new provider. Better is to inherit from the ***DefaultViewProvider*** and override the methods that need to be changed in order to get other behavior.

## DefaultViewProvider

A ***IViewProvider*** that creates views for a given view model instance if both the views and the view models are located within the same assembly based on naming conventions. The default naming conventions can overridden by one of the constructors of the class.

| | ViewModel | View | Example | Example |
| :-: | :-: | :-: | :- | :- |
| Class | Model | --- | MainWindowModel → MainWindow |  DetailsViewModel → DetailsView |
| Namespace | ViewModels | Views | [Assembly].ViewModels → [Assembly].Views | |

An important aspect of the ***DefaultViewProvider*** is its ability to invoke setup callbacks once the view for a view model was successfully resolved. Those callbacks must be passed to the constructor of the provider. The separate **Nuget** package ***Phoenix.UI.Wpf.Architecture.VMFirst*** provides some **ViewModel Interfaces** that can automatically be hooked up via this function.

## ViewAssemblyViewProvider

If the views that should be bound to view models are located in separate _view assemblies_ the ***ViewAssemblyViewProvider*** can be used. It is a child of the [***DefaultViewProvider***](#DefaultViewProvider) that searches those discrete _view assemblies_ for views matching view models. This is done with the help of the [**ViewAssemblyProvider**](#ViewAssemblyProvider) and needs the _view assemblies_ to provide a type implementing [***IViewHolder***](#IViewHolder).

## WrappingViewProvider

This special ***IViewProvider*** is just a wrapper for multiple other ***IViewProvider***'s that are chained together and are used in sequence to obtain a view for a view model.

# ViewAssemblyProvider

Searches and continuously updates all loaded assemblies for instances of ***IViewHolder*** implementations and stores the so provided ***IViewHolder.ViewModelAssemblies***. The [***ViewAssemblyViewProvider***](#ViewAssemblyViewProvider) then uses the so found view assemblies to resolve views for view models.

The ***IViewHolder.ViewModelAssemblies*** doesn't need to be setup manually as it is a static class that will started upon first access.

## IViewHolder

A simple interface that needs to be implemented by a type in a _view assembly_ so that the ***ViewAssemblyProvider*** knows for which assembly this _view assembly_ provides views.

# Authors

* **Felix Leistner** - _Initial release_