using System;
using TabScript;

class PluginScriptImportResolver : PluginImportResolver{
	TebasProjectImportGenerator tpgen;
	
	public PluginScriptImportResolver(Project p, Plugin plg) : base(plg){
		tpgen = p.importGenerator;
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "tebasproject":
				return tpgen.Generate(true, plugin.hasPermission);
			default:				
				if(import.StartsWith("scripts.")){
					string gn = import.Substring(8);
					ResolvedImport r = plugin.getScriptAsImport(gn);
					if(r != null){
						return r;
					}
				}
				
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}