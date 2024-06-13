using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Socket server;
        Thread atender;


        delegate void DelegadoParaEscribir(string[] conectados);

        public Form1()
        {

            InitializeComponent();
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("10.4.119.5");
            //IPAddress direc = IPAddress.Parse("192.168.56.102");
            //IPEndPoint ipep = new IPEndPoint(direc, 6060);
            IPEndPoint ipep = new IPEndPoint(direc, 50075);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
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
        
        private delegate void DelegadoParaEscribirTexto(string text);
        private delegate void DelegadoParaEjecutar();


        private void mensagePrinc(string texto)
        {
            notificacionesLbl.Text = texto;
        }
        private void seleccion1(string texto)
        {
            seleccion1Lbl.Text = texto;
        }
        private void seleccion2(string texto)
        {
            seleccion2Lbl.Text = texto;
        }
        private void seleccion3(string texto)
        {
            seleccion3Lbl.Text = texto;
        }
        private void seleccion4(string texto)
        {
            seleccion4Lbl.Text = texto;
        }
        private void seleccion5(string texto)
        {
            seleccion5Lbl.Text = texto;
        }
        private void estTab(string texto)
        {
            estadoTableroLbl.Text = texto;
        }
        private void estPart(string texto)
        {
            estadoPartidaLbl.Text = texto;
        }
        private void atacar(string texto)
        {
            atacarLbl.Text = texto;
        }
        private void estNombre(string texto)
        {
            Estado.Text = texto;
        }
        private void barcohund1(string texto)
        {
            barcoshundidos1.Text = texto;
        }
        private void barcohund2(string texto)
        {
            barcoshundidos2.Text = texto;
        }
        private void listaUsers(string texto)
        {
            string[] subtrozos = texto.Split(',');
            int num = Convert.ToInt32(subtrozos.Length);

            this.conectados.Rows.Clear();

            for (int i = 0; i < num; i++)
            {
                this.conectados.Rows.Insert(i, subtrozos[i]);
                if (subtrozos[i] == Estado.Text)
                    this.conectados.Rows[i].DefaultCellStyle.ForeColor = Color.Gray;
            }
            this.conectados.ClearSelection();

        }
        private void listaInv(string texto)
        {
            int row = this.invitaciones.RowCount;
            this.invitaciones.Rows.Insert(row - 1, texto);
            this.invitaciones.ClearSelection();
        }
        private void esconderInicioSession()
        {
            inicioSession.Visible = false;
            Registro.Visible = false;
            invit.Visible = true;
            consultaBox.Visible = true;
        }
        private void esconderMenu()
        {
            invit.Visible = false;
            consultaBox.Visible = false;
            tablero.Visible = true;
            chat.Visible = true;
        }
        private void mostrarMenu()
        {
            tablero.Visible = false;
            chat.Visible = false;
            invit.Visible = true;
            consultaBox.Visible = true;
        }
        private void listaChat(string texto)
        {
            int row = this.msg.RowCount;
            this.msg.Rows.Insert(row-1, texto);
            this.msg.ClearSelection();

        }

        private void AtenderServidor()
        {
            bool fin = false;
            while (fin == false)
            {
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];

                switch (codigo)
                {
                    case 1: //Iniciar sesion
                        MessageBox.Show(mensaje);
                        EstColor.BackColor = Color.Green;
                        //Estado.Text = usuarioInicio.Text;
                        this.Invoke(new DelegadoParaEscribirTexto(estNombre), new object[] {usuarioInicio.Text});
                        //inicioSession.Visible = false;
                        //Registro.Visible = false;
                        //invit.Visible = true;
                        //consultaBox.Visible = true;
                        this.Invoke(new DelegadoParaEjecutar(esconderInicioSession), new object[] {});
                        break;

                    case 2://Registrarse
                        //inicioSession.Visible = false;
                        //Registro.Visible = false;
                        //invit.Visible = true;
                        //consultaBox.Visible = true;
                        break;
                    case 3://Desconexion
                        MessageBox.Show(mensaje);
                        EstColor.BackColor = Color.Red;
                        Estado.Text = "No iniciado";
                        this.BackColor = Color.Gray;
                        server.Shutdown(SocketShutdown.Both);
                        server.Close();
                        break;
                    case 4:

                        break;
                    case 5:     //Recibimos notificacion
                        MessageBox.Show(mensaje);
                        break;
                    case 6:     //Notificación de actualización de la lista de conectados
                        /*string[] subtrozos = mensaje.Split(',');
                        int num = Convert.ToInt32(subtrozos.Length);

                        this.conectados.Rows.Clear();

                        for (int i = 0; i < num; i++)
                        {
                            this.conectados.Rows.Insert(i, subtrozos[i]);
                            if (subtrozos[i] == Estado.Text)
                                this.conectados.Rows[i].DefaultCellStyle.ForeColor = Color.Gray;
                        }
                        this.conectados.ClearSelection();*/

                        this.Invoke(new DelegadoParaEscribirTexto(listaUsers), new object[] {mensaje});

                        break;
                    case 7:    //Petición de invitación
                        this.Invoke(new DelegadoParaEscribirTexto(listaInv), new object[] { mensaje });


                        break;

                    case 8:
                        break;

                    case 9: //Notificación de inicio de partida a los jugadores implicados
                        MessageBox.Show("Ganador es " + mensaje);
                        this.Invoke(new DelegadoParaEjecutar(mostrarMenu), new object[] { });
                        reiniciarTabla1();
                        reiniciarTabla2();
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEjecutar(mostrarMenu), new object[] { });
                        break;

                    case 10://Fin de Partida
                        

                        break;

                    case 11://Iniciar partida
                        //inicioSession.Visible = false;
                        //Registro.Visible = false;
                        //invit.Visible = false;
                        //consultaBox.Visible = false;
                        //chat.Visible = true;
                        //tablero.Visible = true;
                        reiniciarTabla1();
                        reiniciarTabla2();
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "0" });
                        this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "0" });
                        this.Invoke(new DelegadoParaEscribirTexto(seleccion1), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEscribirTexto(seleccion2), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEscribirTexto(seleccion3), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEscribirTexto(seleccion4), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEscribirTexto(seleccion5), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
                        this.Invoke(new DelegadoParaEscribirTexto(barcohund1), new object[] { "0" });
                        this.Invoke(new DelegadoParaEscribirTexto(barcohund2), new object[] { "0" });
                        //Puedo hacer que de estar en invisible el lbl de adversario junto a su caja y todo este oculto hasta ahora
                        this.Invoke(new DelegadoParaEjecutar(esconderMenu), new object[] { });
                        MessageBox.Show("Partida iniciada");
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 3 posiciones para tus botes" });
                        break;

                    case 12: //Aceptacion de botes y paso al siguiente estado bergantin
                        posicionesConfirmadas();
                        this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "1" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 3 posiciones en linea (juntos) para tu bergantín" });
                        break;
                    case 13: //Aceptacion del 1r bergantin y cambio al 2ndo bergantin
                        posicionesConfirmadas();
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 3 posiciones en linea (juntos) para tu bergantín" });
                        break;
                    case 14: //Aceptacion del 2ndo bergantin y cambio al estado del destructor
                        posicionesConfirmadas();
                        this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "2" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 4 posiciones en linea (juntos) para tu destructor" });
                        break;
                    case 15: //Aceptacion del destructor y cambio al estado del portaaviones
                        posicionesConfirmadas();
                        this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "3" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 5 posiciones en linea (juntos) para tu portaaviones" });
                        break;
                    case 16: //Aceptacion del portaaviones y bloqueo de mi tablero
                        posicionesConfirmadas();                                
                        this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "-1" });
                        break;
                    case 17: //Estado de partida para poder atacar (empezar)
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "1" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge una posicion para atacar"});
                        break;
                    case 18: //Estado de partida esperar al turno del enemigo
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "2" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Espera a tu turno" });
                        break;
                    case 19: //Estado para recibir si te han undido algun barco de mi tablero y actualizar su color a negro
                        int bh1 = Convert.ToInt32(barcoshundidos1.Text);
                        bh1++;
                        this.Invoke(new DelegadoParaEscribirTexto(barcohund1), new object[] {Convert.ToString(bh1)});
                        int tipobarco = Convert.ToInt32(trozos[1]);
                        string[] posiciones = trozos[2].Split(',');
                        if (tipobarco == 1)
                        {
                            hundirBarco1(Convert.ToInt32(posiciones[0]));
                        }
                        else if (tipobarco == 2)
                        {
                            hundirBarco1(Convert.ToInt32(posiciones[0]));
                            hundirBarco1(Convert.ToInt32(posiciones[1]));
                            hundirBarco1(Convert.ToInt32(posiciones[2]));
                        }
                        else if (tipobarco == 3)
                        {
                            hundirBarco1(Convert.ToInt32(posiciones[0]));
                            hundirBarco1(Convert.ToInt32(posiciones[1]));
                            hundirBarco1(Convert.ToInt32(posiciones[2]));
                            hundirBarco1(Convert.ToInt32(posiciones[3]));
                        }
                        else if (tipobarco == 4)
                        {
                            hundirBarco1(Convert.ToInt32(posiciones[0]));
                            hundirBarco1(Convert.ToInt32(posiciones[1]));
                            hundirBarco1(Convert.ToInt32(posiciones[2]));
                            hundirBarco1(Convert.ToInt32(posiciones[3]));
                            hundirBarco1(Convert.ToInt32(posiciones[4]));
                        }
                        if(bh1 == 7)
                        {
                            MessageBox.Show("Has Perdido!");
                            this.Invoke(new DelegadoParaEjecutar(mostrarMenu), new object[] { });
                            reiniciarTabla1();
                            reiniciarTabla2();
                            this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "-1" });
                            this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "-1" });
                        }
                        break;
                    case 20: // Estado para recibir si has hundido algun barco del tablero del adversario y actualizar a negro
                        int bh2 = Convert.ToInt32(barcoshundidos2.Text);
                        bh2++;
                        this.Invoke(new DelegadoParaEscribirTexto(barcohund2), new object[] { Convert.ToString(bh2) });
                        int tipobarco2 = Convert.ToInt32(trozos[1]);
                        string[] posiciones2 = trozos[2].Split(',');
                        if (tipobarco2 == 1)
                        {
                            hundirBarco2(Convert.ToInt32(posiciones2[0]));
                        }
                        else if (tipobarco2 == 2)
                        {
                            hundirBarco2(Convert.ToInt32(posiciones2[0]));
                            hundirBarco2(Convert.ToInt32(posiciones2[1]));
                            hundirBarco2(Convert.ToInt32(posiciones2[2]));
                        }
                        else if (tipobarco2 == 3)
                        {
                            hundirBarco2(Convert.ToInt32(posiciones2[0]));
                            hundirBarco2(Convert.ToInt32(posiciones2[1]));
                            hundirBarco2(Convert.ToInt32(posiciones2[2]));
                            hundirBarco2(Convert.ToInt32(posiciones2[3]));
                        }
                        else if (tipobarco2 == 4)
                        {
                            hundirBarco2(Convert.ToInt32(posiciones2[0]));
                            hundirBarco2(Convert.ToInt32(posiciones2[1]));
                            hundirBarco2(Convert.ToInt32(posiciones2[2]));
                            hundirBarco2(Convert.ToInt32(posiciones2[3]));
                            hundirBarco2(Convert.ToInt32(posiciones2[4]));
                        }
                        if (bh2 == 7)
                        {
                            MessageBox.Show("Has Ganado !");
                            this.Invoke(new DelegadoParaEjecutar(mostrarMenu), new object[] { });
                            reiniciarTabla1();
                            reiniciarTabla2();
                            this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "-1" });
                            this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "-1" });
                        }
                        break;
                    case 21: //Estado para ver si al atacar no se ha dado a ningun barco y actualizar su color a azul
                        tocaAgua2(Convert.ToInt32(mensaje));
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "2" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Espera a tu turno" });
                        break;
                    case 22: // Estado para ver si al recicibir el ataque no te han dado a un barco y actualizar a azul
                        tocaAgua1(Convert.ToInt32(mensaje));
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "1" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge una posicion para atacar" });
                        break;
                    case 23: //Estado para si al atacar le has dado a algun barco y poner rojo
                        tocaBarco2(Convert.ToInt32(mensaje));
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "1" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge una posicion para atacar" });
                        break;
                    case 24: //Estado para si al atacar me han dado a algun barco y poner rojo
                        tocaBarco1(Convert.ToInt32(mensaje));
                        this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "2" });
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Espera a tu turno" });
                        break;
                    case 25://Notifica mensaje del chat
                        this.Invoke(new DelegadoParaEscribirTexto(listaChat), new object[] {mensaje});
                        break;
                    case 26://has ganado
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Has ganado!" });
                        break;
                    case 27://has perdido
                        this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Perdiste!" });
                        break;
                    case 28:
                        break;
                    case 29:
                        break;
                    case 30:
                        break;
                    case 31:
                        break;
                    case 32:
                        break;
                    case 33:
                        break;
                    case 34:
                        break;
                    case 35://Consulta 1
                        break;
                    case 36://Consulta 2
                        break;
                    case 37://Consulta 3
                        break;

                        
                }
            }
        }

        private void confirmar_Click(object sender, EventArgs e)
        {
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            string mensaje;
            if (estadoTablero != -1)
            { //Estado de Seleccion de flota por estados
                int v1 = Convert.ToInt32(seleccion1Lbl.Text);
                int v2 = Convert.ToInt32(seleccion2Lbl.Text);
                int v3 = Convert.ToInt32(seleccion3Lbl.Text);
                int v4 = Convert.ToInt32(seleccion4Lbl.Text);
                int v5 = Convert.ToInt32(seleccion5Lbl.Text);
                

                if (estadoTablero == 0)
                {//Estado de seleccion de los botes
                    if (v1 == -1 || v2 == -1 || v3 == -1) //Comprobamos que esten los 3 barcos seleccionados
                        MessageBox.Show("Has de seleccionar 3 posiciones para los botes");
                    else
                    {
                        mensaje = "9/" + v1 + "," + v2 + "," + v3;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                else if (estadoTablero == 1)
                {
                    if (v1 == -1 || v2 == -1 || v3 == -1) //Comprobamos que esten las 3 posiciones del bergantin seleccionados
                        MessageBox.Show("Has de seleccionar 3 posiciones en fila para el bergantín");
                    else
                    {
                        mensaje = "10/" + v1 + "," + v2 + "," + v3;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                else if (estadoTablero == 2)
                {
                    if (v1 == -1 || v2 == -1 || v3 == -1 || v4 == -1) //Comprobamos que esten las 4 posiciones del destructor seleccionados
                        MessageBox.Show("Has de seleccionar 4 posiciones en fila para el destrucctor");
                    else
                    {
                        mensaje = "11/" + v1 + "," + v2 + "," + v3 + "," + v4;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                else if (estadoTablero == 3)
                {
                    if (v1 == -1 || v2 == -1 || v3 == -1 || v4 == -1 || v5 == -1) //Comprobamos que esten las 5 posiciones del portaaviones seleccionados
                        MessageBox.Show("Has de seleccionar 5 posiciones en fila para el portaaviones");
                    else
                    {
                        mensaje = "12/" + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                
            }
            if (estadoPartida == 1)
            {
                int v1 = Convert.ToInt32(atacarLbl.Text);
                if (v1 == -1 )
                {
                    MessageBox.Show("Has de seleccionar 1 casilla para atacar");
                }
                else
                {
                    mensaje = "13/" + v1;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
            }

        }

        private int seleccionado(int pos)
        {
            int v1 = Convert.ToInt32(seleccion1Lbl.Text);
            int v2 = Convert.ToInt32(seleccion2Lbl.Text);
            int v3 = Convert.ToInt32(seleccion3Lbl.Text);
            int v4 = Convert.ToInt32(seleccion4Lbl.Text);
            int v5 = Convert.ToInt32(seleccion5Lbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (v1 == -1)
            {
                this.Invoke(new DelegadoParaEscribirTexto(seleccion1), new object[] { pos.ToString() });
                return 1;
            }
            if (v2 == -1)
            {
                this.Invoke(new DelegadoParaEscribirTexto(seleccion2), new object[] { pos.ToString() });
                return 1;
            }
            if (v3 == -1)
            {
                this.Invoke(new DelegadoParaEscribirTexto(seleccion3), new object[] { pos.ToString() });
                return 1;
            }
            if (estadoTablero == 2 || estadoTablero == 3)
            {
                if (v4 == -1)
                {
                    this.Invoke(new DelegadoParaEscribirTexto(seleccion4), new object[] { pos.ToString() });
                    return 1;
                }
            }
            if (estadoTablero == 3)
            {
                if (v5 == -1)
                {
                    this.Invoke(new DelegadoParaEscribirTexto(seleccion5), new object[] { pos.ToString() });
                    return 1;
                }
            }
            MessageBox.Show("No puedes hacer mas selecciones, si quieres cambiar alguna seleccion primero clica en la seleccion a cambiar");
            return 0;
        }

        private void eliminarSeleccion(int pos)
        {
            int restablecer = -1;
            int v1 = Convert.ToInt32(seleccion1Lbl.Text);
            int v2 = Convert.ToInt32(seleccion2Lbl.Text);
            int v3 = Convert.ToInt32(seleccion3Lbl.Text);
            int v4 = Convert.ToInt32(seleccion4Lbl.Text);
            int v5 = Convert.ToInt32(seleccion5Lbl.Text);

            if (v1 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion1), new object[] { restablecer.ToString() });
            if (v2 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion2), new object[] { restablecer.ToString() });
            if (v3 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion3), new object[] { restablecer.ToString() });
            if (v4 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion4), new object[] { restablecer.ToString() });
            if (v5 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion5), new object[] { restablecer.ToString() });
        }
        private int ataque(int pos)
        {
            int atck = Convert.ToInt32(atacarLbl.Text);
            if (atck == -1)
            {
                this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { pos.ToString() });
                return 1;
            }
            MessageBox.Show("No puedes hacer mas de un ataque");
            return 0;
        }

        private void eliminarAtaque()
        {
            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }

        private void posicionesConfirmadas() //CAMBIA DE COLOR DEL CYAN AL VERDE LOS BARCOS CONFIRMADOS Y CAMBIA LOS VALORES A -1 DE LOS LBL'S
        {

            if (A1_1.BackColor == Color.Cyan)
                A1_1.BackColor = Color.Green;
            if (A2_1.BackColor == Color.Cyan)
                A2_1.BackColor = Color.Green;
            if (A3_1.BackColor == Color.Cyan)
                A3_1.BackColor = Color.Green;
            if (A4_1.BackColor == Color.Cyan)
                A4_1.BackColor = Color.Green;
            if (A5_1.BackColor == Color.Cyan)
                A5_1.BackColor = Color.Green;
            if (A6_1.BackColor == Color.Cyan)
                A6_1.BackColor = Color.Green;

            if (B1_1.BackColor == Color.Cyan)
                B1_1.BackColor = Color.Green;
            if (B2_1.BackColor == Color.Cyan)
                B2_1.BackColor = Color.Green;
            if (B3_1.BackColor == Color.Cyan)
                B3_1.BackColor = Color.Green;
            if (B4_1.BackColor == Color.Cyan)
                B4_1.BackColor = Color.Green;
            if (B5_1.BackColor == Color.Cyan)
                B5_1.BackColor = Color.Green;
            if (B6_1.BackColor == Color.Cyan)
                B6_1.BackColor = Color.Green;

            if (C1_1.BackColor == Color.Cyan)
                C1_1.BackColor = Color.Green;
            if (C2_1.BackColor == Color.Cyan)
                C2_1.BackColor = Color.Green;
            if (C3_1.BackColor == Color.Cyan)
                C3_1.BackColor = Color.Green;
            if (C4_1.BackColor == Color.Cyan)
                C4_1.BackColor = Color.Green;
            if (C5_1.BackColor == Color.Cyan)
                C5_1.BackColor = Color.Green;
            if (C6_1.BackColor == Color.Cyan)
                C6_1.BackColor = Color.Green;

            if (D1_1.BackColor == Color.Cyan)
                D1_1.BackColor = Color.Green;
            if (D2_1.BackColor == Color.Cyan)
                D2_1.BackColor = Color.Green;
            if (D3_1.BackColor == Color.Cyan)
                D3_1.BackColor = Color.Green;
            if (D4_1.BackColor == Color.Cyan)
                D4_1.BackColor = Color.Green;
            if (D5_1.BackColor == Color.Cyan)
                D5_1.BackColor = Color.Green;
            if (D6_1.BackColor == Color.Cyan)
                D6_1.BackColor = Color.Green;

            if (E1_1.BackColor == Color.Cyan)
                E1_1.BackColor = Color.Green;
            if (E2_1.BackColor == Color.Cyan)
                E2_1.BackColor = Color.Green;
            if (E3_1.BackColor == Color.Cyan)
                E3_1.BackColor = Color.Green;
            if (E4_1.BackColor == Color.Cyan)
                E4_1.BackColor = Color.Green;
            if (E5_1.BackColor == Color.Cyan)
                E5_1.BackColor = Color.Green;
            if (E6_1.BackColor == Color.Cyan)
                E6_1.BackColor = Color.Green;

            if (F1_1.BackColor == Color.Cyan)
                F1_1.BackColor = Color.Green;
            if (F2_1.BackColor == Color.Cyan)
                F2_1.BackColor = Color.Green;
            if (F3_1.BackColor == Color.Cyan)
                F3_1.BackColor = Color.Green;
            if (F4_1.BackColor == Color.Cyan)
                F4_1.BackColor = Color.Green;
            if (F5_1.BackColor == Color.Cyan)
                F5_1.BackColor = Color.Green;
            if (F6_1.BackColor == Color.Cyan)
                F6_1.BackColor = Color.Green;

            this.Invoke(new DelegadoParaEscribirTexto(seleccion1), new object[] { "-1" });
            this.Invoke(new DelegadoParaEscribirTexto(seleccion2), new object[] { "-1" });
            this.Invoke(new DelegadoParaEscribirTexto(seleccion3), new object[] { "-1" });
            this.Invoke(new DelegadoParaEscribirTexto(seleccion4), new object[] { "-1" });
            this.Invoke(new DelegadoParaEscribirTexto(seleccion5), new object[] { "-1" });
        }
        
        private void contraseñaInicio_TextChanged(object sender, EventArgs e)
        {

        }

        private void verPass_Click(object sender, EventArgs e)
        {
            if(contraseñaInicio.UseSystemPasswordChar == true)
            {
                verPass.Text = "👁️";
                contraseñaInicio.UseSystemPasswordChar = false;
            }
            else if(contraseñaInicio.UseSystemPasswordChar == false)
            {
                verPass.Text = "-";
                contraseñaInicio.UseSystemPasswordChar = true;
            }
        }

        private void inSession_Click(object sender, EventArgs e)
        {
            if(Estado.Text == "No iniciado")
            {
                string mensaje;
                mensaje = "1/" + usuarioInicio.Text + "/" + contraseñaInicio.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            

        }

        private void Registro_Enter(object sender, EventArgs e)
        {

        }

        private void registrarse_Click(object sender, EventArgs e)
        {
            string mensaje;
            mensaje = "1/" + nombreReg + "/" + userReg.Text + "/" + contraReg.Text + "/" + edadReg.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void invitaciones_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void eliminarSeleccion2(int pos)
        {
            int v1 = Convert.ToInt32(atacarLbl.Text);
            if (v1 == pos)
                eliminarAtaque();
        }
        private void A1_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (A1_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (A1_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(11);
                        A1_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (A1_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(11);
                        if (sel == 1)
                            A1_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void A2_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (A2_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (A2_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(12);
                        A2_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (A2_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(12);
                        if (sel == 1)
                            A2_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void A3_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (A3_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (A3_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(13);
                        A3_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (A3_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(13);
                        if (sel == 1)
                            A3_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void A4_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (A4_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (A4_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(14);
                        A4_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (A4_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(14);
                        if (sel == 1)
                            A4_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void A5_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (A5_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (A5_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(15);
                        A5_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (A5_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(15);
                        if (sel == 1)
                            A5_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void A6_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (A6_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (A6_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(16);
                        A6_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (A6_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(16);
                        if (sel == 1)
                            A6_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void B1_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (B1_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (B1_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(21);
                        B1_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (B1_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(21);
                        if (sel == 1)
                            B1_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }

        }

        private void B2_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (B2_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (B2_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(22);
                        B2_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (B2_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(22);
                        if (sel == 1)
                            B2_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void B3_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (B3_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (B3_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(23);
                        B3_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (B3_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(23);
                        if (sel == 1)
                            B3_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void B4_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (B4_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (B4_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(24);
                        B4_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (B4_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(24);
                        if (sel == 1)
                            B4_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void B5_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (B5_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (B5_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(25);
                        B5_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (B5_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(25);
                        if (sel == 1)
                            B5_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void B6_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (B6_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (B6_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(26);
                        B6_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (B6_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(26);
                        if (sel == 1)
                            B6_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void C1_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (C1_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (C1_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(31);
                        C1_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (C1_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(31);
                        if (sel == 1)
                            C1_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void C2_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (C2_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (C2_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(32);
                        C2_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (C2_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(32);
                        if (sel == 1)
                            C2_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void C3_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (C3_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (C3_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(33);
                        C3_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (C3_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(33);
                        if (sel == 1)
                            C3_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void C4_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (C4_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (C4_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(34);
                        C4_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (C4_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(34);
                        if (sel == 1)
                            C4_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void C5_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (C5_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (C5_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(35);
                        C5_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (C5_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(35);
                        if (sel == 1)
                            C5_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void C6_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (C6_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (C6_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(36);
                        C6_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (C6_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(36);
                        if (sel == 1)
                            C6_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void D1_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (D1_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (D1_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(41);
                        D1_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (D1_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(41);
                        if (sel == 1)
                            D1_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void D2_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (D2_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (D2_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(42);
                        D2_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (D2_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(42);
                        if (sel == 1)
                            D2_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void D3_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (D3_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (D3_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(43);
                        D3_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (D3_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(43);
                        if (sel == 1)
                            D3_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void D4_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (D4_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (D4_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(44);
                        D4_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (D4_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(44);
                        if (sel == 1)
                            D4_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void D5_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (D5_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (D5_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(45);
                        D5_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (D5_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(45);
                        if (sel == 1)
                            D5_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void D6_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (D6_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (D6_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(46);
                        D6_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (D6_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(46);
                        if (sel == 1)
                            D6_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void E1_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (E1_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (E1_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(51);
                        E1_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (E1_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(51);
                        if (sel == 1)
                            E1_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void E2_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (E2_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (E2_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(52);
                        E2_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (E2_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(52);
                        if (sel == 1)
                            E2_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void E3_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (E3_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (E3_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(53);
                        E3_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (E3_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(53);
                        if (sel == 1)
                            E3_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void E4_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (E4_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (E4_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(54);
                        E4_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (E4_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(54);
                        if (sel == 1)
                            E4_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void E5_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (E5_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (E5_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(55);
                        E5_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (E5_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(55);
                        if (sel == 1)
                            E5_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void E6_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (E6_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (E6_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(56);
                        E6_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (E6_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(56);
                        if (sel == 1)
                            E6_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void F1_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (F1_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (F1_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(61);
                        F1_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (F1_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(61);
                        if (sel == 1)
                            F1_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void F2_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (F2_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (F2_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(62);
                        F2_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (F2_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(62);
                        if (sel == 1)
                            F2_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void F3_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (F3_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (F3_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(63);
                        F3_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (F3_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(63);
                        if (sel == 1)
                            F3_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void F4_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (F4_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (F4_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(64);
                        F4_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (F4_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(64);
                        if (sel == 1)
                            F4_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void F5_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (F5_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (F5_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(65);
                        F5_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (F5_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(65);
                        if (sel == 1)
                            F5_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }

        private void F6_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida == 0)
            {
                if (estadoTablero == -1) { }
                else
                {
                    if (F6_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (F6_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(66);
                        F6_1.BackColor = SystemColors.Control;//COLOR POR DEFECTO
                    }
                    else if (F6_1.BackColor == SystemColors.Control)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(66);
                        if (sel == 1)
                            F6_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }
        private void tocaAgua1(int pos) //CAMBIA DE COLOR DEL Yellow AL Blue LOS ATAQUES QUE NO IMPACTAN
        {
            if (A1_1.BackColor == SystemColors.Control && pos == 11)
                A1_1.BackColor = Color.Blue;
            else if (A2_1.BackColor == SystemColors.Control && pos == 12)
                A2_1.BackColor = Color.Blue;
            else if (A3_1.BackColor == SystemColors.Control && pos == 13)
                A3_1.BackColor = Color.Blue;
            else if (A4_1.BackColor == SystemColors.Control && pos == 14)
                A4_1.BackColor = Color.Blue;
            else if (A5_1.BackColor == SystemColors.Control && pos == 15)
                A5_1.BackColor = Color.Blue;
            else if (A6_1.BackColor == SystemColors.Control && pos == 16)
                A6_1.BackColor = Color.Blue;

            else if (B1_1.BackColor == SystemColors.Control && pos == 21)
                B1_1.BackColor = Color.Blue;
            else if (B2_1.BackColor == SystemColors.Control && pos == 22)
                B2_1.BackColor = Color.Blue;
            else if (B3_1.BackColor == SystemColors.Control && pos == 23)
                B3_1.BackColor = Color.Blue;
            else if (B4_1.BackColor == SystemColors.Control && pos == 24)
                B4_1.BackColor = Color.Blue;
            else if (B5_1.BackColor == SystemColors.Control && pos == 25)
                B5_1.BackColor = Color.Blue;
            else if (B6_1.BackColor == SystemColors.Control && pos == 26)
                B6_1.BackColor = Color.Blue;

            else if (C1_1.BackColor == SystemColors.Control && pos == 31)
                C1_1.BackColor = Color.Blue;
            else if (C2_1.BackColor == SystemColors.Control && pos == 32)
                C2_1.BackColor = Color.Blue;
            else if (C3_1.BackColor == SystemColors.Control && pos == 33)
                C3_1.BackColor = Color.Blue;
            else if (C4_1.BackColor == SystemColors.Control && pos == 34)
                C4_1.BackColor = Color.Blue;
            else if (C5_1.BackColor == SystemColors.Control && pos == 35)
                C5_1.BackColor = Color.Blue;
            else if (C6_1.BackColor == SystemColors.Control && pos == 36)
                C6_1.BackColor = Color.Blue;

            else if (D1_1.BackColor == SystemColors.Control && pos == 41)
                D1_1.BackColor = Color.Blue;
            else if (D2_1.BackColor == SystemColors.Control && pos == 42)
                D2_1.BackColor = Color.Blue;
            else if (D2_1.BackColor == SystemColors.Control && pos == 43)
                D2_1.BackColor = Color.Blue;
            else if (D2_1.BackColor == SystemColors.Control && pos == 44)
                D2_1.BackColor = Color.Blue;
            else if (D2_1.BackColor == SystemColors.Control && pos == 45)
                D2_1.BackColor = Color.Blue;
            else if (D2_1.BackColor == SystemColors.Control && pos == 46)
                D2_1.BackColor = Color.Blue;

            else if (E1_1.BackColor == SystemColors.Control && pos == 51)
                E1_1.BackColor = Color.Blue;
            else if (E2_1.BackColor == SystemColors.Control && pos == 52)
                E2_1.BackColor = Color.Blue;
            else if (E3_1.BackColor == SystemColors.Control && pos == 53)
                E3_1.BackColor = Color.Blue;
            else if (E4_1.BackColor == SystemColors.Control && pos == 54)
                E4_1.BackColor = Color.Blue;
            else if (E5_1.BackColor == SystemColors.Control && pos == 55)
                E5_1.BackColor = Color.Blue;
            else if (E6_1.BackColor == SystemColors.Control && pos == 56)
                E6_1.BackColor = Color.Blue;

            else if (F1_1.BackColor == SystemColors.Control && pos == 61)
                F1_1.BackColor = Color.Blue;
            else if (F2_1.BackColor == SystemColors.Control && pos == 62)
                F2_1.BackColor = Color.Blue;
            else if (F3_1.BackColor == SystemColors.Control && pos == 63)
                F3_1.BackColor = Color.Blue;
            else if (F4_1.BackColor == SystemColors.Control && pos == 64)
                F4_1.BackColor = Color.Blue;
            else if (F5_1.BackColor == SystemColors.Control && pos == 65)
                F5_1.BackColor = Color.Blue;
            else if (F6_1.BackColor == SystemColors.Control && pos == 66)
                F6_1.BackColor = Color.Blue;

            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void tocaAgua2(int pos) //CAMBIA DE COLOR DEL Yellow AL Blue LOS ATAQUES QUE NO IMPACTAN
        {
            if (A1_2.BackColor == Color.Yellow)
                A1_2.BackColor = Color.Blue;
            else if (A2_2.BackColor == Color.Yellow)
                A2_2.BackColor = Color.Blue;
            else if (A3_2.BackColor == Color.Yellow)
                A3_2.BackColor = Color.Blue;
            else if (A4_2.BackColor == Color.Yellow)
                A4_2.BackColor = Color.Blue;
            else if (A5_2.BackColor == Color.Yellow)
                A5_2.BackColor = Color.Blue;
            else if (A6_2.BackColor == Color.Yellow)
                A6_2.BackColor = Color.Blue;

            else if (B1_2.BackColor == Color.Yellow)
                B1_2.BackColor = Color.Blue;
            else if (B2_2.BackColor == Color.Yellow)
                B2_2.BackColor = Color.Blue;
            else if (B3_2.BackColor == Color.Yellow)
                B3_2.BackColor = Color.Blue;
            else if (B4_2.BackColor == Color.Yellow)
                B4_2.BackColor = Color.Blue;
            else if (B5_2.BackColor == Color.Yellow)
                B5_2.BackColor = Color.Blue;
            else if (B6_2.BackColor == Color.Yellow)
                B6_2.BackColor = Color.Blue;

            else if (C1_2.BackColor == Color.Yellow)
                C1_2.BackColor = Color.Blue;
            else if (C2_2.BackColor == Color.Yellow)
                C2_2.BackColor = Color.Blue;
            else if (C3_2.BackColor == Color.Yellow)
                C3_2.BackColor = Color.Blue;
            else if (C4_2.BackColor == Color.Yellow)
                C4_2.BackColor = Color.Blue;
            else if (C5_2.BackColor == Color.Yellow)
                C5_2.BackColor = Color.Blue;
            else if (C6_2.BackColor == Color.Yellow)
                C6_2.BackColor = Color.Blue;

            else if (D1_2.BackColor == Color.Yellow)
                D1_2.BackColor = Color.Blue;
            else if (D2_2.BackColor == Color.Yellow)
                D2_2.BackColor = Color.Blue;
            else if (D3_2.BackColor == Color.Yellow)
                D3_2.BackColor = Color.Blue;
            else if (D4_2.BackColor == Color.Yellow)
                D4_2.BackColor = Color.Blue;
            else if (D5_2.BackColor == Color.Yellow)
                D5_2.BackColor = Color.Blue;
            else if (D6_2.BackColor == Color.Yellow)
                D6_2.BackColor = Color.Blue;

            else if (E1_2.BackColor == Color.Yellow)
                E1_2.BackColor = Color.Blue;
            else if (E2_2.BackColor == Color.Yellow)
                E2_2.BackColor = Color.Blue;
            else if (E3_2.BackColor == Color.Yellow)
                E3_2.BackColor = Color.Blue;
            else if (E4_2.BackColor == Color.Yellow)
                E4_2.BackColor = Color.Blue;
            else if (E5_2.BackColor == Color.Yellow)
                E5_2.BackColor = Color.Blue;
            else if (E6_2.BackColor == Color.Yellow)
                E6_2.BackColor = Color.Blue;

            else if (F1_2.BackColor == Color.Yellow)
                F1_2.BackColor = Color.Blue;
            else if (F2_2.BackColor == Color.Yellow)
                F2_2.BackColor = Color.Blue;
            else if (F3_2.BackColor == Color.Yellow)
                F3_2.BackColor = Color.Blue;
            else if (F4_2.BackColor == Color.Yellow)
                F4_2.BackColor = Color.Blue;
            else if (F5_2.BackColor == Color.Yellow)
                F5_2.BackColor = Color.Blue;
            else if (F6_2.BackColor == Color.Yellow)
                F6_2.BackColor = Color.Blue;

            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void tocaBarco1(int pos) //CAMBIA DE COLOR DEL Yellow AL Red LOS ATAQUES EXISTOSOS
        {
            if (A1_1.BackColor == Color.Green && pos == 11)
                A1_1.BackColor = Color.Red;
            else if (A2_1.BackColor == Color.Green && pos == 12)
                A2_1.BackColor = Color.Red;
            else if (A3_1.BackColor == Color.Green && pos == 13)
                A3_1.BackColor = Color.Red;
            else if (A4_1.BackColor == Color.Green && pos == 14)
                A4_1.BackColor = Color.Red;
            else if (A5_1.BackColor == Color.Green && pos == 15)
                A5_1.BackColor = Color.Red;
            else if (A6_1.BackColor == Color.Green && pos == 16)
                A6_1.BackColor = Color.Red;

            else if (B1_1.BackColor == Color.Green && pos == 21)
                B1_1.BackColor = Color.Red;
            else if (B2_1.BackColor == Color.Green && pos == 22)
                B2_1.BackColor = Color.Red;
            else if (B3_1.BackColor == Color.Green && pos == 23)
                B3_1.BackColor = Color.Red;
            else if (B4_1.BackColor == Color.Green && pos == 24)
                B4_1.BackColor = Color.Red;
            else if (B5_1.BackColor == Color.Green && pos == 25)
                B5_1.BackColor = Color.Red;
            else if (B6_1.BackColor == Color.Green && pos == 26)
                B6_1.BackColor = Color.Red;

            else if (C1_1.BackColor == Color.Green && pos == 31)
                C1_1.BackColor = Color.Red;
            else if (C2_1.BackColor == Color.Green && pos == 32)
                C2_1.BackColor = Color.Red;
            else if (C3_1.BackColor == Color.Green && pos == 33)
                C3_1.BackColor = Color.Red;
            else if (C4_1.BackColor == Color.Green && pos == 34)
                C4_1.BackColor = Color.Red;
            else if (C5_1.BackColor == Color.Green && pos == 35)
                C5_1.BackColor = Color.Red;
            else if (C6_1.BackColor == Color.Green && pos == 36)
                C6_1.BackColor = Color.Red;

            else if (D1_1.BackColor == Color.Green && pos == 41)
                D1_1.BackColor = Color.Red;
            else if (D2_1.BackColor == Color.Green && pos == 42)
                D2_1.BackColor = Color.Red;
            else if (D3_1.BackColor == Color.Green && pos == 43)
                D3_1.BackColor = Color.Red;
            else if (D4_1.BackColor == Color.Green && pos == 44)
                D4_1.BackColor = Color.Red;
            else if (D5_1.BackColor == Color.Green && pos == 45)
                D5_1.BackColor = Color.Red;
            else if (D6_1.BackColor == Color.Green && pos == 46)
                D6_1.BackColor = Color.Red;

            else if (E1_1.BackColor == Color.Green && pos == 51)
                E1_1.BackColor = Color.Red;
            else if (E2_1.BackColor == Color.Green && pos == 52)
                E2_1.BackColor = Color.Red;
            else if (E3_1.BackColor == Color.Green && pos == 53)
                E3_1.BackColor = Color.Red;
            else if (E4_1.BackColor == Color.Green && pos == 54)
                E4_1.BackColor = Color.Red;
            else if (E5_1.BackColor == Color.Green && pos == 55)
                E5_1.BackColor = Color.Red;
            else if (E6_1.BackColor == Color.Green && pos == 56)
                E6_1.BackColor = Color.Red;

            else if (F1_1.BackColor == Color.Green && pos == 61)
                F1_1.BackColor = Color.Red;
            else if (F2_1.BackColor == Color.Green && pos == 62)
                F2_1.BackColor = Color.Red;
            else if (F3_1.BackColor == Color.Green && pos == 63)
                F3_1.BackColor = Color.Red;
            else if (F4_1.BackColor == Color.Green && pos == 64)
                F4_1.BackColor = Color.Red;
            else if (F5_1.BackColor == Color.Green && pos == 65)
                F5_1.BackColor = Color.Red;
            else if (F6_1.BackColor == Color.Green && pos == 66)
                F6_1.BackColor = Color.Red;

            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void tocaBarco2(int pos) //CAMBIA DE COLOR DEL Yellow AL Red LOS ATAQUES EXISTOSOS
        {

            if (A1_2.BackColor == Color.Yellow)
                A1_2.BackColor = Color.Red;
            else if (A2_2.BackColor == Color.Yellow)
                A2_2.BackColor = Color.Red;
            else if (A3_2.BackColor == Color.Yellow)
                A3_2.BackColor = Color.Red;
            else if (A4_2.BackColor == Color.Yellow)
                A4_2.BackColor = Color.Red;
            else if (A5_2.BackColor == Color.Yellow)
                A5_2.BackColor = Color.Red;
            else if (A6_2.BackColor == Color.Yellow)
                A6_2.BackColor = Color.Red;

            else if (B1_2.BackColor == Color.Yellow)
                B1_2.BackColor = Color.Red;
            else if (B2_2.BackColor == Color.Yellow)
                B2_2.BackColor = Color.Red;
            else if (B3_2.BackColor == Color.Yellow)
                B3_2.BackColor = Color.Red;
            else if (B4_2.BackColor == Color.Yellow)
                B4_2.BackColor = Color.Red;
            else if (B5_2.BackColor == Color.Yellow)
                B5_2.BackColor = Color.Red;
            else if (B6_2.BackColor == Color.Yellow)
                B6_2.BackColor = Color.Red;

            else if (C1_2.BackColor == Color.Yellow)
                C1_2.BackColor = Color.Red;
            else if (C2_2.BackColor == Color.Yellow)
                C2_2.BackColor = Color.Red;
            else if (C3_2.BackColor == Color.Yellow)
                C3_2.BackColor = Color.Red;
            else if (C4_2.BackColor == Color.Yellow)
                C4_2.BackColor = Color.Red;
            else if (C5_2.BackColor == Color.Yellow)
                C5_2.BackColor = Color.Red;
            else if (C6_2.BackColor == Color.Yellow)
                C6_2.BackColor = Color.Red;

            else if (D1_2.BackColor == Color.Yellow)
                D1_2.BackColor = Color.Red;
            else if (D2_2.BackColor == Color.Yellow)
                D2_2.BackColor = Color.Red;
            else if (D3_2.BackColor == Color.Yellow)
                D3_2.BackColor = Color.Red;
            else if (D4_2.BackColor == Color.Yellow)
                D4_2.BackColor = Color.Red;
            else if (D5_2.BackColor == Color.Yellow)
                D5_2.BackColor = Color.Red;
            else if (D6_2.BackColor == Color.Yellow)
                D6_2.BackColor = Color.Red;

            else if (E1_2.BackColor == Color.Yellow)
                E1_2.BackColor = Color.Red;
            else if (E2_2.BackColor == Color.Yellow)
                E2_2.BackColor = Color.Red;
            else if (E3_2.BackColor == Color.Yellow)
                E3_2.BackColor = Color.Red;
            else if (E4_2.BackColor == Color.Yellow)
                E4_2.BackColor = Color.Red;
            else if (E5_2.BackColor == Color.Yellow)
                E5_2.BackColor = Color.Red;
            else if (E6_2.BackColor == Color.Yellow)
                E6_2.BackColor = Color.Red;

            else if (F1_2.BackColor == Color.Yellow)
                F1_2.BackColor = Color.Red;
            else if (F2_2.BackColor == Color.Yellow)
                F2_2.BackColor = Color.Red;
            else if (F3_2.BackColor == Color.Yellow)
                F3_2.BackColor = Color.Red;
            else if (F4_2.BackColor == Color.Yellow)
                F4_2.BackColor = Color.Red;
            else if (F5_2.BackColor == Color.Yellow)
                F5_2.BackColor = Color.Red;
            else if (F6_2.BackColor == Color.Yellow)
                F6_2.BackColor = Color.Red;

            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void hundirBarco1(int pos) //CAMBIA DE COLOR DEL Red AL Black LOS BARCOS HUNDIDOS
        {
            if (A1_1.BackColor == Color.Red && pos == 11)
                A1_1.BackColor = Color.Black;
            else if (A2_1.BackColor == Color.Red && pos == 12)
                A2_1.BackColor = Color.Black;
            else if (A3_1.BackColor == Color.Red && pos == 13)
                A3_1.BackColor = Color.Black;
            else if (A4_1.BackColor == Color.Red && pos == 14)
                A4_1.BackColor = Color.Black;
            else if (A5_1.BackColor == Color.Red && pos == 15)
                A5_1.BackColor = Color.Black;
            else if (A6_1.BackColor == Color.Red && pos == 16)
                A6_1.BackColor = Color.Black;

            else if (B1_1.BackColor == Color.Red && pos == 21)
                B1_1.BackColor = Color.Black;
            else if (B2_1.BackColor == Color.Red && pos == 22)
                B2_1.BackColor = Color.Black;
            else if (B3_1.BackColor == Color.Red && pos == 23)
                B3_1.BackColor = Color.Black;
            else if (B4_1.BackColor == Color.Red && pos == 24)
                B4_1.BackColor = Color.Black;
            else if (B5_1.BackColor == Color.Red && pos == 25)
                B5_1.BackColor = Color.Black;
            else if (B6_1.BackColor == Color.Red && pos == 26)
                B6_1.BackColor = Color.Black;

            else if (C1_1.BackColor == Color.Red && pos == 31)
                C1_1.BackColor = Color.Black;
            else if (C2_1.BackColor == Color.Red && pos == 32)
                C2_1.BackColor = Color.Black;
            else if (C3_1.BackColor == Color.Red && pos == 33)
                C3_1.BackColor = Color.Black;
            else if (C4_1.BackColor == Color.Red && pos == 34)
                C4_1.BackColor = Color.Black;
            else if (C5_1.BackColor == Color.Red && pos == 35)
                C5_1.BackColor = Color.Black;
            else if (C6_1.BackColor == Color.Red && pos == 36)
                C6_1.BackColor = Color.Black;

            else if (D1_1.BackColor == Color.Red && pos == 41)
                D1_1.BackColor = Color.Black;
            else if (D2_1.BackColor == Color.Red && pos == 42)
                D2_1.BackColor = Color.Black;
            else if (D3_1.BackColor == Color.Red && pos == 43)
                D3_1.BackColor = Color.Black;
            else if (D4_1.BackColor == Color.Red && pos == 44)
                D4_1.BackColor = Color.Black;
            else if (D5_1.BackColor == Color.Red && pos == 45)
                D5_1.BackColor = Color.Black;
            else if (D6_1.BackColor == Color.Red && pos == 46)
                D6_1.BackColor = Color.Black;

            else if (E1_1.BackColor == Color.Red && pos == 51)
                E1_1.BackColor = Color.Black;
            else if (E2_1.BackColor == Color.Red && pos == 52)
                E2_1.BackColor = Color.Black;
            else if (E3_1.BackColor == Color.Red && pos == 53)
                E3_1.BackColor = Color.Black;
            else if (E4_1.BackColor == Color.Red && pos == 54)
                E4_1.BackColor = Color.Black;
            else if (E5_1.BackColor == Color.Red && pos == 55)
                E5_1.BackColor = Color.Black;
            else if (E6_1.BackColor == Color.Red && pos == 56)
                E6_1.BackColor = Color.Black;

            else if (F1_1.BackColor == Color.Red && pos == 61)
                F1_1.BackColor = Color.Black;
            else if (F2_1.BackColor == Color.Red && pos == 62)
                F2_1.BackColor = Color.Black;
            else if (F3_1.BackColor == Color.Red && pos == 63)
                F3_1.BackColor = Color.Black;
            else if (F4_1.BackColor == Color.Red && pos == 64)
                F4_1.BackColor = Color.Black;
            else if (F5_1.BackColor == Color.Red && pos == 65)
                F5_1.BackColor = Color.Black;
            else if (F6_1.BackColor == Color.Red && pos == 66)
                F6_1.BackColor = Color.Black;
        }
        private void hundirBarco2(int pos) //CAMBIA DE COLOR DEL Red AL Black LOS BARCOS HUNDIDOS
        {
            if (A1_2.BackColor == Color.Red && pos == 11)
                A1_2.BackColor = Color.Black;
            else if (A2_2.BackColor == Color.Red && pos == 12)
                A2_2.BackColor = Color.Black;
            else if (A3_2.BackColor == Color.Red && pos == 13)
                A3_2.BackColor = Color.Black;
            else if (A4_2.BackColor == Color.Red && pos == 14)
                A4_2.BackColor = Color.Black;
            else if (A5_2.BackColor == Color.Red && pos == 15)
                A5_2.BackColor = Color.Black;
            else if (A6_2.BackColor == Color.Red && pos == 16)
                A6_2.BackColor = Color.Black;

            else if (B1_2.BackColor == Color.Red && pos == 21)
                B1_2.BackColor = Color.Black;
            else if (B2_2.BackColor == Color.Red && pos == 22)
                B2_2.BackColor = Color.Black;
            else if (B3_2.BackColor == Color.Red && pos == 23)
                B3_2.BackColor = Color.Black;
            else if (B4_2.BackColor == Color.Red && pos == 24)
                B4_2.BackColor = Color.Black;
            else if (B5_2.BackColor == Color.Red && pos == 25)
                B5_2.BackColor = Color.Black;
            else if (B6_2.BackColor == Color.Red && pos == 26)
                B6_2.BackColor = Color.Black;

            else if (C1_2.BackColor == Color.Red && pos == 31)
                C1_2.BackColor = Color.Black;
            else if (C2_2.BackColor == Color.Red && pos == 32)
                C2_2.BackColor = Color.Black;
            else if (C3_2.BackColor == Color.Red && pos == 33)
                C3_2.BackColor = Color.Black;
            else if (C4_2.BackColor == Color.Red && pos == 34)
                C4_2.BackColor = Color.Black;
            else if (C5_2.BackColor == Color.Red && pos == 35)
                C5_2.BackColor = Color.Black;
            else if (C6_2.BackColor == Color.Red && pos == 36)
                C6_2.BackColor = Color.Black;

            else if (D1_2.BackColor == Color.Red && pos == 41)
                D1_2.BackColor = Color.Black;
            else if (D2_2.BackColor == Color.Red && pos == 42)
                D2_2.BackColor = Color.Black;
            else if (D3_2.BackColor == Color.Red && pos == 43)
                D3_2.BackColor = Color.Black;
            else if (D4_2.BackColor == Color.Red && pos == 44)
                D4_2.BackColor = Color.Black;
            else if (D5_2.BackColor == Color.Red && pos == 45)
                D5_2.BackColor = Color.Black;
            else if (D6_2.BackColor == Color.Red && pos == 46)
                D6_2.BackColor = Color.Black;

            else if (E1_2.BackColor == Color.Red && pos == 51)
                E1_2.BackColor = Color.Black;
            else if (E2_2.BackColor == Color.Red && pos == 52)
                E2_2.BackColor = Color.Black;
            else if (E3_2.BackColor == Color.Red && pos == 53)
                E3_2.BackColor = Color.Black;
            else if (E4_2.BackColor == Color.Red && pos == 54)
                E4_2.BackColor = Color.Black;
            else if (E5_2.BackColor == Color.Red && pos == 55)
                E5_2.BackColor = Color.Black;
            else if (E6_2.BackColor == Color.Red && pos == 56)
                E6_2.BackColor = Color.Black;

            else if (F1_2.BackColor == Color.Red && pos == 61)
                F1_2.BackColor = Color.Black;
            else if (F2_2.BackColor == Color.Red && pos == 62)
                F2_2.BackColor = Color.Black;
            else if (F3_2.BackColor == Color.Red && pos == 63)
                F3_2.BackColor = Color.Black;
            else if (F4_2.BackColor == Color.Red && pos == 64)
                F4_2.BackColor = Color.Black;
            else if (F5_2.BackColor == Color.Red && pos == 65)
                F5_2.BackColor = Color.Black;
            else if (F6_2.BackColor == Color.Red && pos == 66)
                F6_2.BackColor = Color.Black;
        }
        private void reiniciarTabla1() //CAMBIA DE COLOR DE TODAS LAS CASILLAS AL Default 
        {

            if (A1_1.BackColor == Color.Red || A1_1.BackColor == Color.Black || A1_1.BackColor == Color.Yellow || A1_1.BackColor == Color.Blue || A1_1.BackColor == Color.Green)
                A1_1.BackColor = SystemColors.Control;
            if (A2_1.BackColor == Color.Red || A2_1.BackColor == Color.Black || A2_1.BackColor == Color.Yellow || A2_1.BackColor == Color.Blue || A2_1.BackColor == Color.Green)
                A2_1.BackColor = SystemColors.Control;
            if (A3_1.BackColor == Color.Red || A3_1.BackColor == Color.Black || A3_1.BackColor == Color.Yellow || A3_1.BackColor == Color.Blue || A3_1.BackColor == Color.Green)
                A3_1.BackColor = SystemColors.Control;
            if (A4_1.BackColor == Color.Red || A4_1.BackColor == Color.Black || A4_1.BackColor == Color.Yellow || A4_1.BackColor == Color.Blue || A4_1.BackColor == Color.Green)
                A4_1.BackColor = SystemColors.Control;
            if (A5_1.BackColor == Color.Red || A5_1.BackColor == Color.Black || A5_1.BackColor == Color.Yellow || A5_1.BackColor == Color.Blue || A5_1.BackColor == Color.Green)
                A5_1.BackColor = SystemColors.Control;
            if (A6_1.BackColor == Color.Red || A6_1.BackColor == Color.Black || A6_1.BackColor == Color.Yellow || A6_1.BackColor == Color.Blue || A6_1.BackColor == Color.Green)
                A6_1.BackColor = SystemColors.Control;

            if (B1_1.BackColor == Color.Red || B1_1.BackColor == Color.Black || B1_1.BackColor == Color.Yellow || B1_1.BackColor == Color.Blue || B1_1.BackColor == Color.Green)
                B1_1.BackColor = SystemColors.Control;
            if (B2_1.BackColor == Color.Red || B2_1.BackColor == Color.Black || B2_1.BackColor == Color.Yellow || B2_1.BackColor == Color.Blue || B2_1.BackColor == Color.Green)
                B2_1.BackColor = SystemColors.Control;
            if (B3_1.BackColor == Color.Red || B3_1.BackColor == Color.Black || B3_1.BackColor == Color.Yellow || B3_1.BackColor == Color.Blue || B3_1.BackColor == Color.Green)
                B3_1.BackColor = SystemColors.Control;
            if (B4_1.BackColor == Color.Red || B4_1.BackColor == Color.Black || B4_1.BackColor == Color.Yellow || B4_1.BackColor == Color.Blue || B4_1.BackColor == Color.Green)
                B4_1.BackColor = SystemColors.Control;
            if (B5_1.BackColor == Color.Red || B5_1.BackColor == Color.Black || B5_1.BackColor == Color.Yellow || B5_1.BackColor == Color.Blue || B5_1.BackColor == Color.Green)
                B5_1.BackColor = SystemColors.Control;
            if (B6_1.BackColor == Color.Red || B6_1.BackColor == Color.Black || B6_1.BackColor == Color.Yellow || B6_1.BackColor == Color.Blue || B6_1.BackColor == Color.Green)
                B6_1.BackColor = SystemColors.Control;

            if (C1_1.BackColor == Color.Red || C1_1.BackColor == Color.Black || C1_1.BackColor == Color.Yellow || C1_1.BackColor == Color.Blue || C1_1.BackColor == Color.Green)
                C1_1.BackColor = SystemColors.Control;
            if (C2_1.BackColor == Color.Red || C2_1.BackColor == Color.Black || C2_1.BackColor == Color.Yellow || C2_1.BackColor == Color.Blue || C2_1.BackColor == Color.Green)
                C2_1.BackColor = SystemColors.Control;
            if (C3_1.BackColor == Color.Red || C3_1.BackColor == Color.Black || C3_1.BackColor == Color.Yellow || C3_1.BackColor == Color.Blue || C3_1.BackColor == Color.Green)
                C3_1.BackColor = SystemColors.Control;
            if (C4_1.BackColor == Color.Red || C4_1.BackColor == Color.Black || C4_1.BackColor == Color.Yellow || C4_1.BackColor == Color.Blue || C4_1.BackColor == Color.Green)
                C4_1.BackColor = SystemColors.Control;
            if (C5_1.BackColor == Color.Red || C5_1.BackColor == Color.Black || C5_1.BackColor == Color.Yellow || C5_1.BackColor == Color.Blue || C5_1.BackColor == Color.Green)
                C5_1.BackColor = SystemColors.Control;
            if (C6_1.BackColor == Color.Red || C6_1.BackColor == Color.Black || C6_1.BackColor == Color.Yellow || C6_1.BackColor == Color.Blue || C6_1.BackColor == Color.Green)
                C6_1.BackColor = SystemColors.Control;

            if (D1_1.BackColor == Color.Red || D1_1.BackColor == Color.Black || D1_1.BackColor == Color.Yellow || D1_1.BackColor == Color.Blue || D1_1.BackColor == Color.Green)
                D1_1.BackColor = SystemColors.Control;
            if (D2_1.BackColor == Color.Red || D2_1.BackColor == Color.Black || D2_1.BackColor == Color.Yellow || D2_1.BackColor == Color.Blue || D2_1.BackColor == Color.Green)
                D2_1.BackColor = SystemColors.Control;
            if (D2_1.BackColor == Color.Red || D3_1.BackColor == Color.Black || D3_1.BackColor == Color.Yellow || D3_1.BackColor == Color.Blue || D3_1.BackColor == Color.Green)
                D2_1.BackColor = SystemColors.Control;
            if (D2_1.BackColor == Color.Red || D4_1.BackColor == Color.Black || D4_1.BackColor == Color.Yellow || D4_1.BackColor == Color.Blue || D4_1.BackColor == Color.Green)
                D2_1.BackColor = SystemColors.Control;
            if (D2_1.BackColor == Color.Red || D5_1.BackColor == Color.Black || D5_1.BackColor == Color.Yellow || D5_1.BackColor == Color.Blue || D5_1.BackColor == Color.Green)
                D2_1.BackColor = SystemColors.Control;
            if (D2_1.BackColor == Color.Red || D6_1.BackColor == Color.Black || D6_1.BackColor == Color.Yellow || D6_1.BackColor == Color.Blue || D6_1.BackColor == Color.Green)
                D2_1.BackColor = SystemColors.Control;

            if (E1_1.BackColor == Color.Red || E1_1.BackColor == Color.Black || E1_1.BackColor == Color.Yellow || E1_1.BackColor == Color.Blue || E1_1.BackColor == Color.Green)
                E1_1.BackColor = SystemColors.Control;
            if (E2_1.BackColor == Color.Red || E2_1.BackColor == Color.Black || E2_1.BackColor == Color.Yellow || E2_1.BackColor == Color.Blue || E2_1.BackColor == Color.Green)
                E2_1.BackColor = SystemColors.Control;
            if (E3_1.BackColor == Color.Red || E3_1.BackColor == Color.Black || E3_1.BackColor == Color.Yellow || E3_1.BackColor == Color.Blue || E3_1.BackColor == Color.Green)
                E3_1.BackColor = SystemColors.Control;
            if (E4_1.BackColor == Color.Red || E4_1.BackColor == Color.Black || E4_1.BackColor == Color.Yellow || E4_1.BackColor == Color.Blue || E4_1.BackColor == Color.Green)
                E4_1.BackColor = SystemColors.Control;
            if (E5_1.BackColor == Color.Red || E5_1.BackColor == Color.Black || E5_1.BackColor == Color.Yellow || E5_1.BackColor == Color.Blue || E5_1.BackColor == Color.Green)
                E5_1.BackColor = SystemColors.Control;
            if (E6_1.BackColor == Color.Red || E6_1.BackColor == Color.Black || E6_1.BackColor == Color.Yellow || E6_1.BackColor == Color.Blue || E6_1.BackColor == Color.Green)
                E6_1.BackColor = SystemColors.Control;

            if (F1_1.BackColor == Color.Red || F1_1.BackColor == Color.Black || F1_1.BackColor == Color.Yellow || F1_1.BackColor == Color.Blue || F1_1.BackColor == Color.Green)
                F1_1.BackColor = SystemColors.Control;
            if (F2_1.BackColor == Color.Red || F2_1.BackColor == Color.Black || F2_1.BackColor == Color.Yellow || F2_1.BackColor == Color.Blue || F2_1.BackColor == Color.Green)
                F2_1.BackColor = SystemColors.Control;
            if (F3_1.BackColor == Color.Red || F3_1.BackColor == Color.Black || F3_1.BackColor == Color.Yellow || F3_1.BackColor == Color.Blue || F3_1.BackColor == Color.Green)
                F3_1.BackColor = SystemColors.Control;
            if (F4_1.BackColor == Color.Red || F4_1.BackColor == Color.Black || F4_1.BackColor == Color.Yellow || F4_1.BackColor == Color.Blue || F4_1.BackColor == Color.Green)
                F4_1.BackColor = SystemColors.Control;
            if (F5_1.BackColor == Color.Red || F5_1.BackColor == Color.Black || F5_1.BackColor == Color.Yellow || F5_1.BackColor == Color.Blue || F5_1.BackColor == Color.Green)
                F5_1.BackColor = SystemColors.Control;
            if (F6_1.BackColor == Color.Red || F6_1.BackColor == Color.Black || F6_1.BackColor == Color.Yellow || F6_1.BackColor == Color.Blue || F6_1.BackColor == Color.Green)
                F6_1.BackColor = SystemColors.Control;
        }
        private void reiniciarTabla2() //CAMBIA DE COLOR DE TODAS LAS CASILLAS AL Default 
        {
            if (A1_2.BackColor == Color.Red || A1_2.BackColor == Color.Black || A1_2.BackColor == Color.Yellow || A1_2.BackColor == Color.Blue || A1_2.BackColor == Color.Green)
                A1_2.BackColor = SystemColors.Control;
            if (A2_2.BackColor == Color.Red || A2_2.BackColor == Color.Black || A2_2.BackColor == Color.Yellow || A2_2.BackColor == Color.Blue || A2_2.BackColor == Color.Green)
                A2_2.BackColor = SystemColors.Control;
            if (A3_2.BackColor == Color.Red || A3_2.BackColor == Color.Black || A3_2.BackColor == Color.Yellow || A3_2.BackColor == Color.Blue || A3_2.BackColor == Color.Green)
                A3_2.BackColor = SystemColors.Control;
            if (A4_2.BackColor == Color.Red || A4_2.BackColor == Color.Black || A4_2.BackColor == Color.Yellow || A4_2.BackColor == Color.Blue || A4_2.BackColor == Color.Green)
                A4_2.BackColor = SystemColors.Control;
            if (A5_2.BackColor == Color.Red || A5_2.BackColor == Color.Black || A5_2.BackColor == Color.Yellow || A5_2.BackColor == Color.Blue || A5_2.BackColor == Color.Green)
                A5_2.BackColor = SystemColors.Control;
            if (A6_2.BackColor == Color.Red || A6_2.BackColor == Color.Black || A6_2.BackColor == Color.Yellow || A6_2.BackColor == Color.Blue || A6_2.BackColor == Color.Green)
                A6_2.BackColor = SystemColors.Control;

            if (B1_2.BackColor == Color.Red || B1_2.BackColor == Color.Black || B1_2.BackColor == Color.Yellow || B1_2.BackColor == Color.Blue || A1_2.BackColor == Color.Green)
                B1_2.BackColor = SystemColors.Control;
            if (B2_2.BackColor == Color.Red || B2_2.BackColor == Color.Black || B2_2.BackColor == Color.Yellow || B2_2.BackColor == Color.Blue || B2_2.BackColor == Color.Green)
                B2_2.BackColor = SystemColors.Control;
            if (B3_2.BackColor == Color.Red || B3_2.BackColor == Color.Black || B3_2.BackColor == Color.Yellow || B3_2.BackColor == Color.Blue || B3_2.BackColor == Color.Green)
                B3_2.BackColor = SystemColors.Control;
            if (B4_2.BackColor == Color.Red || B4_2.BackColor == Color.Black || B4_2.BackColor == Color.Yellow || B4_2.BackColor == Color.Blue || B4_2.BackColor == Color.Green)
                B4_2.BackColor = SystemColors.Control;
            if (B5_2.BackColor == Color.Red || B5_2.BackColor == Color.Black || B5_2.BackColor == Color.Yellow || B5_2.BackColor == Color.Blue || B5_2.BackColor == Color.Green)
                B5_2.BackColor = SystemColors.Control;
            if (B6_2.BackColor == Color.Red || B6_2.BackColor == Color.Black || B6_2.BackColor == Color.Yellow || B6_2.BackColor == Color.Blue || B6_2.BackColor == Color.Green)
                B6_2.BackColor = SystemColors.Control;

            if (C1_2.BackColor == Color.Red || C1_2.BackColor == Color.Black || C1_2.BackColor == Color.Yellow || C1_2.BackColor == Color.Blue || C1_2.BackColor == Color.Green)
                C1_2.BackColor = SystemColors.Control;
            if (C2_2.BackColor == Color.Red || C2_2.BackColor == Color.Black || C2_2.BackColor == Color.Yellow || C2_2.BackColor == Color.Blue || C2_2.BackColor == Color.Green)
                C2_2.BackColor = SystemColors.Control;
            if (C3_2.BackColor == Color.Red || C3_2.BackColor == Color.Black || C3_2.BackColor == Color.Yellow || C3_2.BackColor == Color.Blue || C3_2.BackColor == Color.Green)
                C3_2.BackColor = SystemColors.Control;
            if (C4_2.BackColor == Color.Red || C4_2.BackColor == Color.Black || C4_2.BackColor == Color.Yellow || C4_2.BackColor == Color.Blue || C4_2.BackColor == Color.Green)
                C4_2.BackColor = SystemColors.Control;
            if (C5_2.BackColor == Color.Red || C5_2.BackColor == Color.Black || C5_2.BackColor == Color.Yellow || C5_2.BackColor == Color.Blue || C5_2.BackColor == Color.Green)
                C5_2.BackColor = SystemColors.Control;
            if (C6_2.BackColor == Color.Red || C6_2.BackColor == Color.Black || C6_2.BackColor == Color.Yellow || C6_2.BackColor == Color.Blue || C6_2.BackColor == Color.Green)
                C6_2.BackColor = SystemColors.Control;

            if (D1_2.BackColor == Color.Red || D1_2.BackColor == Color.Black || D1_2.BackColor == Color.Yellow || D1_2.BackColor == Color.Blue || D1_2.BackColor == Color.Green)
                D1_2.BackColor = SystemColors.Control;
            if (D2_2.BackColor == Color.Red || D2_2.BackColor == Color.Black || D2_2.BackColor == Color.Yellow || D2_2.BackColor == Color.Blue || D2_2.BackColor == Color.Green)
                D2_2.BackColor = SystemColors.Control;
            if (D3_2.BackColor == Color.Red || D3_2.BackColor == Color.Black || D3_2.BackColor == Color.Yellow || D3_2.BackColor == Color.Blue || D3_2.BackColor == Color.Green)
                D3_2.BackColor = SystemColors.Control;
            if (D4_2.BackColor == Color.Red || D4_2.BackColor == Color.Black || D4_2.BackColor == Color.Yellow || D4_2.BackColor == Color.Blue || D4_2.BackColor == Color.Green)
                D4_2.BackColor = SystemColors.Control;
            if (D5_2.BackColor == Color.Red || D5_2.BackColor == Color.Black || D5_2.BackColor == Color.Yellow || D5_2.BackColor == Color.Blue || D5_2.BackColor == Color.Green)
                D5_2.BackColor = SystemColors.Control;
            if (D6_2.BackColor == Color.Red || D6_2.BackColor == Color.Black || D6_2.BackColor == Color.Yellow || D6_2.BackColor == Color.Blue || D6_2.BackColor == Color.Green)
                D6_2.BackColor = SystemColors.Control;

            if (E1_2.BackColor == Color.Red || E1_2.BackColor == Color.Black || E1_2.BackColor == Color.Yellow || E1_2.BackColor == Color.Blue || E1_2.BackColor == Color.Green)
                E1_2.BackColor = SystemColors.Control;
            if (E2_2.BackColor == Color.Red || E2_2.BackColor == Color.Black || E2_2.BackColor == Color.Yellow || E2_2.BackColor == Color.Blue || E2_2.BackColor == Color.Green)
                E2_2.BackColor = SystemColors.Control;
            if (E3_2.BackColor == Color.Red || E3_2.BackColor == Color.Black || E3_2.BackColor == Color.Yellow || E3_2.BackColor == Color.Blue || E3_2.BackColor == Color.Green)
                E3_2.BackColor = SystemColors.Control;
            if (E4_2.BackColor == Color.Red || E4_2.BackColor == Color.Black || E4_2.BackColor == Color.Yellow || E4_2.BackColor == Color.Blue || E4_2.BackColor == Color.Green)
                E4_2.BackColor = SystemColors.Control;
            if (E5_2.BackColor == Color.Red || E5_2.BackColor == Color.Black || E5_2.BackColor == Color.Yellow || E5_2.BackColor == Color.Blue || E5_2.BackColor == Color.Green)
                E5_2.BackColor = SystemColors.Control;
            if (E6_2.BackColor == Color.Red || E6_2.BackColor == Color.Black || E6_2.BackColor == Color.Yellow || E6_2.BackColor == Color.Blue || E6_2.BackColor == Color.Green)
                E6_2.BackColor = SystemColors.Control;

            if (F1_2.BackColor == Color.Red || F1_2.BackColor == Color.Black || F1_2.BackColor == Color.Yellow || F1_2.BackColor == Color.Blue || F1_2.BackColor == Color.Green)
                F1_2.BackColor = SystemColors.Control;
            if (F2_2.BackColor == Color.Red || F2_2.BackColor == Color.Black || F2_2.BackColor == Color.Yellow || F2_2.BackColor == Color.Blue || F2_2.BackColor == Color.Green)
                F2_2.BackColor = SystemColors.Control;
            if (F3_2.BackColor == Color.Red || F3_2.BackColor == Color.Black || F3_2.BackColor == Color.Yellow || F3_2.BackColor == Color.Blue || F3_2.BackColor == Color.Green)
                F3_2.BackColor = SystemColors.Control;
            if (F4_2.BackColor == Color.Red || F4_2.BackColor == Color.Black || F4_2.BackColor == Color.Yellow || F4_2.BackColor == Color.Blue || F4_2.BackColor == Color.Green)
                F4_2.BackColor = SystemColors.Control;
            if (F5_2.BackColor == Color.Red || F5_2.BackColor == Color.Black || F5_2.BackColor == Color.Yellow || F5_2.BackColor == Color.Blue || F5_2.BackColor == Color.Green)
                F5_2.BackColor = SystemColors.Control;
            if (F6_2.BackColor == Color.Red || F6_2.BackColor == Color.Black || F6_2.BackColor == Color.Yellow || F6_2.BackColor == Color.Blue || F6_2.BackColor == Color.Green)
                F6_2.BackColor = SystemColors.Control;

        }

        private void A1_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (A1_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(12);
                    A1_2.BackColor = SystemColors.Control;
                }
                else if (A1_2.BackColor == Color.Red || A1_2.BackColor == Color.Blue || A1_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (A1_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(11);
                    if (atck == 1)
                        A1_2.BackColor = Color.Yellow;
                }

            }

        }
        private void A2_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (A2_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(12);
                    A2_2.BackColor = SystemColors.Control;
                }
                else if (A2_2.BackColor == Color.Red || A2_2.BackColor == Color.Blue || A2_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if(A2_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(12);
                    if (atck == 1)
                        A2_2.BackColor = Color.Yellow;
                }

            }
        }

        private void estadoTableroLbl_Click(object sender, EventArgs e)
        {

        }

        private void preg_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                {

                    if (ganadas.Checked)
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
                        string mensaje = "5/" + nombresBox.Text;
                        // Enviamos al servidor el nombre tecleado
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);

                    }


                }
            }
            catch (Exception)
            {
                MessageBox.Show("ERROR: Compruebe que está conectado al servidor.");
            }

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void InvitarBoton_Click(object sender, EventArgs e)
        {
            //Validar los datos
            int n = this.conectados.SelectedCells.Count;

            if (n != 1)
            {
                MessageBox.Show("Numero de jugadores inválido.\n (Maximo 2 Jugadores)");
                return;
            }

            //Generar string de invitados
            n = this.conectados.Rows.Count;
            string oponente = "";
            for (int i = 0; i < n; i++)
            {
                if (conectados.Rows[i].Cells["Usuario"].Selected == true)
                    oponente = conectados.Rows[i].Cells["Usuario"].Value.ToString();
            }
            if (oponente != Estado.Text)
            {
                //Mensaje hacia el servidor: 10/string de invitados
                string mensaje = "6/" + oponente;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                this.conectados.ClearSelection();
            }
        }

        private void AceptarBtn_Click(object sender, EventArgs e)
        {
            int n = this.invitaciones.SelectedCells.Count;

            if (n != 1)
            {
                MessageBox.Show("Numero de jugadores inválido.\n (Maximo 2 Jugadores)");
                return;
            }

            //Generar string de invitados
            n = this.invitaciones.Rows.Count;
            string oponente = "";
            for (int i = 0; i < n; i++)
            {
                if (this.invitaciones.Rows[i].Cells["User"].Selected == true && this.invitaciones.Rows[i].Cells["User"].Value.ToString() != this.usuarioInicio.Text)
                    oponente = this.invitaciones.Rows[i].Cells["User"].Value.ToString();
            }


            //Mensaje hacia el servidor: 10/string de invitados
            string mensaje = "7/" + oponente;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            this.invitaciones.ClearSelection();


        }
        private void msg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (estadoPartidaLbl.Text == "1" || estadoPartidaLbl.Text == "2" || estadoPartidaLbl.Text == "0")
            {
                if (this.textBox1.Text != "")
                {
                    string mensaje = "8/" + textBox1.Text;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                }
                this.textBox1.Clear(); 

            }
            
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void conectados_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void connect_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            //IPAddress direc = IPAddress.Parse("10.4.119.5");
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 6060);
            //IPEndPoint ipep = new IPEndPoint(direc, 50075);


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

        private void desc_Click(object sender, EventArgs e)
        {
            string mensaje = "0/" + this.textBox1.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void atacarLbl_Click(object sender, EventArgs e)
        {

        }

        private void A3_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (A3_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(13);
                    A3_2.BackColor = SystemColors.Control;
                }
                else if (A3_2.BackColor == Color.Red || A3_2.BackColor == Color.Blue || A3_2.BackColor == Color.Black)
        {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
        else if (A3_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(13);
                    if (atck == 1)
                        A3_2.BackColor = Color.Yellow;
                }

            }
        }

        private void A4_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (A4_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(14);
                    A4_2.BackColor = SystemColors.Control;
                }
                else if (A4_2.BackColor == Color.Red || A4_2.BackColor == Color.Blue || A4_2.BackColor == Color.Black)
        {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
        else if (A4_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(14);
                    if (atck == 1)
                        A4_2.BackColor = Color.Yellow;
                }

            }
        }

        private void A5_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (A5_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(15);
                    A5_2.BackColor = SystemColors.Control;
                }
                else if (A5_2.BackColor == Color.Red || A5_2.BackColor == Color.Blue || A5_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
        else if (A5_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(15);
                    if (atck == 1)
                        A5_2.BackColor = Color.Yellow;
                }

            }
        }

        private void A6_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (A6_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(16);
                    A6_2.BackColor = SystemColors.Control;
                }
                else if (A6_2.BackColor == Color.Red || A6_2.BackColor == Color.Blue || A6_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (A6_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(16);
                    if (atck == 1)
                        A6_2.BackColor = Color.Yellow;
                }

            }
        }

        private void B1_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (B1_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(21);
                    B1_2.BackColor = SystemColors.Control;
                }
                else if (B1_2.BackColor == Color.Red || B1_2.BackColor == Color.Blue || B1_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
               else if (B1_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(21);
                    if (atck == 1)
                        B1_2.BackColor = Color.Yellow;
                }

            }
        }

        private void B2_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (B2_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(22);
                    B2_2.BackColor = SystemColors.Control;
                }
                else if (B2_2.BackColor == Color.Red || B2_2.BackColor == Color.Blue || B2_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (B2_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(22);
                    if (atck == 1)
                        B2_2.BackColor = Color.Yellow;
                }

            }
        }

        private void B3_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (B3_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(23);
                    B3_2.BackColor = SystemColors.Control;
                }
                else if (B3_2.BackColor == Color.Red || B3_2.BackColor == Color.Blue || B3_2.BackColor == Color.Black)
               {
                    MessageBox.Show("No puedes seleccionar esta casilla");
               }
               else if (B3_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(23);
                    if (atck == 1)
                        B3_2.BackColor = Color.Yellow;
                }

            }
        }

        private void B4_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (B4_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(24);
                    B4_2.BackColor = SystemColors.Control;
                }
                else if (B4_2.BackColor == Color.Red || B4_2.BackColor == Color.Blue || B4_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (B4_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(24);
                    if (atck == 1)
                        B4_2.BackColor = Color.Yellow;
                }

            }
        }

        private void B5_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (B5_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(25);
                    B5_2.BackColor = SystemColors.Control;
                }
                else if (B5_2.BackColor == Color.Red || B5_2.BackColor == Color.Blue || B5_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (B5_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(25);
                    if (atck == 1)
                        B5_2.BackColor = Color.Yellow;
                }

            }
        }

        private void B6_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (B6_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(26);
                    B6_2.BackColor = SystemColors.Control;
                }
                else if (B6_2.BackColor == Color.Red || B6_2.BackColor == Color.Blue || B6_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (B6_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(26);
                    if (atck == 1)
                        B6_2.BackColor = Color.Yellow;
                }

            }
        }

        private void C1_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (C1_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(31);
                    C1_2.BackColor = SystemColors.Control;
                }
                else if (C1_2.BackColor == Color.Red || C1_2.BackColor == Color.Blue || C1_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (C1_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(31);
                    if (atck == 1)
                        C1_2.BackColor = Color.Yellow;
                }

            }
        }

        private void C2_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (C2_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(32);
                    C2_2.BackColor = SystemColors.Control;
                }
                else if (C2_2.BackColor == Color.Red || C2_2.BackColor == Color.Blue || C2_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (C2_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(32);
                    if (atck == 1)
                        C2_2.BackColor = Color.Yellow;
                }

            }
        }

        private void C3_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (C3_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(33);
                    C3_2.BackColor = SystemColors.Control;
                }
                else if (C3_2.BackColor == Color.Red || C3_2.BackColor == Color.Blue || C3_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (C3_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(33);
                    if (atck == 1)
                        C3_2.BackColor = Color.Yellow;
                }

            }
        }

        private void C4_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (C4_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(34);
                    C4_2.BackColor = SystemColors.Control;
                }
                else if (C4_2.BackColor == Color.Red || C4_2.BackColor == Color.Blue || C4_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (C4_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(34);
                    if (atck == 1)
                        C4_2.BackColor = Color.Yellow;
                }

            }
        }

        private void C5_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (C5_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(35);
                    C5_2.BackColor = SystemColors.Control;
                }
                else if (C5_2.BackColor == Color.Red || C5_2.BackColor == Color.Blue || C5_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (C5_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(35);
                    if (atck == 1)
                        C5_2.BackColor = Color.Yellow;
                }

            }
        }

        private void C6_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (C6_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(36);
                    C6_2.BackColor = SystemColors.Control;
                }
                else if (C6_2.BackColor == Color.Red || C6_2.BackColor == Color.Blue || C6_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (C6_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(36);
                    if (atck == 1)
                        C6_2.BackColor = Color.Yellow;
                }

            }
        }

        private void D1_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (D1_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(41);
                    D1_2.BackColor = SystemColors.Control;
                }
                else if (D1_2.BackColor == Color.Red || D1_2.BackColor == Color.Blue || D1_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (D1_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(41);
                    if (atck == 1)
                        D1_2.BackColor = Color.Yellow;
                }

            }
        }

        private void D2_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (D2_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(42);
                    D2_2.BackColor = SystemColors.Control;
                }
                else if (D2_2.BackColor == Color.Red || D2_2.BackColor == Color.Blue || D2_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (D2_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(42);
                    if (atck == 1)
                        D2_2.BackColor = Color.Yellow;
                }

            }
        }

        private void D3_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (D3_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(43);
                    D3_2.BackColor = SystemColors.Control;
                }
                else if (D3_2.BackColor == Color.Red || D3_2.BackColor == Color.Blue || D3_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (D3_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(43);
                    if (atck == 1)
                        D3_2.BackColor = Color.Yellow;
                }

            }
        }

        private void D4_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (D4_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(44);
                    D4_2.BackColor = SystemColors.Control;
                }
                else if (D4_2.BackColor == Color.Red || D4_2.BackColor == Color.Blue || D4_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (D4_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(44);
                    if (atck == 1)
                        D4_2.BackColor = Color.Yellow;
                }

            }
        }

        private void D5_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (D5_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(45);
                    D5_2.BackColor = SystemColors.Control;
                }
                else if (D5_2.BackColor == Color.Red || D5_2.BackColor == Color.Blue || D5_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (D5_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(45);
                    if (atck == 1)
                        D5_2.BackColor = Color.Yellow;
                }

            }
        }

        private void D6_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (D6_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(46);
                    D6_2.BackColor = SystemColors.Control;
                }
                else if (D6_2.BackColor == Color.Red || D6_2.BackColor == Color.Blue || D6_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (D6_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(46);
                    if (atck == 1)
                        D6_2.BackColor = Color.Yellow;
                }

            }
        }

        private void E1_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (E1_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(51);
                    E1_2.BackColor = SystemColors.Control;
                }
                else if (E1_2.BackColor == Color.Red || E1_2.BackColor == Color.Blue || E1_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (E1_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(51);
                    if (atck == 1)
                        E1_2.BackColor = Color.Yellow;
                }

            }
        }

        private void E2_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (E2_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(52);
                    E2_2.BackColor = SystemColors.Control;
                }
                else if (E2_2.BackColor == Color.Red || E2_2.BackColor == Color.Blue || E2_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (E2_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(52);
                    if (atck == 1)
                        E2_2.BackColor = Color.Yellow;
                }

            }
        }

        private void E3_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (E3_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(53);
                    E3_2.BackColor = SystemColors.Control;
                }
                else if (E3_2.BackColor == Color.Red || E3_2.BackColor == Color.Blue || E3_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (E3_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(53);
                    if (atck == 1)
                        E3_2.BackColor = Color.Yellow;
                }

            }
        }

        private void E4_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (E4_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(54);
                    E4_2.BackColor = SystemColors.Control;
                }
                else if (E4_2.BackColor == Color.Red || E4_2.BackColor == Color.Blue || E4_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (E4_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(54);
                    if (atck == 1)
                        E4_2.BackColor = Color.Yellow;
                }

            }
        }

        private void E5_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (E5_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(55);
                    E5_2.BackColor = SystemColors.Control;
                }
                else if (E5_2.BackColor == Color.Red || E5_2.BackColor == Color.Blue || E5_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (E5_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(55);
                    if (atck == 1)
                        E5_2.BackColor = Color.Yellow;
                }

            }
        }

        private void E6_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (E6_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(56);
                    E6_2.BackColor = SystemColors.Control;
                }
                else if (E6_2.BackColor == Color.Red || E6_2.BackColor == Color.Blue || E6_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (E6_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(56);
                    if (atck == 1)
                        E6_2.BackColor = Color.Yellow;
                }

            }
        }

        private void F1_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (F1_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(61);
                    F1_2.BackColor = SystemColors.Control;
                }
                else if (F1_2.BackColor == Color.Red || F1_2.BackColor == Color.Blue || F1_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (F1_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(61);
                    if (atck == 1)
                        F1_2.BackColor = Color.Yellow;
                }

            }
        }

        private void F2_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (F2_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(62);
                    F2_2.BackColor = SystemColors.Control;
                }
                else if (F2_2.BackColor == Color.Red || F2_2.BackColor == Color.Blue || F2_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (F2_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(62);
                    if (atck == 1)
                        F2_2.BackColor = Color.Yellow;
                }

            }
        }

        private void F3_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (F3_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(63);
                    F3_2.BackColor = SystemColors.Control;
                }
                else if (F3_2.BackColor == Color.Red || F3_2.BackColor == Color.Blue || F3_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (F3_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(63);
                    if (atck == 1)
                        F3_2.BackColor = Color.Yellow;
                }

            }
        }

        private void F4_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (F4_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(64);
                    F4_2.BackColor = SystemColors.Control;
                }
                else if (F4_2.BackColor == Color.Red || F4_2.BackColor == Color.Blue || F4_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (F4_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(64);
                    if (atck == 1)
                        F4_2.BackColor = Color.Yellow;
                }

            }
        }

        private void F5_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (F5_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(65);
                    F5_2.BackColor = SystemColors.Control;
                }
                else if (F5_2.BackColor == Color.Red || F5_2.BackColor == Color.Blue || F5_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (F5_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(65);
                    if (atck == 1)
                        F5_2.BackColor = Color.Yellow;
                }

            }
        }

        private void F6_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1)
            {
                if (F6_2.BackColor == Color.Yellow)
                {
                    eliminarSeleccion2(66);
                    F6_2.BackColor = SystemColors.Control;
                }
                else if (F6_2.BackColor == Color.Red || F6_2.BackColor == Color.Blue || F6_2.BackColor == Color.Black)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else if (F6_2.BackColor == SystemColors.Control)
                {
                    int atck = ataque(66);
                    if (atck == 1)
                        F6_2.BackColor = Color.Yellow;
                }

            }
        }

        private void notificacionesLbl_Click(object sender, EventArgs e)
        {

        }

        private void rendirse_Click(object sender, EventArgs e)
        {
            string mensaje = "14/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
    }
}
