using System;
using TabScript;

class PluginDummyImportResolver : TebasDummyImportResolver{
	Dictionary<string, ResolvedImport> dictionary;
	
	public PluginDummyImportResolver(Dictionary<string, ResolvedImport> dict){
		dictionary = dict;
	}
	
	protected ResolvedImport getFromDict(string n){
		if(dictionary.TryGetValue(n, out ResolvedImport r)){
			return r;
		}
		return null;
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "tebasplugin":
				return TebasPluginImportGenerator.Dummy.Generate();
			default:
				if(import.StartsWith("globals.")){
					ResolvedImport r = getFromDict(import);
					if(r != null){
						return r;
					}
				}
				
				if(import.StartsWith("utils.")){
					ResolvedImport r = getFromDict(import);
					if(r != null){
						return r;
					}
				}
				
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}