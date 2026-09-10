using System;
using AshLib;
using AshLib.Formatting;
using TableScript;
using TableScript.Generator;

[TableScriptLibrary("tebastemplate.cs")]
partial class TebasTemplateImportGenerator{
	static TebasTemplateImportGenerator _dummy = null;
	public static TebasTemplateImportGenerator Dummy{get{
		if(_dummy == null){
			_dummy = new TebasTemplateImportGenerator();
		}
		return _dummy;
	}}
	
	Template template;
	ProcessExecuter px;
	FileUnit fu;
	
	FunctionStmt[] allFuncs => TableScriptFunctions.Concat(px.allFuncs).Concat(fu.allFuncs).ToArray();
	
	public TebasTemplateImportGenerator(Template t){
		template = t;
		
		px = new ProcessExecuter(getPath(), "template", false, hasPermission);
		fu = new FileUnit(getPath(), "template", "t.tbtem", false, hasPermission);
		
		path = getPath();
		name = getName();
		author = getAuthor();
		description = getDescription();
	}
	
	//Dummy
	private TebasTemplateImportGenerator(){
		px = ProcessExecuter.Dummy;
		fu = FileUnit.Dummy;
		
		//For docs
		//px = new ProcessExecuter("", "template", false, k => false);
		//fu = new FileUnit("", "template", "", false, k => false);
		
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
	/// Get the template path
	/// </summary>
	[TableScriptFunction]
	public string getPath(){
		return template.path;
	}
	
	/// <summary>
	/// Get the template name
	/// </summary>
	[TableScriptFunction]
	public string getName(){
		return template.name;
	}
	
	/// <summary>
	/// Get the template author if possible, or an empty string
	/// </summary>
	[TableScriptFunction]
	public string getAuthor(){
		return template.getAuthor() ?? "";
	}
	
	/// <summary>
	/// Get the template description if possible, or an empty string
	/// </summary>
	[TableScriptFunction]
	public string getDescription(){
		return template.getDescription() ?? "";
	}
	
	/// <summary>
	/// Get all script names
	/// </summary>
	[TableScriptFunction]
	public Table getAllScripts(){
		return new Table(template.getAllScriptNames());
	}
	
	/// <summary>
	/// Get all global script names
	/// </summary>
	[TableScriptFunction]
	public Table getAllGlobals(){
		return new Table(template.getAllGlobalNames());
	}
	
	/// <summary>
	/// Run a global script. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public bool runGlobal(string global, Table args){
		return template.tryRunGlobal(global, args.contents);
	}
	
	/// <summary>
	/// Check if the template has a permission
	/// </summary>
	[TableScriptFunction]
	public bool hasPermission(string key){
		return template.hasPermission(key);
	}
	
	/// <summary>
	/// Cleanup this template: cleans internal invalid or empty values
	/// </summary>
	[TableScriptFunction]
	public void cleanup(){
		template.cleanupInstance();
	}
	
	/// <summary>
	/// Get a template resource
	/// </summary>
	[TableScriptFunction]
	public string getResource(string key){
		return template.getResource(key);
	}
	
	/// <summary>
	/// Set a template resource
	/// </summary>
	[TableScriptFunction]
	public void setResource(string key, string value){
		template.setResource(key, value);
	}
	
	/// <summary>
	/// Append to the end of a template resource
	/// </summary>
	[TableScriptFunction]
	public void appendResource(string key, string value){
		template.appendResource(key, value);
	}
	
	/// <summary>
	/// Get all keys with a value in template resources
	/// </summary>
	[TableScriptFunction]
	public Table getAllResourceKeys(){
		return new Table(template.getAllResourceKeys());
	}
}