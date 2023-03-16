// ここにソースコードの注釈を書きます。

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Girl.Drawing;

namespace Girl.Windows.Forms.Drawing2D
{
	/// <summary>
	/// CanvasLine, CanvasPolygon, CanvasLines を統合します。
	/// </summary>
	public class CanvasLineEx : CanvasPolygon
	{
		public CanvasLineEx() : base() {}
		public CanvasLineEx(PointF[] points) : base(points) {}
		public CanvasLineEx(PointF pt, PointF[] points) : base(pt, points) {}
		public CanvasLineEx(float x, float y, PointF[] points) : base(x, y, points) {}
		public CanvasLineEx(PointF pt1, PointF pt2) : base(pt1, pt2) {}
	}
}
