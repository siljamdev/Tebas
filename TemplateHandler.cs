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
	
	public static bool runScript(string name, IEnumerable<string> args = null){
		if(!(Tebas.template is null)){
			string code = "";
			if(!Tebas.template.CanGetCamp("script." + name, out code)){
				if(!Tebas.template.CanGetCamp("global." + name, out code)){
					return false;
				}
			}
			Script s = new Script(Tebas.tn + " " + name, code);
			s.run(args);
			return true;
		}
		return false;
	}
	
	public static bool runGlobal(string name, string script, IEnumerable<string> args = null){
		if(!Tebas.setContextTemplate(name)){
			return false;
		}
		if(!(Tebas.template is null)){
			string code = "";
			if(!Tebas.template.CanGetCamp("global." + script, out code)){
				return false;
			}
			Script s = new Script(Tebas.tn + " " + script, code);
			s.run(args);
			return true;
		}
		return false;
	}
	
	public static void install(string path, IEnumerable<string> args){
		
		path = StringHelper.removeQuotesSingle(path);
		
		if(!File.Exists(path)){
			Tebas.consoleError("That file does not exist");
			return;
		}
		
		AshFile template = new AshFile(path);
		
		string name;
		if(!template.CanGetCamp("name", out name)){
			Tebas.consoleError("Template is incorrectly formatted: name missing");
			return;
		}
		
		
		
		string desc = template.GetCampOrDefault<string>("description", null);
		if(desc != null){
			Tebas.consoleOutput("Template description: " + desc);
		}
		
		if(!Tebas.forced && Tebas.isConsoleInteractive() && exists(name)){
			Console.WriteLine("A template called " + name + " is already installed, do you want to update it? (Y/N)");
			string ans = Console.ReadLine();
			
			if(ans.ToLower() != "y"){
				return;
			}
		}
		
		if(template.CanGetCamp("version", out string vs)){
			int v = Tebas.isVersionNewer(vs);
			if(v == -1){
				Console.WriteLine("The template version(" + vs + ") is newer than the current tebas version(" + Tebas.currentVersion + "). Please update your client");
				return;
			}else if(v == 1 && !Tebas.forced){
				Console.WriteLine("The template version(" + vs + ") is older than the current tebas version(" + Tebas.currentVersion + "), do you want to install it? (Y/N)");
				string ans = Console.ReadLine();
				
				if(ans.ToLower() != "y"){
					return;
				}
			}
		}
		
		Tebas.tn = name;
		
		Tebas.template = template;
		
		Tebas.templateDirectory = Tebas.dep.path + "/templates/" + name;
		
		Tebas.workingDirectory = Tebas.templateDirectory;
		
		Directory.CreateDirectory(Tebas.templateDirectory);
		
		Directory.SetCurrentDirectory(Tebas.templateDirectory);
		
		template.Save(Tebas.templateDirectory + "/template.tbtem");
		
		runScript("install", args);
		
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
		
		if(Tebas.forced || Tebas.askDeletionConfirmation()){
			runScript("uninstall");
			Directory.Delete(Tebas.templateDirectory, true);
			Tebas.consoleOutput("Template uninstalled succesfully");
		}else{
			Tebas.consoleOutput("Uninstallation cancelled");
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
		
		string desc = Tebas.template.GetCampOrDefault<string>("description", null);
		if(desc != null){
			Tebas.consoleOutput("Template description: " + desc);
		}
		
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
		
		List<string> scripts = new List<string>();
		
		foreach(string k in Tebas.template.data.Keys){
			if(k.StartsWith("script.")){
				scripts.Add(k.Substring(7));
			}
		}
		
		if(scripts.Count > 0){
			Tebas.consoleOutput("This template has " + scripts.Count + " scripts:");
			
			foreach(string h in scripts){
				Tebas.consoleOutput("    " + h);
			}
		}
		
		List<string> globals = new List<string>();
		
		foreach(string k in Tebas.template.data.Keys){
			if(k.StartsWith("global.")){
				globals.Add(k.Substring(7));
			}
		}
		
		if(globals.Count > 0){
			Tebas.consoleOutput("This template has " + globals.Count + " global scripts:");
			
			foreach(string h in scripts){
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
			if(c == ""){
				Tebas.template.DeleteCamp("resources." + n);
			}else{
				Tebas.template.SetCamp("resources." + n, c);
			}
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
	
	public static bool isNameValid(string name){		
		if(name.Any(StringHelper.isWhitespace)){
			Tebas.consoleError("A template name cannot have spaces, template name was: '" + name + "'");
			return false;
		}
		
		if(name.Contains('*')){
			Tebas.consoleError("A template name cannot have '*', template name was: '" + name + "'");
			return false;
		}
		
		if(name.Contains('@')){
			Tebas.consoleError("A template name cannot have '@', template name was: '" + name + "'");
			return false;
		}
		
		if(name == ""){
			Tebas.consoleError("A template name cannot be empty, template name was: '" + name + "'");
			return false;
		}
		
		return true;
	}
	
	public static bool isScriptNameValid(string name){		
		if(name == "" || name.Any(StringHelper.isWhitespace) || name.Contains('*') || name.Contains('@')){
			return false;
		}
		return true;
	}
	
	public static string fixName(string name){
		if(name.StartsWith("@")){
			return name.Substring(1);
		}
		return name;
	}
}