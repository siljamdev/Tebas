# Tebas
<img src="res/icon.png" width="200"/>

Tebas is a project manager based on templates & plugins  
**Tebas is still in development, so expect breaking changes**

## Usage
Tebas is a command based utility to handle projects. Its strength is [user-made templates and plugins](https://github.com/siljamdev/Tebas-Registry), which provide the useful functionalities.  
It adds a `.tebas` file into a folder, registering it as a project.

The interface to use Tebas is the command line. For example:  
`tebas template list` will output a list of installed templates.  
There is an extensive command line help menu, accessed with `tebas --help`.  
Additionally, the [numerical exit codes](./documentation/exitCodes.md) can give a lot of information about what went wrong.  

### Templates
Each project has a template linked to it, that provides functionality in the form of scripts and globals.  
Templates are user made. They are files with the `.tbtem` extension. You can double click them to install them.  
Install templates directly from the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry).

### Plugins
Plugins are global, and also provide functionality in the form of scripts and globals.  
They are user made. They are files with the `.tbplg` extension. You can double click them to install them.  
Install plugins directly from the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry).

## Installation
You can install Tebas for Windows, Linux and MacOS with the portable executable from the [releases](https://github.com/siljamdev/Tebas/releases/latest).  

## License
This software is licensed under the [MIT License](./LICENSE).

## Development
If you want to develop templates or plugins, first of all familiarize yourself with the [TableScript](https://github.com/siljamdev/TableScript) scripting language.  
Then, read about the [imports](./documentation/imports/imports.md) that are available in Tebas scripts.  
Also, try to keep in mind the [design guidelines](./documentation/designGuidelines.md).  
Once you have developed a working template/plugin, make a PR to the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry).  

### Templates
Before creating templates, read about their [structure](./documentation/templateStructure.md).  
Also, we suggest using the [tbtem template](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/tbtem).  

### Plugins
Before creating plugins, read about their [structure](./documentation/pluginStructure.md).  
Also, we suggest using the [tbplg template](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/tbplg).  

## A Note about security
Please, be careful with templates and plugins. They cannot access private files and cannot run process without confirmation/permission, but could contain code that can damage your project.  
Check the source from where you download things. All templates and plugins in the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry) are certified safe to use.  