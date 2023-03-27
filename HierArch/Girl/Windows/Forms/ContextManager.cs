using System;
using System.Collections;
using System.ComponentModel;
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

            len = MaxActions;
            flags = new bool[len];
            for (i = 0; i < len; i++)
            {
                flags[i] = false;
            }
            cmdList = new ArrayList[len];
            for (i = 0; i < len; i++)
            {
                cmdList[i] = new ArrayList();
            }
            handlers = null;
            toolBars = new ArrayList();
            toolBarButtonHandlers = new Hashtable();
        }

        public virtual int MaxActions => 0;

        public void SetCommand(int action, object target)
        {
            ArrayList targets;

            targets = cmdList[action];
            if (targets.Contains(target))
            {
                return;
            }

            _ = targets.Add(target);
            SetProperty(target, flags[action]);
            SetHandler(action, target);

            // Dispose されたらリストから削除する
            if (target is Component)
            {
                var c = target as Component;
                c.Disposed += (sender, e) => targets.Remove(target);
            }
            else if (target is Control)
            {
                var c = target as Control;
                c.Disposed += (sender, e) => targets.Remove(target);
            }
        }

        protected virtual void SetHandler(int action, object target)
        {
            if (target is MenuItem)
            {
                (target as MenuItem).Click += handlers[action];
            }
            else if (target is ToolBarButton)
            {
                ToolBarButton tbb = target as ToolBarButton;
                ToolBar tb = tbb.Parent;
                if (!toolBars.Contains(tb))
                {
                    _ = toolBars.Add(tb);
                    tb.ButtonClick += new ToolBarButtonClickEventHandler(toolBar_ButtonClick);
                }
                toolBarButtonHandlers[tbb] = handlers[action];
            }
            else if (target is Button)
            {
                (target as Button).Click += handlers[action];
            }
        }

        protected void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            EventHandler eh;

            if (!toolBarButtonHandlers.Contains(e.Button))
            {
                return;
            }

            eh = toolBarButtonHandlers[e.Button] as EventHandler;
            eh.Invoke(sender, EventArgs.Empty);
        }

        public void SetCommand(int action, params object[] targets)
        {
            foreach (object obj in targets)
            {
                SetCommand(action, obj);
            }
        }

        public void SetStatus(int action, bool status)
        {
            ArrayList targets;

            if (flags[action] == status)
            {
                return;
            }

            flags[action] = status;
            targets = cmdList[action];
            foreach (object obj in targets)
            {
                SetProperty(obj, status);
            }
        }

        protected virtual void SetProperty(object target, bool status)
        {
            if (target is MenuItem)
            {
                MenuItem mi = target as MenuItem;
                if (mi.Enabled != status)
                {
                    mi.Enabled = status;
                }
            }
            else if (target is ToolBarButton)
            {
                ToolBarButton tbb = target as ToolBarButton;
                if (tbb.Enabled != status)
                {
                    tbb.Enabled = status;
                }
            }
            else if (target is Button)
            {
                Button b = target as Button;
                if (b.Enabled != status)
                {
                    b.Enabled = status;
                }
            }
        }

        public void SetStatus(bool status)
        {
            int len;
            int i;

            len = MaxActions;
            for (i = 0; i < len; i++)
            {
                SetStatus(i, status);
            }
        }
    }
}
