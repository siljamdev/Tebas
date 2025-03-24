# Tebas
<img src="res/icon.png" width="200"/>

Project manager with git support

Command based utility to handle projects. The main focus is user-made templates, that are what provide the useful functionalities.
It will add a **project.tebas** file into your project.

There are channels, which are folders where projects will be created. Each project is in a separate folder and has a **project.tebas** file.
Each project is created with a template, which gives additional features.

There is an extensive command line help menu.

You can add -q before all your arguments to make it quiet

**ALL DOCUMENTATION WAS MADE BY HAND**

## Templates
These are user made. If you want to make a template, i suggest using the Creator Utility, looking into the structure of the [example templates](./documentation/example templates).
They are [AshFiles](https://github.com/Dumbelfo08/AshLib) with the **.tbtem** extension. You can double click them to install them.

## Plugins
These are user made. They are like templates, but instead of applying to a project, they are global. If you want to create a plugin, i suggest using the Creator Utility, looking into the structure of the [example templates](./documentation/example templates) (the format is the same, but it only uses script and resources folder)
They are [AshFiles](https://github.com/Dumbelfo08/AshLib) with the **.tbplg** extension. You can double click them to install them.

## Scripting language
The templates use a custom scripting language to provide functionality. You can look into [here](./documentation/scripts) for documentation and examples.  
Also, there is a Notepad++ Language file to provide syntax highlighting. You can find it [here](./n++). Note that this custom language is thought for the dark theme