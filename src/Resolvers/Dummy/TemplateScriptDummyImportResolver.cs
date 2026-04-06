using System;
using TabScript;

class TemplateScriptDummyImportResolver : TemplateDummyImportResolver{
	public TemplateScriptDummyImportResolver(Dictionary<string, ResolvedImport> dict) : base(dict){
		
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "tebasproject":
				return TebasProjectImportGenerator.GenerateDummy();
			default:
				if(import == "properties"){
					ResolvedImport r = getFromDict(import);
					if(r != null){
						return r;
					}
				}
				
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