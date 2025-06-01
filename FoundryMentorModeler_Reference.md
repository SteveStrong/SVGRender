# FoundryMentorModeler Library Reference

this project is full of examples for using FoundryMentorModeler.  please create a reference document for this library that a copilot could use to make decisions about calling functions or integrating this library

## Overview

FoundryMentorModeler is a C# .NET 9.0 library for creating knowledge-based modeling systems. It provides a framework for building parametric models with hierarchical relationships, formula evaluation, and dependency tracking. The library is particularly suited for engineering applications where you need to model complex systems with mathematical relationships between components.

## Core Concepts

### Knowledge Base Classes

#### KnBase
The base class for all knowledge entities in the system.

#### KnConcept
Represents a template or blueprint that can be instantiated.
```csharp
var concept = new KnConcept("Box");
concept.Calculation<KnVariable>("Width: 10");
concept.Calculation<KnVariable>("Height: 20");
concept.Calculation<KnVariable>("Volume: Width * Height * Depth");
```

#### KnModel
A container for model instances and relationships.
```csharp
var model = new KnModel("MyModel");
var instance = concept.MakeInstance(model, "Box1");
```

#### KnInstance
A concrete instantiation of a concept with specific values.
```csharp
var box1 = concept.MakeInstance(model, "Box1");
var width = box1.GetValueOf("Width");
box1.SetValueOf("Depth", 40);
```

#### KnComponent
A general-purpose component that can contain calculations and subcomponents.
```csharp
var component = new KnComponent("Part");
component.Calculation("Width: 10");
component.AddSubcomponent<KnComponent>(childComponent);
```

#### KnVariable/KnParameter
Represents a named value with optional formula.
```csharp
component.Calculation<KnVariable>("Volume: Width * Height * Depth");
component.EstablishInstance<KnParameter>("Width", 10);
```

#### KnTrait
Provides reusable characteristics that can be applied to concepts.
```csharp
var geometry = new KnTrait("Geometry");
geometry.Calculation<KnVariable>("GeomType: 'Box'");
concept.Add<KnTrait>(geometry);
```

#### KnFeature
Represents a feature within a concept hierarchy.
```csharp
var feature = new KnFeature("TopSurface");
Connect<KnConcept, KnFeature>(table, ConnectType.HasSubcomponent, feature);
feature.Add<KnConcept>(surfaceConcept);
```

## Formula System

### Parser
Parses mathematical expressions and references.
```csharp
var parser = new Parser("Width * Height * Depth");
var formula = parser.ReadFormula();
```

### Formula Evaluation
```csharp
formula.Evaluate(context, OPResult.R(), (result) => {
    var value = result.AsNumber();
    // Process result
});
```

### Reference Types

#### Direct References
Reference parameters within the same component:
```csharp
component.Calculation("Volume: Width * Height * Depth");
```

#### At References (@)
Reference values from parent components:
```csharp
legComponent.Calculation("Height: LegHeight@");
```

#### Parent References (@Parent)
Explicitly reference parent component:
```csharp
child.Calculation("A: A@Parent + 1");
```

#### Superior References (^)
Reference values from superior hierarchy:
```csharp
child.Calculation("A: B^ + 1");
```

## Component Relationships

### Subcomponents
```csharp
parent.AddSubcomponent<KnComponent>(child);
var subcomponents = parent.Subcomponents<KnComponent>();
```

### Connections
```csharp
private void Connect<U,V>(U from, ConnectType type, V to) where U : KnBase where V : KnBase
{
    from.AddConnection(type, to);
    to.AddConnection(type.GetInverse(), from);
}
```

### Finding Components
```csharp
var result = model.FindSubcomponent<KnComponent>("ComponentName");
var components = model.Members<KnComponent>();
```

## Value Management

### Getting Values
```csharp
var value = component.GetValueOf("PropertyName");
var number = value.AsNumber();
var text = value.AsString();
```

### Setting Values
```csharp
component.SetValueOf("Width", 25.0);
```

### Value Invalidation
```csharp
component.SmashValueOf("Volume"); // Forces recalculation
```

### Dependency Management
```csharp
var pVolume = component.LocalReference<KnParameter>("Volume");
var pWidth = component.LocalReference<KnParameter>("Width");
pVolume.IDependOn(pWidth);
```

## Advanced Features

### Units Support
```csharp
component.Calculations([
    "Width|mm: 1.5",
    "Height|cm: 2",
    "Volume|m3: Width * Height * Depth"
]);
component.Units<KnVariable>("Width: mm");
```

### Async Functions
```csharp
var formula = new Parser("Get('https://api.example.com/data')").ReadFormula();
```

### Custom Methods
```csharp
public bool CustomMethod(KnInstance context, List<OPResult> args, OPResult result)
{
    // Custom logic
    result.SetValue(calculatedValue);
    return true;
}

parameter.ApplyMethod("MethodName", CustomMethod, args);
```

### Computational Methods
```csharp
component.ComputeAll<KnComponent>(true); // Compute all values in hierarchy
```

## Service Integration

### Dependency Injection Setup
```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.AddFoundryMentorModelerServices();
var serviceProvider = serviceCollection.BuildServiceProvider();

var mentorManager = serviceProvider.GetService<IMentorModelManager>();
var mentorServices = serviceProvider.GetService<IMentorServices>();
```

### Model Management
```csharp
var model = mentorManager.CreateModel<KnModel>("ModelName", mentorServices);
mentorManager.AddKnowledge<KnConcept>(concept);
```

## Error Handling

### Reference Errors
```csharp
var value = component.GetValueOf("UnknownProperty");
if (value.IsError()) {
    // Handle error
}
if (value.IsUnknownReference()) {
    // Handle unknown reference
}
if (value.IsCircularReference()) {
    // Handle circular reference
}
```

### Value Validation
```csharp
var result = formula.Evaluate(context, OPResult.R(), (x) => {
    if (x.IsNAN()) {
        // Handle NaN result
    }
});
```

## Common Patterns

### Building Hierarchical Models
```csharp
// Create concepts
var tableConcept = new KnConcept("Table");
tableConcept.Calculation<KnVariable>("Width: 10");
tableConcept.Calculation<KnVariable>("Height: 20");

var legConcept = new KnConcept("Leg");
legConcept.Calculation<KnVariable>("Height: LegHeight@");
legConcept.Calculation<KnVariable>("Size: 1");

// Create model and instances
var model = new KnModel("FurnitureModel");
var table = tableConcept.MakeInstance(model, "DiningTable");

// Add subcomponents
var leg1 = legConcept.MakeInstance(table, "LeftFrontLeg");
table.AddSubcomponent<KnInstance>(leg1);
leg1.Calculation("X: Left@");
leg1.Calculation("Z: Front@");
```

### Formula Processing
```csharp
// Parse and evaluate formulas
var parser = new Parser("Width * Height * Depth");
var formula = parser.ReadFormula();

formula.Evaluate(component, OPResult.R(), (result) => {
    if (result.IsError()) {
        Console.WriteLine("Evaluation error");
        return;
    }
    var volume = result.AsNumber();
    Console.WriteLine($"Volume: {volume}");
});
```

### Dynamic Value Updates
```csharp
// Set up automatic dependency tracking
var instance = concept.MakeInstance(model, "Instance1");
instance.SetValueOf("Width", 30);  // Automatically triggers recalculation of dependent values
var newVolume = instance.GetValueOf("Volume");  // Gets updated volume
```

## Best Practices

1. **Use Concepts as Templates**: Define concepts with formulas and instantiate them as needed.

2. **Leverage @ References**: Use @ references to create parametric relationships between parent and child components.

3. **Manage Dependencies**: Use `IDependOn()` when automatic dependency detection isn't sufficient.

4. **Handle Errors Gracefully**: Always check for errors, circular references, and unknown references.

5. **Use Service Injection**: Register services properly for model management and knowledge tracking.

6. **Organize Hierarchically**: Structure your models with clear parent-child relationships for better organization and reference resolution.

7. **Test Formulas**: Use the parser and evaluation system to validate formulas before deployment.

## Integration Guidelines

When integrating FoundryMentorModeler into a project:

1. Add the NuGet package reference
2. Configure dependency injection with `AddFoundryMentorModelerServices()`
3. Create concepts and models using the service providers
4. Build your component hierarchy
5. Define relationships and formulas
6. Implement proper error handling
7. Test evaluation logic thoroughly

This library is particularly powerful for applications requiring:
- Parametric design systems
- Engineering calculations with dependencies
- Hierarchical model structures
- Dynamic formula evaluation
- Knowledge-based systems
