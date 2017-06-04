using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KernelDemo
{
    public partial class Form1 : Form
    {
        private void _plotPoints(IEnumerable<(double x, double y)> points, string legend, SeriesChartType type = SeriesChartType.Point, ChartDashStyle dashStyle = ChartDashStyle.Solid)
        {
            chart1.Series.Remove(chart1.Series.FindByName(legend));
            chart1.Series.Add(legend);
            chart1.Series[legend].ChartType = type;
            chart1.Series[legend].BorderDashStyle = dashStyle;
            chart1.Series[legend].BorderWidth = 2;
            chart1.ChartAreas[0].AxisX.Minimum = -7;
            chart1.ChartAreas[0].AxisX.Maximum = 7;
            chart1.ChartAreas[0].AxisX.Interval = 1;
            foreach (var point in points)
            {
                chart1.Series[legend].Points.AddXY(point.x, point.y);
            }
        }

        public Form1()
        {
            InitializeComponent();
            chart1.Series.Clear();

            var data = new List<(double x, double y)>();
            using (var sr = new StreamReader("GaussianProcessData20121209.csv"))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(',').Select(double.Parse).ToArray();
                    data.Add((line[0], line[1]));
                }
            }
            _plotPoints(data, "observed data");

            var means = new List<(double x, double y)>();
            var upper = new List<(double x, double y)>();
            var lower = new List<(double x, double y)>();
            for (var x = -7.0; x < 7; x += 0.1)
            {
                var pred = Kernel.Predict(data.ToArray(), x, Kernel.PorynomialKernel, 0.16, 1.0);
                means.Add((x, pred.mean));
                upper.Add((x, pred.mean + Math.Sqrt(pred.deviation)));
                lower.Add((x, pred.mean - Math.Sqrt(pred.deviation)));
            }
            _plotPoints(means, "inferred function", SeriesChartType.Line);
            _plotPoints(upper, "+1 S.D.", SeriesChartType.Line, ChartDashStyle.Dash);
            _plotPoints(lower, "-1 S.D.", SeriesChartType.Line, ChartDashStyle.Dash);
        }
    }
}