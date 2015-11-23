using System;
using System.Drawing;
using System.Collections.Generic;

namespace CA.Gfx
{
	public class ColorInterpolator
	{
	    delegate byte ComponentSelector(Color color);
		static ComponentSelector _alphaSelector = color => color.A;
	    static ComponentSelector _redSelector = color => color.R;
	    static ComponentSelector _greenSelector = color => color.G;
	    static ComponentSelector _blueSelector = color => color.B;
	
	    public static Color InterpolateBetween(
	        Color endPoint1,
	        Color endPoint2,
	        double lambda
		) {
	        if (lambda < 0 || lambda > 1) {
	            throw new ArgumentOutOfRangeException("lambda");
	        }
	        Color color = Color.FromArgb(
			    InterpolateComponent(endPoint1, endPoint2, lambda, _alphaSelector),
	            InterpolateComponent(endPoint1, endPoint2, lambda, _redSelector),
	            InterpolateComponent(endPoint1, endPoint2, lambda, _greenSelector),
	            InterpolateComponent(endPoint1, endPoint2, lambda, _blueSelector)
	        );
	
	        return color;
	    }
	
	    static byte InterpolateComponent(
	        Color endPoint1,
	        Color endPoint2,
	        double lambda,
	        ComponentSelector selector
		) {
	        return (byte)(selector(endPoint1)
	            + (selector(endPoint2) - selector(endPoint1)) * lambda);
	    }
		
		public static IEnumerable<Color> GetGradients(Color start, Color end, int steps) {
			for (int i = 0; i < steps; i++) {
    			var aAverage = start.A + (int)((end.A - start.A) * i / steps);
    			var rAverage = start.R + (int)((end.R - start.R) * i / steps);
    			var gAverage = start.G + (int)((end.G - start.G) * i / steps);
    			var bAverage = start.B + (int)((end.B - start.B) * i / steps);
    			yield return Color.FromArgb(aAverage, rAverage, gAverage, bAverage);
			}
		}
		
	}
}