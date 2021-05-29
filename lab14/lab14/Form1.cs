using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab14
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }


        public double[] data_check()
        {
            double[] mean_var = new double[2];
            mean_var[0] = Convert.ToDouble(ed_ExpectedValue.Text);
            mean_var[1] = Convert.ToDouble(ed_Dispersion.Text);
            return mean_var;
        }

        public double calc_ExpectedValue(List<float> values)
        {
            double EV = 0;
            for (int i = 0; i < values.Count; i++)
            {
                EV += values[i];
            }
            return EV/values.Count;
        }
        public double calc_Dispersion(List<float> values, double mean)
        {
            double E = mean;
            double Dispersion = 0;
            for (int i = 0; i < values.Count; i++)
            {
                Dispersion += values[i]* values[i];
            }
            Dispersion /= values.Count;
            Dispersion -= E * E;
            return Dispersion;
        }
        private int Check(List<float> values, double a, double b)
        {
            return values.Count(t => t >= a && t < b);
        }
        public double calc_statistics(List<float> values, double mean, double d)
        {
            chart1.Series.Clear();
            Series series = new Series();
            series.ChartType = SeriesChartType.Column;
            series.Name = "Distribution";
            chart1.Series.Add(series);
            double max = values.Max();
            double min = values.Min();
            double K = (max - min) * 0.05;
            double interval = (max - min) + K;

            int k = (int)Math.Round(Math.Log(values.Count) / Math.Log(2) + 1);
            double step = interval / k;

            double q = min - K / 2;
            int[] stat = new int[k];
            float chi = 0;

            for (int i = 0; i < k; i++)
            {
                stat[i] = Check(values,q, q + step);
                float p = (float)(q + step - q) * Density((float)(q + step + q) / 2, mean, d);
                chi += (float)Math.Pow(stat[i], 2) / (values.Count * p);
                chart1.Series[0].Points.AddXY(q, (double)stat[i] / values.Count);
                q += step;
            }

            chi -= values.Count;
            return chi;
        }
        private float Density(float x, double mean, double d)
        {
            var value = (float)Math.Pow(Math.E, Math.Pow((x - mean), 2) / (-2 * Math.Pow(d, 2)));
            return value / (float)(d * Math.Sqrt(2 * Math.PI));
        }
        public List<float> calculation(double[] mean_var, int number_experiment)
        {
            Random rnd = new Random();
            List<float> values = new List<float>();
            for (int i = 0; i < number_experiment; i++)
            {
                double a1 = rnd.NextDouble();
                double a2 = rnd.NextDouble();
                double kor = Math.Sqrt(-2 * Math.Log(a1));
                double cos = Math.Cos(2 * Math.PI * a2);
                float x = (float)(kor * cos * Math.Sqrt(mean_var[1]) + mean_var[0]);
                values.Add(x);
            }
            return values;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            double[] mean_var = data_check();
            int sample_size;
            sample_size = Convert.ToInt32(ed_NumberOfExperiments.Text);
  
            List<float> samples = calculation(mean_var, sample_size);
            double mean = Math.Round(calc_ExpectedValue(samples),2);
            double deviation1 = Math.Round(Math.Abs(mean - mean_var[0]) / Math.Abs(mean) * 100);
            label7.Text = Convert.ToString($"Average: {mean}, deviation: {deviation1}%" );
            double d = calc_Dispersion(samples, mean);
            double deviation2 = Math.Round(Math.Abs(d - mean_var[1]) / Math.Abs(mean_var[1]) * 100, 2);
            label8.Text = Convert.ToString($"Dispersion: {d}, deviation: {deviation2}%");
      
            double chi = calc_statistics(samples, mean, d);
            label19.Text = "Chi = " + Convert.ToString(chi);
        }

    }
}
