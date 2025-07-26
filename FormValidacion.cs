// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Formulario: FormValidacion.cs
// Estudiante: María Jesús Venegas Ugalde
// Descripción: Permite validar el número de identificación
// del cliente antes de permitir hacer pedidos.
// ========================================================

using System;
using System.Windows.Forms;
using System.Drawing;                    // Para usar colores en etiquetas
using Servidor.Entidad;                 // Para usar la clase Cliente
using ClienteEntrega;                   // Para ClienteTCP y ClienteLogueado

namespace ClienteEntrega
{

    public partial class FormValidacion : Form
    {
        // ================================================
        // Propiedad que almacena el cliente validado
        // ================================================
        public Cliente ClienteValidado { get; private set; }

        private bool conectado;


        // ================================================
        // Constructor del formulario
        // ================================================
        public FormValidacion()
        {
            InitializeComponent();           // Inicializa controles visuales
            this.Load += FormValidacion_Load; // Asigna evento de carga
            btnValidar.Enabled = false;       // Desactiva validación al inicio
        }

        // =========================================
        // Evento al hacer clic en el botón Validar
        // =========================================
        private void btnValidar_Click(object sender, EventArgs e)
        {
            // Verifica si hay conexión activa antes de continuar
            if (!ClienteTCP.EstaConectado())
            {
                MessageBox.Show("Debe conectarse al servidor antes de validar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtiene la identificación digitada
            string id = txtIdentificacion.Text.Trim();

            // Verifica que no esté vacío
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Debe ingresar su identificación.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Solicita al servidor validar el cliente
            Cliente cliente = ClienteTCP.ValidarCliente(id);

            // Si no se encontró
            if (cliente == null)
            {
                MessageBox.Show("Identificación inválida o cliente inactivo.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Si se validó correctamente, se guarda y se cierra el formulario
            ClienteValidado = cliente;
            ClienteLogueado.Identificacion = cliente.Identificacion.ToString();
            this.DialogResult = DialogResult.OK;
        }

        // ===================================================
        // Muestra visualmente si hay conexión o no
        // y habilita o deshabilita el botón de validar
        // ===================================================
        private void ActualizarEstadoConexion()
        {

            if (conectado)
            {
                lblEstadoConexion.Text = "Estado: Conectado";
                lblEstadoConexion.ForeColor = Color.Green;
                btnValidar.Enabled = true; // Habilita el botón Validar
            }
            else
            {
                lblEstadoConexion.Text = "Estado: Desconectado";
                lblEstadoConexion.ForeColor = Color.Red;
                btnValidar.Enabled = false; // Desactiva el botón Validar
            }
        }

        // ===================================================
        // Evento al hacer clic en el botón Conectar
        // ===================================================
        private void btnConectar_Click(object sender, EventArgs e)
        {
            conectado = ClienteTCP.Conectar();
            ActualizarEstadoConexion(); // Intenta conectar y actualiza estado
        }

        // ===================================================
        // Evento al hacer clic en el botón Desconectar
        // ===================================================
        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            ClienteTCP.Desconectar();             // Cierra conexión TCP
            lblEstadoConexion.Text = "Estado: Desconectado";
            lblEstadoConexion.ForeColor = Color.Red;
            btnValidar.Enabled = false;           // Desactiva el botón Validar
        }

        // =====================================================
        // Se ejecuta al cargar el formulario por primera vez
        // =====================================================
        private void FormValidacion_Load(object sender, EventArgs e)
        {
            ActualizarEstadoConexion(); // Verifica si ya hay conexión al iniciar
        }
    }
}
