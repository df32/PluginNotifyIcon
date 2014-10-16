using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Rainmeter;

namespace Rainmeter.NotifyIconPlugin
{
	public partial class PluginNotifyIcon
	{
		internal partial class Measure
		{
			
			bool _Initialized = false;
			bool _DynamicText = false;
			API rm;
			IntPtr skin;
			internal Measure()
			{
			}

			internal void Finalize()
			{
				notifyIcon.Dispose();
			}

			internal void Reload(Rainmeter.API api, ref double maxValue)
			{
				rm = api;
				skin = rm.GetSkin();
				if (_DynamicText || !_Initialized)
					_SetText();

				if (_Initialized) return;
				_Initialized = true;

				_SetIcon();
				
				if (rm.ReadInt("VisibleOnLoad", 0) > 0)
					_SetVisible(true);

				_SetAction();

				_DynamicText = rm.ReadInt("DynamicText", 0) > 0;
			}


			internal double Update()
			{
				return notifyIcon.Visible? 1.0: 0.0;
			}

			internal string GetString()
			{
			    return notifyIcon.Text;
			}

			internal void ExecuteBang(string args)
			{
				if (string.Compare(args, "Show", true) == 0)
				{
					_SetVisible(true);
				}
				else if (string.Compare(args, "Hide", true) == 0)
				{
					_SetVisible(false);
				}
				else if (string.Compare(args, "Toggle", true) == 0)
				{
					_SetVisible(null);
				}
				else if (string.Compare(args, "ReloadIcon", true) == 0)
				{
					_SetIcon();
				}
				else if (string.Compare(args, "ShowBalloonTip", true) == 0)
				{
					int timeout = rm.ReadInt("BalloonTipTimeOut", 1000);
					string text = rm.ReadString("BalloonTipText", "");
					string title = rm.ReadString("BalloonTipTitle", "");
					string icon = rm.ReadString("BalloonTipIcon", "");
					ToolTipIcon tipIcon = ToolTipIcon.None;

					icon.ToLower();
					if (icon.Contains("error"))
						tipIcon = ToolTipIcon.Error;
					else if (icon.Contains("info"))
						tipIcon = ToolTipIcon.Info;
					else if (icon.Contains("warning"))
						tipIcon = ToolTipIcon.Warning;

					try
					{
						notifyIcon.ShowBalloonTip(timeout, title, text, tipIcon);
					}
					catch (Exception ex)
					{
						API.Log(API.LogType.Error, "NotifyIcon: Unable to Show Balloon Tip: " + ex.Message);
					}
				}
				else
				{
					notifyIcon.Text = args;
				}
			}

			
		}

		public static class Plugin
		{
			[DllExport]
			public unsafe static void Initialize(void** data, void* rm)
			{
				uint id = (uint)((void*)*data);
				Measures.Add(id, new Measure());
			}

			[DllExport]
			public unsafe static void Finalize(void* data)
			{
				uint id = (uint)data;
				Measures[id].Finalize();
				Measures.Remove(id);
			}

			[DllExport]
			public unsafe static void Reload(void* data, void* rm, double* maxValue)
			{
				uint id = (uint)data;
				Measures[id].Reload(new Rainmeter.API((IntPtr)rm), ref *maxValue);
			}

			[DllExport]
			public unsafe static double Update(void* data)
			{
				uint id = (uint)data;
				return Measures[id].Update();
			}

			[DllExport]
			public unsafe static char* GetString(void* data)
			{
				uint id = (uint)data;
				fixed (char* s = Measures[id].GetString()) return s;
			}

			[DllExport]
			public unsafe static void ExecuteBang(void* data, char* args)
			{
				uint id = (uint)data;
				Measures[id].ExecuteBang(new string(args));
			}

			internal static Dictionary<uint, Measure> Measures = new Dictionary<uint, Measure>();
		}
	}
}
