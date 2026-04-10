using System;
using AshLib;
using AshLib.Formatting;
using TabScript;

static class PathsImport{	
	static (Delegate func, string description)[] staticFunctions => new (Delegate, string)[]{
		(getExtension, "Get extension of a file path"),
		(getFilename, "Get file name with extension of a file path"),
		(getFilenameNoExtension, "Get file name without extension of a file path"),
		(getDirectory, "Get parent directory of a path"),
		(getSeparator, "Get default OS separator of paths"),
	};
	
	static ResolvedImport _compiled;
	public static ResolvedImport AsImport {get{
		if(_compiled == null){
			_compiled = Library.BuildLibrary("paths", staticFunctions);
		}
		return _compiled;
	}}
	
	static string getExtension(string path){
		return Path.GetExtension(path);
	}
	
	static string getFilename(string path){
		return Path.GetFileName(path);
	}
	
	static string getFilenameNoExtension(string path){
		return Path.GetFileNameWithoutExtension(path);
	}
	
	static string getDirectory(string path){
		return Path.GetDirectoryName(path);
	}
	
	static string getSeparator(){
		return Path.DirectorySeparatorChar.ToString();
	}
}