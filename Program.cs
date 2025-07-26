// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Archivo: Program.cs
// Estudiante: María Jesús Venegas Ugalde
// Fecha: 21/07/2025
// ========================================================

// Descripción: Punto de entrada de la aplicación cliente. Abre primero FormValidacion, y si es exitoso, muestra el formulario principal del cliente.
using System; // Funcionalidades básicas
using System.Windows.Forms; // Controles visuales
using Servidor.Entidad; // Para usar la clase Cliente

namespace ClienteEntrega
{
    internal static class Program
    {
        // Propiedad estática para acceder al cliente logueado desde cualquier parte de la app
        public static Cliente ClienteLogueado { get; private set; }

        // ==================================================
        // Método principal que inicia la aplicación cliente
        // ==================================================
        [STAThread] // Atributo requerido para aplicaciones de Windows Forms
        static void Main()
        {
            // Habilita los estilos visuales modernos para los controles
            Application.EnableVisualStyles();
            // Establece el modo de renderizado de texto compatible por defecto
            Application.SetCompatibleTextRenderingDefault(false);

            // Crea una instancia del formulario de validación
            FormValidacion validacion = new FormValidacion();
            // Muestra el formulario de validación de forma modal (bloquea el resto de la app)
            DialogResult resultado = validacion.ShowDialog();

            // Comprueba si el formulario de validación se cerró con resultado "OK" y si hay un cliente validado
            if (resultado == DialogResult.OK && validacion.ClienteValidado != null)
            {
                // Guarda el cliente validado en la propiedad estática para acceso global
                ClienteLogueado = validacion.ClienteValidado;
                // Inicia la aplicación con el formulario principal, pasándole el cliente validado
                Application.Run(new FormPrincipalCliente(validacion.ClienteValidado));
            }
            else
            {
                // Si la validación se cancela o falla, se cierra la aplicación
                Application.Exit();
            }
        }
    }
}


