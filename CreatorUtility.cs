using System;
using AshLib.AshFiles;

static class CreatorUtility{
	
	static AshFile t;
	static string path;
	
	public static void template(string p){
		path = p.removeQuotesSingle();
		
		Console.WriteLine("Welcome to the Tebas template creator!");
		Console.WriteLine();
		
		t = new AshFile();
		string name = ask("Name of the template:");
		while(!TemplateHandler.isNameValid(name)){
			Console.WriteLine("Invalid name. Try again");
			name = ask("Name of the template:");
		}
		t.SetCamp("name", name);
		
		t.SetCamp("version", Tebas.currentVersion);
		
		t.SetCamp("git.defaultUse", askTF("Uses git? (Y/N):"));
		
		loadFile("default.gitignore", "git.gitignore");
		
		if(File.Exists(path + "/readme.md")){
			t.SetCamp("addReadme", true);
			loadFile("readme.md", "readme");
		}else{
			t.SetCamp("addReadme", askTF("Add readme file? (Y/N):"));
		}
		
		if(File.Exists(path + "/description.txt")){
			loadFile("description.txt", "description");
		}
		
		loadFile("extensions.txt", "codeExtensions");
		loadFile("blacklist.txt", "codeFilesFolderBlacklist");
		
		if(Directory.Exists(path + "/scripts")){
			string[] scripts = Directory.GetFiles(path + "/scripts", "*.tbscr", SearchOption.TopDirectoryOnly);
			
			foreach(string scr in scripts){
				string scrn = Path.GetFileNameWithoutExtension(scr);
				if(!TemplateHandler.isScriptNameValid(scrn)){
					Console.WriteLine("Error in the name of script " + scrn);
					continue;
				}
				t.SetCamp("script." + scrn, File.ReadAllText(scr));
				Console.WriteLine("Loaded script " + scrn);
			}
		}else{
			Console.WriteLine("We could not find the scripts folder");
		}
		
		if(Directory.Exists(path + "/scripts/global")){
			string[] scripts = Directory.GetFiles(path + "/scripts/global", "*.tbscr", SearchOption.TopDirectoryOnly);
			
			foreach(string scr in scripts){
				string scrn = Path.GetFileNameWithoutExtension(scr);
				if(!TemplateHandler.isScriptNameValid(scrn)){
					Console.WriteLine("Error in the name of global script " + scrn);
					continue;
				}
				t.SetCamp("global." + scrn, File.ReadAllText(scr));
				Console.WriteLine("Loaded global script " + scrn);
			}
		}else{
			Console.WriteLine("We could not find the global scripts folder");
		}
		
		if(Directory.Exists(path + "/resources")){
			string[] resources = Directory.GetFiles(path + "/resources", "*", SearchOption.TopDirectoryOnly);
			
			foreach(string res in resources){
				string resn = Path.GetFileNameWithoutExtension(res);
				if(resn.Contains(' ')){
					Console.WriteLine("Error in the name of resource " + res);
					continue;
				}
				t.SetCamp("resources." + resn, File.ReadAllText(res));
				Console.WriteLine("Loaded resource " + res);
			}
		}else{
			Console.WriteLine("We could not find the resources folder");
		}
		
		t.Save(path + "/" + name + ".tbtem");
		
		Console.WriteLine("Output path: " + path + "/" + name + ".tbtem");
		
		Console.WriteLine("Bye!");
	}
	
	public static void plugin(string p){
		path = p.removeQuotesSingle();
		
		Console.WriteLine("Welcome to the Tebas plugin creator!");
		Console.WriteLine();
		
		t = new AshFile();
		string name = ask("Name of the plugin:");
		while(!PluginHandler.isNameValid(name)){
			Console.WriteLine("Invalid name. Try again");
			name = ask("Name of the template:");
		}
		t.SetCamp("name", name);
		
		t.SetCamp("version", Tebas.currentVersion);
		
		if(File.Exists(path + "/description.txt")){
			loadFile("description.txt", "description");
		}
		
		if(Directory.Exists(path + "/scripts")){
			string[] scripts = Directory.GetFiles(path + "/scripts", "*.tbscr", SearchOption.TopDirectoryOnly);
			
			foreach(string scr in scripts){
				string scrn = Path.GetFileNameWithoutExtension(scr);
				if(PluginHandler.isScriptNameValid(scrn)){
					Console.WriteLine("Error in the name of script " + scrn);
					continue;
				}
				t.SetCamp("script." + scrn, File.ReadAllText(scr));
				Console.WriteLine("Loaded script " + scrn);
			}
		}else{
			Console.WriteLine("We could not find the scripts folder");
		}
		
		if(Directory.Exists(path + "/resources")){
			string[] resources = Directory.GetFiles(path + "/resources", "*", SearchOption.TopDirectoryOnly);
			
			foreach(string res in resources){
				string resn = Path.GetFileNameWithoutExtension(res);
				if(resn.Contains(' ')){
					Console.WriteLine("Error in the name of resource " + res);
					continue;
				}
				t.SetCamp("resources." + resn, File.ReadAllText(res));
				Console.WriteLine("Loaded resource " + res);
			}
		}else{
			Console.WriteLine("We could not find the resources folder");
		}
		
		t.Save(path + "/" + name + ".tbplg");
		
		Console.WriteLine("Output path: " + path + "/" + name + ".tbplg");
		
		Console.WriteLine("Bye!");
	}
	
	static string ask(string q){
		Console.Write(q + " ");
		return Console.ReadLine();
	}
	
	static bool askTF(string q){
		string s;
		do{
			s = ask(q).ToLower();
		}while(s != "y" && s != "n");
		
		return (s == "y" ? true : false);
	}
	
	static void loadFile(string p, string o){
		if(File.Exists(path + "/" + p)){
			t.SetCamp(o, File.ReadAllText(path + "/" + p));
			Console.WriteLine("Read file " + path + "/" + p);
		}
	}
}