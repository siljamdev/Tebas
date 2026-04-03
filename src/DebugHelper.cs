using System;

public static class DebugHelper{
	public static void firstTest(){
		//int exitCode = ProcessExecuter.runProcessExitCode("git", "remote get-url origin2", "");
		//Console.WriteLine(exitCode);
		
		Script s = loadFromFile("test.tbscr");
		s.run(null);
		
		//Console.ReadLine();
	}
	
	static Script loadFromFile(string path){
		Script s = new Script(Path.GetFileName(path), ScriptType.Standalone, File.ReadAllText(path));
		return s;
	}
}