using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Threading;

using System.Net;
using System.Net.NetworkInformation;

using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data.SqlClient;

using System.Configuration;
using System.Net.Mail;


[Flags]
enum InternetConnectionState : int
{
    INTERNET_CONNECTION_MODEM = 0x1,
    INTERNET_CONNECTION_LAN = 0x2,
    INTERNET_CONNECTION_PROXY = 0x4,
    INTERNET_RAS_INSTALLED = 0x10,
    INTERNET_CONNECTION_OFFLINE = 0x20,
    INTERNET_CONNECTION_CONFIGURED = 0x40
}

namespace WindowsFormsApplication7
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            loadPorts();
        }

        private static string problem = "";
        private static string problem0 = "";

        [DllImport("WININET", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref InternetConnectionState lpdwFlags, int dwReserved);

        private void loadPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                cboPorts.Items.Add(port);
            }
        }

        public bool PingTest(string monip)
        {
            Ping ping = new Ping();
            PingReply pingStatus = ping.Send(IPAddress.Parse(monip));
            if (pingStatus.Status == IPStatus.Success)
                return true;
            else
                return false;
        }


        public int temp()
        {
            int nb = 0;
            foreach (DataRow row in dataSet1.Tables[0].Rows)
            {
                Match match = Regex.Match(row[0].ToString(), @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                if (match.Success)
                {
                    nb = nb + trackBar1.Value;
                }

            }
            return nb;
        }

        public bool connec()
        {
            InternetConnectionState flags = 0;
            bool isConnected = InternetGetConnectedState(ref flags, 0);
            if (isConnected)
            {
                foreach (DataRow row in dataSet1.Tables[0].Rows)
                {
                    bool b = true;
                    Match match = Regex.Match(row[0].ToString(), @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
                    if (match.Success)
                    {
                        try
                        {
                            b = PingTest(row[0].ToString());
                        }
                        catch (Exception ex)
                        {
                            b = false;
                        }
                        if (!b)
                        {
                            this.timer1.Interval = this.timer1.Interval + trackBar1.Value;
                            try
                            {
                                b = PingTest(row[0].ToString());
                            }
                            catch (Exception ex)
                            {
                                // MessageBox.Show("erreur ping "+ex);
                                b = false;
                            }
                            if (!b)
                            {
                                problem = row[1].ToString() + " , " + problem;
                            }
                        }
                    }

                }
                if (problem != "")
                    return false;
            }
            else
            {
                problem = "Connection reseau ";
                return false;
            }
            return true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox2.Text = "" + (trackBar1.Value * 0.001).ToString("0.0") + " s";
            problem0 = problem;
            problem = "";
            if (temp() != 0)
            {
                toolStripStatusLabel1.Text = "Execution";
                statusStrip1.Refresh();
                this.timer1.Interval = temp();
                if (!connec())
                {
                    toolStripStatusLabel1.Text = problem + ": indisponible pour le moment";
                    statusStrip1.Refresh();
                    if (problem != problem0 && problem != "")
                    {
                        string ch;
                        string message;
                        ch = String.Format("Le {0:00}-{1:00}-{2:0000} a {3:00}:{4:00}:{5:00}\n",
                        System.DateTime.Now.Day, System.DateTime.Now.Month, System.DateTime.Now.Year,
                        System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);

                        message = ("HELPDESK CYNAPSYS\n" + ch + problem + ": indisponible pour le moment");
                        if (cboPorts.SelectedItem != null)
                        {
                            foreach (DataRow row in dataSet2.Tables[0].Rows)
                            {
                                SerialPort _serialPort = new SerialPort(cboPorts.Text, 115200);
                                Thread.Sleep(1000);
                                _serialPort.Open();
                                Thread.Sleep(1000);
                                _serialPort.Write("AT+CMGF=1\r");
                                Thread.Sleep(1000);
                                _serialPort.Write("AT+CMGS=\"" + (string)row[2] + "\"\r\n");
                                Thread.Sleep(1000);
                                _serialPort.Write(message + "\x1A");
                                Thread.Sleep(1000);
                                _serialPort.Close();
                            }
                        }
                        if (problem != "Connection reseau ")
                        {
                            foreach (DataRow row in dataSet3.Tables[0].Rows)
                            {
                                try
                                {
                                    MailMessage mail = new MailMessage();
                                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                    mail.From = new MailAddress((string)row[0]);
                                    mail.To.Add((string)row[0]);
                                    mail.Subject = "HELPDESK CYNAPSYS";
                                    mail.Body = message;

                                    SmtpServer.Port = 587;
                                    SmtpServer.Credentials = new System.Net.NetworkCredential((string)row[0], (string)row[1]);
                                    SmtpServer.EnableSsl = true;

                                    SmtpServer.Send(mail);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                    }
                }
                else
                {
                    toolStripStatusLabel1.Text = "Connecté";
                    statusStrip1.Refresh();
                    if ((problem0 != "")&&(cboPorts.SelectedItem != null))
                    {
                        string message = "HELPDESK CYNAPSYS\nConnection reseau: disponible";
                        foreach (DataRow row in dataSet2.Tables[0].Rows)
                        {
                            MessageBox.Show(message);
                            SerialPort _serialPort = new SerialPort(cboPorts.Text, 115200);
                            Thread.Sleep(1000);
                            _serialPort.Open();
                            Thread.Sleep(1000);
                            _serialPort.Write("AT+CMGF=1\r");
                            Thread.Sleep(1000);
                            _serialPort.Write("AT+CMGS=\"" + (string)row[2] + "\"\r\n");
                            Thread.Sleep(1000);
                            _serialPort.Write(message + "\x1A");
                            Thread.Sleep(1000);
                            _serialPort.Close();
                        }
                    }
                    problem = "";
                    problem0 = "";

                }
            }
            else
            {
                toolStripStatusLabel1.Text = "Pas d'adresse a verifier";
                statusStrip1.Refresh();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            dataSet1 = null;
            dataSet2 = null;
            dataSet3 = null;
            dataSet1 = importadr();
            dataSet2 = importadmin();
            dataSet3 = importmail();
            if (dataSet1 == null || dataSet2 == null)
            {
                MessageBox.Show("problemme de connection a la base");
            }
            else if (dataSet1.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("pas d'adresse dans la base");
            }
            else if (dataSet2.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("pas d'admin dans la base");
            }
            else if (dataSet3.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("pas d'adresses mails dans la base");
            }
            else
            {
                timer1.Enabled = true;
                toolStripStatusLabel1.Text = "Execution";
                statusStrip1.Refresh();
                button1.Visible = false;
                button2.Visible = true;
                cboPorts.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button3.Enabled = false;

                foreach (DataRow row in dataSet2.Tables[0].Rows)
                {
                    textBox1.Text = textBox1.Text + (String)row[0] + " " + (String)row[1] + "\r\n";
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            toolStripStatusLabel1.Text = "Arret";
            statusStrip1.Refresh();
            button1.Visible = true;
            button2.Visible = false;
            problem0 = "";
            problem = "";
            cboPorts.Enabled = true;
            this.timer1.Interval = 300;
            textBox1.Text = "";
            button5.Enabled = true;
            button6.Enabled = true;
            button3.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.ShowDialog();
        }

        public static DataSet importmail()
        {
            string ch = GetConnectionStrings();
            DataSet ds = new DataSet();
            try
            {
                SqlConnection myConnection = new SqlConnection(ch);
                string selectCmd = String.Format("SELECT Adresse, Passe FROM Mail");
                SqlDataAdapter myCommand = new SqlDataAdapter(selectCmd, myConnection);
                myCommand.Fill(ds);
                return (ds);
            }
            catch (Exception ex)
            {
                return (ds = null);
            }
        }

        public static DataSet importadr()
        {
            string ch = GetConnectionStrings();
            DataSet ds = new DataSet();
            try
            {
                SqlConnection myConnection = new SqlConnection(ch);
                string selectCmd = String.Format("SELECT adresse, serveur FROM adresses");
                SqlDataAdapter myCommand = new SqlDataAdapter(selectCmd, myConnection);
                myCommand.Fill(ds);
                return (ds);
            }
            catch (Exception ex)
            {
                return (ds = null);
            }
        }

        public static DataSet importadmin()
        {
            string ch = GetConnectionStrings();
            DataSet ds = new DataSet();
            try
            {
                SqlConnection myConnection = new SqlConnection(ch);
                string selectCmd = String.Format("SELECT nom, prenom, numtel FROM administrateurs");
                SqlDataAdapter myCommand = new SqlDataAdapter(selectCmd, myConnection);
                myCommand.Fill(ds);
                return (ds);
            }
            catch (Exception ex)
            {
                return (ds = null);
            }
        }

        static string GetConnectionStrings()
        {
            try
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["TestConnexionBD"];
                return (settings.ConnectionString);
            }
            catch (Exception ex)
            {
                return ("");
            }
        }

    }
}
