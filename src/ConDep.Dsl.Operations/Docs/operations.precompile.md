<!---NOTE: If this file changes name or path, remember to update the link in README.md on root of this repo.--->
#PreCompile

Precompiles Asp.Net code to prevent precompilation during the first hit of the web page.

Usage:

```cs
public class MyArtifact : Artifact.Local {
    public override void Configure(IOfferLocalOperations onLocalMachine, ConDepSettings settings)
    {
        onLocalMachine.PreCompile("MyWebApp", @"C:\WebApps\MyWebApp", @"C:\MyPreCompWebApps\MyWebApp");
    }
} 
```
