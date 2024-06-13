using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private delegate void DelegadoParaEscribirTexto(string text);

        private void mensagePrinc(string texto)
        {
            seleccion1Lbl.Text = texto; // TIENES QUE CAMBIAR ESTO SI O SIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII
        }
        private void seleccion1(string texto){
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
        private void adversario(string texto)
        {
            adversarioLbl.Text = texto;
        }
        private void atacar(string texto)
        {
            atacarLbl.Text = texto;
        }

        private void AtenderServidor() {

            byte[] msg2 = new byte[80];
            server.Receice(msg2);
            string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
            int codigo = Convert.ToInt32(trozos[0]);
            mensaje = trozos[1].Split('\0');

            switch (codigo)
            {
                case 0: //Desconexion
                    
                    break;
                case 1: //Inicio de sesion
                    break;
                case 2://
                    break;
                case 3://Notificaciones message box
                    break;
                case 4://Invitaciones
                    break;
                case 5://Grid de usuarios conectados
                    break;
                case 6:
                    break;
                case 7://Iniciar partida
                    this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "0" });
                    this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "0" });
                    this.Invoke(new DelegadoParaEscribirTexto(adversario), new object[] { trozos[1] });
                    //Puedo hacer que de estar en invisible el lbl de adversario junto a su caja y todo este oculto hasta ahora
                    MessageBox.Show("Partida iniciada");
                    this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 3 posiciones para tus botes" });
                    break;
                case 8: //Aceptacion de botes y paso al siguiente estado bergantin (se repetira para el 2ndo bergantin)
                    posicionesConfirmadas();
                    this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "1" });
                    this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 3 posiciones en linea (juntos) para tu bergantín" });
                    break;
                case 9: //Aceptacion del 2ndo bergantin y cambio al estado del destructor
                    posicionesConfirmadas();
                    this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "2" });
                    this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 4 posiciones en linea (juntos) para tu destructor" });
                    break;
                case 10: //Aceptacion del destructor y cambio al estado del portaaviones
                    posicionesConfirmadas();
                    this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "3" });
                    this.Invoke(new DelegadoParaEscribirTexto(mensagePrinc), new object[] { "Escoge 5 posiciones en linea (juntos) para tu portaaviones" });
                    break;
                case 11: //Aceptacion del portaaciones y bloqueo de mi tablero
                    posicionesConfirmadas();
                    this.Invoke(new DelegadoParaEscribirTexto(estTab), new object[] { "-1" });
                    break;
                case 12: //Estado de partida para poder atacar
                    this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "1" });
                    break;
                case 13: //Estado de partida esperar al turno del enemigo
                    this.Invoke(new DelegadoParaEscribirTexto(estPart), new object[] { "2" });
                    break;
            }
                
        }


        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void notificaciones_Click(object sender, EventArgs e)
        {

        }


        private void confirmar_Click(object sender, EventArgs e)
        {
            int estadoTablero = Convert.ToInt32(estadoLbl.Text);
            string mensaje;
            if (estadoTablero != -1){ //Estado de Seleccion de flota por estados
                int v1 = Convert.ToInt32(seleccion1Lbl.Text);
                int v2 = Convert.ToInt32(seleccion2Lbl.Text);
                int v3 = Convert.ToInt32(seleccion3Lbl.Text);
                int v4 = Convert.ToInt32(seleccion4Lbl.Text);
                int v5 = Convert.ToInt32(seleccion5Lbl.Text);

                if (estadoTablero == 0){//Estado de seleccion de los botes
                    if (v1 == -1 || v2 == -1 || v3 == -1) //Comprobamos que esten los 3 barcos seleccionados
                        MessageBox.Show("Has de seleccionar 3 posiciones para los botes");
                    else{
                        mensaje = "8/" + v1 + v2 + v3;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                else if(estadoTablero == 1){
                    if (v1 == -1 || v2 == -1 || v3 == -1) //Comprobamos que esten las 3 posiciones del bergantin seleccionados
                        MessageBox.Show("Has de seleccionar 3 posiciones en fila para el bergantín");
                    else{
                        mensaje = "9/" + v1 +","+ v2 +","+ v3;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                else if (estadoTablero == 2)
                {
                    if (v1 == -1 || v2 == -1 || v3 == -1 || v4 == -1) //Comprobamos que esten las 4 posiciones del destructor seleccionados
                        MessageBox.Show("Has de seleccionar 3 posiciones en fila para el bergantín");
                    else
                    {
                        mensaje = "10/" + v1 +","+ v2 +","+ v3 +","+ v4;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
                else if (estadoTablero == 3)
                {
                    if (v1 == -1 || v2 == -1 || v3 == -1 || v4 == -1 || v5 == -1) //Comprobamos que esten las 5 posiciones del portaaviones seleccionados
                        MessageBox.Show("Has de seleccionar 3 posiciones en fila para el bergantín");
                    else
                    {
                        mensaje = "10/" + v1 +","+ v2 +","+ v3 +","+ v4 +","+ v5;
                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                        server.Send(msg);
                    }
                }
            }




        }



        private void eliminarSeleccion(int pos){
            int restablecer = -1;
            int v1 = Convert.ToInt32(seleccion1Lbl.Text);
            int v2 = Convert.ToInt32(seleccion2Lbl.Text);
            int v3 = Convert.ToInt32(seleccion3Lbl.Text);
            int v4 = Convert.ToInt32(seleccion4Lbl.Text);
            int v5 = Convert.ToInt32(seleccion5Lbl.Text);

            if(v1 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion1), new object[] { restablecer.ToString() });
            if(v2 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion2), new object[] { restablecer.ToString() });
            if(v3 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion3), new object[] { restablecer.ToString() });
            if(v4 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion4), new object[] { restablecer.ToString() });
            if(v5 == pos)
                this.Invoke(new DelegadoParaEscribirTexto(seleccion5), new object[] { restablecer.ToString() });
        }

        private int seleccionado(int pos) {
            int v1 = Convert.ToInt32(seleccion1Lbl.Text);
            int v2 = Convert.ToInt32(seleccion2Lbl.Text);
            int v3 = Convert.ToInt32(seleccion3Lbl.Text);
            int v4 = Convert.ToInt32(seleccion4Lbl.Text);
            int v5 = Convert.ToInt32(seleccion5Lbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (v1 == -1) {
                this.Invoke(new DelegadoParaEscribirTexto(seleccion1), new object[] {pos.ToString()});
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
            if(estadoTablero == 2 || estadoTablero == 3)
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
        private int ataque(int pos)
        {
            int atck = Convert.ToInt32(ataqueLbl.Text);
            if (atck == -1)
            {
                this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { pos.ToString() });
                return 1;
            }
            MessageBox.Show("No puedes hacer mas de un ataque");
            return 0;
        }
        private void eliminarAtaque(){
            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void posicionesConfirmadas() //CAMBIA DE COLOR DEL CYAN AL VERDE LOS BARCOS CONFIRMADOS Y CAMBIA LOS VALORES A -1 DE LOS LBL'S
        {

            if (A1_1.BackColor == Color.Cyan)
                A1_1.BackColor == Color.Green;
            if (A2_1.BackColor == Color.Cyan)
                A2_1.BackColor == Color.Green;
            if (A3_1.BackColor == Color.Cyan)
                A3_1.BackColor == Color.Green;
            if (A4_1.BackColor == Color.Cyan)
                A4_1.BackColor == Color.Green;
            if (A5_1.BackColor == Color.Cyan)
                A5_1.BackColor == Color.Green;
            if (A6_1.BackColor == Color.Cyan)
                A6_1.BackColor == Color.Green;

            if (B1_1.BackColor == Color.Cyan)
                B1_1.BackColor == Color.Green;
            if (B2_1.BackColor == Color.Cyan)
                B2_1.BackColor == Color.Green;
            if (B3_1.BackColor == Color.Cyan)
                B3_1.BackColor == Color.Green;
            if (B4_1.BackColor == Color.Cyan)
                B4_1.BackColor == Color.Green;
            if (B5_1.BackColor == Color.Cyan)
                B5_1.BackColor == Color.Green;
            if (B6_1.BackColor == Color.Cyan)
                B6_1.BackColor == Color.Green;

            if (C1_1.BackColor == Color.Cyan)
                C1_1.BackColor == Color.Green;
            if (C2_1.BackColor == Color.Cyan)
                C2_1.BackColor == Color.Green;
            if (C3_1.BackColor == Color.Cyan)
                C3_1.BackColor == Color.Green;
            if (C4_1.BackColor == Color.Cyan)
                C4_1.BackColor == Color.Green;
            if (C5_1.BackColor == Color.Cyan)
                C5_1.BackColor == Color.Green;
            if (C6_1.BackColor == Color.Cyan)
                C6_1.BackColor == Color.Green;

            if (D1_1.BackColor == Color.Cyan)
                D1_1.BackColor == Color.Green;
            if (D2_1.BackColor == Color.Cyan)
                D2_1.BackColor == Color.Green;
            if (D2_1.BackColor == Color.Cyan)
                D2_1.BackColor == Color.Green;
            if (D2_1.BackColor == Color.Cyan)
                D2_1.BackColor == Color.Green;
            if (D2_1.BackColor == Color.Cyan)
                D2_1.BackColor == Color.Green;
            if (D2_1.BackColor == Color.Cyan)
                D2_1.BackColor == Color.Green;

            if (E1_1.BackColor == Color.Cyan)
                E1_1.BackColor == Color.Green;
            if (E2_1.BackColor == Color.Cyan)
                E2_1.BackColor == Color.Green;
            if (E3_1.BackColor == Color.Cyan)
                E3_1.BackColor == Color.Green;
            if (E4_1.BackColor == Color.Cyan)
                E4_1.BackColor == Color.Green;
            if (E5_1.BackColor == Color.Cyan)
                E5_1.BackColor == Color.Green;
            if (E6_1.BackColor == Color.Cyan)
                E6_1.BackColor == Color.Green;

            if (F1_1.BackColor == Color.Cyan)
                F1_1.BackColor == Color.Green;
            if (F2_1.BackColor == Color.Cyan)
                F2_1.BackColor == Color.Green;
            if (F3_1.BackColor == Color.Cyan)
                F3_1.BackColor == Color.Green;
            if (F4_1.BackColor == Color.Cyan)
                F4_1.BackColor == Color.Green;
            if (F5_1.BackColor == Color.Cyan)
                F5_1.BackColor == Color.Green;
            if (F6_1.BackColor == Color.Cyan)
                F6_1.BackColor == Color.Green;

            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void tocaAgua() //CAMBIA DE COLOR DEL Yellow AL Blue LOS ATAQUES QUE NO IMPACTAN
        {

            if (A1_1.BackColor == Color.Yellow)
                A1_1.BackColor == Color.Blue;
            if (A2_1.BackColor == Color.Yellow)
                A2_1.BackColor == Color.Blue;
            if (A3_1.BackColor == Color.Yellow)
                A3_1.BackColor == Color.Blue;
            if (A4_1.BackColor == Color.Yellow)
                A4_1.BackColor == Color.Blue;
            if (A5_1.BackColor == Color.Yellow)
                A5_1.BackColor == Color.Blue;
            if (A6_1.BackColor == Color.Yellow)
                A6_1.BackColor == Color.Blue;

            if (B1_1.BackColor == Color.Yellow)
                B1_1.BackColor == Color.Blue;
            if (B2_1.BackColor == Color.Yellow)
                B2_1.BackColor == Color.Blue;
            if (B3_1.BackColor == Color.Yellow)
                B3_1.BackColor == Color.Blue;
            if (B4_1.BackColor == Color.Yellow)
                B4_1.BackColor == Color.Blue;
            if (B5_1.BackColor == Color.Yellow)
                B5_1.BackColor == Color.Blue;
            if (B6_1.BackColor == Color.Yellow)
                B6_1.BackColor == Color.Blue;

            if (C1_1.BackColor == Color.Yellow)
                C1_1.BackColor == Color.Blue;
            if (C2_1.BackColor == Color.Yellow)
                C2_1.BackColor == Color.Blue;
            if (C3_1.BackColor == Color.Yellow)
                C3_1.BackColor == Color.Blue;
            if (C4_1.BackColor == Color.Yellow)
                C4_1.BackColor == Color.Blue;
            if (C5_1.BackColor == Color.Yellow)
                C5_1.BackColor == Color.Blue;
            if (C6_1.BackColor == Color.Yellow)
                C6_1.BackColor == Color.Blue;

            if (D1_1.BackColor == Color.Yellow)
                D1_1.BackColor == Color.Blue;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Blue;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Blue;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Blue;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Blue;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Blue;

            if (E1_1.BackColor == Color.Yellow)
                E1_1.BackColor == Color.Blue;
            if (E2_1.BackColor == Color.Yellow)
                E2_1.BackColor == Color.Blue;
            if (E3_1.BackColor == Color.Yellow)
                E3_1.BackColor == Color.Blue;
            if (E4_1.BackColor == Color.Yellow)
                E4_1.BackColor == Color.Blue;
            if (E5_1.BackColor == Color.Yellow)
                E5_1.BackColor == Color.Blue;
            if (E6_1.BackColor == Color.Yellow)
                E6_1.BackColor == Color.Blue;

            if (F1_1.BackColor == Color.Yellow)
                F1_1.BackColor == Color.Blue;
            if (F2_1.BackColor == Color.Yellow)
                F2_1.BackColor == Color.Blue;
            if (F3_1.BackColor == Color.Yellow)
                F3_1.BackColor == Color.Blue;
            if (F4_1.BackColor == Color.Yellow)
                F4_1.BackColor == Color.Blue;
            if (F5_1.BackColor == Color.Yellow)
                F5_1.BackColor == Color.Blue;
            if (F6_1.BackColor == Color.Yellow)
                F6_1.BackColor == Color.Blue;

            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void tocaBarco() //CAMBIA DE COLOR DEL Yellow AL Red LOS ATAQUES EXISTOSOS
        {

            if (A1_1.BackColor == Color.Yellow)
                A1_1.BackColor == Color.Red;
            if (A2_1.BackColor == Color.Yellow)
                A2_1.BackColor == Color.Red;
            if (A3_1.BackColor == Color.Yellow)
                A3_1.BackColor == Color.Red;
            if (A4_1.BackColor == Color.Yellow)
                A4_1.BackColor == Color.Red;
            if (A5_1.BackColor == Color.Yellow)
                A5_1.BackColor == Color.Red;
            if (A6_1.BackColor == Color.Yellow)
                A6_1.BackColor == Color.Red;

            if (B1_1.BackColor == Color.Yellow)
                B1_1.BackColor == Color.Red;
            if (B2_1.BackColor == Color.Yellow)
                B2_1.BackColor == Color.Red;
            if (B3_1.BackColor == Color.Yellow)
                B3_1.BackColor == Color.Red;
            if (B4_1.BackColor == Color.Yellow)
                B4_1.BackColor == Color.Red;
            if (B5_1.BackColor == Color.Yellow)
                B5_1.BackColor == Color.Red;
            if (B6_1.BackColor == Color.Yellow)
                B6_1.BackColor == Color.Red;

            if (C1_1.BackColor == Color.Yellow)
                C1_1.BackColor == Color.Red;
            if (C2_1.BackColor == Color.Yellow)
                C2_1.BackColor == Color.Red;
            if (C3_1.BackColor == Color.Yellow)
                C3_1.BackColor == Color.Red;
            if (C4_1.BackColor == Color.Yellow)
                C4_1.BackColor == Color.Red;
            if (C5_1.BackColor == Color.Yellow)
                C5_1.BackColor == Color.Red;
            if (C6_1.BackColor == Color.Yellow)
                C6_1.BackColor == Color.Red;

            if (D1_1.BackColor == Color.Yellow)
                D1_1.BackColor == Color.Red;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Red;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Red;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Red;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Red;
            if (D2_1.BackColor == Color.Yellow)
                D2_1.BackColor == Color.Red;

            if (E1_1.BackColor == Color.Yellow)
                E1_1.BackColor == Color.Red;
            if (E2_1.BackColor == Color.Yellow)
                E2_1.BackColor == Color.Red;
            if (E3_1.BackColor == Color.Yellow)
                E3_1.BackColor == Color.Red;
            if (E4_1.BackColor == Color.Yellow)
                E4_1.BackColor == Color.Red;
            if (E5_1.BackColor == Color.Yellow)
                E5_1.BackColor == Color.Red;
            if (E6_1.BackColor == Color.Yellow)
                E6_1.BackColor == Color.Red;

            if (F1_1.BackColor == Color.Yellow)
                F1_1.BackColor == Color.Red;
            if (F2_1.BackColor == Color.Yellow)
                F2_1.BackColor == Color.Red;
            if (F3_1.BackColor == Color.Yellow)
                F3_1.BackColor == Color.Red;
            if (F4_1.BackColor == Color.Yellow)
                F4_1.BackColor == Color.Red;
            if (F5_1.BackColor == Color.Yellow)
                F5_1.BackColor == Color.Red;
            if (F6_1.BackColor == Color.Yellow)
                F6_1.BackColor == Color.Red;

            this.Invoke(new DelegadoParaEscribirTexto(atacar), new object[] { "-1" });
        }
        private void A1_1_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            int estadoTablero = Convert.ToInt32(estadoTableroLbl.Text);
            if (estadoPartida != -1){
                if (estadoTalero == -1) { }
                else
                {
                    if (A1_1.BackColor == Color.Green)
                        MessageBox.Show("No puedes seleccionar esta casilla");

                    else if (A1_1.BackColor == Color.Cyan)
                    {
                        eliminarSeleccion(11);
                        A1_1.BackColor = ;//COLOR POR DEFECTO
                    }
                    else if (A1_1.BackColor == Color.Cyan)
                    { //COLOR POR DEFECTO
                        int sel = seleccionado(11);
                        if (sel == 1)
                            A1_1.BackColor = Color.Cyan; //HACEMOS LA SELECCION
                    }
                }
            }
        }


        private void A1_2_Click(object sender, EventArgs e)
        {
            int estadoPartida = Convert.ToInt32(estadoPartidaLbl.Text);
            if (estadoPartida == 1) { 
                if(A1_2.BackColor == Color.Yellow){

                }
                else if(A1_2.BackColor == Color.Red || A1_2.BackColor == Color.Blue || A1_2.BackColor == Color.Grey)
                {
                    MessageBox.Show("No puedes seleccionar esta casilla");
                }
                else{
                    int atck = ataque(11);
                    A1_2.BackColor = Color.Yellow;
                }

            }
        }
    }
}
