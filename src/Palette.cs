using AshLib;
using AshLib.Formatting;

static class Palette{
	public static CharFormat? error = new CharFormat(new Color3("#E54548"));
	public static CharFormat? warn = new CharFormat(new Color3("#C19C00"));
	public static CharFormat? confirmation = new CharFormat(new Color3("#B1FF57"));
	
	public static CharFormat? plugin = new CharFormat(new Color3("#00AFFF"));
	public static CharFormat? template = new CharFormat(new Color3("#B542FD"));
	public static CharFormat? process = new CharFormat(new Color3("#FFA811"));
	
	public static bool useColors = false;
	
	public static void init(){
		useColors = Tebas.config.GetValue<bool>("useColors") && !Console.IsOutputRedirected;
	}
}