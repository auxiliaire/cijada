// DEPRECATED

using System.Collections.Generic;

namespace CA.Gfx.Palette
{
    public class Map
    {
        //private Dictionary<int, ColorStop> map;
        private Dictionary<int, GradientEditor.GradientStop> map;

        public Map()
        {
            //map = new Dictionary<int, ColorStop>();
            map = new Dictionary<int, GradientEditor.GradientStop>();
        }

        //public void setColorStop(int order, ColorStop s) {
        public void setColorStop(int order, GradientEditor.GradientStop s)
        {
            map.Add(order, s);
        }

        public void removeColorStop(int order)
        {
            map.Remove(order);
        }

        //public Dictionary<int, ColorStop> getMap() {
        public Dictionary<int, GradientEditor.GradientStop> getMap()
        {
            return map;
        }
    }
}
