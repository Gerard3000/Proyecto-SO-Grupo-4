using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

           
        }

        private void AtenderServidor()
        {
            while (true)
            {
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = mensaje = trozos[1].Split('\0')[0];

                switch (codigo)
                {
                    case 1: //Todo tipo de notificaciones
                        MessageBox.Show(mensaje);
                        break;
                    case 2:      //LOGIN VALIDO
                        MessageBox.Show(mensaje);
                        EstColor.BackColor = Color.Green;
                        Estado.Text = usuario2.Text;
                        break;
                    case 3:     //DESCONEXION
                        MessageBox.Show(mensaje);
                        EstColor.BackColor = Color.Red;
                        Estado.Text = "No iniciado";
                        this.BackColor = Color.Gray;
                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                        break;
                    case 4:     //Recibimos notificacion

                        break;
                    case 5:     //Recibimos notificacion

                        break;
                    case 6:     //Recibimos notificacion

                        break;
                    case 7:     //Recibimos notificacion

                        break;
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9061);
            

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado");

                //pongo en marcha el thread que atenderá los mensajes del servidor
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();

            }
            catch (SocketException ex)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(EstColor.BackColor == Color.Red)
                MessageBox.Show("Inicia sesion para realizar consultas.");
            else if (ganadas.Checked)
            {
                string mensaje = "3/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (puntos.Checked)
            {
                string mensaje = "4/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);


            }
            else if (partidasg.Checked)
            {
                // Enviamos nombre 
                string mensaje = "5/" + busqUser.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/" +Estado.Text;
        
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Comprobar datos válidos para iniciar session
            if (usuario2.TextLength == 0 || usuario2.TextLength > 20 || contraseña2.TextLength == 0 || contraseña2.TextLength > 20)
            {
                MessageBox.Show("Datos inválidos");
                return;
            }

            string mensaje = "1/" + usuario2.Text + "," + contraseña2.Text ;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Comprobación datos válidos para poder hacer el registro
            if (usuario1.TextLength == 0 || usuario1.TextLength > 20 || nombre1.TextLength == 0 || nombre1.TextLength > 20 ||
                contraseña1.TextLength == 0 || contraseña1.TextLength > 20) 
            {
                MessageBox.Show("Datos inválidos");
                return;
            }

            //Preparamos la petición
            string mensaje = "2/" + nombre1.Text + "," + usuario1.Text + "," + contraseña1.Text + "," + edad1.Text ;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
       
        private void Bonito_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void altura_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void alturaBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void usuario2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (this.BackColor == Color.Green)
            {
                string mensaje = "1/" + usuario2.Text + "/" + contraseña2.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else
                MessageBox.Show("No estas conectado al servidor");


        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            if (this.BackColor == Color.Green)
            {
                //Comprobación datos válidos para poder hacer el registro
                if (usuario1.TextLength == 0 || usuario1.TextLength > 20)
                    MessageBox.Show("Nombre < 20 caracteres");
                else if (nombre1.TextLength == 0 || nombre1.TextLength > 20)
                    MessageBox.Show("Nombre < 20 caracteres");
                else if (contraseña1.TextLength == 0 || contraseña1.TextLength > 20)
                    MessageBox.Show("contraseña < 20 caracteres");
                else
                {
                    //Preparamos la petici?n//Preparamos la petición
                    string mensaje = "2/" + nombre1.Text + "/" + usuario1.Text + "/" + contraseña1.Text + "/" + edad1.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }
            else
                MessageBox.Show("No estas conectado al servidor");
            
            
        }

        private void nombre2_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void usuario_lbl_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (this.BackColor == Color.Green)
            {
                string mensaje = "6/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            }
            else
                MessageBox.Show("No estas conectado al servidor");
        }

        private void Estado_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
