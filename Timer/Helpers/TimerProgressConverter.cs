using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;


namespace Timer.Helpers
{
    public class TimerProgressConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            double progress = value is double number
                ? Math.Clamp(number, 0, 100)
                : 0;

            if (progress <= 0)
                return Geometry.Empty;

            // Полный круг
            if (progress >= 100)
            {
                return new EllipseGeometry(
                    new Point(100, 100), 90, 90);
            }

            double angle = progress * 3.6;

            double radians = (angle - 90) * Math.PI / 180;

            Point endPoint = new Point(
                100 + 90 * Math.Cos(radians),
                100 + 90 * Math.Sin(radians));

            PathFigure figure = new PathFigure
            {
                StartPoint = new Point(100, 10),
                IsClosed = false
            };

            figure.Segments.Add(
                new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(90, 90),
                    IsLargeArc = angle > 180,
                    SweepDirection = SweepDirection.Clockwise
                });

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
