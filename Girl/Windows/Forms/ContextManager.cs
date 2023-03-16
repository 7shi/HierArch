// このファイルは ..\..\..\Girl.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Collections;
using System.Windows.Forms;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// MenuItem, ToolBarButton, Button の状態を管理します。
	/// </summary>
	public class ContextManager
	{
		protected bool[] flags;
		protected ArrayList[] cmdList;
		protected EventHandler[] handlers;
		protected ArrayList toolBars;
		protected Hashtable toolBarButtonHandlers;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public ContextManager()
		{
			int len;
			int i;

			len = this.MaxActions;
			
			this.flags = new bool[len];
			for (i = 0; i < len; i++)
			{
				this.flags[i] = false;
			}
			
			this.cmdList = new ArrayList[len];
			for (i = 0; i < len; i++)
			{
				this.cmdList[i] = new ArrayList();
			}
			
			this.handlers = null;
			
			this.toolBars = new ArrayList();
			this.toolBarButtonHandlers = new Hashtable();
		}

		public virtual int MaxActions
		{
			get
			{
				return 0;
			}
		}

		public void SetCommand(int action, object target)
		{
			ArrayList targets;

			targets = cmdList[action];
			if (targets.Contains(target)) return;
			
			targets.Add(target);
			this.SetProperty(target, flags[action]);
			
			this.SetHandler(action, target);
		}

		protected virtual void SetHandler(int action, object target)
		{
			if (target is MenuItem)
			{
				(target as MenuItem).Click += this.handlers[action];
			}
			else if (target is ToolBarButton)
			{
				ToolBarButton tbb = target as ToolBarButton;
				ToolBar tb = tbb.Parent;
				if (!this.toolBars.Contains(tb))
				{
					this.toolBars.Add(tb);
					tb.ButtonClick += new ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
				}
				this.toolBarButtonHandlers[tbb] = this.handlers[action];
			}
			else if (target is Button)
			{
				(target as Button).Click += this.handlers[action];
			}
		}

		protected void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			EventHandler eh;

			if (!this.toolBarButtonHandlers.Contains(e.Button)) return;
			
			eh = this.toolBarButtonHandlers[e.Button] as EventHandler;
			eh.Invoke(sender, EventArgs.Empty);
		}

		public void SetCommand(int action, params object[] targets)
		{
			foreach (object obj in targets)
			{
				this.SetCommand(action, obj);
			}
		}

		public void SetStatus(int action, bool status)
		{
			ArrayList targets;

			if (flags[action] == status) return;
			
			flags[action] = status;
			
			targets = cmdList[action];
			foreach (object obj in targets)
			{
				this.SetProperty(obj, status);
			}
		}

		protected virtual void SetProperty(object target, bool status)
		{
			if (target is MenuItem)
			{
				MenuItem mi = target as MenuItem;
				if (mi.Enabled != status) mi.Enabled = status;
			}
			else if (target is ToolBarButton)
			{
				ToolBarButton tbb = target as ToolBarButton;
				if (tbb.Enabled != status) tbb.Enabled = status;
			}
			else if (target is Button)
			{
				Button b = target as Button;
				if (b.Enabled != status) b.Enabled = status;
			}
		}

		public void SetStatus(bool status)
		{
			int len;
			int i;

			len = this.MaxActions;
			for (i = 0; i < len; i++)
			{
				this.SetStatus(i, status);
			}
		}
	}
}
