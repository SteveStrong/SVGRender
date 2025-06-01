# FoundryMentorModeler Library - Copilot Reference Guide

please create a reference document for this library that a copilot could use to make decisions about calling functions or integrating this library

## Overview

FoundryMentorModeler is a comprehensive knowledge modeling and visualization library for .NET/Blazor applications. It provides tools for creating, managing, and visualizing knowledge structures, concepts, relationships, and their 2D/3D representations. The library supports interactive model building, formula evaluation, SYSML parsing, and sophisticated shape-based visualization.

## Core Architecture

### Knowledge Type System

The library is built around a comprehensive [`KnowledgeType`](Mentor/KnowledgeType.cs) enumeration that defines 40+ types of knowledge entities:

**Fundamental Types:**
- `Concept` - Basic conceptual entities and abstractions
- `Component` - Physical or logical components/instances  
- `Feature` - Characteristics, capabilities, or attributes
- `Property` - Specific properties and attributes
- `Variable` - Named values with formulas and units
- `Parameter` - Computed values with dependencies

**Structural Types:**
- `Context` - Contextual groupings and scopes
- `Relation`/`Relationship` - Connections between entities
- `Folder` - Organizational containers
- `Reference` - Links and references

**Behavioral Types:**
- `Formula` - Mathematical expressions and calculations
- `Workflow` - Process definitions and workflows
- `WorkTask` - Individual work items and tasks
- `ProcessStep` - Steps in a process

**Organizational Types:**
- `Role` - Functional roles and responsibilities
- `Resource` - Available resources and assets
- `Model` - Top-level knowledge models
- `Solution` - Solution definitions

**Technical Types:**
- `Port`/`Plug` - Connection points and interfaces
- `Factory` - Object creation patterns
- `Agent` - Autonomous agents
- `Tool` - Tools and utilities
- `Problem` - Problem definitions
- `Geometry2D`/`Geometry3D`/`Geometry1D` - Geometric representations
- `Editor2D` - 2D editor components

## Key Service Interfaces

### IMentorServices - Central Service Hub
```csharp
public interface IMentorServices
{
    // Core Managers
    IMentorModelManager MentorModel { get; set; }
    IMentorDiagramManager MentorEditor { get; set; }
    IMentor2DDrawingManager Mentor2DDrawing { get; set; }
    IMentor3DGeometryManager Mentor3DGeometry { get; set; }
    
    // Infrastructure
    IWorkspace Workspace { get; set; }
    IFoundryService Foundry { get; set; }
    IToast Toast { get; set; }
    IPopupDialog PopupDialog { get; set; }
    ComponentBus PubSub { get; set; }

    // Model Management
    T EstablishModel<T>(string title) where T : KnModel;
    T? FindModel<T>(string title) where T : KnModel;
    T CreateModel<T>(string title) where T : KnModel, new();
    T? FindKnowledge<T>(string key) where T : KnBase;
    
    // Diagram Management
    T EstablishDiagram<T>(string pageName) where T : MentorDiagram;
    (bool found, MentorDiagram diagram) CurrentDiagram();
    MentorDiagram SetCurrentDiagram(MentorDiagram current);
    
    // Drawing and 3D
    IDrawing EstablishDrawing();
    T EstablishPage<T>(string view, bool clear) where T : FoPage2D;
    IWorld3D CreateWorld<T>(string title) where T : FoWorld3D;
    FoShape3D Load3DModel(string filename);
}
```

### IMentorModelManager - Knowledge Model Management
```csharp
public interface IMentorModelManager : IKnTreeNode
{
    // Model Lifecycle
    T EstablishModel<T>(string title, IMentorServices services) where T : KnModel;
    T? FindModel<T>(string title) where T : KnModel;
    T CreateModel<T>(string title, IMentorServices services) where T : KnModel;
    KnModel AddModel(KnModel model, bool clear = true);
    
    // Knowledge Management
    T AddKnowledge<T>(T value) where T : KnBase;
    T RemoveKnowledge<T>(string key) where T : KnBase;
    T? FindKnowledge<T>(string key) where T : KnBase;
    int FullCountOf<T>() where T : KnBase;
    
    // Persistence
    void SaveModel<T>(bool compress = false) where T : KnowledgePersist;
    bool RestoreModel<T>() where T : KnowledgePersist;
    void SaveDrawing<T>(bool compress = false) where T : DrawingPersist;
    bool RestoreDrawing<T>() where T : DrawingPersist;
}
```

### IMentorPlayground - Interactive Model Building
```csharp
public interface IMentorPlayground : IKnTreeNode
{
    // Shape Creation
    MentorShape2D CreateShape<T>(string title="") where T : KnBase;
    V CreateNodeShape<V>(KnBase model) where V : MentorShape2D;
    MentorShape2D Attach(MentorShape2D shape, MentorShape2D target);
    
    // Menu System
    Dictionary<string, Action> Menu();
}
```

## Core Knowledge Classes

### Knowledge Hierarchy
```
KnBase (abstract base)
├── KnKnowledge (knowledge definitions)
│   ├── KnConcept (conceptual definitions)
│   ├── KnFeature (feature definitions)
│   ├── KnRole (role definitions)
│   └── KnTrait (trait definitions)
├── KnClass (class-based knowledge)
│   ├── KnVariable (variables with formulas)
│   ├── KnProperty (properties)
│   ├── KnValidValues (validation rules)
│   └── KnDefaultValue (default values)
└── KnInstance (runtime instances)
    ├── KnComponent (components/instances)
    │   ├── KnModel (top-level models)
    │   ├── KnFactory (factories)
    │   └── KnGeometry (geometric objects)
    └── KnParameter (computed parameters)
        ├── KnGeometryParameter (geometric parameters)
        └── KnEditor2DParameter (2D editor parameters)
```

### KnConcept - Fundamental Concepts
```csharp
public class KnConcept : KnClass, ICreatable
{
    // Unit definitions
    T Units<T>(string assignment) where T : KnVariable;
    void Units(string[] lines);
    
    // Calculations
    T Calculation<T>(string assignment) where T : KnVariable;
    void Calculations(string[] lines);
    
    // Feature management
    KnFeature AddFeature(string name, KnConcept type);
    
    // Instance creation
    KnInstance MakeInstance(KnInstance context, string name="");
}
```

### KnComponent - Runtime Components
```csharp
public class KnComponent : KnInstance, IModelRenderable
{
    // Child management
    T AddChildComponent<T>(T child) where T : KnComponent;
    T RemoveChildComponent<T>(T child) where T : KnComponent;
    List<T> Subcomponents<T>() where T : KnInstance;
    
    // 2D Geometry
    (KnGeometry geom, KnGeometryParameter param) EstablishGeometry2D(string view, FoPage2D? page=null);
    OPResult Geometry2DValueFor(string view);
    OPResult Geometry2DRender(string view, FoPage2D page, bool deep);
    
    // 3D Geometry
    (KnGeometry geom, KnGeometryParameter param) EstablishGeometry3D(string view, IArena? arena=null);
    OPResult Geometry3DValueFor(string view);
    OPResult Geometry3DRender(string view, IArena arena, bool deep);
    
    // Editor Integration
    IComponentViewer? RenderEditor(string view, MentorDiagram diagram, bool deep);
    KnEditor2DParameter EstablishEditor(string view, MentorDiagram diagram);
}
```

### KnParameter - Computed Values
```csharp
public class KnParameter : KnInstance
{
    // Formula and evaluation
    bool ApplyFormula(string formula);
    bool ApplyFormula(Operator formula);
    OPResult GetCurrentValue(Action<OPResult>? OnComplete = null);
    T GetCurrentValueAs<T>(Action<OPResult>? OnComplete = null) where T : class;
    bool Evaluate(KnInstance context, OPResult result, Action<OPResult>? OnComplete = null);
    
    // Value management
    OPResult SetValue(object val);
    OPResult GetValue();
    bool Smash(Action<OPResult>? OnComplete = null);
    
    // Dependencies
    void IDependOn(KnParameter param);
    List<KnParameter> DependsOn { get; set; }
    List<KnParameter> ContributesTo { get; set; }
    
    // Status checks
    bool IsUnknown();
    bool IsValid();
    bool CanEvaluate();
}
```

### KnVariable - Formula Variables
```csharp
public class KnVariable : KnClass 
{
    // Formula definition
    KnVariable ApplyFormula(string formula);
    KnVariable ApplyUnits(string units);
    string Decompile();
    
    // Evaluation
    bool Evaluate(KnInstance context, OPResult result, Action<OPResult>? OnComplete = null);
    
    // Instance creation
    KnInstance MakeInstance(KnInstance context, string name="");
}
```

## Shape and Visualization System

### MentorShape2D - 2D Knowledge Shapes
```csharp
public class MentorShape2D : FoText2D, IMentorBlockShape
{
    // Knowledge integration
    bool IsKnowledge(KnowledgeType type);
    KnowledgeType GetKnowledgeType();
    string GetKnowTag();
    
    // Parent-child relationships
    void AttachToParent(IComposableShape parent, string guid);
    IComposableShape? ParentShape { get; }
    
    // Upstream connections
    void AttachToUpstream(IGlueableShape connection);
    IGlueableShape? UpstreamShape { get; }
}
```

### Shape Creation Patterns
```csharp
// Create shapes for different knowledge types
var conceptShape = playground.CreateShape<KnConcept>("TableConcept");
var variableShape = playground.CreateShape<KnVariable>("Width: 100");
var parameterShape = playground.CreateShape<KnParameter>("Height");

// Attach shapes to create visual relationships
playground.Attach(variableShape, conceptShape);
playground.Attach(parameterShape, conceptShape);
```

## Connection and Relationship System

### ConnectType Enumeration
```csharp
public enum ConnectType
{
    HasSubcomponent,    // Parent-child component relationships
    ComponentOf,        // Inverse of HasSubcomponent
    HasSubcontext,      // Contextual containment
    SubcontextOf,       // Inverse of HasSubcontext
    HasSubclass,        // Inheritance relationships
    SubclassOf,         // Inverse of HasSubclass
    HasInstance,        // Class-instance relationships
    InstanceOf,         // Inverse of HasInstance
    HasExample,         // Example relationships
    ExampleOf,          // Inverse of HasExample
    CreatedBy,          // Creation relationships
    DependsOn,          // Dependency relationships
    UsedBy              // Usage relationships
}
```

### Creating Connections
```csharp
// Connect knowledge entities
from.AddConnection(ConnectType.HasSubcomponent, to);
to.AddConnection(ConnectType.ComponentOf, from);

// Use connection helpers
var feature = concept.AddFeature("Weight", KnConcept.Create("Mass"));
```

## Formula and Evaluation System

### Reference Syntax
```csharp
// Parameter references
"Width@"          // Look up Width in parent context
"^Width"          // Look up Width in parent's parent context
"component.Width" // Access Width property of component

// Example formulas
var area = new KnVariable("Area: Width @ * Height @");
var volume = new KnVariable("Volume: Area @ * Depth @");
```

### Formula Creation
```csharp
// Create variables with calculations
var concept = new KnConcept("Table");
concept.Calculations([
    "Width: 100",
    "Height: 50", 
    "Area: Width * Height"
]);

// Create parameters with formulas
var instance = concept.MakeInstance(context, "MyTable");
var areaParam = instance.FindParameter("Area");
var result = areaParam.GetCurrentValue();
```

## SYSML Integration

### SYSMLTools Interface
```csharp
public interface ISYSMLTools
{
    List<SourceSpec> GetSourceSYSMLFiles(string root, string target="");
    Dictionary<string,PackageNode> BatchCompileSYSML(List<SourceSpec> sources);
    List<IMentorModelManager> SYSMLKnowledge(IMentorWorkbook workbook);
}
```

### SYSML Usage Pattern
```csharp
var sysmlTools = new SYSMLTools();
var sources = sysmlTools.GetSourceSYSMLFiles(rootPath);
var packages = sysmlTools.BatchCompileSYSML(sources);
var knowledge = sysmlTools.SYSMLKnowledge(workbook);
```

## Persistence System

### Model Persistence
```csharp
// Save and restore models
mentorModel.SaveModel<KnowledgePersist>();
mentorModel.RestoreModel<KnowledgePersist>();

// Save and restore drawings
mentorModel.SaveDrawing<DrawingPersist>();
mentorModel.RestoreDrawing<DrawingPersist>();
```

### Persistence Types
- `KnowledgePersist` - Knowledge model data
- `DrawingPersist` - 2D drawing data
- `ModelPersist` - General model data
- `Shape2DPersist` - 2D shape data
- `Shape1DPersist` - 1D connector data

## Usage Patterns and Best Practices

### 1. Creating a Complete Knowledge Model
```csharp
// Get services
var mentorServices = serviceProvider.GetService<IMentorServices>();
var playground = serviceProvider.GetService<IMentorPlayground>();

// Create model
var model = mentorServices.EstablishModel<KnModel>("ProductModel");

// Create concept with properties
var product = new KnConcept("Product");
product.Units<KnVariable>("Price: USD");
product.Units<KnVariable>("Weight: kg");
product.Calculations([
    "Price: 100",
    "Weight: 5.5",
    "ShippingCost: Weight * 2.5"
]);

// Create instance
var productInstance = product.MakeInstance(model, "MyProduct");

// Create visual representation
var productShape = playground.CreateShape<KnConcept>("Product");
var priceShape = playground.CreateShape<KnVariable>("Price: 100");
playground.Attach(priceShape, productShape);
```

### 2. Working with Formulas and References
```csharp
// Create parameters that reference other parameters
var width = new KnVariable("Width: 100");
var height = new KnVariable("Height: 50");  
var area = new KnVariable("Area: Width @ * Height @");

// The @ symbol indicates parent context lookup for evaluation
var result = area.Evaluate(context, new OPResult());
```

### 3. Creating 2D and 3D Visualizations
```csharp
// 2D Geometry
var (geom2D, param2D) = component.EstablishGeometry2D("front", page);
param2D.ApplyMethod("rectangle", RectangleMethod, parameters);

// 3D Geometry
var (geom3D, param3D) = component.EstablishGeometry3D("model", arena);
param3D.ApplyMethod("box", BoxMethod, parameters);
```

### 4. Interactive Shape Management
```csharp
// Create shapes with auto-layout
var shape = playground.CreateShape<KnConcept>("Table");
shape.AutoSize();
shape.AutoSizeAndArrange();

// Create connectors
var connector = shape.CreateConnector();
connector.GlueStartTo(sourceShape);
connector.GlueFinishTo(targetShape);
```

### 5. Feature and Trait Patterns
```csharp
// Define feature with concept
var weightFeature = tableConcept.AddFeature("Weight", massConcept);

// Define trait for reuse
var dimensionTrait = new KnTrait("Dimensions", [
    "Width: mm",
    "Height: mm", 
    "Depth: mm"
]);

// Apply trait to concept
tableConcept.Add<KnTrait>(dimensionTrait);
```

## Service Registration

```csharp
// Register all FoundryMentorModeler services
services.AddFoundryMentorModelerServices();

// Register individual services
services.AddSingleton<IMentorServices, MentorServices>();
services.AddSingleton<IMentorModelManager, MentorModelManager>();
services.AddSingleton<IMentorPlayground, MentorPlayground>();
services.AddSingleton<ISYSMLTools, SYSMLTools>();
```

## Decision Guidelines for Copilots

### When to Use This Library
- ✅ Building knowledge management systems
- ✅ Creating domain-specific modeling tools
- ✅ Implementing visual programming interfaces
- ✅ Developing CAD/CAM applications
- ✅ Building educational simulation tools
- ✅ Creating system architecture visualizers

### Key Decision Points

**Choose `KnConcept` when:**
- Defining reusable conceptual templates
- Creating class-like definitions with variables
- Building hierarchical knowledge structures

**Choose `KnComponent` when:**
- Creating runtime instances
- Building component hierarchies
- Implementing geometric objects
- Managing 2D/3D visualizations

**Choose `KnVariable` when:**
- Defining formulas and calculations
- Creating template variables with units
- Building reusable calculation patterns

**Choose `KnParameter` when:**
- Computing runtime values
- Managing parameter dependencies
- Evaluating complex formulas

**Use `MentorShape2D` when:**
- Creating interactive visualizations
- Building drag-and-drop interfaces
- Implementing visual knowledge editors

### Common Anti-Patterns to Avoid
- ❌ Using `KnParameter` for static definitions (use `KnVariable` instead)
- ❌ Creating shapes without knowledge context
- ❌ Ignoring the reference syntax (`@`, `^`) in formulas
- ❌ Not calling `Smash()` after parameter changes
- ❌ Mixing template definitions with runtime instances

### Performance Considerations
- Use `EstablishModel<T>()` instead of `CreateModel<T>()` to avoid duplicates
- Call `AutoSizeAndArrange()` for better visual layouts
- Use persistence for large models to avoid recreation overhead
- Leverage the playground menu system for common operations

This library provides a comprehensive foundation for knowledge modeling with rich visualization capabilities, making it ideal for complex domain modeling, system design, and interactive knowledge management applications.
