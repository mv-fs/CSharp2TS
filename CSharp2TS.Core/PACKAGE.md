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
