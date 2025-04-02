# .NET 8 Structuremap Integration

Available on Nuget here: https://www.nuget.org/packages/StructureMap.Microsoft.DependencyInjection.Forked/

This was forked from [this underlying library](https://github.com/structuremap/StructureMap.Microsoft.DependencyInjection) due to [this bug](https://github.com/dustinsoftware/StructureMap.Microsoft.DependencyInjection/commit/0f1d8e445bfc430e1cdc7792045f5bb3356b68af) around Disposing objects.

See the [sample app](./sample) for an example of how to integrate this in a .NET 8 app.
