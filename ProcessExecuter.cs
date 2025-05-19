using System;
using System.Text;
using System.Diagnostics;
using AshLib.Formatting;
using AshLib;

public static class ProcessExecuter{
    public static void runProcess(string name, string command, string arguments, string workingDirectory)
    {
        Tebas.initializeConfig();
		
		ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
			StandardOutputEncoding = Encoding.UTF8,
			StandardErrorEncoding = Encoding.UTF8
        };

        using (Process process = new Process { StartInfo = processInfo })
        {			
			FormatString nam = getName(name);
			FormatString err = getErrorName(name);
			bool useColors = Tebas.useColors();
			
			process.ErrorDataReceived += (sender, args) => {
                if(!string.IsNullOrEmpty(args.Data)){
                    // Display the standard error
					if(useColors){
						FormatString fs = err + new FormatString(args.Data, CharFormat.ResetAll);
						Console.Error.WriteLine(fs);
					}else{
						Console.Error.WriteLine(err.content + args.Data);
					}
                }
            };
			
            // Set up event handlers for capturing output and error asynchronously
            process.OutputDataReceived += (sender, args) => {
                if(!string.IsNullOrEmpty(args.Data)){
                    // Display the standard output
					if(useColors){
						FormatString fs = nam + new FormatString(args.Data, CharFormat.ResetAll);
						Console.WriteLine(fs);
					}else{
						Console.WriteLine(nam.content + args.Data);
					}
                }
            };
			
			// Start the process
            process.Start();
			
            // Begin asynchronous reading of the output and error streams
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for the process to exit
            process.WaitForExit();
        }
    }
	
	public static void runProcessNewWindow(string command, string arguments, string workingDirectory)
    {
		ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = true
        };

        using (Process process = new Process { StartInfo = processInfo })
        {
			// Start the process
            process.Start();
        }
    }
	
	public static bool isExecutableInPath(string exeName, out string p){
		// If no extension is provided, add .exe on Windows
		if(!exeName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)){
			exeName += ".exe";
		}
		
		// Get all PATH directories
		string? pathEnv = Environment.GetEnvironmentVariable("PATH");
		if(pathEnv == null){
			p = "";
			return false;
		}
		
		string windowsAppsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Microsoft", "WindowsApps");
		
		foreach(string path in pathEnv.Split(Path.PathSeparator)){
			string fullPath = path.Trim();
			
			if(fullPath.Equals(windowsAppsDir, StringComparison.OrdinalIgnoreCase)){
				continue;
			}
			
			fullPath = Path.Combine(fullPath, exeName);
			
			if(File.Exists(fullPath)){
				p = fullPath;
				return true;
			}
		}
		
		p = "";
		return false;
	}
	
	static FormatString getName(string n){
		if(Tebas.config.CanGetCamp("processShowsName", out bool b) && b){
			return new FormatString("[" + n + "] ", Tebas.blueCharFormat);
		}
		return new FormatString("");
	}
	
	static FormatString getErrorName(string n){
		if(Tebas.config.CanGetCamp("processShowsName", out bool b) && b){
			return new FormatString("[" + n + " ERROR] ", Tebas.errorCharFormat);
		}
		return new FormatString("[ERROR] ", Tebas.errorCharFormat);
	}
	
	public static void openLink(string url){
		try{
			if(OperatingSystem.IsWindows()){
				Process.Start(new ProcessStartInfo{
					FileName = url,
					UseShellExecute = true
				});
			}
			else if(OperatingSystem.IsLinux()){
				Process.Start("xdg-open", url);
			}
			else if(OperatingSystem.IsMacOS()){
				Process.Start("open", url);
			}
		}
		catch(Exception e){}
	}
	
	public static void runProcessWithOutput(string command, string arguments, string workingDirectory, List<string> output, List<string> error)
    {		
		ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
			StandardOutputEncoding = Encoding.UTF8,
			StandardErrorEncoding = Encoding.UTF8
        };

        using (Process process = new Process { StartInfo = processInfo })
        {			
			// Start the process
            process.Start();
			
			string o = process.StandardOutput.ReadToEnd();
			output.AddRange(o.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None));
			
			string e = process.StandardError.ReadToEnd();
			error.AddRange(e.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None));

            // Wait for the process to exit
            process.WaitForExit();
        }
    }
	
	public static int runProcessExitCode(string command, string arguments, string workingDirectory){
		ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true, // Output is redirected but ignored
            RedirectStandardError = true,  // Error is redirected but ignored
            UseShellExecute = false,
            CreateNoWindow = true
        };
		
		using (Process process = new Process { StartInfo = processInfo }){
			// Start the process
            process.Start();
			// Wait for the process to exit
            process.WaitForExit();
			return process.ExitCode;
		}
	}
}