using System;
using TabScript;

class TemplateScriptImportResolver : TemplateImportResolver{
	TebasProjectImportGenerator tpgen;
	
	public TemplateScriptImportResolver(Project p, Template t) : base(t){
		tpgen = p.importGenerator;
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "tebasproject":
				return tpgen.Generate(false, template.hasPermission);
			default:
				if(import == "properties"){
					ResolvedImport r = template.getPropertiesAsImport();
					if(r != null){
						return r;
					}
				}
				
				if(import.StartsWith("scripts.")){
					string gn = import.Substring(8);
					ResolvedImport r = template.getScriptAsImport(gn);
					if(r != null){
						return r;
					}
				}
				
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}