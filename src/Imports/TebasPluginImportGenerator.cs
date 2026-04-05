using System;
using AshLib;
using AshLib.Formatting;
using TabScript;

class TebasPluginImportGenerator{
	(Delegate func, string description)[] AllFunctions => new (Delegate, string)[]{
		(getPath, "Get the plugin path"),
		(getName, "Get the plugin name"),
		(getAuthor, "Get the plugin author if possible"),
		(getDescription, "Get the plugin description if possible"),
		(getAllScripts, "Get all script names"),
		(getAllGlobals, "Get all global script names"),
		
		(runGlobal, "Run a global script"),
		
		(getResource, "Get a plugin resource"),
		(setResource, "Set a plugin resource"),
		(appendResource, "Append to a plugin resource"),
		
		(hasPermission, "Get if the plugin has a permission"),
		(cleanup, "Cleanup this plugin"),
	};
	
	Plugin plugin;
	ProcessExecuter px;
	FileUnit fu;
	
	ResolvedImport _generated;
	
	public TebasPluginImportGenerator(Plugin t){
		plugin = t;
		
		px = new ProcessExecuter(getPath(), "plugin", true, hasPermission);
		fu = new FileUnit(getPath(), "plugin", "p.tbplg", true, null);
	}
	
	public ResolvedImport Generate(){
		if(_generated == null){
			_generated = Library.BuildLibrary("tebasplugin", AllFunctions.Concat(px.Functions).Concat(fu.Functions).ToArray());
		}
		
		return _generated;
	}
	
	string getPath(){
		return plugin.path;
	}
	
	string getName(){
		return plugin.name;
	}
	
	string getAuthor(){
		return plugin.getAuthor() ?? "";
	}
	
	string getDescription(){
		return plugin.getDescription() ?? "";
	}
	
	Table getAllScripts(){
		return new Table(plugin.getAllScriptNames());
	}
	
	Table getAllGlobals(){
		return new Table(plugin.getAllGlobalNames());
	}
	
	bool runGlobal(string global, Table args){
		return plugin.tryRunGlobal(global, args.contents);
	}
	
	bool hasPermission(string key){
		return plugin.hasPermission(key);
	}
	
	string getResource(string key){
		return plugin.getResource(key);
	}
	
	void setResource(string key, string value){
		plugin.setResource(key, value);
	}
	
	void appendResource(string key, string value){
		plugin.appendResource(key, value);
	}
	
	void cleanup(){
		plugin.cleanupInstance();
	}
}