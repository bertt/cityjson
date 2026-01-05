# .NET Framework 4.8 Runtime issue
Note, this tool does not need to be compiled against net48, and is not released as such, 
this is purely for development or other adoptation purposes.

Running cj2glb targeting .NET Framework 4.8 results in the following runtime error:
```
System.IO.FileLoadException: Could not load file or assembly 'OneOf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' or one of its dependencies. A strongly-named assembly is required. (Exception from HRESULT: 0x80131044)
```

## Root Cause: Strong-Naming Conflict inside SharpGLTF.Toolkit:
- `SharpGLTF.Toolkit` version 1.0.5 is **strongly-named** (PublicKeyToken: 6F8B67557D279FE6) when targeting .NET Framework
- `OneOf` version 3.0.271 is **NOT strongly-named** (PublicKeyToken: null)
- .NET Framework 4.8 requires ALL dependencies of a strongly-named assembly to also be strongly-named
- .NET Core/.NET 5+/9 do NOT have this restriction (tests pass on .NET 9.0)

## Workaround for .NET Framework 4.8 support
- Build against a local version of SharpGLTF, clone the repository and compile against the locally compiled version.
- This assembly was tested in net48 and seems compatible otherwise.
- This requires changing the cj2glb.csproj to point to the local SharpGLTF build output folder.