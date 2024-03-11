namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.nombre1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labbel2 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.usuario1 = new System.Windows.Forms.TextBox();
            this.contraseña1 = new System.Windows.Forms.TextBox();
            this.labbel3 = new System.Windows.Forms.Label();
            this.edad1 = new System.Windows.Forms.TextBox();
            this.labbel4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.contraseña2 = new System.Windows.Forms.TextBox();
            this.usuario2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.nombre2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.puntos = new System.Windows.Forms.RadioButton();
            this.partidasg = new System.Windows.Forms.RadioButton();
            this.ganadas = new System.Windows.Forms.RadioButton();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Nombre";
            this.label1.Click += new System.EventHandler(this.label2_Click);
            // 
            // nombre1
            // 
            this.nombre1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)), true);
            this.nombre1.ForeColor = System.Drawing.Color.Black;
            this.nombre1.Location = new System.Drawing.Point(22, 63);
            this.nombre1.Margin = new System.Windows.Forms.Padding(4);
            this.nombre1.Name = "nombre1";
            this.nombre1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.nombre1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.nombre1.Size = new System.Drawing.Size(154, 23);
            this.nombre1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(199, 53);
            this.button1.TabIndex = 4;
            this.button1.Text = "Conectar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(694, 223);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 31);
            this.button2.TabIndex = 5;
            this.button2.Text = "Enviar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.nombre2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.puntos);
            this.groupBox1.Controls.Add(this.partidasg);
            this.groupBox1.Controls.Add(this.ganadas);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(13, 74);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(962, 383);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.nombre1);
            this.groupBox3.Controls.Add(this.labbel2);
            this.groupBox3.Controls.Add(this.button5);
            this.groupBox3.Controls.Add(this.usuario1);
            this.groupBox3.Controls.Add(this.contraseña1);
            this.groupBox3.Controls.Add(this.labbel3);
            this.groupBox3.Controls.Add(this.edad1);
            this.groupBox3.Controls.Add(this.labbel4);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(7, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(239, 339);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Registrar";
            // 
            // labbel2
            // 
            this.labbel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labbel2.Location = new System.Drawing.Point(17, 98);
            this.labbel2.Name = "labbel2";
            this.labbel2.Size = new System.Drawing.Size(92, 25);
            this.labbel2.TabIndex = 12;
            this.labbel2.Text = "Usuario";
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(22, 287);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(114, 32);
            this.button5.TabIndex = 11;
            this.button5.Text = "Registrar";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // usuario1
            // 
            this.usuario1.Location = new System.Drawing.Point(22, 126);
            this.usuario1.Name = "usuario1";
            this.usuario1.Size = new System.Drawing.Size(154, 26);
            this.usuario1.TabIndex = 10;
            // 
            // contraseña1
            // 
            this.contraseña1.Location = new System.Drawing.Point(22, 186);
            this.contraseña1.Name = "contraseña1";
            this.contraseña1.Size = new System.Drawing.Size(154, 26);
            this.contraseña1.TabIndex = 11;
            this.contraseña1.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // labbel3
            // 
            this.labbel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labbel3.Location = new System.Drawing.Point(17, 158);
            this.labbel3.Name = "labbel3";
            this.labbel3.Size = new System.Drawing.Size(126, 25);
            this.labbel3.TabIndex = 13;
            this.labbel3.Text = "Contraseña";
            // 
            // edad1
            // 
            this.edad1.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.edad1.Location = new System.Drawing.Point(23, 251);
            this.edad1.Margin = new System.Windows.Forms.Padding(4);
            this.edad1.Name = "edad1";
            this.edad1.Size = new System.Drawing.Size(81, 26);
            this.edad1.TabIndex = 9;
            this.edad1.TextChanged += new System.EventHandler(this.alturaBox_TextChanged);
            // 
            // labbel4
            // 
            this.labbel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labbel4.Location = new System.Drawing.Point(17, 224);
            this.labbel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labbel4.Name = "labbel4";
            this.labbel4.Size = new System.Drawing.Size(92, 20);
            this.labbel4.TabIndex = 8;
            this.labbel4.Text = "Edad";
            this.labbel4.Click += new System.EventHandler(this.label3_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.contraseña2);
            this.groupBox2.Controls.Add(this.usuario2);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(302, 46);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(204, 254);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Iniciar session";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(31, 194);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(134, 31);
            this.button4.TabIndex = 14;
            this.button4.Text = "Iniciar session";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // contraseña2
            // 
            this.contraseña2.Location = new System.Drawing.Point(22, 152);
            this.contraseña2.Name = "contraseña2";
            this.contraseña2.Size = new System.Drawing.Size(147, 26);
            this.contraseña2.TabIndex = 16;
            // 
            // usuario2
            // 
            this.usuario2.Location = new System.Drawing.Point(22, 87);
            this.usuario2.Name = "usuario2";
            this.usuario2.Size = new System.Drawing.Size(147, 26);
            this.usuario2.TabIndex = 17;
            this.usuario2.TextChanged += new System.EventHandler(this.usuario2_TextChanged);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 27);
            this.label4.TabIndex = 12;
            this.label4.Text = "Contraseña";
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.Location = new System.Drawing.Point(17, 55);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(96, 29);
            this.label.TabIndex = 11;
            this.label.Text = "Usuario";
            this.label.Click += new System.EventHandler(this.label3_Click_1);
            // 
            // nombre2
            // 
            this.nombre2.Location = new System.Drawing.Point(720, 185);
            this.nombre2.Name = "nombre2";
            this.nombre2.Size = new System.Drawing.Size(100, 23);
            this.nombre2.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(607, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 28);
            this.label2.TabIndex = 15;
            this.label2.Text = "Consultas";
            this.label2.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // puntos
            // 
            this.puntos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.puntos.Location = new System.Drawing.Point(612, 107);
            this.puntos.Margin = new System.Windows.Forms.Padding(4);
            this.puntos.Name = "puntos";
            this.puntos.Size = new System.Drawing.Size(259, 32);
            this.puntos.TabIndex = 7;
            this.puntos.TabStop = true;
            this.puntos.Text = "Jugador con mas puntos";
            this.puntos.UseVisualStyleBackColor = true;
            // 
            // partidasg
            // 
            this.partidasg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.partidasg.Location = new System.Drawing.Point(612, 140);
            this.partidasg.Margin = new System.Windows.Forms.Padding(4);
            this.partidasg.Name = "partidasg";
            this.partidasg.Size = new System.Drawing.Size(259, 75);
            this.partidasg.TabIndex = 7;
            this.partidasg.TabStop = true;
            this.partidasg.Text = "Dime cuantas partidas ha jugado:";
            this.partidasg.UseVisualStyleBackColor = true;
            this.partidasg.CheckedChanged += new System.EventHandler(this.altura_CheckedChanged);
            // 
            // ganadas
            // 
            this.ganadas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ganadas.Location = new System.Drawing.Point(612, 61);
            this.ganadas.Margin = new System.Windows.Forms.Padding(4);
            this.ganadas.Name = "ganadas";
            this.ganadas.Size = new System.Drawing.Size(327, 38);
            this.ganadas.TabIndex = 8;
            this.ganadas.TabStop = true;
            this.ganadas.Text = "Jugador que ha ganado mas partidas";
            this.ganadas.UseVisualStyleBackColor = true;
            this.ganadas.CheckedChanged += new System.EventHandler(this.Bonito_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(13, 465);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(196, 65);
            this.button3.TabIndex = 10;
            this.button3.Text = "Desconectar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 692);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton puntos;
        private System.Windows.Forms.RadioButton ganadas;
        private System.Windows.Forms.RadioButton partidasg;
        private System.Windows.Forms.Label labbel4;
        private System.Windows.Forms.TextBox edad1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox contraseña1;
        private System.Windows.Forms.TextBox usuario1;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox nombre1;
        private System.Windows.Forms.Label labbel3;
        private System.Windows.Forms.Label labbel2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nombre2;
        private System.Windows.Forms.TextBox usuario2;
        private System.Windows.Forms.TextBox contraseña2;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

