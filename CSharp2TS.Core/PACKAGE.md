CSharp2TS.Core is a lightweight package that contains the attributes required for the **CSharp2TS.CLI** dotnet tool to generate TypeScript models and API services.

## Getting Started

Add the following attributes to your project to include or exclude items in the CSharp2TS.CLI tool's TypeScript generation.

**TSInterface** can be added to a class to generate a TypeScript interface.

```c#
[TSInterface]
public class TestModel {
    ...
}
```



**TSEnum** can be added to enums to generate a TypeScript enum.

```c#
[TSEnum]
public enum TestEnum {
    ...
}
```



**TSService** can be added to classes which inherit from `ControllerBase` to generate an api client.

```c#
[TSService]
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase {
    ...
}
```



**TSEndpoint** can be added to an API endpoint to specify / override the return type.

```c#
[HttpGet]
[TSEndpoint(typeof(string))]
public IActionResult Get() {
    return Ok("Hello World");
}
```



**TSExclude** can be added to properties and API endpoints to exclude it from the TypeScript generation.

```c#
[TSExclude]
public int ExcludedProperty { get; set; } // Property will not be included in the TypeScript file
```

```c#
[HttpGet]
[TSExclude] // Endpoint will not be included in the TypeScript file
public IActionResult Get() {
    return Ok("Hello World");
}
```



**TSNullable** can be added to properties to mark the type as nullable in the TypeScript generation.

```c#
[TSNullable]
public string NullableString { get; set; } // Produces nullableString: string | null
```



**TSDefault** can be added to a property to specify the default value used in the generated TypeScript stub class. This is useful for types such as `Guid` where the value cannot be expressed as a C# compile-time constant.

```c#
[TSDefault("018b1a05-8f0f-477d-8d95-c7effcde2eeb")]
public Guid PredicateId { get; set; } // Stub produces predicateId: string = '018b1a05-8f0f-477d-8d95-c7effcde2eeb'
```



**TSImport**

```c#
[TSService]
[TSImport("CustomType", "../types/customType")]
[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase {
    [HttpGet]
    [TSEndpoint("CustomType")]
    public IActionResult Get() {
        return Ok(...);
    }
}
```

This generates the following import in the TypeScript service file:

```ts
import CustomType from '../types/customType';
```

Multiple `TSImport` attributes can be added to a single controller.



## Additional Options

**TypeName** can be passed to `TSInterface`, `TSEnum`, or `TSService` to override the generated TypeScript type name.

```c#
[TSInterface("MyCustomName")]
public class TestModel {
    ...
}
```

**Folder** can be set on `TSInterface`, `TSEnum`, or `TSService` to place the generated file in a subfolder of the output directory.

```c#
[TSInterface(Folder = "subfolder")]
public class TestModel {
    ...
}
```

**IncludeMethods** can be passed to `TSInterface` to include public methods in the generated TypeScript type.

```c#
[TSInterface(IncludeMethods = true)]
public class TestModel {
    public bool Test() {
        return true;
    }
    ...
}
```

**GenerateClass** can be set on `TSInterface` to also generate a function that returns a default instance of the interface.

```c#
[TSInterface(GenerateClass = true)]
public class TestModel {
    ...
}
```

**TSEndpoint with a string** can be used instead of a `Type` to specify a raw TypeScript return type.

```c#
[HttpGet]
[TSEndpoint("CustomType")]
public IActionResult Get() {
    return Ok(...);
}
```
