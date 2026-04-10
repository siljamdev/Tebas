# TableScript imports

In [TableScript](https://github.com/siljamdev/TableScript), imports are used to join scripts together and to provide additional functions.  

## Standard libraries
In Tebas, all the [TableScript default libraries](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.html) are still available (stdlib is modified). Check them out [here](./TableScriptLibs.md).  

## Context imports
On top of that, template/plugin scripts can import each other:  
|import|Imports as|Availability|What is imported|
|---|---|---|---|
|`globals.<name>`|`globals_<name>`|All template/plugin scripts, globals, utils and properties|All the functions marked export of that global script|
|`utils.<name>`|`utils_<name>`|All template/plugin scripts, globals, utils and properties|All the functions marked export of that util script|
|`scripts.<name>`|`scripts_<name>`|All template/plugin scripts, utils(when added from scripts) and properties|All the functions marked export of that script|
|`properties`|`properties`|All template scripts and utils(when added from scripts)|All the functions marked export of the properties script|

The purpose of utils is just to be imported into other scripts. They are not runnable, they only have functions that other scripts can import. They can be used to avoid repetition.  

---
**Tebas provides another 5 imports**:  

## paths import
This import is available in all scripts, globals, utils and properties. More info [here](./pathsImport.md).  

## tebas import
This import is available in all scripts, globals, utils and properties. More info [here](./tebasImport.md).  

## tebastemplate import
This import is available in all template scripts, globals, utils and properties. More info [here](./tebastemplateImport.md).  

## tebasplugin import
This import is available in all plugin scripts, globals and utils. More info [here](./tebaspluginImport.md).  

## tebasproject import
This import is available in all template/plugin scripts, utils (when added from scripts) and template properties. More info [here](./tebasprojectImport.md).  
