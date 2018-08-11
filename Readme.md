# ![CakeBuild](https://github.com/cake-build/graphics/raw/master/png/cake-small.png) Cake.Incubator

[![Build status](https://ci.appveyor.com/api/projects/status/github/cake-contrib/cake.incubator?branch=master&svg=true)](https://ci.appveyor.com/project/cakecontrib/cake-incubator/branch/master)
[![NuGet version](https://badge.fury.io/nu/Cake.Incubator.svg)](https://badge.fury.io/nu/Cake.Incubator)

This project contains various experimental but useful extension methods and aliases for [Cake](http://cakebuild.net) that over time may become part of the core project

* Repository: [https://github.com/cake-contrib/cake.incubator](https://github.com/cake-contrib/cake.incubator)
* Docs: [http://cakebuild.net/api/Cake.Incubator/](http://cakebuild.net/api/Cake.Incubator/)
* [Release Notes](https://github.com/cake-contrib/Cake.Incubator/releases)

### Usage: inside build.cake

This addin is designed to be used inside of cake scripts. To start using it, first you must add a cake [preprocessor directive](http://cakebuild.net/docs/fundamentals/preprocessor-directives) to your script as below.

```cs
// NB: always pin your version to avoid breaking changes in newer releases 
#addin "Cake.Incubator&version=x.x.x"
// or
#addin "nuget:?package=Cake.Incubator&version=x.x.x"
```

When the cake script is run, this will download the latest version of the `Cake.Incubator` nuget package and will now be available to use inside of the script.


### Contributions

Feel free to submit your PR's with your handy aliases or extensions. 
The only requirement is that they do not introduce additional dependencies to the package.
