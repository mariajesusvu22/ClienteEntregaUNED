// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Formulario: FormValidacion.cs
// Estudiante: María Jesús Venegas Ugalde
// Fecha: 21/07/2025
// ========================================================

// Descripción: Permite validar el número de identificación del cliente antes de permitir hacer pedidos.

using System; // Funcionalidades básicas
using System.Windows.Forms; // Controles visuales
using System.Drawing; // Para usar colores en etiquetas
using Servidor.Entidad; // Para usar la clase Cliente
using ClienteEntrega; // Para ClienteTCP y ClienteLogueado

namespace ClienteEntrega
{
    public partial class FormValidacion : Form
    {
        // ================================================
        // Propiedad que almacena el cliente validado
        // ================================================
        public Cliente ClienteValidado { get; private set; } // Guarda al cliente si la validación es exitosa

        // Campo para mantener el estado de la conexión con el servidor
        private bool conectado;

        // ================================================
        // Constructor del formulario
        // ================================================
        public FormValidacion()
        {
            InitializeComponent();   // Inicializa los controles visuales del diseñador
            this.Load += FormValidacion_Load; // Asocia el evento Load al método correspondiente
            btnValidar.Enabled = false;  // Deshabilita el botón de validar al inicio
        }

        // =========================================
        // Evento al hacer clic en el botón Validar
        // =========================================
        private void btnValidar_Click(object sender, EventArgs e)
        {
            // Verifica si hay una conexión activa antes de continuar
            if (!ClienteTCP.EstaConectado())
            {
                MessageBox.Show("Debe conectarse al servidor antes de validar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detiene la ejecución del método
            }

            // Obtiene la identificación digitada y quita espacios en blanco
            string id = txtIdentificacion.Text.Trim();

            // Verifica que el campo de identificación no esté vacío
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Debe ingresar su identificación.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detiene la ejecución del método
            }

            // Llama a la capa de red para enviar la identificación al servidor
            Cliente cliente = ClienteTCP.ValidarCliente(id);

            // Si el servidor no devuelve un cliente (identificación no encontrada o inactiva)
            if (cliente == null)
            {
                MessageBox.Show("Identificación inválida o cliente inactivo.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Detiene la ejecución del método
            }

            // Si la validación es exitosa, se guarda el cliente y se cierra el formulario
            ClienteValidado = cliente; // Almacena el cliente validado en la propiedad pública
            ClienteLogueado.Identificacion = cliente.Identificacion.ToString(); // Guarda el ID para futuras consultas
            this.DialogResult = DialogResult.OK; // Establece el resultado del formulario como "OK"
        }

        // ===================================================
        // Muestra visualmente si hay conexión o no
        // y habilita o deshabilita el botón de validar
        // ===================================================
        private void ActualizarEstadoConexion()
        {
            // Si la variable 'conectado' es verdadera
            if (conectado)
            {
                lblEstadoConexion.Text = "Estado: Conectado"; // Actualiza el texto de la etiqueta
                lblEstadoConexion.ForeColor = Color.Green; // Cambia el color del texto a verde
                btnValidar.Enabled = true; // Habilita el botón para validar
            }
            else // Si 'conectado' es falso
            {
                lblEstadoConexion.Text = "Estado: Desconectado"; // Actualiza el texto de la etiqueta
                lblEstadoConexion.ForeColor = Color.Red; // Cambia el color del texto a rojo
                btnValidar.Enabled = false; // Deshabilita el botón para validar
            }
        }

        // ==========================================
        // Evento al hacer clic en el botón Conectar
        // ==========================================
        private void btnConectar_Click(object sender, EventArgs e)
        {
            conectado = ClienteTCP.Conectar(); // Intenta conectar al servidor y guarda el resultado (true/false)
            ActualizarEstadoConexion(); // Actualiza la interfaz de usuario según el estado de la conexión
        }

        // ==============================================
        // Evento al hacer clic en el botón Desconectar
        // ==============================================
        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            ClienteTCP.Desconectar(); // Llama al método para cerrar la conexión TCP
            lblEstadoConexion.Text = "Estado: Desconectado"; // Actualiza el texto de la etiqueta
            lblEstadoConexion.ForeColor = Color.Red; // Cambia el color a rojo
            btnValidar.Enabled = false; // Deshabilita el botón para validar
        }

        // ====================================================
        // Se ejecuta al cargar el formulario por primera vez
        // ====================================================
        private void FormValidacion_Load(object sender, EventArgs e)
        {
            ActualizarEstadoConexion(); // Verifica el estado de la conexión al iniciar el formulario
        }
    }
}
