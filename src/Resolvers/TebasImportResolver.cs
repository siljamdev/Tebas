using System;
using TableScript;
using TableScript.StandardLibraries;

class TebasImportResolver : StandardImportResolver{
	#region stdlib
	static FunctionStmt[] stdlibFuncs => StdLib.TableScriptFunctions.Where(f => f.identifier != "print" && f.identifier != "error" && f.identifier != "input").ToArray();
	
	static ResolvedImport _stdlibImport = null;
	public static ResolvedImport stdlibImport {get{
		if(_stdlibImport == null){
			_stdlibImport = new ResolvedImport(StdLib.TableScriptFilename, null, StdLib.TableScriptGlobals, stdlibFuncs);
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
			case "paths":
				return PathsImport.TableScriptImport;
			case "tebas":
				return tgen.TableScriptImport;
			case "tebasproject":
			case "tebastemplate":
			case "tebasplugin":
				base.OnReport(new TableScriptException(TableScriptErrorType.Resolver, callingFilename, -1, "Import '" + import + "' is not available right now because of the type of the script"));
				return new ResolvedImport("tebas import resolver error", null, null, null);
			default:
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}