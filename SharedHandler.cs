using System;
using AshLib.AshFiles;

public static class SharedHandler{
	static AshFile shared;
	
	static bool init;
	
	public static void initialize(){
		if(init){
			return;
		}
		
		//Tebas.initializeLocalSilent();
		
		shared = new AshFile(Tebas.dep.path + "/shared.ash");
		init = true;
	}
	
	public static void write(string name, string content){
		initialize();
		
		if(content == ""){
			shared.DeleteCamp(name);
		}else{
			shared.SetCamp(name, content);
		}
		
		shared.Save();
	}
	
	public static void append(string name, string content){
		initialize();
		
		if(shared.CanGetCamp(name, out string v)){
			shared.SetCamp(name, v + content);
		}else{
			shared.SetCamp(name, content);
		}
		
		shared.Save();
	}
	
	public static string read(string name){
		initialize();
		
		if(shared.CanGetCamp(name, out string v)){
			return v;
		}else{
			return "";
		}
	}
}