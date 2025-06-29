using System;
using System.Collections.Generic;
using System.Reflection;

namespace VT.ReusableSystems.Events
{
    /// <summary>
    /// A utility class, PredefinedAssemblyUtil, provides methods to interact with predefined assemblies.
    /// It allows to get all types in the current AppDomain that implement from a specific Interface type.
    /// For more details, <see href="https://docs.unity3d.com/2023.3/Documentation/Manual/ScriptCompileOrderFolders.html">visit Unity Documentation</see>
    /// </summary>
    public static class PredefinedAssemblyUtil
    {
        /// <summary>
        /// Enum that defines the specific predefined types of assemblies for navigation.
        /// </summary>    
        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass,
            VTReusableSystemsEventsExamples, // Custom assembly for VT.ReusableSystems.Events.Examples
            VTReusableSystemsHealthSystem, // Custom assembly for VT.ReusableSystems.HealthSystem
        }

        /// <summary>
        /// Maps the assembly name to the corresponding AssemblyType.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>AssemblyType corresponding to the assembly name, null if no match.</returns>
        static AssemblyType? GetAssemblyType(string assemblyName)
        {
            return assemblyName switch
            {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                "VT.ReusableSystems.Events.Examples" => AssemblyType.VTReusableSystemsEventsExamples,
                "VT.ReusableSystems.HealthSystem" => AssemblyType.VTReusableSystemsHealthSystem,
                _ => null
            };
        }

        /// <summary>
        /// Method looks through a given assembly and adds types that fulfill a certain interface to the provided collection.
        /// </summary>
        /// <param name="assemblyTypes">Array of Type objects representing all the types in the assembly.</param>
        /// <param name="interfaceType">Type representing the interface to be checked against.</param>
        /// <param name="results">Collection of types where result should be added.</param>
        /// <param name="assemblyName">Name of the assembly being processed (for debugging).</param>
        static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results, string assemblyName = "Unknown")
        {
            if (assemblyTypes == null) 
            {
                return;
            }
            
            int foundCount = 0;
            for (int i = 0; i < assemblyTypes.Length; i++)
            {
                Type type = assemblyTypes[i];
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                {
                    results.Add(type);
                    foundCount++;
                }
            }
        }

        /// <summary>
        /// Gets all Types from all assemblies in the current AppDomain that implement the provided interface type.
        /// </summary>
        /// <param name="interfaceType">Interface type to get all the Types for.</param>
        /// <returns>List of Types implementing the provided interface type.</returns>    
        public static List<Type> GetTypes(Type interfaceType)
        {            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Dictionary<AssemblyType, Type[]> assemblyTypes = new();
            List<Type> types = new();
            for (int i = 0; i < assemblies.Length; i++)
            {
                string assemblyName = assemblies[i].GetName().Name;
                AssemblyType? assemblyType = GetAssemblyType(assemblyName);
                if (assemblyType != null)
                {
                    assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
                }
            }

            // Second pass: process predefined assemblies in order
            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
            AddTypesFromAssembly(assemblyCSharpTypes, interfaceType, types, "Assembly-CSharp");

            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
            AddTypesFromAssembly(assemblyCSharpFirstPassTypes, interfaceType, types, "Assembly-CSharp-firstpass");

            assemblyTypes.TryGetValue(AssemblyType.VTReusableSystemsEventsExamples, out var vtReusableSystemsEventsExamplesTypes);
            AddTypesFromAssembly(vtReusableSystemsEventsExamplesTypes, interfaceType, types, "VT.ReusableSystems.Events.Examples");

            assemblyTypes.TryGetValue(AssemblyType.VTReusableSystemsHealthSystem, out var vtReusableSystemsHealthSystemTypes);
            AddTypesFromAssembly(vtReusableSystemsHealthSystemTypes, interfaceType, types, "VT.ReusableSystems.HealthSystem");
            
            return types;
        }
    }
}
