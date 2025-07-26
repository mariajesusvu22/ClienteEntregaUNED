// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Formulario: FormPrincipalCliente.cs
// Estudiante: María Jesús Venegas Ugalde
// Descripción: Formulario principal del cliente para
// realizar pedidos y consultarlos.
// ========================================================

using Servidor.Entidad;          // Para usar Cliente, Articulo, DetallePedido
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ClienteEntrega
{
    public partial class FormPrincipalCliente : Form


    {
        // ============================================
        // Campo privado para guardar cliente logueado
        // ============================================
        private Cliente clienteLogueado;

        // =======================================================
        // Constructor que recibe al cliente autenticado
        // =======================================================
        public FormPrincipalCliente(Cliente cliente)
        {
            InitializeComponent();
            clienteLogueado = cliente; // Se guarda el cliente para uso en el formulario
        }

        // =======================================================
        // Evento que se ejecuta al cargar el formulario
        // Carga los artículos disponibles y muestra el nombre del cliente
        // =======================================================
        private void FormPrincipalCliente_Load(object sender, EventArgs e)
        {
            try
            {
                // Habilitar generación automática de columnas en el DataGridView
                dgvDetallesArticulo.AutoGenerateColumns = true;

                // Mostrar nombre del cliente en la etiqueta
                lblBienvenida.Text = "Bienvenido: " + clienteLogueado.Nombre + " " + clienteLogueado.PrimerApellido + " " + clienteLogueado.SegundoApellido;

                // Obtener lista de artículos activos desde el servidor
                List<Articulo> articulos = ClienteTCP.ObtenerArticulosDisponibles();


                cboArticulo.DataSource = null;           // Limpia cualquier fuente anterior
                cboArticulo.DataSource = articulos;      // Asigna la lista como origen
                cboArticulo.DisplayMember = "Nombre";    // Muestra el nombre del artículo
                cboArticulo.ValueMember = "Id";          // Internamente maneja el ID
                cboArticulo.DropDownStyle = ComboBoxStyle.DropDownList; // Impide escritura manual

                // Opcional: seleccionar el primer artículo si hay al menos uno
                if (cboArticulo.Items.Count > 0)
                {
                    cboArticulo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Muestra error si algo falla
                MessageBox.Show("Error al cargar artículos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =======================================================
        // Muestra el detalle completo del artículo seleccionado
        // en un DataGridView independiente
        // =======================================================
        private void MostrarDetalleArticulo(int id)
        {
            try
            {
                // Consulta el artículo específico al servidor
                Articulo articulo = ClienteTCP.ConsultarArticuloPorId(id);


                // Si lo encuentra
                if (articulo != null)
                {
                    // Se crea una lista de un solo artículo para asignar al DataGridView
                    List<Articulo> detalle = new List<Articulo> { articulo };

                    // Limpia columnas existentes antes de asignar nuevo DataSource
                    dgvDetallesArticulo.Columns.Clear();

                    dgvDetallesArticulo.DataSource = detalle.Select(a => new
                    {
                        ID = a.Id,
                        Nombre = a.Nombre,
                        Tipo = a.TipoArticulo.Nombre, // Así mostramos el nombre del tipo
                        Disponible = a.Inventario,
                        Valor = a.Valor
                    }).ToList();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // =======================================================
        // Evento cuando el usuario selecciona un artículo
        // Actualiza el inventario disponible en pantalla
        // =======================================================
        private void cboArticulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboArticulo.SelectedItem != null)
            {
                Articulo seleccionado = (Articulo)cboArticulo.SelectedItem;
                lblDisponible.Text = seleccionado.Inventario.ToString();

            // Mostrar detalle en dgvDetallesArticulo
            MostrarDetalleArticulo(seleccionado.Id);
            }
        }

        // =======================================================
        // Evento al hacer clic en "Realizar Pedido"
        // Valida datos, descuenta inventario y envía al servidor
        // =======================================================
        private void btnRealizarPedido_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que se haya seleccionado un artículo
                if (cboArticulo.SelectedItem == null)
                    throw new Exception("Debe seleccionar un artículo.");

                // Validar que la cantidad sea número y mayor que 0
                if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
                    throw new Exception("La cantidad debe ser un número mayor a 0.");

                // Obtener artículo seleccionado
                Articulo art = (Articulo)cboArticulo.SelectedItem;

                // Validar que haya suficiente inventario
                if (cantidad > art.Inventario)
                    throw new Exception("No hay suficiente inventario disponible.");

                // Validar que se haya ingresado una dirección
                if (string.IsNullOrWhiteSpace(txtDireccion.Text))
                    throw new Exception("Debe ingresar la dirección para el pedido.");

                // Validar que la fecha del pedido no sea anterior a hoy
                if (dtpFecha.Value.Date < DateTime.Today)
                    throw new Exception("La fecha del pedido no puede ser anterior a hoy.");


                // Obtener valores faltantes
                string direccion = txtDireccion.Text.Trim();
                DateTime fecha = dtpFecha.Value;

                // Enviar al servidor usando TCP y obtener respuesta
                string respuesta = ClienteTCP.EnviarDetalleTexto(Convert.ToInt32(ClienteLogueado.Identificacion), direccion, art.Id, fecha, cantidad);

                if (respuesta.StartsWith("OK"))
                {
                    MessageBox.Show("Pedido realizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Actualizar inventario local
                    art.Inventario -= cantidad;
                    lblDisponible.Text = art.Inventario.ToString();
                    txtCantidad.Clear();
                }
                else
                {
                    MessageBox.Show("Error: " + respuesta, "Error del servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =======================================================
        // Evento al hacer clic en "Consultar mis pedidos"
        // Muestra los pedidos realizados por el cliente
        // =======================================================
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                // Consulta la lista de pedidos realizados por el cliente actual
                List<DetallePedido> detalles = ClienteTCP.ConsultarPedidos(clienteLogueado.Identificacion.ToString());

                if (detalles.Count == 0)
                {
                    MessageBox.Show("No tiene pedidos registrados.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvPedidos.DataSource = null; // Limpia el grid
                    return;
                }

                // Convertimos la lista a un formato anónimo legible para mostrar
                var datosParaMostrar = detalles.Select(d => new
                {
                    Pedido = d.NumeroPedido,
                    Articulo = d.Articulo.Nombre,
                    Fecha = d.FechaPedido.ToShortDateString(),
                    Cantidad = d.Cantidad
                }).ToList();

                // Mostramos en el grid
                dgvPedidos.DataSource = datosParaMostrar;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar pedidos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================================
        // Evento al hacer clic en "Consultar por ID"
        // Muestra un pedido específico según el número ingresado
        // ====================================================================
        private void btnConsultarPorId_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que se escribió algo en el cuadro de texto
                if (string.IsNullOrWhiteSpace(txtIdPedido.Text))
                {
                    MessageBox.Show("Debe digitar el número del pedido que desea consultar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvPedidos.DataSource = null;
                    return;
                }

                // Intentar convertir el texto a número entero (ID del pedido)
                if (!int.TryParse(txtIdPedido.Text.Trim(), out int idPedido))
                {
                    MessageBox.Show("El ID del pedido debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvPedidos.DataSource = null;
                    return;
                }

                // Enviar la solicitud al servidor para consultar el pedido con ese ID
                List<DetallePedido> resultado = ClienteTCP.ConsultarPedidoPorId(idPedido);

                // Verificar si la lista está vacía o vino como null desde el servidor
                if (resultado == null || resultado.Count == 0)
                {
                    MessageBox.Show("No se encontró ningún pedido con ese ID.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvPedidos.DataSource = null;
                    return;
                }

                // Crear una lista anónima para mostrar en el DataGridView
                var datosParaMostrar = resultado.Select(d => new
                {
                    Pedido = d.NumeroPedido,
                    Articulo = d.Articulo.Nombre,
                    Fecha = d.FechaPedido.ToShortDateString(),
                    Cantidad = d.Cantidad
                }).ToList();

                // Asignar los datos al DataGridView
                dgvPedidos.DataSource = datosParaMostrar;
            }
            catch (Exception ex)
            {
                // Mostrar cualquier error que ocurra
                MessageBox.Show("Error al consultar el pedido: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvPedidos.DataSource = null;
            }
        }


    }
}
