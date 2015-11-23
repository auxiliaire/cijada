using System;
using System.Drawing;
using System.Collections.Generic;

namespace CA.Gfx
{
	public class PaletteMap
	{
		private Dictionary<int, ColorStop> map;
		
		public PaletteMap ()
		{
			map = new Dictionary<int, ColorStop>();
		}
		
		public void setColorStop(int order, ColorStop s) {
			map.Add(order, s);
		}
		
		public void removeColorStop(int order) {
			map.Remove(order);
		}
		
		public Dictionary<int, ColorStop> getMap() {
			return map;
		}
	}
}

