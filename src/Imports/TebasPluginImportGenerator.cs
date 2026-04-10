using System;
using AshLib;
using AshLib.Formatting;
using TabScript;

class TebasPluginImportGenerator{
	static TebasPluginImportGenerator _dummy = null;
	public static TebasPluginImportGenerator Dummy{get{
		if(_dummy == null){
			_dummy = new TebasPluginImportGenerator();
		}
		return _dummy;
	}}
	
	(Delegate func, string description)[] AllFunctions => new (Delegate, string)[]{
		(getPath, "Get the plugin path"),
		(getName, "Get the plugin name"),
		(getAuthor, "Get the plugin author if possible, or an empty string"),
		(getDescription, "Get the plugin description if possible, or an empty string"),
		(getAllScripts, "Get all script names"),
		(getAllGlobals, "Get all global script names"),
		
		(runGlobal, "Run a global script. Returns true if the operation was successful"),
		
		(hasPermission, "Check if the plugin has a permission"),
		(cleanup, "Cleanup this plugin: cleans internal invalid or empty values"),
		
		(getResource, "Get a plugin resource"),
		(setResource, "Set a plugin resource"),
		(appendResource, "Append to the end of a plugin resource"),
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
	
	//Dummy
	private TebasPluginImportGenerator(){
		px = ProcessExecuter.Dummy;
		fu = FileUnit.Dummy;
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