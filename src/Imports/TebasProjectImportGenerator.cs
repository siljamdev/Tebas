using System;
using AshLib;
using AshLib.Dates;
using AshLib.Formatting;
using TableScript;
using TableScript.Generator;

[TableScriptLibrary("tebasproject.cs")]
partial class TebasProjectImportGenerator{
	static ResolvedImport _dummy = null;
	public static ResolvedImport GenerateDummy(){
		if(_dummy == null){
			TebasProjectImportGenerator d = new TebasProjectImportGenerator();
			
			ProcessExecuter px = ProcessExecuter.Dummy;
			FileUnit fu = FileUnit.Dummy;
			InstallTool it = InstallTool.Dummy;
			
			//For docs
			//ProcessExecuter px = new ProcessExecuter("", "project", false, null);
			//FileUnit fu = new FileUnit("", "project", ".tebas", false, null);
			
			FunctionStmt[] allFuncs = d.TableScriptFunctions.Concat(px.allFuncs).Concat(fu.allFuncs).Concat(it.TableScriptFunctions).ToArray();
			
			return new ResolvedImport(d.TableScriptFilename, null, d.TableScriptGlobals, allFuncs);
		}
		return _dummy;
	}
	
	Project proj;
	
	public TebasProjectImportGenerator(Project p){
		proj = p;
		
		path = getPath();
		name = getName();
		templateName = getTemplateName();
	}
	
	//Dummy
	public TebasProjectImportGenerator(){
		path = "";
		name = "";
		templateName = "";
	}
	
	public ResolvedImport GenerateImport(bool isPlugin, Predicate<string> hasPermission){
		//Processes
		ProcessExecuter px = new ProcessExecuter(getPath(), "project", isPlugin, hasPermission);
		
		//Files
		FileUnit fu = new FileUnit(getPath(), "project", ".tebas", isPlugin, hasPermission);
		
		//Install
		InstallTool it = new InstallTool(getPath(), hasPermission);
		
		FunctionStmt[] allFuncs = TableScriptFunctions.Concat(px.allFuncs).Concat(fu.allFuncs).Concat(it.TableScriptFunctions).ToArray();
		
		return new ResolvedImport(TableScriptFilename, null, TableScriptGlobals, allFuncs);
	}
	
	/// <summary>
	/// Project path
	/// </summary>
	[TableScriptGlobal]
	public readonly string path;
	
	/// <summary>
	/// Project name
	/// </summary>
	[TableScriptGlobal]
	public readonly string name;
	
	/// <summary>
	/// Name of the template used in the project
	/// </summary>
	[TableScriptGlobal]
	public readonly string templateName;
	
	/// <summary>
	/// Get the project path
	/// </summary>
	[TableScriptFunction]
	public string getPath(){
		return proj.path;
	}
	
	/// <summary>
	/// Get the project name
	/// </summary>
	[TableScriptFunction]
	public string getName(){
		return proj.name;
	}
	
	/// <summary>
	/// Get the date and hour of creation in [yy, MM, dd, hh, mm, ss] format
	/// </summary>
	[TableScriptFunction]
	public Table getCreationDate(){
		Date d = proj.creationDate;
		return new Table(d.years.ToString(), d.months.ToString(), d.days.ToString(), d.hours.ToString(), d.minutes.ToString(), d.seconds.ToString());
	}
	
	/// <summary>
	/// Get the name of the template used in the project
	/// </summary>
	[TableScriptFunction]
	public string getTemplateName(){
		return proj.templateName;
	}
	
	/// <summary>
	/// Run a template script or global. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public bool runScriptOrGlobal(string script, Table args){
		return proj.tryRunScriptOrGlobal(script, args.contents);
	}
	
	/// <summary>
	/// Get all script names
	/// </summary>
	[TableScriptFunction]
	public bool runScript(string script, Table args){
		return proj.tryRunScript(script, args.contents);
	}
	
	/// <summary>
	/// Get all script names
	/// </summary>
	[TableScriptFunction]
	public bool runPluginScriptOrGlobal(string plugin, string script, Table args){
		Plugin p = Plugin.get(plugin);
		if(p == null){
			return false;
		}
		return proj.tryRunPluginScriptOrGlobal(p, script, args.contents);
	}
	
	/// <summary>
	/// Run a template script. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public bool runPluginScript(string plugin, string script, Table args){
		Plugin p = Plugin.get(plugin);
		if(p == null){
			return false;
		}
		return proj.tryRunPluginScript(p, script, args.contents);
	}
	
	/// <summary>
	/// Get a project property or an empty table if not implemented
	/// </summary>
	[TableScriptFunction]
	public Table getProperty(string key){
		return proj.getProperty(key);
	}
	
	/// <summary>
	/// Cleanup this project: cleans internal invalid or empty values
	/// </summary>
	[TableScriptFunction]
	public void cleanup(){
		proj.cleanupInstance();
	}
	
	/// <summary>
	/// Get a project resource
	/// </summary>
	[TableScriptFunction]
	public string getResource(string key){
		return proj.getResource(key);
	}
	
	/// <summary>
	/// Set a project resource
	/// </summary>
	[TableScriptFunction]
	public void setResource(string key, string value){
		proj.setResource(key, value);
	}
	
	/// <summary>
	/// Append to the end of a project resource
	/// </summary>
	[TableScriptFunction]
	public void appendResource(string key, string value){
		proj.appendResource(key, value);
	}
	
	/// <summary>
	/// Get all keys with a value in project resources
	/// </summary>
	[TableScriptFunction]
	public Table getAllResourceKeys(){
		return new Table(proj.getAllResourceKeys());
	}
	
	//Build
	/// <summary>
	/// Build a template from source in a directory in the project path. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public bool templateBuild(string sourceDirectory, string outDirectory){
		return Template.build(getPath() + "/" + sourceDirectory, getPath() + "/" + outDirectory);
	}
	
	/// <summary>
	/// Build a plugin from source in a directory in the project path. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public bool pluginBuild(string sourceDirectory, string outDirectory){
		return Plugin.build(getPath() + "/" + sourceDirectory, getPath() + "/" + outDirectory);
	}
}