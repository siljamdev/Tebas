using AshLib.AshFiles;

public static class SharedHandler{
	static AshFile shared;
	
	public static void init(){
		shared = new AshFile(Tebas.dep.path + "/shared.ash");
	}
	
	public static void set(string key, string value){
		if(string.IsNullOrEmpty(value)){
			shared.Remove(key);
		}else{
			shared.Set(key, value);
		}
		shared.Save();
	}
	
	public static void append(string key, string value){
		if(string.IsNullOrEmpty(value)){
			return;
		}
		
		if(shared.TryGetValue(key, out string s)){
			shared.Set(key, s + value);
		}
		shared.Save();
	}
	
	public static string get(string key){
		if(shared.TryGetValue(key, out string s)){
			return s;
		}
		return null;
	}
	
	public static string[] getAll(){
		return shared.Keys.ToArray();
	}
	
	public static void cleanup(){
		foreach(KeyValuePair<string, object> keyVal in shared.Where(kvp => !(kvp.Value is string s && s.Length > 0)).ToList()){
			shared.Remove(keyVal.Key);
		}
		
		shared.Save();
	}
}