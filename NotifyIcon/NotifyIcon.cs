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
			
			NotifyIcon notifyIcon = new NotifyIcon();

			internal void _SetIcon()
			{
				var icoUrl = rm.ReadPath("Icon", "");
				if (!File.Exists(icoUrl))
				{
					API.Log(API.LogType.Error, "NotifyIcon: Invalid Icon: " + icoUrl);
					return;
				}
				try
				{
					Icon ico = new Icon(icoUrl);
					notifyIcon.Icon = ico;
				}
				catch (Exception ex)
				{
					API.Log(API.LogType.Error, "NotifyIcon: Invalid Icon: " + ex.Message);
				}
			}

			internal void _SetText()
			{
				notifyIcon.Text = rm.ReadString("Text", rm.GetSkinName());
			}

			internal void _SetVisible(bool? visible)
			{
				if (visible == true)
					notifyIcon.Visible = true;
				else if (visible == false)
					notifyIcon.Visible = false;
				else
					notifyIcon.Visible = !notifyIcon.Visible;
			}

			internal void _SetAction()
			{
				notifyIcon.Click += (sender, args) =>
					{
						_ExecuteAction("OnClickAction");
					};

				#region 一大波Action接近中
				notifyIcon.DoubleClick += (sender, args) =>
					{
						_ExecuteAction("OnDoubleClickAction");
					};

				notifyIcon.MouseClick += (sender, args) =>
				{
					_ExecuteMouseAction("MouseClick", args);
				};

				notifyIcon.MouseDoubleClick += (sender, args) =>
				{

					_ExecuteMouseAction("MouseDoubleClick", args);
				};


				notifyIcon.MouseDown += (sender, args) =>
				{
					_ExecuteMouseAction("MouseDown", args);
				};

				notifyIcon.MouseMove += (sender, args) =>
				{
					_ExecuteAction("MouseMoveAction");
				};

				notifyIcon.MouseUp += (sender, args) =>
				{
					_ExecuteMouseAction("MouseUp", args);
				};

				notifyIcon.BalloonTipShown += (sender, args) =>
				{
					_ExecuteAction("BalloonTipShownAction");
				};


				notifyIcon.BalloonTipClicked += (sender, args) =>
				{
					_ExecuteAction("BalloonTipClickedAction");
				};


				notifyIcon.BalloonTipClosed += (sender, args) =>
				{
					_ExecuteAction("BalloonTipClosedAction");
				};

				#endregion
			}

			void _ExecuteAction(string option)
			{
				var action = rm.ReadString(option, "");
				if (!string.IsNullOrEmpty(action))
					API.Execute(skin, action);
			}

			void _ExecuteMouseAction(string option, MouseEventArgs args)
			{
				option += "Action";

				if (args.Button == MouseButtons.Left)
				{
					option = ("Left" + option);
				}
				else if (args.Button == MouseButtons.Right)
				{
					option = ("Right" + option);
				}
				else if (args.Button == MouseButtons.Middle)
				{
					option = ("Middle" + option);
				}
				else
				{
					option = ("Other" + option);
				}

				var action = rm.ReadString(option, "");
				if (!string.IsNullOrEmpty(action))
				{
					//action = action
					//	.Replace("$Mouse:X$", args.X.ToString())
					//	.Replace("$Mouse:Y$", args.Y.ToString());
					//API.Log(API.LogType.Debug, action);
					API.Execute(skin, action);
				}
			}
			
		}
	}
}
