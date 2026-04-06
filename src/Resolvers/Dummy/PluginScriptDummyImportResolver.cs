using System;
using TabScript;

class PluginScriptDummyImportResolver : PluginDummyImportResolver{
	public PluginScriptDummyImportResolver(Dictionary<string, ResolvedImport> dict) : base(dict){
		
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "tebasproject":
				return TebasProjectImportGenerator.GenerateDummy();
			default:
				if(import.StartsWith("scripts.")){
					ResolvedImport r = getFromDict(import);
					if(r != null){
						return r;
					}
				}
				
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}