# ![CakeBuild](https://github.com/cake-build/graphics/raw/master/png/cake-small.png) Cake.Extensions

[![Build status](https://ci.appveyor.com/api/projects/status/2wn5r21h6hkpuyrx/branch/master?svg=true)](https://ci.appveyor.com/project/MacLeanElectrical/cake-extensions/branch/master)
[![NuGet version](https://badge.fury.io/nu/Cake.Extensions.svg)](https://badge.fury.io/nu/Cake.Extensions)

A set of extension methods and aliases for [Cake](http://cakebuild.net)

* Repository: [http://github.org/wwwlicious/cake.extensions](http://github.org/wwwlicious/cake.extensions)
* Docs: [https://wwwlicious.gitbooks.io/cake-extensions/](https://wwwlicious.gitbooks.io/cake-extensions/)

### Usage: inside build.cake

This addin is designed to be used inside of cake scripts. To start using it, first you must add a cake [preprocessor directive](http://cakebuild.net/docs/fundamentals/preprocessor-directives) to your script as below.

```cs
#addin "Cake.Extensions"
// or
#addin "nuget?package=Cake.Extensions"
```

When the cake script is run, this will download the latest version of the `Cake.Extensions` nuget package and will now be available to use inside of the script.


### Contributions

Feel free to submit your PR's with your handy aliases or extensions. 
The only requirement is that they do not introduce additional dependencies to the package.