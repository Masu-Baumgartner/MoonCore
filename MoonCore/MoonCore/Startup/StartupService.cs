using System.Reflection;
using Microsoft.Extensions.Logging;
using MoonCore.Attributes;

namespace MoonCore.Startup;

public class StartupService<T> where T : class
{
    private T[] StartupLayers;

    public void Prepare(Assembly[] assemblies)
    {
        // Find all classes that implement IStartupLayer
        var startupLayers = assemblies.SelectMany(asm => asm.GetTypes())
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToList();

        // Sort the layers based on the RunAfter attribute
        var sortedLayers = SortLayers(startupLayers);

        StartupLayers = sortedLayers
            .Select(x => (Activator.CreateInstance(x) as T)!)
            .ToArray();
    }

    public async Task Run(Func<T, Task> method)
    {
        foreach (var layer in StartupLayers)
            await method.Invoke(layer);
    }

    private List<Type> SortLayers(List<Type> startupLayers)
    {
        // Dependency graph: each Type depends on a set of other Types.
        var dependencyGraph = new Dictionary<Type, List<Type>>();
        var reverseDependencyGraph = new Dictionary<Type, List<Type>>();

        // Initialize dependency graph
        foreach (var layer in startupLayers)
        {
            dependencyGraph[layer] = new List<Type>();
            reverseDependencyGraph[layer] = new List<Type>();
        }

        // Populate dependency graph based on RunAfter and RunBefore attributes
        foreach (var layer in startupLayers)
        {
            // Handle RunAfter
            var runAfterAttr = layer.GetCustomAttribute<RunAfterAttribute>();
            if (runAfterAttr != null)
            {
                if (!dependencyGraph.ContainsKey(runAfterAttr.RunAfterType))
                {
                    throw new InvalidOperationException($"Type {runAfterAttr.RunAfterType.Name} specified in RunAfter attribute is not a valid IStartupLayer.");
                }

                dependencyGraph[layer].Add(runAfterAttr.RunAfterType);  // This layer depends on another layer
                reverseDependencyGraph[runAfterAttr.RunAfterType].Add(layer);
            }

            // Handle RunBefore
            var runBeforeAttr = layer.GetCustomAttribute<RunBeforeAttribute>();
            if (runBeforeAttr != null)
            {
                if (!dependencyGraph.ContainsKey(runBeforeAttr.RunBeforeType))
                {
                    throw new InvalidOperationException($"Type {runBeforeAttr.RunBeforeType.Name} specified in RunBefore attribute is not a valid IStartupLayer.");
                }

                dependencyGraph[runBeforeAttr.RunBeforeType].Add(layer);  // The layer we depend on needs to run after this
                reverseDependencyGraph[layer].Add(runBeforeAttr.RunBeforeType);
            }
        }

        // Now perform topological sort (Kahn's algorithm)
        var sorted = TopologicalSort(dependencyGraph);

        sorted.Reverse();
        
        return sorted;
    }

    private List<Type> TopologicalSort(Dictionary<Type, List<Type>> dependencyGraph)
    {
        var inDegree = new Dictionary<Type, int>();
        var sorted = new List<Type>();
        var queue = new Queue<Type>();

        // Calculate in-degrees (number of incoming edges)
        foreach (var node in dependencyGraph.Keys)
        {
            inDegree[node] = 0; // Initialize
        }

        foreach (var dependencies in dependencyGraph.Values)
        {
            foreach (var dependency in dependencies)
            {
                if (inDegree.ContainsKey(dependency))
                {
                    inDegree[dependency]++;
                }
            }
        }

        // Enqueue nodes with zero in-degree (no dependencies)
        foreach (var node in inDegree.Keys)
        {
            if (inDegree[node] == 0)
            {
                queue.Enqueue(node);
            }
        }

        // Perform topological sort
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sorted.Add(current);

            // Reduce the in-degree of dependent nodes
            foreach (var dependent in dependencyGraph[current])
            {
                inDegree[dependent]--;

                if (inDegree[dependent] == 0)
                {
                    queue.Enqueue(dependent);
                }
            }
        }

        // If we couldn't sort all nodes, a cycle must exist
        if (sorted.Count != dependencyGraph.Count)
        {
            throw new InvalidOperationException("Circular dependency detected in startup layers.");
        }

        return sorted;
    }
}