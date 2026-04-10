using System;
using TabScript;
using TabScript.StandardLibraries;

class TebasDummyImportResolver : StandardImportResolver{	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "stdlib": //Custom stdlib
				return TebasImportResolver.stdlibImport;
			case "paths":
				return PathsImport.AsImport;
			case "tebas":
				return TebasImportGenerator.Dummy.Generate();
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