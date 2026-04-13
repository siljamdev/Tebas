using System;
using System.Text;
using System.Diagnostics;
using TabScript;
using TabScript.StandardLibraries;

class ProcessExecuter{
	#region static
	static ProcessExecuter _dummy = null;
	public static ProcessExecuter Dummy{get{
		if(_dummy == null){
			_dummy = new ProcessExecuter(null, null, false, k => false);
		}
		return _dummy;
	}}
	
	
	static bool hasSeenProcessHint = false;
	
	static void displayProcesshint(){
		if(!hasSeenProcessHint){
			Tebas.hint("To skip this, do 'tebas <template|plugin> permission <name> skipProcessConfirmation allow'");
			hasSeenProcessHint = true;
		}
	}
	
	public static void openUrl(string url){
		try{
			if(OperatingSystem.IsWindows()){
				Process.Start(new ProcessStartInfo{
					FileName = url,
					UseShellExecute = true
				});
			}else if(OperatingSystem.IsLinux()){
				Process.Start("xdg-open", url);
			}else if(OperatingSystem.IsMacOS()){
				Process.Start("open", url);
			}
		}catch(Exception e){
			Tebas.report(e.ToString());
		}
	}
	#endregion
	
	public (string name, Delegate func, string description)[] NamedFunctions => new (string, Delegate, string)[]{
		("runProcess", runProcess, "Run a process in the " + pathName + " path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned"),
		("runProcessDetached", runProcessDetached, "Run a process detached in the " + pathName + " path, not printing its output. Returns false if any error occurred"),
		("runProcessWithOutput", runProcessWithOutput, "Run a process in the " + pathName + " path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num. If any error occurred, an empty table will be returned"),
		("runProcessSilent", runProcessSilent, "Run a process in the " + pathName + " path, not printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned"),
		("open", open, "Open a url, folder or file in the " + pathName + " path. Returns false if any error occurred"),
	};
	
	public (Delegate func, string description)[] Functions => NamedFunctions.Select(t => (t.func, t.description)).ToArray();
	
	Predicate<string> hasPermission;
	Action<Exception> report;
	
	readonly string basePath;
	readonly string pathName;
	
	public ProcessExecuter(string path, string name, bool isPlugin, Predicate<string> hp){
		basePath = path;
		pathName = name;
		hasPermission = hp;
		
		if(Tebas.config.GetValue<bool>("script.showLabel")){
			report = x => Tebas.labelReport("PROCESS", isPlugin ? Palette.plugin : Palette.template, x.GetType() + ": " + x.Message);
		}else{
			report = x => Tebas.report(x.GetType() + ": " + x.Message);
		}
	}
	
	string getFinalPath(string directory){
		return basePath + "/" + directory;
	}
	
	bool processAllowed(string command, string directory, Table arguments){
		string full = Path.GetFullPath(getFinalPath(directory));
		string normalizedBase = Path.GetFullPath(basePath);
		
		if(!full.StartsWith(normalizedBase + Path.DirectorySeparatorChar) && full != normalizedBase){
			report(new UnauthorizedAccessException("Process directory escape attempt: '" + directory + "'"));
			return false;
		}
		
		if(Tebas.config.GetValue<bool>("script.allowAllProcesses") || hasPermission("skipProcessConfirmation")){
			return true;
		}
		
		displayProcesshint();
		
		string n = command + (arguments != null ? (" " + string.Join(" ", arguments.contents)) : "");
		return Tebas.askConfirmation("Do you want to run '" + n + "'?");
	}
	
	public Table runProcess(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return new Table(0);
		}
		
		directory = getFinalPath(directory);
		
		try{
			string name = Path.GetFileNameWithoutExtension(command).ToUpper();
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
	
			using Process process = new Process{StartInfo = processInfo};
			
			//Figure out actions
			Action<string> stdout;
			Action<string> stderr;
			if(Tebas.config.GetValue<bool>("process.showLabel")){
				stdout = t => Tebas.labelOutput(name, Palette.process, t);
				stderr = t => Tebas.labelReport(name, Palette.process, t);
			}else{
				stdout = t => Tebas.output(t);
				stderr = t => Tebas.report(t);
			}
			
			process.OutputDataReceived += (sender, args) => {
				if(!string.IsNullOrEmpty(args.Data)){
					stdout(args.Data);
				}
			};
			
			// Set up event handlers for capturing output and error asynchronously
			process.ErrorDataReceived += (sender, args) => {
				if(!string.IsNullOrEmpty(args.Data)){
					stderr(args.Data);
				}
			};
			
			// Start the process
			process.Start();
			
			// Begin asynchronous reading of the output and error streams
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		
			// Wait for the process to exit
			process.WaitForExit();
			return new Table(process.ExitCode.ToString());
		}catch(Exception e){
			report(e);
			return new Table(0);
		}
	}
	
	public bool runProcessDetached(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return false;
		}
		
		directory = getFinalPath(directory);
		
		try{
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				UseShellExecute = true
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
	
			using Process process = new Process{StartInfo = processInfo};
			process.Start();
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	public Table runProcessWithOutput(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return new Table(0);
		}
		
		directory = getFinalPath(directory);
		
		try{
			List<string> output = new();
			List<string> error = new();
			
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
	
			using Process process = new Process{StartInfo = processInfo};
			process.Start();
			
			string[] o = process.StandardOutput.ReadToEnd().Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
			string[] e = process.StandardError.ReadToEnd().Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
			
			// Wait for the process to exit
			process.WaitForExit();
			return StdList.Build(new Table(o), new Table(e), new Table(process.ExitCode.ToString()));
		}catch(Exception e){
			report(e);
			return new Table(0);
		}
	}
	
	public Table runProcessSilent(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return new Table(0);
		}
		
		directory = getFinalPath(directory);
		
		try{
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				RedirectStandardOutput = true, // Output is redirected but ignored
				RedirectStandardError = true,  // Error is redirected but ignored
				UseShellExecute = false,
				CreateNoWindow = true
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
			
			using Process process = new Process{StartInfo = processInfo};
			process.Start();
			
			// Wait for the process to exit
			process.WaitForExit();
			return new Table(process.ExitCode.ToString());
		}catch(Exception e){
			report(e);
			return new Table(0);
		}
	}
	
	public bool open(string target){
		if(!processAllowed(target, basePath, null)){
			return false;
		}
		
		try{
			if(OperatingSystem.IsWindows()){
				Process.Start(new ProcessStartInfo{
					FileName = target,
					WorkingDirectory = basePath,
					UseShellExecute = true
				});
				return true;
			}else if(OperatingSystem.IsLinux()){
				Process.Start(new ProcessStartInfo{
					FileName = "xdg-open",
					Arguments = target,
					WorkingDirectory = basePath,
				});
				return true;
			}else if(OperatingSystem.IsMacOS()){
				Process.Start(new ProcessStartInfo{
					FileName = "open",
					Arguments = target,
					WorkingDirectory = basePath,
				});
				return true;
			}
			return false;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
}