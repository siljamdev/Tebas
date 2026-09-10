using System;
using AshLib;
using AshLib.Formatting;
using TableScript;
using TableScript.Generator;

[TableScriptLibrary("tebasplugin.cs")]
partial class TebasPluginImportGenerator{
	static TebasPluginImportGenerator _dummy = null;
	public static TebasPluginImportGenerator Dummy{get{
		if(_dummy == null){
			_dummy = new TebasPluginImportGenerator();
		}
		return _dummy;
	}}
	
	Plugin plugin;
	ProcessExecuter px;
	FileUnit fu;
	
	FunctionStmt[] allFuncs => TableScriptFunctions.Concat(px.allFuncs).Concat(fu.allFuncs).ToArray();
	
	public TebasPluginImportGenerator(Plugin t){
		plugin = t;
		
		px = new ProcessExecuter(getPath(), "plugin", true, hasPermission);
		fu = new FileUnit(getPath(), "plugin", "p.tbplg", true, null);  //Null is not an error here
		
		path = getPath();
		name = getName();
		author = getAuthor();
		description = getDescription();
	}
	
	//Dummy
	private TebasPluginImportGenerator(){
		px = ProcessExecuter.Dummy;
		fu = FileUnit.Dummy;
		
		//For docs
		//px = new ProcessExecuter("", "plugin", true, null);
		//fu = new FileUnit("", "plugin", "", true, null);
		
		//Globals need init regardless
		path = "";
		name = "";
		author = "";
		description = "";
	}
	
	ResolvedImport _generated = null;
	public ResolvedImport GenerateImport(){
		if(_generated == null){
			_generated = new ResolvedImport(TableScriptFilename, null, TableScriptGlobals, allFuncs);
		}
		return _generated;
	}
	
	/// <summary>
	/// Template path
	/// </summary>
	[TableScriptGlobal]
	public readonly string path;
	
	/// <summary>
	/// Template name
	/// </summary>
	[TableScriptGlobal]
	public readonly string name;
	
	/// <summary>
	/// Template author if possible, or an empty string
	/// </summary>
	[TableScriptGlobal]
	public readonly string author;
	
	/// <summary>
	/// Template description if possible, or an empty string
	/// </summary>
	[TableScriptGlobal]
	public readonly string description;
	
	/// <summary>
	/// Get the plugin path
	/// </summary>
	[TableScriptFunction]
	public string getPath(){
		return plugin.path;
	}
	
	/// <summary>
	/// Get the plugin name
	/// </summary>
	[TableScriptFunction]
	public string getName(){
		return plugin.name;
	}
	
	/// <summary>
	/// Get the plugin author if possible, or an empty string
	/// </summary>
	[TableScriptFunction]
	public string getAuthor(){
		return plugin.getAuthor() ?? "";
	}
	
	/// <summary>
	/// Get the plugin description if possible, or an empty string
	/// </summary>
	[TableScriptFunction]
	public string getDescription(){
		return plugin.getDescription() ?? "";
	}
	
	/// <summary>
	/// Get all script names
	/// </summary>
	[TableScriptFunction]
	public Table getAllScripts(){
		return new Table(plugin.getAllScriptNames());
	}
	
	/// <summary>
	/// Get all global script names
	/// </summary>
	[TableScriptFunction]
	public Table getAllGlobals(){
		return new Table(plugin.getAllGlobalNames());
	}
	
	/// <summary>
	/// Run a global script. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public bool runGlobal(string global, Table args){
		return plugin.tryRunGlobal(global, args.contents);
	}
	
	/// <summary>
	/// Check if the plugin has a permission
	/// </summary>
	[TableScriptFunction]
	public bool hasPermission(string key){
		return plugin.hasPermission(key);
	}
	
	/// <summary>
	/// Cleanup this plugin: cleans internal invalid or empty values
	/// </summary>
	[TableScriptFunction]
	public void cleanup(){
		plugin.cleanupInstance();
	}
	
	/// <summary>
	/// Get a plugin resource
	/// </summary>
	[TableScriptFunction]
	public string getResource(string key){
		return plugin.getResource(key);
	}
	
	/// <summary>
	/// Set a plugin resource
	/// </summary>
	[TableScriptFunction]
	public void setResource(string key, string value){
		plugin.setResource(key, value);
	}
	
	/// <summary>
	/// Append to the end of a plugin resource
	/// </summary>
	[TableScriptFunction]
	public void appendResource(string key, string value){
		plugin.appendResource(key, value);
	}
	
	/// <summary>
	/// Get all keys with a value in plugin resources
	/// </summary>
	[TableScriptFunction]
	public Table getAllResourceKeys(){
		return new Table(plugin.getAllResourceKeys());
	}
}