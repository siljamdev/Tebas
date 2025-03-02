using System;
using AshLib.AshFiles;

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
	
	public static bool runScript(string name, IEnumerable<string> args){
		if(!(Tebas.template is null)){
			string code = "";
			if(!Tebas.template.CanGetCamp("script." + name, out code)){
				return false;
			}
			Script s = new Script(Tebas.tn + " " + name, code);
			s.run(args);
			return true;
		}
		return false;
	}
	
	public static bool runScript(string name){
		if(!(Tebas.template is null)){
			string code = "";
			if(!Tebas.template.CanGetCamp("script." + name, out code)){
				return false;
			}
			Script s = new Script(Tebas.tn + " " + name, code);
			s.run(null);
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
		if(!template.CanGetCamp("name", out name)){
			Tebas.consoleOutput("Template is incorrectly formatted: name missing");
			return;
		}
		
		if(exists(name)){
			Console.WriteLine("A template with that name is already installed, do you want to update it? (Y/N)");
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
		
		if(Tebas.askDeletionConfirmation()){
			runScript("uninstall");
			Directory.Delete(Tebas.templateDirectory, true);
			Tebas.consoleOutput("Template uninstalled succesfully");
		}else{
			Tebas.consoleOutput("Deletion cancelled");
		}
	}
	
	public static List<string> getList(){
		string dir = Tebas.dep.path + "/templates";
		
		string[] directories = Directory.GetDirectories(dir);
		
		List<string> c = new List<string>();
		
		foreach(string s in directories){
			if(File.Exists(s + "/template.tbtem")){
				c.Add(Path.GetFileName(s));
			}
		}
		
		return c;
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
			Tebas.consoleOutput("Number of templates installed: " + c.Count);
			Tebas.consoleOutput("List of templates:");
		}else{
			Tebas.consoleOutput("No templates installed");
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
		if(Tebas.template.CanGetCamp("git.defaultUse", out bool b)){
			Tebas.consoleOutput("Use git: " + b);
		}
		
		if(Tebas.template.CanGetCamp("codeExtensions", out string s)){
			Tebas.consoleOutput("Code files extensions:");
			string[] p = s.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
			
			foreach(string h in p){
				Tebas.consoleOutput("    " + h);
			}
		}
	}
	
	public static string resourceRead(string n){
		if(!(Tebas.template is null)){
			if(Tebas.template.CanGetCamp("resources." + n, out string v)){
				return v;
			}else{
				return "";
			}
		}else{
			return "";
		}
	}
	
	public static void resourceWrite(string n, string c){
		if(!(Tebas.template is null)){
			Tebas.template.SetCamp("resources." + n, c);
			Tebas.template.Save();
		}
	}
	
	public static void resourceAppend(string n, string c){
		if(!(Tebas.template is null)){
			if(Tebas.template.CanGetCamp("resources." + n, out string v)){
				Tebas.template.SetCamp("resources." + n, v + c);
				Tebas.template.Save();
			}else{
				Tebas.template.SetCamp("resources." + n, c);
				Tebas.template.Save();
			}
		}
	}
}