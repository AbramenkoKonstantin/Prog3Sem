using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using MathNet.Symbolics;

namespace ProgLab2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                GraphPane pane = zedGraph.GraphPane;
                pane.CurveList.Clear();
                double.TryParse(textBoxA.Text, out double aBord);
                double.TryParse(textBoxB.Text, out double bBord);
                double.TryParse(textBoxE.Text, out double esp);
                Expression func = Infix.ParseOrThrow(textBoxF.Text);

                if (esp < 0)
                {
                    throw new Exception();
                }

                double espValue = esp;
                int counter = 0;
                while (espValue < 1)
                {
                    espValue *= 10;
                    counter += 1;
                }
                PointPairList mPoint = new PointPairList();
                List<double> xPoints = new List<double>();
                List<double> yPoints = new List<double>();
                int stepsCounter = -1;

                LineDraw(aBord, bBord, esp, func);
                double minPoint = 0;
                while (Math.Abs(bBord - aBord) >= esp)
                {
                    double f = (1 + Math.Sqrt(5)) / 2;
                    double x1 = bBord - ((bBord - aBord) / f);
                    double x2 = aBord + ((bBord - aBord) / f);
                    if (FuncValue(x1, func) >= FuncValue(x2, func))
                    {
                        aBord = x1;
                    }
                    else
                    {
                        bBord = x2;
                    }
                    minPoint = (aBord + bBord) / 2;
                    stepsCounter += 1;
                    xPoints.Add(minPoint);
                    yPoints.Add(FuncValue(minPoint, func));
                }
                double point = Math.Round(FuncValue(minPoint, func), counter);
                mPoint.Add(minPoint, point);
                LineItem linePoint = pane.AddCurve("Point", mPoint, Color.Black, SymbolType.Default);
                textBoxAnswer.Text = "(" + Math.Round(minPoint, counter).ToString() + ":" + Math.Round(FuncValue(minPoint, func), counter).ToString() + ")";
            }
            catch
            {
                if (textBoxA.Text == "" || textBoxB.Text == "" || textBoxE.Text == "" || textBoxF.Text == "")
                {
                    MessageBox.Show("Пустые поля недопустимы");
                }
                else if (double.Parse(textBoxE.Text) < 0)
                {
                    MessageBox.Show("Точность не может быть меньше 0");
                }
                else if (double.Parse(textBoxA.Text) >= double.Parse(textBoxB.Text))
                {
                    MessageBox.Show("Параметр a должен быть меньше параметра b");
                }
                else
                {
                    MessageBox.Show("Некорректно задана вычисляемая функция");
                }
            }
        }
        private void LineDraw(double aBord, double bBord, double esp, Expression func)
        {
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();

            double espValue = esp;
            int counter = 0;
            while (espValue < 1)
            {
                espValue *= 10;
                counter += 1;
            }

            if ((bBord - aBord) / 50000 > esp)
            {
                esp = (bBord - aBord) / 50000;
            }

            PointPairList list = new PointPairList();


            for (double x = aBord; x <= bBord; x += esp)
            {
                double funcValue = Math.Round(FuncValue(x, func), counter);
                list.Add(x, funcValue);
            }

            LineItem myCurve = pane.AddCurve("Sinc", list, Color.Blue, SymbolType.None);
            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }
        private double FuncValue(double point, Expression func)
        {
            Dictionary<string, FloatingPoint> x = new Dictionary<string, FloatingPoint>()
            {
                { "x", point }
            };
            return Evaluate.Evaluate(x, func).RealValue;
        }

        private void Params_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (char.IsDigit(e.KeyChar) || (e.KeyChar == ',' && textBox.Text.Contains(",") == false) || (e.KeyChar == '-' && textBox.Text == "") || (e.KeyChar == (char)Keys.Back))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
