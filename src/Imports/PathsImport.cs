using System;
using AshLib;
using AshLib.Formatting;
using TableScript;
using TableScript.Generator;

[TableScriptLibrary("paths.cs")]
static partial class PathsImport{	
	
	/// <summary>
	/// Default OS separator of paths
	/// </summary>
	[TableScriptGlobal]
	public static readonly string separator = Path.DirectorySeparatorChar.ToString();
	
	/// <summary>
	/// Get extension of a file path
	/// </summary>
	[TableScriptFunction]
	public static string getExtension(string path){
		return Path.GetExtension(path);
	}
	
	/// <summary>
	/// Get file name with extension of a file path
	/// </summary>
	[TableScriptFunction]
	public static string getFilename(string path){
		return Path.GetFileName(path);
	}
	
	/// <summary>
	/// Get file name without extension of a file path
	/// </summary>
	[TableScriptFunction]
	public static string getFilenameNoExtension(string path){
		return Path.GetFileNameWithoutExtension(path);
	}
	
	/// <summary>
	/// Get parent directory of a path
	/// </summary>
	[TableScriptFunction]
	public static string getDirectory(string path){
		return Path.GetDirectoryName(path);
	}
	
	/// <summary>
	/// Get default OS separator of paths
	/// </summary>
	[TableScriptFunction]
	public static string getSeparator(){
		return Path.DirectorySeparatorChar.ToString();
	}
}