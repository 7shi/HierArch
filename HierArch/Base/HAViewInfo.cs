// このファイルは ..\..\HierArch.haprj から自動生成されています。
// 編集は必ずそちらを通すようにして、直接書き換えないでください。

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Girl.HierArch
{
	/// <summary>
	/// ウィンドウの状態を保持します。
	/// </summary>
	public class HAViewInfo
	{
		public int X;
		public int Y;
		public int Width;
		public int Height;
		public FormWindowState State;
		public bool ShowClass;
		public bool ShowFunc;
		public bool ShowComment;
		public bool ShowMember;
		public bool ShowArg;
		public bool ShowObject;
		public int LeftPanelWidth;
		public int ClassHeight;
		public int CommentHeight;
		public int RightPanelWidth;
		public int MemberHeight;
		public int ObjectHeight;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public HAViewInfo()
		{
			this.Init();
		}

		public void Init()
		{
			this.X = this.Y = this.Width = this.Height = 0;
			this.State = FormWindowState.Normal;
			this.ShowClass = this.ShowFunc = this.ShowComment = this.ShowMember = this.ShowArg = this.ShowObject = true;
			this.LeftPanelWidth = this.ClassHeight = this.CommentHeight = this.RightPanelWidth = this.MemberHeight = this.ObjectHeight = 0;
		}

		public void InitHds()
		{
			this.Init();
			this.ShowClass = this.ShowComment = this.ShowMember = this.ShowArg = this.ShowObject = false;
		}

		public void Apply(Form1 form1)
		{
			if (this.Width > 0 && this.Height > 0)
			{
				Rectangle r1 = new Rectangle(this.X, this.Y, this.Width, this.Height);
				Rectangle r2 = Screen.GetWorkingArea(r1);
				if (r1.IntersectsWith(r2))
				{
					form1.SetDesktopBounds(r1.X, r1.Y, r1.Width, r1.Height);
				}
				else
				{
					form1.Size = new Size(this.Width, this.Height);
				}
				form1.WindowState = this.State;
			}
			form1.SetClassVisible(this.ShowClass);
			form1.SetFuncVisible(this.ShowFunc);
			form1.SetCommentVisible(this.ShowComment);
			form1.SetMemberVisible(this.ShowMember);
			form1.SetArgVisible(this.ShowArg);
			form1.SetObjectVisible(this.ShowObject);
			if (this.LeftPanelWidth > 0) form1.view1.panel1.Width = this.LeftPanelWidth;
			if (this.ClassHeight > 0) form1.view1.tabClass.Height = this.ClassHeight;
			if (this.CommentHeight > 0) form1.view1.txtComment.Height = this.CommentHeight;
			if (this.RightPanelWidth > 0) form1.view1.panel3.Width = this.RightPanelWidth;
			if (this.MemberHeight > 0) form1.view1.tabMember.Height = this.MemberHeight;
			if (this.ObjectHeight > 0) form1.view1.tabObject.Height = this.ObjectHeight;
		}

		public void Store(Form1 form1)
		{
			Rectangle rect = form1.sizeMonitor.Rect;
			this.X = rect.X;
			this.Y = rect.Y;
			this.Width = rect.Width;
			this.Height = rect.Height;
			this.State = form1.WindowState;
			this.ShowClass = form1.view1.tabClass.Visible;
			this.ShowFunc = form1.view1.tabFunc.Visible;
			this.ShowComment = form1.view1.txtComment.Visible;
			this.ShowMember = form1.view1.tabMember.Visible;
			this.ShowArg = form1.view1.tabArg.Visible;
			this.ShowObject = form1.view1.tabObject.Visible;
			this.LeftPanelWidth = form1.view1.panel1.Width;
			this.ClassHeight = form1.view1.tabClass.Height;
			this.CommentHeight = form1.view1.txtComment.Height;
			this.RightPanelWidth = form1.view1.panel3.Width;
			this.MemberHeight = form1.view1.tabMember.Height;
			this.ObjectHeight = form1.view1.tabObject.Height;
		}
	}
}
