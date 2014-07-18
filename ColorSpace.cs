// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Drawing;

namespace NetCams
{
	public class ColorSpace
	{
		//--------------------------------------------------------------------------------
		// ??: HSIToRGB
		// ??: ????HSI??RGB?????
		//       ??(Hue)???(Saturation)???(Intensity)
		//--------------------------------------------------------------------------------
		public static void HSIToRGB( int h, int s, int i, out int r, out int g, out int b )
		{
			if ( 0 > h && h > 359 ) throw new ArgumentOutOfRangeException( "h", "??H?0~359??????????????" );
			if ( 0 > s && s > 255 ) throw new ArgumentOutOfRangeException( "s", "??S?0~255??????????????" );
			if ( 0 > i && i > 255 ) throw new ArgumentOutOfRangeException( "i", "??I?0~255??????????????" );
            
			if ( s == 0 )
			{
				r = g = b = i;
			}
			else
			{
				int ht = h * 6;
				double d = (double)( ht % 360 );
				int t1 = (int)( i * ( 255.0d - s ) / 255.0d );
				int t2 = (int)( i * ( 255.0d - s * d / 360.0d ) / 255.0d );
				int t3 = (int)( i * ( 255.0d - s * ( 360.0d - d ) / 360.0d ) / 255.0d );

				switch ( ht / 360 )
				{
					case 0:
						r =  i; g = t3; b = t1; break;
					case 1:
						r = t2; g =  i; b = t1; break;
					case 2:
						r = t1; g =  i; b = t3; break;
					case 3:
						r = t1; g = t2; b =  i; break;
					case 4:
						r = t3; g = t1; b =  i; break;
					default:
						r =  i; g = t1; b = t2; break;
				}
			}
		}

		public static Color HSIToRGB( int h, int s, int i )
		{
			int r, g, b;

			HSIToRGB( h, s, i, out r, out g, out b );

			return Color.FromArgb( r, g, b );
		}

		//--------------------------------------------------------------------------------
		// ??: RGBToHSI
		// ??: ????RGB??HSI?????
		//       ??(Hue)???(Saturation)???(Intensity)
		//--------------------------------------------------------------------------------
		public static void RGBToHSI( int r, int g, int b, out int h, out int s, out int i )
		{
			if ( 0 > r && r > 255 ) throw new ArgumentOutOfRangeException( "r", "?R?0~255??????????????" );
			if ( 0 > g && g > 255 ) throw new ArgumentOutOfRangeException( "g", "?G?0~255??????????????" );
			if ( 0 > b && b > 255 ) throw new ArgumentOutOfRangeException( "b", "?B?0~255??????????????" );

			int max = GetGreatestValue( r, g, b );
			int min = GetSmallestValue( r, g, b );

			double d = (double)( max - min );

			i = max;

			if ( d == 0 )
			{
				s = 0;
			}
			else
			{
				s = (int)( d * 255.0d / (double)max );
			}
            
			if ( s == 0 )
			{
				h = 0;
			}
			else
			{
				int rt = max - (int)( r * 60.0d / d );
				int gt = max - (int)( g * 60.0d / d );
				int bt = max - (int)( b * 60.0d / d );

				if ( r == max )
				{
					h = bt - gt;
				}
				else if ( g == max )
				{
					h = 120 + rt - bt;
				}
				else
				{
					h = 240 + gt - rt;
				}

				if ( h < 0 ) h += 360;
			}
		}

		public static void RGBToHSI( Color col, out int h, out int s, out int i )
		{
			RGBToHSI( col.R, col.G, col.B, out h, out s, out i );
		}

		//--------------------------------------------------------------------------------
		// ??: GetGreatestValue
		// ??: ??????????????????????
		//--------------------------------------------------------------------------------
		private static int GetGreatestValue( int x, int y, int z )
		{
			if ( x < y )
			{
				return ( y < z ) ? z : y;
			}
			else if ( x < z )
			{
				return ( z < y ) ? y : z;
			}
			else
			{
				return x;
			}
		}

		//--------------------------------------------------------------------------------
		// ??: GetSmallestValue
		// ??: ??????????????????????
		//--------------------------------------------------------------------------------
		private static int GetSmallestValue( int x, int y, int z )
		{
			if ( y < x )
			{
				return ( z < y ) ? z : y;
			}
			else if ( z < x )
			{
				return ( y < z ) ? y : z;
			}
			else
			{
				return x;
			}
		}
	}
}