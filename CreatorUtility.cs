using System;
using AshLib.AshFiles;

static class CreatorUtility{
	
	static AshFile t;
	static string path;
	
	public static void template(string p){
		path = StringHelper.removeQuotesSingle(p);
		
		Console.WriteLine("Welcome to the Tebas template creator!");
		Console.WriteLine();
		
		t = new AshFile();
		string name = ask("Name of the template:");
		t.SetCamp("name", name);
		
		t.SetCamp("git.defaultUse", askTF("Uses git?:"));
		
		loadFile("default.gitignore", "git.gitignore");
		
		if(File.Exists(path + "/readme.md")){
			t.SetCamp("addReadme", true);
			loadFile("readme.md", "readme");
		}else{
			t.SetCamp("addReadme", askTF("Add readme file?"));
		}
		
		
		loadFile("extensions.txt", "codeExtensions");
		loadFile("blacklist.txt", "codeFilesFolderBlacklist");
		
		if(Directory.Exists(path + "/scripts")){
			string[] scripts = Directory.GetFiles(path + "/scripts", "*.tbscr", SearchOption.TopDirectoryOnly);
			
			foreach(string scr in scripts){
				t.SetCamp("script." + Path.GetFileNameWithoutExtension(scr), File.ReadAllText(scr));
				Console.WriteLine("Loaded script " + Path.GetFileNameWithoutExtension(scr));
			}
		}else{
			Console.WriteLine("We could not find the scripts folder");
		}
		
		if(Directory.Exists(path + "/resources")){
			string[] resources = Directory.GetFiles(path + "/resources", "*", SearchOption.TopDirectoryOnly);
			
			foreach(string res in resources){
				t.SetCamp("resources." + Path.GetFileNameWithoutExtension(res), File.ReadAllText(res));
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
		path = StringHelper.removeQuotesSingle(p);
		
		Console.WriteLine("Welcome to the Tebas plugin creator!");
		Console.WriteLine();
		
		t = new AshFile();
		string name = ask("Name of the plugin:");
		t.SetCamp("name", name);
		
		if(Directory.Exists(path + "/scripts")){
			string[] scripts = Directory.GetFiles(path + "/scripts", "*.tbscr", SearchOption.TopDirectoryOnly);
			
			foreach(string scr in scripts){
				t.SetCamp(name + ".script." + Path.GetFileNameWithoutExtension(scr), File.ReadAllText(scr));
				Console.WriteLine("Loaded script " + Path.GetFileNameWithoutExtension(scr));
			}
		}else{
			Console.WriteLine("We could not find the scripts folder");
		}
		
		if(Directory.Exists(path + "/resources")){
			string[] resources = Directory.GetFiles(path + "/resources", "*", SearchOption.TopDirectoryOnly);
			
			foreach(string res in resources){
				t.SetCamp(name + ".resources." + Path.GetFileNameWithoutExtension(res), File.ReadAllText(res));
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
		}while(s != "true" && s != "false");
		
		return (s == "true" ? true : false);
	}
	
	static void loadFile(string p, string o){
		if(File.Exists(path + "/" + p)){
			t.SetCamp(o, File.ReadAllText(path + "/" + p));
			Console.WriteLine("Read file " + path + "/" + p);
		}
	}
}