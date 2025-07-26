// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Archivo: Program.cs
// Descripción: Punto de entrada de la aplicación cliente.
// Abre primero FormValidacion, y si es exitoso, muestra
// el formulario principal del cliente.
// ========================================================

using System;
using System.Windows.Forms;
using Servidor.Entidad;

namespace ClienteEntrega
{
    internal static class Program
    {
        public static Cliente ClienteLogueado { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Mostrar el formulario de validación como diálogo
            FormValidacion validacion = new FormValidacion();
            DialogResult resultado = validacion.ShowDialog();

            // Si la validación fue correcta, abrir el formulario principal del cliente
            if (resultado == DialogResult.OK && validacion.ClienteValidado != null)
            {
                ClienteLogueado = validacion.ClienteValidado;
                Application.Run(new FormPrincipalCliente(validacion.ClienteValidado));
            }
            else
            {
                // Si la validación fue cancelada o fallida, cerrar el programa
                Application.Exit();
            }
        }
    }
}


