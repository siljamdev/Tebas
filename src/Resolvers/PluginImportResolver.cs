using System;
using TabScript;

class PluginImportResolver : TebasImportResolver{
	protected Plugin plugin;
	TebasPluginImportGenerator tpgen;
	
	public PluginImportResolver(Plugin p) : base(p.tebasImportGenerator){
		plugin = p;
		tpgen = p.importGenerator;
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "tebasplugin":
				return tpgen.Generate();
			default:
				if(import.StartsWith("globals.")){
					string gn = import.Substring(8);
					ResolvedImport r = plugin.getGlobalAsImport(gn);
					if(r != null){
						return r;
					}
				}
				
				if(import.StartsWith("utils.")){
					string gn = import.Substring(6);
					ResolvedImport r = plugin.getUtilAsImport(gn);
					if(r != null){
						return r;
					}
				}
				
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}