// ========================================================
// UNED - II Cuatrimestre 2025 
// Proyecto 2: ENTREGAS S.A. – ServidorEntrega
// Formulario: FormPrincipalCliente.cs
// Estudiante: María Jesús Venegas Ugalde
// Fecha: 21/07/2025
// ========================================================

// Descripción: Formulario principal del cliente para realizar pedidos y consultarlos.

using Servidor.Entidad; // Para usar Cliente, Articulo, DetallePedido
using System; // Funcionalidades básicas
using System.Collections.Generic; // Para usar listas
using System.Net.Sockets; // Para comunicación por red (TCP)
using System.Windows.Forms; // Controles visuales

namespace ClienteEntrega
{
    public partial class FormPrincipalCliente : Form
    {
        // ============================================
        // Campo privado para guardar cliente logueado
        // ============================================
        private Cliente clienteLogueado; // Almacena la información del cliente que inició sesión

        // ==============================================
        // Constructor que recibe al cliente autenticado
        // ==============================================
        public FormPrincipalCliente(Cliente cliente)
        {
            InitializeComponent(); // Inicializa los componentes visuales del diseñador
            clienteLogueado = cliente; // Se guarda el cliente para uso en el resto del formulario
        }

        // =================================================================
        // Evento que se ejecuta al cargar el formulario
        // Carga los artículos disponibles y muestra el nombre del cliente
        // =================================================================
        private void FormPrincipalCliente_Load(object sender, EventArgs e)
        {
            try
            {
                // Permite que el DataGridView cree sus columnas a partir de la fuente de datos
                dgvDetallesArticulo.AutoGenerateColumns = true;

                // Muestra un mensaje de bienvenida personalizado con el nombre del cliente
                lblBienvenida.Text = "Bienvenido: " + clienteLogueado.Nombre + " " + clienteLogueado.PrimerApellido + " " + clienteLogueado.SegundoApellido;

                // Obtiene la lista de artículos disponibles desde el servidor a través de la red
                List<Articulo> articulos = ClienteTCP.ObtenerArticulosDisponibles();

                // Configuración del ComboBox de artículos
                cboArticulo.DataSource = null; // Limpia cualquier fuente de datos anterior para evitar errores
                cboArticulo.DataSource = articulos; // Asigna la lista de artículos como origen de datos
                cboArticulo.DisplayMember = "Nombre"; // Muestra el nombre del artículo en la lista desplegable
                cboArticulo.ValueMember = "Id"; // Usa el ID del artículo como valor interno
                cboArticulo.DropDownStyle = ComboBoxStyle.DropDownList; // Impide que el usuario escriba manualmente en el ComboBox

                // Si la lista de artículos no está vacía, selecciona el primer elemento
                if (cboArticulo.Items.Count > 0)
                {
                    cboArticulo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si falla la carga inicial
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
                // Consulta al servidor el artículo específico por su ID
                Articulo articulo = ClienteTCP.ConsultarArticuloPorId(id);

                // Si se encontró el artículo, lo muestra en la tabla
                if (articulo != null)
                {
                    // Se crea una lista que contiene solo el artículo encontrado
                    List<Articulo> detalle = new List<Articulo> { articulo };

                    // Limpia las columnas existentes para evitar que se acumulen
                    dgvDetallesArticulo.Columns.Clear();

                    // Se asigna al DataGridView una lista de objetos anónimos con el formato deseado
                    dgvDetallesArticulo.DataSource = detalle.Select(a => new
                    {
                        ID = a.Id,
                        Nombre = a.Nombre,
                        Tipo = a.TipoArticulo.Nombre, // Accede al nombre del tipo de artículo
                        Disponible = a.Inventario,
                        Valor = a.Valor
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle del artículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =================================================
        // Evento cuando el usuario selecciona un artículo
        // Actualiza el inventario disponible en pantalla
        // =================================================
        private void cboArticulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Valida que haya un artículo seleccionado
            if (cboArticulo.SelectedItem != null)
            {
                // Obtiene el objeto Articulo completo del ítem seleccionado
                Articulo seleccionado = (Articulo)cboArticulo.SelectedItem;
                // Muestra la cantidad de inventario disponible en una etiqueta
                lblDisponible.Text = seleccionado.Inventario.ToString();

                // Llama al método para mostrar todos los detalles del artículo en su tabla
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
                // Validaciones de los campos del formulario
                if (cboArticulo.SelectedItem == null)
                    throw new Exception("Debe seleccionar un artículo.");
                if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
                    throw new Exception("La cantidad debe ser un número mayor a 0.");

                // Obtiene el artículo seleccionado para las validaciones
                Articulo art = (Articulo)cboArticulo.SelectedItem;

                if (cantidad > art.Inventario)
                    throw new Exception("No hay suficiente inventario disponible.");
                if (string.IsNullOrWhiteSpace(txtDireccion.Text))
                    throw new Exception("Debe ingresar la dirección para el pedido.");
                if (dtpFecha.Value.Date < DateTime.Today)
                    throw new Exception("La fecha del pedido no puede ser anterior a hoy.");

                // Obtiene los datos restantes del formulario
                string direccion = txtDireccion.Text.Trim();
                DateTime fecha = dtpFecha.Value;

                // Envía los datos del pedido al servidor y espera una respuesta
                string respuesta = ClienteTCP.EnviarDetalleTexto(Convert.ToInt32(clienteLogueado.Identificacion), direccion, art.Id, fecha, cantidad);

                // Si la respuesta del servidor indica éxito
                if (respuesta.StartsWith("OK"))
                {
                    MessageBox.Show("Pedido realizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Actualiza el inventario en la vista del cliente
                    art.Inventario -= cantidad;
                    lblDisponible.Text = art.Inventario.ToString();
                    txtCantidad.Clear(); // Limpia el campo de cantidad
                }
                else // Si el servidor devolvió un error
                {
                    MessageBox.Show("Error: " + respuesta, "Error del servidor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Muestra errores de validación o de otro tipo
                MessageBox.Show("Error: " + ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =================================================
        // Evento al hacer clic en "Consultar mis pedidos"
        // Muestra los pedidos realizados por el cliente
        // =================================================
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                // Solicita al servidor la lista de pedidos del cliente logueado
                List<DetallePedido> detalles = ClienteTCP.ConsultarPedidos(clienteLogueado.Identificacion.ToString());

                // Si no hay pedidos, muestra un mensaje informativo
                if (detalles.Count == 0)
                {
                    MessageBox.Show("No tiene pedidos registrados.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvPedidos.DataSource = null; // Limpia la tabla
                    return;
                }

                // Convierte la lista a un formato anónimo para mostrar solo las columnas deseadas
                var datosParaMostrar = detalles.Select(d => new
                {
                    Pedido = d.NumeroPedido,
                    Articulo = d.Articulo.Nombre,
                    Fecha = d.FechaPedido.ToShortDateString(),
                    Cantidad = d.Cantidad
                }).ToList();

                // Asigna la lista formateada a la tabla para mostrarla
                dgvPedidos.DataSource = datosParaMostrar;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar sus pedidos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================================
        // Evento al hacer clic en "Consultar por ID"
        // Muestra un pedido específico según el número ingresado
        // ========================================================
        private void btnConsultarPorId_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida que el campo de texto para el ID no esté vacío
                if (string.IsNullOrWhiteSpace(txtIdPedido.Text))
                {
                    MessageBox.Show("Debe digitar el número del pedido que desea consultar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvPedidos.DataSource = null;
                    return;
                }

                // Valida que el texto ingresado sea un número válido
                if (!int.TryParse(txtIdPedido.Text.Trim(), out int idPedido))
                {
                    MessageBox.Show("El ID del pedido debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvPedidos.DataSource = null;
                    return;
                }

                // Envía la solicitud al servidor para consultar un pedido por su ID
                List<DetallePedido> resultado = ClienteTCP.ConsultarPedidoPorId(idPedido);

                // Si el servidor no devuelve nada, informa al usuario
                if (resultado == null || resultado.Count == 0)
                {
                    MessageBox.Show("No se encontró ningún pedido con ese ID.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvPedidos.DataSource = null;
                    return;
                }

                // Formatea la lista de resultados para mostrarla en la tabla
                var datosParaMostrar = resultado.Select(d => new
                {
                    Pedido = d.NumeroPedido,
                    Articulo = d.Articulo.Nombre,
                    Fecha = d.FechaPedido.ToShortDateString(),
                    Cantidad = d.Cantidad
                }).ToList();

                // Asigna los datos a la tabla
                dgvPedidos.DataSource = datosParaMostrar;
            }
            catch (Exception ex)
            {
                // Muestra cualquier error que ocurra durante la consulta
                MessageBox.Show("Error al consultar el pedido: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvPedidos.DataSource = null;
            }
        }
    }
}
