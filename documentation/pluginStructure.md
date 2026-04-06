# Tebas plugin structure

Plugins are [AshFiles](https://github.com/siljamdev/AshLib), tipically with a `.tbplg` extension.  
These plguin AshFiles have the following structure (\* marks a mandatory field):  
```
Root
├──*name: <string>
├──version: <string>    Tebas version it targets
├──author: <string>
├──description: <string>
├──keepResources: <bool>    If set to true, plguin will keep previous resources when reinstalling
├──scripts
│  ├──[name]: <string>    TableScript source
│  …
├──globals
│  ├──[name]: <string>    TableScript source
│  …
├──utils
│  ├──[name]: <string>    TableScript source
│  …
├──resources
│  ├──[name]: <string>    Content
│  …
```

Remember, the tree structure is a visual representation of a flat field list. The separator is '.'.

## Building from source
When you do `tebas plugin build <directory>`, `directory` points to a folder containing source (\* marks a mandatory file):  
```
Root
├──*name.txt
├──author.txt
├──description.txt
├──KEEPRESOURCES    If present, keepResources will be set to true
├──scripts
│  ├──[name].tbs
│  …
├──globals
│  ├──[name].tbs
│  …
├──utils
│  ├──[name].tbs
│  …
├──resources
│  ├──[name].*    Extension will be trimmed
│  …
```

From the resources folder, all files will be loaded, but their extension will be trimmed: `resources/program.js => resources.program`  
The version field is automatically added by the Tebas version that builds it.  

## Special globals
Some  globals will be called on events, and cannot be called manually:  
Globals:
|Global|Activation|
|---|---|
|install|On plugin installation (`tebas plugin install <path>`)|
|uninstall|On plugin uninstallation (`tebas plugin uninstall <plugin>`)|
|info|On plugin info (`tebas plugin info <plugin>`)|
|cleanup|On plugin cleanup|

## Examples
Find the source of plugins in the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry/tree/main/plugins)
