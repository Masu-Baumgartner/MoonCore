#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MoonCore.PluginFramework.Generator;

public static class AnalyzeHelper
{
    public static IEnumerable<ITypeSymbol> GetExternalTypes(IModuleSymbol moduleSymbol)
    {
        return moduleSymbol.ReferencedAssemblySymbols
            .SelectMany(GetTypesFromAssembly);
    }

    public static IEnumerable<ITypeSymbol> GetTypesFromAssembly(IAssemblySymbol currentAssembly)
    {
        var mainNamespace = GetMainNamespace(currentAssembly);

        if (mainNamespace == null)
            return [];

        return GetAllTypesFromNamespace(mainNamespace);
    }

    private static INamespaceSymbol? GetMainNamespace(IAssemblySymbol assemblySymbol)
    {
        try
        {
            var main = assemblySymbol
                .Identity
                .Name
                .Split('.')
                .Aggregate(
                    assemblySymbol.GlobalNamespace,
                    (s, c) => s.GetNamespaceMembers()
                        .Single(m =>
                            {
                                if (m.Name.StartsWith("Microsoft") || m.Name.StartsWith("System"))
                                    return false;

                                return m.Name.Equals(c);
                            }
                        )
                );

            return main;
        }
        catch (Exception e)
        {
            // TODO: Handle
            return null;
        }
    }

    private static IEnumerable<ITypeSymbol> GetAllTypesFromNamespace(INamespaceSymbol root)
    {
        foreach (var namespaceOrTypeSymbol in root.GetMembers())
        {
            if (namespaceOrTypeSymbol is INamespaceSymbol @namespace)
                foreach (var nested in GetAllTypesFromNamespace(@namespace))
                    yield return nested;

            else if (namespaceOrTypeSymbol is ITypeSymbol type) yield return type;
        }
    }
}