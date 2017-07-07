using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimuladorMotorCD
{
    public partial class Form1 : Form
    {
        double k1, k2, k3, k4;
        double[] i = new double[10000];
        double []Eb = new double[10000];
        double []Tt = new double[10000];
        double[] w = new double[10000];
        int count;

        public Form1()
        {
            InitializeComponent();
            numericUpDownEa.Value = 24;
            numericUpDownRa.Value = (decimal)0.316;
            numericUpDownLa.Value = (decimal)0.082;
            numericUpDownKt.Value = (decimal)30.2;
            numericUpDownKb.Value = (decimal)317;
            numericUpDownJ.Value = (decimal)0.139;
            numericUpDownB.Value = (decimal)0;
            numericUpDownH.Value = (decimal)0.0005;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            i[0] = 0;
            Eb[0] = 0;
            Tt[0] = 0;
            w[0] = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void numericUpDownH_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            count++;

            if (count < 10000)
            {
                calculos();
            }
            else
            {                
                timer1.Stop();
                MessageBox.Show("Simulacion terminada");
            }
        }

        private void calculos()
        {
            double iAct, iSig;
            double wAct, wSig;
            double x, y;
            
            //Lectura de datos de numericUpDown
            double h = (double)(numericUpDownH.Value);
            double La = (double)numericUpDownLa.Value / 1000;
            double Ea = (double)numericUpDownEa.Value;
            double Ra = (double)numericUpDownRa.Value;
            double kt = (double)numericUpDownKt.Value / 1000;
            double kb = (double)(numericUpDownKb.Value) * (double)(1.0 / 60.0) * (double)(2 * Math.PI);
            kb = 1 / kb;
            double J = (double)numericUpDownJ.Value / 1000;
            double B = (double)numericUpDownB.Value;
            double load = (double)numericUpDownLoad.Value;

            //Calculo de corriente por runge kutta
            iAct = i[count - 1];
            k1 = h * (1 / La) * (Ea - (Ra * i[count - 1]) - Eb[count - 1]);
            k2 = h * (1 / La) * (Ea - (Ra * (i[count - 1] + k1 / 2)) - Eb[count - 1]);
            k3 = h * (1 / La) * (Ea - (Ra * (i[count - 1] + k2 / 2)) - Eb[count - 1]);
            k4 = h * (1 / La) * (Ea - (Ra * (i[count - 1] + k3)) - Eb[count - 1]);
            iSig = iAct + (1.0/6.0)*(k1 + (2*k2) + (2*k3) + k4);
            i[count] = iSig;

            textBox1.Text = kb.ToString();

            //Impresion en grafica de resultado de corriente
            x = h * (count - 1);
            y = i[count-1];
            chart1.Series["Series1"].Points.AddXY(x, y);

            //calculo de torque de motor
            Tt[count] = iAct * kt;

            //calculo de velocidad angular por runge kutta
            wAct = w[count - 1];
            k1 = h * (1 / J) * (Tt[count] - (B * w[count - 1]) - load);
            k2 = h * (1 / J) * (Tt[count] - (B * (w[count - 1] + k1 / 2)) - load);
            k3 = h * (1 / J) * (Tt[count] - (B * (w[count - 1] + k2 / 2)) - load);
            k4 = h * (1 / J) * (Tt[count] - (B * (w[count - 1] + k3)) - load);
            wSig = wAct + (1.0 / 6.0) * (k1 + (2 * k2) + (2 * k3) + k4);
            w[count] = wSig;

            //Impresion velocidad angular            
            x = h * (count - 1);
            y = w[count - 1] * (1/(2*Math.PI) * 60);
            chart2.Series["Series1"].Points.AddXY(x, y);

            //Calculo de FEM
            Eb[count] = (kb * wAct);

            listBox1.Items.Add(h*count
                                + "\t" + Ea + "V"
                                + "\t" + iAct + "A"
                                + "\t" + Ra + "ohm"
                                + "\t" + La + "mH"
                                + "\t" + Eb[count-1] + "V"
                                + "\t" + Tt[count-1] + "Nm"
                                + "\t" + kt + "Nm/A"
                                + "\t" + kb + "V/rad/s"
                                + "\t" + wAct + "rad/s" 
                                );
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            count = 0;
            chart1.Series["Series1"].Points.Clear();
            chart2.Series["Series1"].Points.Clear();
            listBox1.Items.Clear();
            timer1.Start();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
    }
}
