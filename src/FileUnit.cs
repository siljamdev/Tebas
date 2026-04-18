using System;
using TabScript;

class FileUnit{
	#region static
	static FileUnit _dummy = null;
	public static FileUnit Dummy{get{
		if(_dummy == null){
			_dummy = new FileUnit(null, null, null, false, k => false);
		}
		return _dummy;
	}}
	
	static bool hasSeenFileHint = false;
	
	static void displayFilehint(){
		if(!hasSeenFileHint){
			Tebas.hint("To skip this, do 'tebas <template|plugin> permission <name> skipFileConfirmation allow'");
			hasSeenFileHint = true;
		}
	}
	#endregion
	
	public (string name, Delegate func, string description)[] NamedFunctions => new (string, Delegate, string)[]{
		("fileExists", fileExists, "Returns true if a file exists in the " + pathName + " path"),
		("fileRead", fileRead, "Reads whole text of a file in the " + pathName + " path"),
		("fileReadLines", fileReadLines, "Returns lines of text of a file in the " + pathName + " path"),
		("fileWrite", fileWrite, "Writes whole content to a file in the " + pathName + " path. Returns true if the operation was successful"),
		("fileWriteLines", fileWriteLines, "Writes whole lines to a file in the " + pathName + " path. Returns true if the operation was successful"),
		("fileAppend", fileAppend, "Appends content to the end of a file in the " + pathName + " path. Returns true if the operation was successful"),
		("fileAppendLines", fileAppendLines, "Appends lines to the end of a file in the " + pathName + " path. Returns true if the operation was successful"),
		("fileDelete", fileDelete, "Deletes a file in the " + pathName + " path. Returns true if the operation was successful"),
		("fileMove", fileMove, "Moves a file to a new location in the " + pathName + " path. Returns true if the operation was successful"),
		("fileCopy", fileCopy, "Copies a file to another location in the " + pathName + " path. Returns true if the operation was successful"),
		("fileSize", fileSize, "Get the size in bytes as a stdnum num of a file in the " + pathName + " path. Returns an empty string if any error occurred"),
		
		("folderExists", folderExists, "Returns true if a folder exists in the " + pathName + " path"),
		("folderCreate", folderCreate, "Create a folder in the " + pathName + " path. Returns true if the operation was successful"),
		("folderDelete", folderDelete, "Delete a folder in the " + pathName + " path. Returns true if the operation was successful"),
		("folderMove", folderMove, "Move a folder to a new location in the " + pathName + " path. Returns true if the operation was successful"),
		("folderListFiles", folderListFiles, "Get all file paths in the top directory of a folder in the " + pathName + " path. Returns a table with length -1 if any error occurred"),
		("folderListChildFiles", folderListChildFiles, "Get all file paths in all directories of a folder in the " + pathName + " path. Returns a table with length -1 if any error occurred"),
		("folderListFolders", folderListFolders, "Get all subfolder paths in a folder in the " + pathName + " path. Returns a table with length -1 if any error occurred"),
	};
	
	public (Delegate func, string description)[] Functions => NamedFunctions.Select(t => (t.func, t.description)).ToArray();
	
	Predicate<string> hasPermission;
	Action<Exception> report;
	
	readonly string basePath;
	readonly string pathName;
	
	readonly bool askConfirmation;
	
	readonly string protectedFile; //Normalized
	readonly bool hasProtectedFile;
	
	public FileUnit(string path, string name, string prot, bool isPlugin, Predicate<string> hp){
		basePath = path;
		pathName = name;
		hasProtectedFile = prot != null;
		if(hasProtectedFile){
			protectedFile = Path.GetFullPath(getFinalPath(prot));
		}
		hasPermission = hp;
		askConfirmation = hasPermission != null;
		
		if(Tebas.config.GetValue<bool>("script.showLabel")){
			report = x => Tebas.labelReport("FILE", isPlugin ? Palette.plugin : Palette.template, x.GetType() + ": " + x.Message);
		}else{
			report = x => Tebas.report(x.GetType() + ": " + x.Message);
		}
	}
	
	string getFinalPath(string path){
		return basePath + "/" + path;
	}
	
	bool checkPath(string path){
		string full = Path.GetFullPath(getFinalPath(path));
		string normalizedBase = Path.GetFullPath(basePath);
		
		if(!full.StartsWith(normalizedBase + Path.DirectorySeparatorChar) && full != normalizedBase){
			report(new UnauthorizedAccessException("File path escape attempt: '" + path + "'"));
			return false;
		}
		
		return true;
	}
	
	bool fileAllowed(string path, string question){
		if(!checkPath(path)){
			return false;
		}
		
		if(!askConfirmation || Tebas.config.GetValue<bool>("script.allowAllFileOperations") || hasPermission("skipFileConfirmation")){
			return true;
		}
		
		displayFilehint();
		
		return Tebas.askConfirmation("Do you want to " + question + " '" + path + "'?");
	}
	
	bool checkProtected(string path){
		if(!hasProtectedFile){
			return true;
		}
		
		string full = Path.GetFullPath(getFinalPath(path));
		
		if(full == protectedFile){
			report(new UnauthorizedAccessException("Protected file modify attempt: '" + path + "'"));
			return false;
		}
		
		return true;
	}
	
	bool checkBase(string path){
		string full = Path.GetFullPath(getFinalPath(path));
		string normalizedBase = Path.GetFullPath(basePath);
		
		if(full == normalizedBase){
			report(new UnauthorizedAccessException("Base path modify attempt: '" + path + "'"));
			return false;
		}
		
		return true;
	}
	
	void ensureParent(string path){
		string parent = Path.GetDirectoryName(getFinalPath(path));
		if(parent != null){
			Directory.CreateDirectory(parent);
		}
	}
	
	public bool fileExists(string path){
		return checkPath(path) ? File.Exists(getFinalPath(path)) : false;
	}
	
	public string fileRead(string path){
		if(!checkPath(path)){
			return null;
		}
		
		if(!File.Exists(getFinalPath(path))){
			return null;
		}
		
		try{
			return File.ReadAllText(getFinalPath(path));
		}catch(Exception e){
			report(e);
			return null;
		}
	}
	
	public Table fileReadLines(string path){
		if(!checkPath(path)){
			return new Table(-1);
		}
		
		if(!File.Exists(getFinalPath(path))){
			return new Table(-1);
		}
		
		try{
			return new Table(File.ReadAllLines(getFinalPath(path)));
		}catch(Exception e){
			report(e);
			return new Table(-1);
		}
	}
	
	public bool fileWrite(string path, string content){
		if(!checkProtected(path)){
			return false;
		}
		
		if(!fileAllowed(path, "write to file")){
			return false;
		}
		
		try{
			ensureParent(path);
			
			File.WriteAllText(getFinalPath(path), content);
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool fileWriteLines(string path, Table content){
		if(!checkProtected(path)){
			return false;
		}
		
		if(!fileAllowed(path, "write to file")){
			return false;
		}
		
		try{
			ensureParent(path);
			
			File.WriteAllLines(getFinalPath(path), content.contents.ToArray());
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool fileAppend(string path, string content){
		if(!checkProtected(path)){
			return false;
		}
		
		if(!fileAllowed(path, "write to file")){
			return false;
		}
		
		try{
			ensureParent(path);
			
			File.AppendAllText(getFinalPath(path), content);
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool fileAppendLines(string path, Table content){
		if(!checkProtected(path)){
			return false;
		}
		
		if(!fileAllowed(path, "write to file")){
			return false;
		}
		
		try{
			ensureParent(path);
			
			File.AppendAllLines(getFinalPath(path), content.contents.ToArray());
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool fileDelete(string path){
		if(!checkProtected(path)){
			return false;
		}
		
		if(!File.Exists(getFinalPath(path))){
			return false;
		}
		
		if(!fileAllowed(path, "delete file")){
			return false;
		}
		
		try{
			File.Delete(getFinalPath(path));
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool fileMove(string path, string newPath){
		if(!checkProtected(path) || !checkPath(newPath)){
			return false;
		}
		
		if(!File.Exists(getFinalPath(path)) || File.Exists(getFinalPath(newPath))){
			return false;
		}
		
		if(!fileAllowed(path, "move file")){
			return false;
		}
		
		try{
			ensureParent(newPath);
			
			File.Move(getFinalPath(path), getFinalPath(newPath));
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool fileCopy(string path, string copyPath){
		if(!checkPath(path) || !checkPath(copyPath)){
			return false;
		}
		
		if(!File.Exists(getFinalPath(path)) || File.Exists(getFinalPath(copyPath))){
			return false;
		}
		
		try{
			ensureParent(copyPath);
			
			File.Copy(getFinalPath(path), getFinalPath(copyPath));
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public string fileSize(string path){
		if(!checkPath(path)){
			return null;
		}
		
		if(!File.Exists(getFinalPath(path))){
			return null;
		}
		
		try{
			return new FileInfo(getFinalPath(path)).Length.ToString();
		}catch(Exception e){
			report(e);
			return null;
		}
	}
	
	public bool folderExists(string path){
		return checkPath(path) ? Directory.Exists(getFinalPath(path)) : false;
	}
	
	public bool folderCreate(string path){
		if(!checkPath(path)){
			return false;
		}
		
		try{
			Directory.CreateDirectory(getFinalPath(path));
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool folderDelete(string path){
		if(!checkBase(path)){
			return false;
		}
		
		if(!Directory.Exists(getFinalPath(path))){
			return false;
		}
		
		if(!fileAllowed(path, "delete folder")){
			return false;
		}
		
		try{
			Directory.Delete(getFinalPath(path), true);
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public bool folderMove(string path, string newPath){
		if(!checkBase(path) || !checkPath(newPath)){
			return false;
		}
		
		if(!Directory.Exists(getFinalPath(path))){
			return false;
		}
		
		if(!fileAllowed(path, "move folder")){
			return false;
		}
		
		try{
			Directory.Move(getFinalPath(path), getFinalPath(newPath));
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public Table folderListFiles(string path, string pattern){
		if(!checkPath(path)){
			return new Table(-1);
		}
		
		if(!Directory.Exists(getFinalPath(path))){
			return new Table(-1);
		}
		
		try{
			string b = getFinalPath("");
			return new Table(Directory.GetFiles(getFinalPath(path), pattern, SearchOption.TopDirectoryOnly).Select(p => p.StartsWith(b) ? p.Substring(b.Length) : p).ToArray());
		}catch(Exception e){
			report(e);
			return new Table(-1);
		}
	}
	
	public Table folderListChildFiles(string path, string pattern){
		if(!checkPath(path)){
			return new Table(-1);
		}
		
		if(!Directory.Exists(getFinalPath(path))){
			return new Table(-1);
		}
		
		try{
			string b = getFinalPath("");
			return new Table(Directory.GetFiles(getFinalPath(path), pattern, SearchOption.AllDirectories).Select(p => p.StartsWith(b) ? p.Substring(b.Length) : p).ToArray());
		}catch(Exception e){
			report(e);
			return new Table(-1);
		}
	}
	
	public Table folderListFolders(string path){
		if(!checkPath(path)){
			return new Table(-1);
		}
		
		if(!Directory.Exists(getFinalPath(path))){
			return new Table(-1);
		}
		
		try{
			string b = getFinalPath("");
			return new Table(Directory.GetDirectories(getFinalPath(path)).Select(p => p.StartsWith(b) ? p.Substring(b.Length) : p).ToArray());
		}catch(Exception e){
			report(e);
			return new Table(-1);
		}
	}
}