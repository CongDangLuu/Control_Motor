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
using System.IO;
using System.IO.Ports;

namespace recive_data
{
    public partial class Form1 : Form
    {
        int TickStart, intMode = 1;
        double angle = 0, angle1 = 0, max = 0, min = 0;
        string send_data;
        double Kp = 7, Ki = 2.85, Kd = 0.5, set_point = 0, config = 000, mode = 0, uk = 0;
        public Form1()
        {
            InitializeComponent();
            //COM Port.
            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            Baud.Items.AddRange(BaudRate);
            

        }

        public double Scale_x_max = 100;
        public double Scale_x_min = 0;
        public double Scale_y_max = 360;
        public double Scale_y_min = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            COM.DataSource = SerialPort.GetPortNames();
            Baud.SelectedIndex = 7;
            

            textBox4.Text = Kp.ToString();
            textBox5.Text = Ki.ToString();
            textBox6.Text = Kd.ToString();
            textBox7.Text = set_point.ToString();

            max = 240;min = 60;
            textBox9.Text = max.ToString();
            textBox10.Text = min.ToString();
            textBox11.Text = 2.ToString();

            checkBox1.Checked = true;
            checkBox2.Checked = false;
            
            GraphPane myPane = zed.GraphPane;

            // Title
            myPane.Title.Text = "Chart";
            myPane.XAxis.Title.Text = "TIME";
            myPane.YAxis.Title.Text = "ANGLE";
            // Tạo số điểm của các list dữ liệu
            RollingPointPairList list1 = new RollingPointPairList(120000);
            RollingPointPairList list2 = new RollingPointPairList(120000);
            // Tạo các đường vẽ dữ liệu
            LineItem curve1 = myPane.AddCurve("Set Point", list1, System.Drawing.Color.Red, SymbolType.None);
            LineItem curve2 = myPane.AddCurve("Current Value", list2, System.Drawing.Color.Blue, SymbolType.None);
            // Cho phép vẽ grid 
            myPane.XAxis.MinorGrid.IsVisible = true;
            myPane.YAxis.MinorGrid.IsVisible = true;

            // Scale_X,Scale_Y
            myPane.XAxis.Scale.Min = Scale_x_min;
            myPane.XAxis.Scale.Max = Scale_x_max;
            myPane.XAxis.Scale.MinorStep = 5;
            myPane.XAxis.Scale.MajorStep = 10;

            myPane.YAxis.Scale.Min = Scale_y_min;
            myPane.YAxis.Scale.Max = Scale_y_max;
            myPane.YAxis.Scale.MinorStep = 1;
            myPane.YAxis.Scale.MajorStep = 2;

            zed.AxisChange();
            zed.Invalidate();
            // Lấy thời gian của hệ thống
            TickStart = Environment.TickCount;
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string rev_data = serialPort1.ReadLine();
            textBox3.Text = rev_data.ToString();
            string[] sub_data = rev_data.Split('|');        
            angle = double.Parse(sub_data[0]);
            uk = double.Parse(sub_data[1]);
            textBox1.Text = angle.ToString();
            textBox2.Text = uk.ToString();

            if(mode == 000)
            {
                textBox4.Text = sub_data[2].ToString();
                textBox5.Text = sub_data[3].ToString();
                textBox6.Text = sub_data[4].ToString();
            }
            
            if (angle - angle1 > 0)
            {
                ovalShape1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
                ovalShape2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Transparent;
            }
            else if(angle - angle1 < 0)
            {
                ovalShape2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Opaque;
                ovalShape1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Transparent;
            }
            else
            {
                ovalShape2.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Transparent;
                ovalShape1.BackStyle = Microsoft.VisualBasic.PowerPacks.BackStyle.Transparent;
            }
            angle1 = angle;
        }
        //set button
        private void write_Click(object sender, EventArgs e)
        {
            try
            {
                Kp = Math.Round(double.Parse(textBox4.Text), 2);
                Ki = Math.Round(double.Parse(textBox5.Text), 2);
                Kd = Math.Round(double.Parse(textBox6.Text), 2);
                set_point = Math.Round(double.Parse(textBox7.Text), 2);
                config = 000;
                if (Kp<1000 & Ki<1000 & Kd<1000 & set_point<1000 & Kp>=0 & Ki>=0 & Kd >=0 & set_point>=-999)
                {
                    send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
                    textBox8.Text = send_data;
                    serialPort1.Write(send_data);
                }
                else
                {
                    MessageBox.Show("Oh No, Please put value larger or equal 0 and below 1000 :))");
                }
            }
            catch
            {
                MessageBox.Show("Invalid Input");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {   
            Kp = 10;
            Ki = 60;
            Kd = 0.08;
            set_point = 0;
            config = 001;
            
            textBox7.Text = set_point.ToString();
            
            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);                       
        }
        //stop button
        private void button3_Click(object sender, EventArgs e)
        {
            config = 111;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Kp = Math.Round(double.Parse(textBox4.Text), 2);
            Ki = Math.Round(double.Parse(textBox5.Text), 2);
            Kd = Math.Round(double.Parse(textBox6.Text), 2);
            set_point = 60;
            textBox7.Text = set_point.ToString();
            config = 000;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Kp = Math.Round(double.Parse(textBox4.Text), 2);
            Ki = Math.Round(double.Parse(textBox5.Text), 2);
            Kd = Math.Round(double.Parse(textBox6.Text), 2);
            set_point = 120;
            textBox7.Text = set_point.ToString();
            config = 000;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Kp = Math.Round(double.Parse(textBox4.Text), 2);
            Ki = Math.Round(double.Parse(textBox5.Text), 2);
            Kd = Math.Round(double.Parse(textBox6.Text), 2);
            set_point = 180;
            textBox7.Text = set_point.ToString();
            config = 000;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Kp = Math.Round(double.Parse(textBox4.Text), 2);
            Ki = Math.Round(double.Parse(textBox5.Text), 2);
            Kd = Math.Round(double.Parse(textBox6.Text), 2);
            set_point = 240;
            textBox7.Text = set_point.ToString();
            config = 000;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Kp = Math.Round(double.Parse(textBox4.Text), 2);
            Ki = Math.Round(double.Parse(textBox5.Text), 2);
            Kd = Math.Round(double.Parse(textBox6.Text), 2);
            set_point = 300;
            textBox7.Text = set_point.ToString();
            config = 000;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Kp = Math.Round(double.Parse(textBox4.Text), 2);
            Ki = Math.Round(double.Parse(textBox5.Text), 2);
            Kd = Math.Round(double.Parse(textBox6.Text), 2);
            set_point = 360;
            textBox7.Text = set_point.ToString();
            config = 000;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Kp = Math.Round(double.Parse(textBox4.Text), 2);
            Ki = Math.Round(double.Parse(textBox5.Text), 2);
            Kd = Math.Round(double.Parse(textBox6.Text), 2);
            set_point = 0;
            textBox7.Text = set_point.ToString();
            config = 000;

            send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
            textBox8.Text = send_data;
            serialPort1.Write(send_data);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                COM.Enabled = true;
                Baud.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (checkBox1.Checked) checkBox2.Checked = false;
                mode = 000;//mode auto
                config = 111;
                timer2.Enabled = false;
                send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
                textBox8.Text = send_data;
                serialPort1.Write(send_data);
            }
            else
            {
                return;
            }
                
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                double temp;
                temp = max;
                max = min;
                min = temp;
                set_point = max;
                textBox7.Text = set_point.ToString();
                send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
                textBox8.Text = send_data;
                serialPort1.Write(send_data);
            }
                
        }
        //connect button
        private void button11_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = COM.Text;
                serialPort1.BaudRate = Convert.ToInt32(Baud.Text);
                serialPort1.Open();

                COM.Enabled = false;
                Baud.Enabled = false;

            }
            else
            {
                return;
            }
                
        }

        private void button10_Click(object sender, EventArgs e)
        {
            timer2.Enabled = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                timer2.Interval = (int)(1000 * (float.Parse(textBox11.Text)));
                max = double.Parse(textBox9.Text);
                min = double.Parse(textBox10.Text);
                timer2.Enabled = true;
            }
            catch
            {
                MessageBox.Show("invalid input");
            }
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (checkBox2.Checked) checkBox1.Checked = false;
                mode = 111;//mode manual
                config = 111;
                timer2.Enabled = false;
                send_data = convert(mode) + convert(config) + convert(Kp) + convert(Ki) + convert(Kd) + convert(set_point);
                textBox8.Text = send_data;
                serialPort1.Write(send_data);
            }
            else
            {
                return;
            }

        }

        string convert(double value)
        {
            int VALUE = (int)(value * 100);
            string str;
            str = (VALUE / 10000).ToString() + ((VALUE % 10000) / 1000).ToString() + ((VALUE % 1000 - VALUE % 100) / 100).ToString();
            str += ((VALUE % 100 - VALUE % 10) / 10).ToString() + (VALUE % 10).ToString();
            return str;
        }

        private void Draw(double intsetpoint, double intcurrent)
        {
            //Tao 1 curve
            if (zed.GraphPane.CurveList.Count <= 0)
                return;
            // Tao item curve trong do thi
            LineItem curve = zed.GraphPane.CurveList[0] as LineItem;
            LineItem curve1 = zed.GraphPane.CurveList[1] as LineItem;
            if (curve == null)
                return;
            if (curve1 == null)
                return;
            //Lay pointpairlist
            IPointListEdit list = curve.Points as IPointListEdit;
            IPointListEdit list1 = curve1.Points as IPointListEdit;

            if (list == null)
                return;
            if (list1 == null)
                return;
            double time = (Environment.TickCount - TickStart) / 1000.0;
            list.Add(time, intsetpoint);
            list1.Add(time, intcurrent);

            Scale xScale = zed.GraphPane.XAxis.Scale;
            if (time > xScale.Max - xScale.MajorStep)
            {
                if (intMode == 1)
                {
                    xScale.Max = time + xScale.MajorStep;
                    xScale.Min = xScale.Max - 30.0;
                }
                else
                {
                    xScale.Max = time + xScale.MajorStep;
                    xScale.Min = 0;
                }
            }
            zed.AxisChange();
            zed.Invalidate();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            GraphPane myPane = zed.GraphPane;
            // Scale_X,Scale_Y

            myPane.YAxis.Scale.Min = Scale_y_min;
            myPane.YAxis.Scale.Max = Scale_y_max;
            myPane.YAxis.Scale.MinorStep = 1;
            myPane.YAxis.Scale.MajorStep = 2;

            zed.AxisChange();
            zed.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Draw(set_point, angle);
        }

     

   

       
    }
}