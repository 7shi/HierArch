using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using Girl.Windows.API;

namespace Girl.Windows.Forms
{
	/// <summary>
	/// ドラッグ＆ドロップができる TreeView です。
	/// </summary>
	public class DnDTreeView : ExTreeView
	{
		private DnDTreeNode m_ndDrag = null;
		private Point m_ptDown = new Point();
		private bool m_bDrag = false;
		private System.Timers.Timer m_tmrScroll = new System.Timers.Timer();
		private System.Timers.Timer m_tmrToggle = new System.Timers.Timer();
		private DnDTreeNode m_ndDragTarget = null;
		private bool m_bDragged = false;
		private bool m_bScrolling = false;
		private bool m_bDisturbSelection = false;
		public ArrayList MoveTarget = new ArrayList();

		enum DragStatus { None, Previous, Child, Next };
		private DragStatus m_DragStatus = DragStatus.None;

		public DnDTreeView()
		{
			m_tmrScroll.Interval = 100;
			m_tmrScroll.Elapsed += new ElapsedEventHandler(OnTimedScroll);

			m_tmrToggle.Interval = 1200;
			m_tmrToggle.Elapsed += new ElapsedEventHandler(OnTimedToggle);
		}

		public bool IsMoveTarget
		{
			get
			{
				if (m_ndDrag != null) return true;
				foreach (Object o in MoveTarget)
				{
					if (o is DnDTreeView && ((DnDTreeView)o).m_ndDrag != null) return true;
				}
				return false;
			}
		}

		#region XML

		protected string dataFormat = DataFormats.UnicodeText;

		public string DataFormat
		{
			get
			{
				return this.dataFormat;
			}

			set
			{
				this.dataFormat = value;
			}
		}

		public virtual void FromXml(XmlTextReader xr, TreeNodeCollection nc, int index)
		{
			DnDTreeNode dn;
			bool first = true;
			while (xr.Read())
			{
				if (xr.Name == "DnDTreeNode" && xr.NodeType == XmlNodeType.Element)
				{
					if (first) this.BeginUpdate();
					dn = new DnDTreeNode();
					nc.Insert(index, dn);
					dn.FromXml(xr);
					index++;
					if (first)
					{
						dn.EnsureVisible();
						SelectedNode = dn;
						first = false;
					}
				}
			}
			if (!first) this.EndUpdate();
		}

		#endregion

		#region Mouse Events

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button != MouseButtons.Left) return;

			m_bDrag = false;
			m_ndDrag = (DnDTreeNode)GetNodeAt(e.X, e.Y);
			m_ptDown.X = e.X;
			m_ptDown.Y = e.Y;
			m_bDisturbSelection = false;
			if (m_ndDrag != null && !m_ndDrag.AllowDrag) m_ndDrag = null;
		}

		protected virtual void StartDrag()
		{
			m_bDrag = true;
			m_ndDragTarget = null;
			SelectedNode = m_ndDrag;
			m_bDisturbSelection = true;
			m_DragStatus = DragStatus.None;

			Cursor curOrig = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			StringWriter sw = new StringWriter();
			XmlTextWriter xw = new XmlTextWriter(sw);
			xw.Formatting = Formatting.Indented;
			m_ndDrag.ToXml(xw);
			xw.Close();
			sw.Close();
			Cursor.Current = curOrig;
			DataObject dobj = new DataObject(this.dataFormat, sw.ToString());
			DragDropEffects result = DoDragDrop(dobj, DragDropEffects.All);
			if (result == DragDropEffects.Move) DragMove();

			m_bDrag = false;
			m_ndDrag = null;
			m_ndDragTarget = null;
			m_bDragged = false;
		}
		
		protected virtual void DragMove()
		{
			DnDTreeNode p = m_ndDrag.Parent as DnDTreeNode;
			m_ndDrag.Remove();
			if (p != null) p.SetIcon();
			DnDTreeNode n = SelectedNode as DnDTreeNode;
			if (n != null) n.EnsureVisible();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (m_ndDrag == null) return;
			if (e.Button == MouseButtons.None)
			{
				m_ndDrag = null;
				return;
			}
			if (m_bDrag || e.Button != MouseButtons.Left ||
				Math.Abs(m_ptDown.X - e.X) < 3 || Math.Abs(m_ptDown.Y - e.Y) < 3) return;

			StartDrag();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (m_ndDrag == null || m_bDrag) return;

			StartDrag();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (!m_bDisturbSelection)
			{
				base.OnMouseUp(e);
			}
			else
			{
				m_bDisturbSelection = false;
			}
		}

		#endregion

		#region Drag

		public virtual bool IsDragValid(IDataObject data)
		{
			return data.GetDataPresent(this.dataFormat);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			if (!this.IsDragValid(e.Data))
			{
				e.Effect = DragDropEffects.None;
				return;
			}

			Point p = PointToClient(new Point(e.X, e.Y));
			SetAutoScroll(p);
			SetDragTarget(p);

			m_bDragged = true;
			if (IsMoveTarget)
			{
				if ((e.KeyState & 8) != 0)  // Control
				{
					e.Effect = DragDropEffects.Copy;
				}
				else if (m_ndDrag != null && m_ndDrag.IsAncestorOf(m_ndDragTarget))
				{
					e.Effect = DragDropEffects.None;
				}
				else
				{
					e.Effect = DragDropEffects.Move;
				}
			}
			else
			{
				if ((e.KeyState & 4) != 0)  // Shift
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.Copy;
				}
			}
		}

		protected override void OnDragLeave(EventArgs e)
		{
			base.OnDragLeave(e);

			m_tmrScroll.Enabled = false;
			m_DragStatus = DragStatus.None;
			m_bDragged = false;
			Refresh();
		}

		private void SetDragTarget(Point p)
		{
			DnDTreeNode n = (DnDTreeNode)GetNodeAt(p.X, p.Y);
			if (n == null)
			{
				DnDTreeNode fn = (DnDTreeNode)FirstVisibleNode;
				if (fn != null)
				{
					if (p.Y < fn.Top)
					{
						n = fn;
					}
					else
					{
						n = (DnDTreeNode)LastVisibleNode;
					}
				}
			}

			DragStatus st = DragStatus.Child;
			DnDTreeNode dn = (DnDTreeNode)n;
			if (n != null)
			{
				int px = p.X + VirtualLeft;
				int py = p.Y + VirtualTop;
				int ind = ((px + 9) / Indent) - (ShowRootLines ? 1 : 0);
				Rectangle nr = n.DisplayRectangle;
				if (py < nr.Top + 3)
				{
					if (n.PrevVisibleNode == null)
					{
						st = DragStatus.Previous;
					}
					else
					{
						st = DragStatus.Next;
						dn = (DnDTreeNode)n.PrevVisibleNode;
					}
				}
				else if (py >= nr.Bottom - 3)
				{
					st = DragStatus.Next;
				}
				if (ind <= dn.Depth)
				{
					st = DragStatus.Next;
					for (; ind < dn.Depth; dn = (DnDTreeNode)dn.Parent);
				}
				else if (st == DragStatus.Next)
				{
					DnDTreeNode nn = (DnDTreeNode)dn.NextVisibleNode;
					if (nn != null && nn.Parent == dn)
					{
						st = DragStatus.Previous;
						dn = nn;
					}
				}
			}
			if (m_ndDragTarget == dn && m_DragStatus == st) return;

			InvalidateDragBox();
			m_ndDragTarget = dn;
			m_DragStatus   = st;
			m_tmrToggle.Enabled = false;

			if (m_DragStatus == DragStatus.Child && m_ndDragTarget != null
				&& m_ndDragTarget.Nodes.Count > 0 && !m_ndDragTarget.IsExpanded)
			{
				m_tmrToggle.Enabled = true;
			}
		}

		#endregion

		#region Drop

		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);

			m_tmrScroll.Enabled = false;
			m_bDragged = false;
			Refresh();
			if (e.Effect == DragDropEffects.None) return;

			Cursor curOrig = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			DnDTreeNode n = null;
			int index = -1;
			switch (m_DragStatus)
			{
				case DragStatus.Previous:
					n = m_ndDragTarget.Parent as DnDTreeNode;
					index = m_ndDragTarget.Index;
					break;
				case DragStatus.Child:
					n = m_ndDragTarget as DnDTreeNode;
					break;
				case DragStatus.Next:
					n = m_ndDragTarget.Parent as DnDTreeNode;
					index = m_ndDragTarget.Index + 1;
					break;
			}
			TreeNodeCollection nc = (n != null) ? n.Nodes : Nodes;
			if (index < 0 || index > nc.Count) index = nc.Count;

			StringReader sr = new StringReader((string)e.Data.GetData(this.dataFormat));
			XmlTextReader xr = new XmlTextReader(sr);
			FromXml(xr, nc, index);
			xr.Close();
			sr.Close();
			m_DragStatus = DragStatus.None;
			if (n != null) n.SetIcon();
			Cursor.Current = curOrig;
		}

		#endregion

		#region Drag Box

		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == (int)Win32API.WM.Paint && m_bDragged && !m_bScrolling) DrawDragBox();
		}

		private void DrawDragBox()
		{
			int nLineTop;
			Rectangle r = GetDragBox(out nLineTop);
			Graphics g = CreateGraphics();
			g.DrawRectangle(SystemPens.ControlText, r.Left, r.Top, r.Width - 1, r.Height - 1);
			if (m_DragStatus == DragStatus.Child)
			{
				g.DrawRectangle(SystemPens.ControlText, r.Left + 1, r.Top + 1, r.Width - 3, r.Height - 3);
			}
			else
			{
				g.DrawRectangle(SystemPens.ControlText, r.Left - 1, nLineTop, 1, r.Top + 4 - nLineTop);
			}
			g.Dispose();
		}

		private void InvalidateDragBox()
		{
			int nLineTop;
			Rectangle r = GetDragBox(out nLineTop);
			Invalidate(r);
			if (m_DragStatus != DragStatus.Child)
			{
				Invalidate(new Rectangle(r.Left - 1, nLineTop, 2, r.Top + 5 - nLineTop));
			}
		}

		private Rectangle GetDragBox(out int nLineTop)
		{
			nLineTop = 0;
			Rectangle ret = new Rectangle();
			if (m_ndDragTarget == null || m_DragStatus == DragStatus.None)
			{
				ret.Size = ClientSize;
				return ret;
			}

			Rectangle r = m_ndDragTarget.DisplayRectangle;
			if (m_DragStatus == DragStatus.Child)
			{
				ret.X = r.Left - VirtualLeft;
				ret.Y = r.Top  - VirtualTop;
				ret.Size = r.Size;
			}
			else
			{
				ret.X = (m_ndDragTarget.Depth - (ShowRootLines ? 0 : 1)) * Indent - VirtualLeft + 9;
				if (m_DragStatus == DragStatus.Previous)
				{
					nLineTop = r.Top - 4 - VirtualTop;
					ret.Y = r.Top - 1 - VirtualTop;
				}
				else
				{
					nLineTop = r.Bottom - 4 - VirtualTop;
					if (m_ndDragTarget.Nodes.Count > 0 && ShowPlusMinus) nLineTop += 3;

					DnDTreeNode n = (DnDTreeNode)m_ndDragTarget.NextNode;
					if (n != null)
					{
						ret.Y = n.DisplayRectangle.Top - 1 - VirtualTop;
					}
					else
					{
						int d = m_ndDragTarget.Depth;
						n = (DnDTreeNode)m_ndDragTarget.NextVisibleNode;
						DnDTreeNode nn = null;
						while (n != null && n.Depth > d)
						{
							nn = n;
							n = (DnDTreeNode)n.NextVisibleNode;
						}
						if (nn != null) r = nn.DisplayRectangle;
                        ret.Y = r.Bottom - 1 - VirtualTop;
					}
				}
				ret.Width  = ClientSize.Width - ret.X;
				ret.Height = 2;
			}
			return ret;
		}

		#endregion

		#region Auto Scroll

		private bool SetAutoScroll(Point p)
		{
			bool b = false;
			if ((p.X < 16 && HScrollBar.CanDecrease)
				|| (p.X >= ClientSize.Width - 16 && HScrollBar.CanIncrease)
				|| (p.Y < 16 && VScrollBar.CanDecrease)
				|| (p.Y >= ClientSize.Height - 16 && VScrollBar.CanIncrease))
			{
				b = true;
			}
			if (m_tmrScroll.Enabled != b) m_tmrScroll.Enabled = true;
			return b;
		}

		protected void OnTimedScroll(object sender, ElapsedEventArgs e)
		{
			if (!SetAutoScroll(PointToClient(Cursor.Position))) return;

			InvalidateDragBox();
			m_bScrolling = true;
			Application.DoEvents();
			m_bScrolling = false;
			if (!m_bDragged) return;

			Point p = PointToClient(Cursor.Position);
			if (p.X < 16) HScrollBar.Decrease(2);
			if (p.X >= ClientSize.Width - 16) HScrollBar.Increase(2);
			if (p.Y < 16) VScrollBar.Decrease(1);
			if (p.Y >= ClientSize.Height - 16) VScrollBar.Increase(1);
		}

		#endregion

		#region Auto Toggle

		protected void OnTimedToggle(object sender, ElapsedEventArgs e)
		{
			if (m_ndDragTarget != null && !m_ndDragTarget.IsExpanded)
			{
				m_ndDragTarget.Toggle();
			}
			m_tmrToggle.Enabled = false;
		}

		#endregion
	}

	/// <summary>
	/// ドラッグ＆ドロップに対応した TreeNode です。
	/// </summary>
	public class DnDTreeNode : ExTreeNode
	{
		public bool AllowDrag = true;

		public DnDTreeNode()
		{
		}

		public DnDTreeNode(string text) : base(text)
		{
		}

		public override object Clone()
		{
			DnDTreeNode ret = (DnDTreeNode)base.Clone();
			ret.AllowDrag = this.AllowDrag;
			return ret;
		}

		public virtual void SetIcon()
		{
		}

		#region XML

		public virtual void ToXml(XmlTextWriter xw)
		{
			xw.WriteStartElement("DnDTreeNode");
			xw.WriteAttributeString("Text", Text);
			xw.WriteAttributeString("IsExpanded", XmlConvert.ToString(IsExpanded));
			DnDTreeNode dn;
			foreach(TreeNode n in Nodes)
			{
				dn = (DnDTreeNode)n;
				if (dn != null) dn.ToXml(xw);
			}
			xw.WriteEndElement();
		}

		public virtual void FromXml(XmlTextReader xr)
		{
			if (xr.Name != "DnDTreeNode" || xr.NodeType != XmlNodeType.Element) return;

			Text = xr.GetAttribute("Text");
			bool exp = XmlConvert.ToBoolean(xr.GetAttribute("IsExpanded"));
			if (xr.IsEmptyElement) return;

			DnDTreeNode n;
			while (xr.Read())
			{
				if (xr.Name == "DnDTreeNode" && xr.NodeType == XmlNodeType.Element)
				{
					n = new DnDTreeNode();
					Nodes.Add(n);
					n.FromXml(xr);
				}
				else if (xr.Name == "DnDTreeNode" && xr.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
			if (exp) Expand();
		} 

		#endregion
	}
}
