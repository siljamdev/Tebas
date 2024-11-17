using System;
using AshLib;

public static class TemplateHandler{
	public static bool exists(string name){
		if(Directory.Exists(Tebas.dep.path + "/templates/" + name)){
			if(File.Exists(Tebas.dep.path + "/templates/" + name + "/template.tbtem")){
				return true;
			}
			return false;
		}
		return false;
	}
	
	public static AshFile get(string name){
		return new AshFile(Tebas.dep.path + "/templates/" + name + "/template.tbtem");
	}
	
	public static bool runScript(string name){
		if(!(Tebas.template is null)){
			string code = "";
			if(!Tebas.template.CanGetCampAsString("script." + name, out code)){
				return false;
			}
			Script s = new Script(Tebas.tn + " " + name, code);
			s.run();
			return true;
		}
		return false;
	}
	
	public static void install(string path){
		
		path = StringHelper.removeQuotesSingle(path);
		
		if(!File.Exists(path)){
			Tebas.consoleOutput("That file does not exist");
			return;
		}
		
		AshFile template = new AshFile(path);
		
		string name;
		if(!template.CanGetCampAsString("name", out name)){
			Tebas.consoleOutput("Template is incorrectly formatted: name missing");
			return;
		}
		
		if(TemplateHandler.exists(name)){
			Console.WriteLine("A template with that name is already installed, do you want to update it?");
			string ans = Console.ReadLine();
			
			if(ans.ToLower() != "y"){
				return;
			}
		}
		
		Tebas.tn = name;
		
		Tebas.template = template;
		
		Tebas.templateDirectory = Tebas.dep.path + "/templates/" + name;
		
		Tebas.workingDirectory = Tebas.templateDirectory;
		
		Directory.CreateDirectory(Tebas.templateDirectory);
		
		Directory.SetCurrentDirectory(Tebas.templateDirectory);
		
		template.Save(Tebas.templateDirectory + "/template.tbtem");
		
		runScript("install");
		
		Tebas.consoleOutput("Template succesfully installed: " + Tebas.tn);
	}
	
	public static void uninstall(string name){		
		if(!exists(name)){
			Tebas.consoleOutput("That template is not installed");
			return;
		}
		
		Tebas.tn = name;
		
		Tebas.template = get(name);
		
		Tebas.templateDirectory = Tebas.dep.path + "/templates/" + name;
		
		Tebas.workingDirectory = Tebas.templateDirectory;
		
		runScript("delete");
		
		if(Tebas.askDeletionConfirmation()){
			Directory.Delete(Tebas.templateDirectory, true);
			Tebas.consoleOutput("Template uninstalled succesfully");
		}else{
			Tebas.consoleOutput("Deletion cancelled");
		}
	}
	
	public static void list(){
		string dir = Tebas.dep.path + "/templates";
		
		string[] directories = Directory.GetDirectories(dir);
		
		List<string> c = new List<string>();
		
		foreach(string s in directories){
			if(File.Exists(s + "/template.tbtem")){
				c.Add(s);
			}
		}
		
		if(c.Count > 0){
			Tebas.consoleOutput("List of templates:");
		}
		
		foreach(string s in c){
			Tebas.consoleOutput("    " + Path.GetFileName(s));
		}
	}
	
	public static void info(string name){
		if(!exists(name)){
			Tebas.consoleOutput("That template is not installed");
			return;
		}
		
		Tebas.tn = name;
		
		Tebas.template = get(name);
		
		Tebas.templateDirectory = Tebas.dep.path + "/templates/" + name;
		
		Tebas.workingDirectory = Tebas.templateDirectory;
		
		Tebas.consoleOutput("Template name: " + Tebas.tn);
		if(Tebas.template.CanGetCampAsBool("git.defaultUse", out bool b)){
			Tebas.consoleOutput("Use git: " + b);
		}
		
		if(Tebas.template.CanGetCampAsString("codeExtensions", out string s)){
			Tebas.consoleOutput("Code files extensions:");
			string[] p = s.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
			
			foreach(string h in p){
				Tebas.consoleOutput("    " + h);
			}
		}
	}
}