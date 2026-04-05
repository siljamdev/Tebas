using System;
using TabScript;
using TabScript.StandardLibraries;

class TebasImportResolver : StandardImportResolver{
	#region stdlib
	static (Delegate func, string description)[] stdlibFuncs => StdLib.AllFunctions.Where(t => t.func != StdLib.print && t.func != StdLib.error && t.func != StdLib.input).ToArray();
	
	static ResolvedImport _stdlibImport = null;
	static ResolvedImport stdlibImport {get{
		if(_stdlibImport == null){
			_stdlibImport = Library.BuildLibrary("stdlib", stdlibFuncs);
		}
		return _stdlibImport;
	}}
	#endregion
	
	TebasImportGenerator tgen;
	
	public TebasImportResolver(TebasImportGenerator tg){
		tgen = tg;
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "stdlib": //Custom stdlib
				return stdlibImport;
			case "tebas":
				return tgen.Generate();
			case "tebasproject":
			case "tebastemplate":
			case "tebasplugin":
				base.OnReport(new TabScriptException(TabScriptErrorType.Resolver, callingFilename, -1, "Import '" + import + "' is not available right now because of the type of the script"));
				return new ResolvedImport("tebas import resolver error", null, null, null);
			default:
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}