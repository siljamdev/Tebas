using System;
using TabScript;

class TemplateImportResolver : TebasImportResolver{
	protected Template template;
	TebasTemplateImportGenerator ttgen;
	
	public TemplateImportResolver(Template t) : base(t.tebasImportGenerator){
		template = t;
		ttgen = t.importGenerator;
	}
	
	public override ResolvedImport Resolve(string import, string callingFilename){
		switch(import){
			case "tebastemplate":
				return ttgen.Generate();
			default:
				if(import.StartsWith("globals.")){
					string gn = import.Substring(8);
					ResolvedImport r = template.getGlobalAsImport(gn);
					if(r != null){
						return r;
					}
				}
				
				if(import.StartsWith("utils.")){
					string gn = import.Substring(6);
					ResolvedImport r = template.getUtilAsImport(gn);
					if(r != null){
						return r;
					}
				}
				
				return base.Resolve(import, callingFilename); //Safely handle anything that wasnt recognized
		}
	}
}