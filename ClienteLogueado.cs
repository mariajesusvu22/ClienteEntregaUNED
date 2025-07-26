// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Clase: ClienteLogueado.cs
// Estudiante: María Jesús Venegas Ugalde
// Fecha: 21/07/2025
// ========================================================

// Descripción: Clase estática para almacenar el cliente que inició sesión en la aplicación cliente.

namespace ClienteEntrega
{
    public static class ClienteLogueado
    {
        // ================================================
        // Propiedad estática para la identificación
        // ================================================
        // Guarda la identificación del cliente validado. Al ser estática, es accesible desde cualquier parte de la app.
        public static string Identificacion { get; set; }
    }
}