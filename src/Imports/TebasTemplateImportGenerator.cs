using System;
using AshLib;
using AshLib.Formatting;
using TabScript;

class TebasTemplateImportGenerator{
	static TebasTemplateImportGenerator _dummy = null;
	public static TebasTemplateImportGenerator Dummy{get{
		if(_dummy == null){
			_dummy = new TebasTemplateImportGenerator();
		}
		return _dummy;
	}}
	
	(Delegate func, string description)[] AllFunctions => new (Delegate, string)[]{
		(getPath, "Get the template path"),
		(getName, "Get the template name"),
		(getAuthor, "Get the template author if possible, or an empty string"),
		(getDescription, "Get the template description if possible, or an empty string"),
		(getAllScripts, "Get all script names"),
		(getAllGlobals, "Get all global script names"),
		
		(runGlobal, "Run a global script. Returns true if the operation was successful"),
		
		(hasPermission, "Check if the template has a permission"),
		(cleanup, "Cleanup this template: cleans internal invalid or empty values"),
		
		(getResource, "Get a template resource"),
		(setResource, "Set a template resource"),
		(appendResource, "Append to the end of a template resource"),
	};
	
	Template template;
	ProcessExecuter px;
	FileUnit fu;
	
	ResolvedImport _generated;
	
	public TebasTemplateImportGenerator(Template t){
		template = t;
		
		px = new ProcessExecuter(getPath(), "template", false, hasPermission);
		fu = new FileUnit(getPath(), "template", "t.tbtem", false, null);
	}
	
	//Dummy
	private TebasTemplateImportGenerator(){
		px = ProcessExecuter.Dummy;
		fu = FileUnit.Dummy;
	}
	
	public ResolvedImport Generate(){
		if(_generated == null){
			_generated = Library.BuildLibrary("tebastemplate", AllFunctions.Concat(px.Functions).Concat(fu.Functions).ToArray());
		}
		
		return _generated;
	}
	
	string getPath(){
		return template.path;
	}
	
	string getName(){
		return template.name;
	}
	
	string getAuthor(){
		return template.getAuthor() ?? "";
	}
	
	string getDescription(){
		return template.getDescription() ?? "";
	}
	
	Table getAllScripts(){
		return new Table(template.getAllScriptNames());
	}
	
	Table getAllGlobals(){
		return new Table(template.getAllGlobalNames());
	}
	
	bool runGlobal(string global, Table args){
		return template.tryRunGlobal(global, args.contents);
	}
	
	bool hasPermission(string key){
		return template.hasPermission(key);
	}
	
	string getResource(string key){
		return template.getResource(key);
	}
	
	void setResource(string key, string value){
		template.setResource(key, value);
	}
	
	void appendResource(string key, string value){
		template.appendResource(key, value);
	}
	
	void cleanup(){
		template.cleanupInstance();
	}
}