// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 � Programaci�n Avanzada con C#
// Proyecto 2: ClienteEntrega
// Archivo: Program.cs
// Estudiante: Mar�a Jes�s Venegas Ugalde
// Fecha: 21/07/2025
// ========================================================

// Descripci�n: Punto de entrada de la aplicaci�n cliente. Abre primero FormValidacion, y si es exitoso, muestra el formulario principal del cliente.
using System; // Funcionalidades b�sicas
using System.Windows.Forms; // Controles visuales
using Servidor.Entidad; // Para usar la clase Cliente

namespace ClienteEntrega
{
    internal static class Program
    {
        // Propiedad est�tica para acceder al cliente logueado desde cualquier parte de la app
        public static Cliente ClienteLogueado { get; private set; }

        // ==================================================
        // M�todo principal que inicia la aplicaci�n cliente
        // ==================================================
        [STAThread] // Atributo requerido para aplicaciones de Windows Forms
        static void Main()
        {
            // Habilita los estilos visuales modernos para los controles
            Application.EnableVisualStyles();
            // Establece el modo de renderizado de texto compatible por defecto
            Application.SetCompatibleTextRenderingDefault(false);

            // Crea una instancia del formulario de validaci�n
            FormValidacion validacion = new FormValidacion();
            // Muestra el formulario de validaci�n de forma modal (bloquea el resto de la app)
            DialogResult resultado = validacion.ShowDialog();

            // Comprueba si el formulario de validaci�n se cerr� con resultado "OK" y si hay un cliente validado
            if (resultado == DialogResult.OK && validacion.ClienteValidado != null)
            {
                // Guarda el cliente validado en la propiedad est�tica para acceso global
                ClienteLogueado = validacion.ClienteValidado;
                // Inicia la aplicaci�n con el formulario principal, pas�ndole el cliente validado
                Application.Run(new FormPrincipalCliente(validacion.ClienteValidado));
            }
            else
            {
                // Si la validaci�n se cancela o falla, se cierra la aplicaci�n
                Application.Exit();
            }
        }
    }
}


