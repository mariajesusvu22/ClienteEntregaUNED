namespace ClienteEntrega
{
    partial class FormValidacion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnValidar = new Button();
            txtIdentificacion = new TextBox();
            lblIngresarCedula = new Label();
            lblError = new Label();
            lblEstadoConexion = new Label();
            btnDesconectar = new Button();
            btnConectar = new Button();
            SuspendLayout();
            // 
            // btnValidar
            // 
            btnValidar.Location = new Point(277, 144);
            btnValidar.Name = "btnValidar";
            btnValidar.Size = new Size(112, 34);
            btnValidar.TabIndex = 5;
            btnValidar.Text = "Validar";
            btnValidar.UseVisualStyleBackColor = true;
            btnValidar.Click += btnValidar_Click;
            // 
            // txtIdentificacion
            // 
            txtIdentificacion.Location = new Point(233, 98);
            txtIdentificacion.Name = "txtIdentificacion";
            txtIdentificacion.Size = new Size(195, 31);
            txtIdentificacion.TabIndex = 4;
            // 
            // lblIngresarCedula
            // 
            lblIngresarCedula.AutoSize = true;
            lblIngresarCedula.Location = new Point(21, 101);
            lblIngresarCedula.Name = "lblIngresarCedula";
            lblIngresarCedula.Size = new Size(206, 25);
            lblIngresarCedula.TabIndex = 3;
            lblIngresarCedula.Text = "Ingrese su identificación:";
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(21, 191);
            lblError.Name = "lblError";
            lblError.Size = new Size(0, 25);
            lblError.TabIndex = 6;
            lblError.Visible = false;
            // 
            // lblEstadoConexion
            // 
            lblEstadoConexion.AutoSize = true;
            lblEstadoConexion.Location = new Point(100, 53);
            lblEstadoConexion.Name = "lblEstadoConexion";
            lblEstadoConexion.Size = new Size(289, 25);
            lblEstadoConexion.TabIndex = 21;
            lblEstadoConexion.Text = "Estado de conexión: Desconectado";
            // 
            // btnDesconectar
            // 
            btnDesconectar.ForeColor = Color.Black;
            btnDesconectar.Location = new Point(245, 16);
            btnDesconectar.Name = "btnDesconectar";
            btnDesconectar.Size = new Size(150, 34);
            btnDesconectar.TabIndex = 20;
            btnDesconectar.Text = "Desconectar";
            btnDesconectar.UseVisualStyleBackColor = true;
            btnDesconectar.Click += btnDesconectar_Click;
            // 
            // btnConectar
            // 
            btnConectar.ForeColor = Color.Black;
            btnConectar.Location = new Point(89, 16);
            btnConectar.Name = "btnConectar";
            btnConectar.Size = new Size(150, 34);
            btnConectar.TabIndex = 19;
            btnConectar.Text = "Conectar";
            btnConectar.UseVisualStyleBackColor = true;
            btnConectar.Click += btnConectar_Click;
            // 
            // FormValidacion
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(480, 225);
            Controls.Add(lblEstadoConexion);
            Controls.Add(btnDesconectar);
            Controls.Add(btnConectar);
            Controls.Add(lblError);
            Controls.Add(btnValidar);
            Controls.Add(txtIdentificacion);
            Controls.Add(lblIngresarCedula);
            Name = "FormValidacion";
            Text = "FormValidacion";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnValidar;
        private TextBox txtIdentificacion;
        private Label lblIngresarCedula;
        private Label lblError;
        private Label lblEstadoConexion;
        private Button btnDesconectar;
        private Button btnConectar;
    }
}