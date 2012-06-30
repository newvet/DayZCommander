using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Dotjosh.DayZCommander.UI.Converters
{
	public class PingToForegroundConverter : IValueConverter
	{
		public static SolidColorBrush Fastest = new SolidColorBrush(Colors.LightGreen);
		public static SolidColorBrush Fast = new SolidColorBrush(Colors.Green);
		public static SolidColorBrush Medium = new SolidColorBrush(Colors.Yellow);
		public static SolidColorBrush Slow = new SolidColorBrush(Colors.Red);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = (long) value;
			if(val > 0 && val < 60)
				return Fastest;
			if(val >= 60 && val < 120)
				return Fast;
			if(val >= 120 && val < 220)
				return Medium;

			return Slow;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}