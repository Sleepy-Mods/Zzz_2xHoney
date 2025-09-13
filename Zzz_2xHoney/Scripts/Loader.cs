using HarmonyLib;
using System.Reflection;

namespace zzz_honey
{
	public class Loader : IModApi
	{
		public void InitMod(Mod mod)
		{
			var harmony = new Harmony("com.sophia.zzz_honey");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			try { Log.Out("zzz_honey: Harmony loaded and patches applied."); } catch { }
		}
	}
}


