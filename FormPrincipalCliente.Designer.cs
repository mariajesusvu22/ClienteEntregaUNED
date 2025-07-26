namespace ClienteEntrega
{
    partial class FormPrincipalCliente

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
            lblBienvenida = new Label();
            cboArticulo = new ComboBox();
            txtCantidad = new TextBox();
            dtpFecha = new DateTimePicker();
            btnRealizarPedido = new Button();
            lblArticulo = new Label();
            lblCantidad = new Label();
            lblFecha = new Label();
            btnConsultar = new Button();
            lblDireccion = new Label();
            txtDireccion = new TextBox();
            dgvPedidos = new DataGridView();
            lblInventario = new Label();
            lblDisponible = new Label();
            dgvDetallesArticulo = new DataGridView();
            lblIdPedidoConsultar = new Label();
            txtIdPedido = new TextBox();
            btnConsultarPorId = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvPedidos).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvDetallesArticulo).BeginInit();
            SuspendLayout();
            // 
            // lblBienvenida
            // 
            lblBienvenida.AutoSize = true;
            lblBienvenida.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBienvenida.Location = new Point(13, 24);
            lblBienvenida.Name = "lblBienvenida";
            lblBienvenida.Size = new Size(108, 25);
            lblBienvenida.TabIndex = 0;
            lblBienvenida.Text = "Bienvenido";
            // 
            // cboArticulo
            // 
            cboArticulo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboArticulo.FormattingEnabled = true;
            cboArticulo.Location = new Point(171, 145);
            cboArticulo.Name = "cboArticulo";
            cboArticulo.Size = new Size(172, 33);
            cboArticulo.TabIndex = 1;
            cboArticulo.SelectedIndexChanged += cboArticulo_SelectedIndexChanged;
            // 
            // txtCantidad
            // 
            txtCantidad.Location = new Point(171, 203);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.Size = new Size(172, 31);
            txtCantidad.TabIndex = 2;
            // 
            // dtpFecha
            // 
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Location = new Point(171, 90);
            dtpFecha.Name = "dtpFecha";
            dtpFecha.Size = new Size(172, 31);
            dtpFecha.TabIndex = 3;
            // 
            // btnRealizarPedido
            // 
            btnRealizarPedido.Location = new Point(181, 305);
            btnRealizarPedido.Name = "btnRealizarPedido";
            btnRealizarPedido.Size = new Size(150, 34);
            btnRealizarPedido.TabIndex = 4;
            btnRealizarPedido.Text = "Realizar pedido";
            btnRealizarPedido.UseVisualStyleBackColor = true;
            btnRealizarPedido.Click += btnRealizarPedido_Click;
            // 
            // lblArticulo
            // 
            lblArticulo.AutoSize = true;
            lblArticulo.Location = new Point(77, 153);
            lblArticulo.Name = "lblArticulo";
            lblArticulo.Size = new Size(77, 25);
            lblArticulo.TabIndex = 5;
            lblArticulo.Text = "Artículo:";
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.Location = new Point(77, 203);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(87, 25);
            lblCantidad.TabIndex = 6;
            lblCantidad.Text = "Cantidad:";
            // 
            // lblFecha
            // 
            lblFecha.AutoSize = true;
            lblFecha.Location = new Point(13, 95);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(152, 25);
            lblFecha.TabIndex = 7;
            lblFecha.Text = "Fecha del pedido:";
            // 
            // btnConsultar
            // 
            btnConsultar.Location = new Point(547, 480);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.Size = new Size(214, 34);
            btnConsultar.TabIndex = 10;
            btnConsultar.Text = "Consultar mis pedidos";
            btnConsultar.UseVisualStyleBackColor = true;
            btnConsultar.Click += btnConsultar_Click;
            // 
            // lblDireccion
            // 
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(77, 257);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(89, 25);
            lblDireccion.TabIndex = 12;
            lblDireccion.Text = "Dirección:";
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new Point(171, 257);
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(172, 31);
            txtDireccion.TabIndex = 11;
            // 
            // dgvPedidos
            // 
            dgvPedidos.AllowUserToAddRows = false;
            dgvPedidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvPedidos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPedidos.Location = new Point(365, 291);
            dgvPedidos.Name = "dgvPedidos";
            dgvPedidos.ReadOnly = true;
            dgvPedidos.RowHeadersWidth = 62;
            dgvPedidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPedidos.Size = new Size(564, 183);
            dgvPedidos.TabIndex = 13;
            // 
            // lblInventario
            // 
            lblInventario.AutoSize = true;
            lblInventario.Location = new Point(12, 349);
            lblInventario.Name = "lblInventario";
            lblInventario.Size = new Size(95, 25);
            lblInventario.TabIndex = 14;
            lblInventario.Text = "Inventario:";
            // 
            // lblDisponible
            // 
            lblDisponible.AutoSize = true;
            lblDisponible.Location = new Point(107, 349);
            lblDisponible.Name = "lblDisponible";
            lblDisponible.Size = new Size(0, 25);
            lblDisponible.TabIndex = 15;
            // 
            // dgvDetallesArticulo
            // 
            dgvDetallesArticulo.AllowUserToAddRows = false;
            dgvDetallesArticulo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDetallesArticulo.Location = new Point(365, 36);
            dgvDetallesArticulo.Name = "dgvDetallesArticulo";
            dgvDetallesArticulo.ReadOnly = true;
            dgvDetallesArticulo.RowHeadersWidth = 62;
            dgvDetallesArticulo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDetallesArticulo.Size = new Size(564, 192);
            dgvDetallesArticulo.TabIndex = 16;
            // 
            // lblIdPedidoConsultar
            // 
            lblIdPedidoConsultar.AutoSize = true;
            lblIdPedidoConsultar.Location = new Point(376, 251);
            lblIdPedidoConsultar.Name = "lblIdPedidoConsultar";
            lblIdPedidoConsultar.Size = new Size(268, 25);
            lblIdPedidoConsultar.TabIndex = 17;
            lblIdPedidoConsultar.Text = "Digite ID del pedido a consultar:";
            // 
            // txtIdPedido
            // 
            txtIdPedido.Location = new Point(641, 251);
            txtIdPedido.Name = "txtIdPedido";
            txtIdPedido.Size = new Size(150, 31);
            txtIdPedido.TabIndex = 18;
            // 
            // btnConsultarPorId
            // 
            btnConsultarPorId.Location = new Point(547, 520);
            btnConsultarPorId.Name = "btnConsultarPorId";
            btnConsultarPorId.Size = new Size(214, 34);
            btnConsultarPorId.TabIndex = 19;
            btnConsultarPorId.Text = "Consultar pedido por ID";
            btnConsultarPorId.UseVisualStyleBackColor = true;
            btnConsultarPorId.Click += btnConsultarPorId_Click;
            // 
            // FormPrincipalCliente
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(941, 564);
            Controls.Add(btnConsultarPorId);
            Controls.Add(txtIdPedido);
            Controls.Add(lblIdPedidoConsultar);
            Controls.Add(dgvDetallesArticulo);
            Controls.Add(lblDisponible);
            Controls.Add(lblInventario);
            Controls.Add(dgvPedidos);
            Controls.Add(lblDireccion);
            Controls.Add(txtDireccion);
            Controls.Add(btnConsultar);
            Controls.Add(lblFecha);
            Controls.Add(lblCantidad);
            Controls.Add(lblArticulo);
            Controls.Add(btnRealizarPedido);
            Controls.Add(dtpFecha);
            Controls.Add(txtCantidad);
            Controls.Add(cboArticulo);
            Controls.Add(lblBienvenida);
            Name = "FormPrincipalCliente";
            Text = "FormPrincipalCliente";
            Load += FormPrincipalCliente_Load;
            ((System.ComponentModel.ISupportInitialize)dgvPedidos).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvDetallesArticulo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblBienvenida;
        private ComboBox cboArticulo;
        private TextBox txtCantidad;
        private DateTimePicker dtpFecha;
        private Button btnRealizarPedido;
        private Label lblArticulo;
        private Label lblCantidad;
        private Label lblFecha;
        private Button btnConsultar;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private DataGridView dgvPedidos;
        private Label lblInventario;
        private Label lblDisponible;
        private DataGridView dgvDetallesArticulo;
        private Label lblIdPedidoConsultar;
        private TextBox txtIdPedido;
        private Button btnConsultarPorId;
    }
}