# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
___

## 2.1.0 (2020-12-12)

### Changed

- The concept of view model setup callbacks (either via `ViewModelSetupCallback`or `Action<object, FrameworkElement>` has been replaced by an event based mechanism. The new event `IViewProvider.ViewLoaded`along with its `ViewLoadedEventArgs` should be used from now on.

###  Deprecated

- All constructors in `DefaultViewProvider` that accepted some kind of view model setup callback.
- The class `ViewModelSetupCallback` itself.
___

## 2.0.0 (2020-11-21)

### Changed

- Changed license to [**LGPL-3.0**](https://www.gnu.org/licenses/lgpl-3.0.html).
___

## 1.1.0 (2020-11-15)

### Added

- Now also targeting **.NET5.0**.
___

## 1.0.0 (2020-02-15)

- Initial release