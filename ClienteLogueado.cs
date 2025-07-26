// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Clase: ClienteLogueado.cs
// Estudiante: María Jesús Venegas Ugalde
// Descripción: Clase estática para almacenar el cliente
// que inició sesión en la aplicación cliente.
// ========================================================

namespace ClienteEntrega
{
    public static class ClienteLogueado
    {
        // Esta propiedad guarda la identificación del cliente validado
        public static string Identificacion { get; set; }
    }
}
