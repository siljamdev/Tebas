# Tebas
<img src="res/icon.png" width="200"/>

Tebas is a project manager for coding projects with git support  
**It is still in development, so expect breaking changes**

## Features
- **Git basic management**: remotes, branches, commits, push and pull
- **.gitignore**: Auto handled by the templates to add sensible entires
- **README**: Auto handled by the templtes to add a default
- **Counting code lines**: Templates can define code file extensions
- **Project management**: Create projects with templates in different channels
- **Many useful templates**: Find templates [here](./templates)

## Usage
Tebas is a command based utility to handle projects. The main focus is [user-made templates](./templates), that are what provide the useful functionalities.  
It will add a **project.tebas** file into your project.

The interface to work with Tebas is command line arguments. For example:  
`tebas template list` will output a list of installed templates.  
There is also an extensive command line help menu.

**ALL DOCUMENTATION WAS MADE BY HAND**

### Channels & Projects
There are channels, which are folders where projects will be created.  
Each project is in a separate folder and has a **project.tebas** file.
The project is created with a template, which gives additional features.

### Templates
These are user made. They are files with the **.tbtem** extension. You can double click them to install them.  
Templates provide functionality by the way of scripts, that can do many tasks.

Many useful templates are present [here](./templates) for you to use, just download the **.tbtem** file and install it.

### Plugins
These are user made. They are like templates, but instead of applying to a project, they are global.  
They are files with the **.tbplg** extension. You can double click them to install them.

## Installation
You can install Tebas for windows x64 with the installer(made with [inno setup](https://jrsoftware.org/isinfo.php)), that will let you choose between installing for everyone or locally or download a portable executable.  
For windows x86, you can download the portable executable.  
For linux x64, you can download the portable executable, that **should** work.  
For mac, you can probably build the executable yourself.

## Development

### Templates
They are [AshFiles](https://github.com/Dumbelfo08/AshLib) with the **.tbtem** extension.
Templates have scripts that are written in a custom scripting language. Read the docs [here](./documentation/Tebas Script.pdf).

If you want to make a template, i suggest using the built-in Creator Utility, looking into the example templates in [templates](./templates) and reading the [template structure guide](./documentation/templateStructure.md). There are also [design guidelines](./documentation/templateGuidelines.md).

## Plugins
They are [AshFiles](https://github.com/Dumbelfo08/AshLib) with the **.tbplg** extension.
All scripts in a plugin are global.

If you want to make a plugin, i suggest using the built-in Creator Utility, looking into the example plugin in [templates](./templates) and reading the [plugin structure guide](./documentation/templateStructure.md). There are also [design guidelines](./documentation/templateGuidelines.md). (the format is the same as templates, but with less things)

## Scripting language
The templates use a custom scripting language to provide functionality. You can look into [here](./documentation/scripts) for documentation and examples.  
Also, there is a Notepad++ Language file to provide syntax highlighting. You can find it [here](./n++). Note that this custom language is thought for the dark theme.

## A Note about security
Please, be carefult with templates and plugins, as they can contain malicious code that can damage your project or even you computer. Scripts are powerful and as so, they can be used for making malware.
Check the source from where you download things. All templates and plugins in this repo are safe to use