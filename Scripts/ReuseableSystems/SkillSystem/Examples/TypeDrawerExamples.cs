using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TypeDrawerExamples : MonoBehaviour
{
    [ShowInInspector]
    public Type Default;

    [Title("Base Type"), ShowInInspector, LabelText("Set")]
    [TypeDrawerSettings(BaseType = typeof(IEnumerable<>))]
    public Type BaseType_Set;

    [ShowInInspector, LabelText("Not Set")]
    [TypeDrawerSettings(BaseType = null)]
    public Type BaseType_NotSet;

    [Title(nameof(TypeDrawerSettingsAttribute.Filter)), ShowInInspector, LabelText("Concrete Types")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeConcreteTypes)]
    public Type Filter_Default;

    [ShowInInspector, LabelText("Concrete- && Generic Types")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeConcreteTypes | TypeInclusionFilter.IncludeGenerics)]
    public Type Filter_Generics;

    [ShowInInspector, LabelText("Concrete- && Interface Types")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeConcreteTypes | TypeInclusionFilter.IncludeInterfaces)]
    public Type Filter_Interfaces;

    [ShowInInspector, LabelText("Concrete- && Abstract Types")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeConcreteTypes | TypeInclusionFilter.IncludeAbstracts)]
    public Type Filter_Abstracts;

    [ShowInInspector, LabelText("Concrete-, Abstract- && Generic Types")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeConcreteTypes |
                                                                                         TypeInclusionFilter.IncludeAbstracts |
                                                                                         TypeInclusionFilter.IncludeGenerics)]
    public Type Filter_Abstracts_Generics;

    [ShowInInspector, LabelText("Concrete-, Interface- && Generic Types")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeConcreteTypes |
                                                                                         TypeInclusionFilter.IncludeInterfaces |
                                                                                         TypeInclusionFilter.IncludeGenerics)]
    public Type Filter_Interfaces_Generics;

    [ShowInInspector, LabelText("Concrete-, Interface- && Abstract Types")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeConcreteTypes |
                                                                                         TypeInclusionFilter.IncludeInterfaces |
                                                                                         TypeInclusionFilter.IncludeAbstracts)]
    public Type Filter_Interfaces_Abstracts;

    [ShowInInspector, LabelText("All")]
    [TypeDrawerSettings(BaseType = typeof(IBaseGeneric<>), Filter = TypeInclusionFilter.IncludeAll)]
    public Type Filter_All;
}

public interface IBaseGeneric<T> { }

public interface IBase : IBaseGeneric<int> { }

public abstract class Base : IBase { }

public class Concrete : Base { }

public class ConcreteGeneric<T> : Base { }

public abstract class BaseGeneric<T> : IBase { }

[CompilerGenerated]
public class ConcreteGenerated : Base { }