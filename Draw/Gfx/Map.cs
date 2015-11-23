using System;
using System.Drawing;
using System.Collections.Generic;

namespace CA.Gfx.Palette
{
	public class Map
	{
		private Dictionary<int, ColorStop> map;
		
		public Map ()
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

