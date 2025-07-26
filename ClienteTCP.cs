// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Clase: ClienteTCP.cs (Versión Modificada)
// Estudiante: María Jesús Venegas Ugalde
// Fecha: 21/07/2025
// ========================================================

// Descripción: Clase modificada para usar una única conexión TCP
//              persistente con el servidor.

using System; // Funcionalidades básicas
using System.Collections.Generic; // Para usar listas
using System.Net.Sockets; // Para comunicación por red (TCP)
using System.Text; // Para codificación de texto (UTF8)
using System.Text.Json; // Para serializar y deserializar JSON
using System.Threading; // Para usar hilos (en este caso, no se usa activamente)
using Servidor.Entidad; // Para usar Articulo, DetallePedido y Cliente

namespace ClienteEntrega
{
    public static class ClienteTCP
    {
        // ==============================================
        // Campos estáticos para la conexión persistente
        // ==============================================
        private static string ipServidor = "127.0.0.1"; // Dirección IP del servidor (local)
        private static int puerto = 14100; // Puerto en el que el servidor está escuchando
        private static TcpClient conexionCliente; // Objeto que mantiene la conexión TCP
        private static NetworkStream stream; // Canal para enviar y recibir datos

        // ===============================================
        // Método para verificar el estado de la conexión
        // ===============================================
        public static bool EstaConectado()
        {
            // Devuelve verdadero solo si el objeto cliente no es nulo y su propiedad Connected es true
            return conexionCliente != null && conexionCliente.Connected;
        }

        // ===================================================
        // Método para establecer la conexión con el servidor
        // ===================================================
        public static bool Conectar()
        {
            // Si ya existe una conexión activa, no hace nada y devuelve true
            if (EstaConectado())
            {
                return true;
            }
            try
            {
                // Intenta crear una nueva instancia de TcpClient, conectándose al servidor
                conexionCliente = new TcpClient(ipServidor, puerto);
                // Obtiene el flujo de datos de la conexión para la comunicación
                stream = conexionCliente.GetStream();
                return true; // Devuelve true si la conexión fue exitosa
            }
            catch // Si ocurre cualquier error durante la conexión
            {
                conexionCliente = null; // Se asegura de que el objeto cliente sea nulo
                stream = null; // Se asegura de que el flujo sea nulo
                return false; // Devuelve false para indicar que la conexión falló
            }
        }

        // ===============================================
        // Método para cerrar la conexión con el servidor
        // ===============================================
        public static void Desconectar()
        {
            // Verifica que haya una conexión activa para cerrar
            if (conexionCliente != null)
            {
                stream?.Close(); // Cierra el flujo de datos (el '?' evita error si es nulo)
                conexionCliente.Close(); // Cierra la conexión TCP
                stream = null; // Libera la referencia al flujo
                conexionCliente = null; // Libera la referencia al cliente
            }
        }

        // ================================================================
        // Método centralizado para enviar comandos y recibir respuestas.
        // Modificado para leer hasta encontrar un delimitador "<EOF>".
        // ================================================================
        private static string EnviarComando(string comando)
        {
            // Si no hay conexión, devuelve un mensaje de error inmediatamente
            if (!EstaConectado())
            {
                return "ERROR: No hay conexión con el servidor.";
            }

            try
            {
                // Convierte el comando de texto a un arreglo de bytes usando codificación UTF-8
                byte[] datos = Encoding.UTF8.GetBytes(comando);
                // Escribe los datos en el flujo de red para enviarlos al servidor
                stream.Write(datos, 0, datos.Length);

                // --- Lógica de lectura mejorada con delimitador ---
                byte[] buffer = new byte[1024]; // Búfer para almacenar los datos recibidos en trozos
                StringBuilder sb = new StringBuilder(); // Para construir la respuesta completa
                string respuestaCompleta = "";

                // Lee del flujo en un bucle hasta que la respuesta contenga el delimitador de fin de mensaje
                while (!respuestaCompleta.Contains("<EOF>"))
                {
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length); // Lee datos del servidor
                    if (bytesLeidos == 0) // Si Read devuelve 0, el servidor cerró la conexión
                    {
                        Desconectar(); // Cierra la conexión del lado del cliente
                        return "ERROR: El servidor cerró la conexión.";
                    }
                    // Convierte los bytes leídos a texto y los agrega al StringBuilder
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesLeidos));
                    respuestaCompleta = sb.ToString(); // Actualiza la cadena de respuesta completa
                }

                // Una vez recibido el delimitador, lo quita para devolver el dato limpio
                return respuestaCompleta.Replace("<EOF>", "");
            }
            catch (Exception ex)
            {
                Desconectar(); // Si hay un error de red, la conexión se considera perdida
                return "ERROR: " + ex.Message; // Devuelve el mensaje de error
            }
        }

        // ====================================================
        // Obtiene la lista de todos los artículos disponibles
        // ====================================================
        public static List<Articulo> ObtenerArticulosDisponibles()
        {
            var lista = new List<Articulo>(); // Crea una lista vacía para los resultados
            string respuesta = EnviarComando("OBTENER_ARTICULOS"); // Envía el comando al servidor

            // Si la respuesta del servidor es un error, lo muestra y devuelve la lista vacía
            if (respuesta.StartsWith("ERROR"))
            {
                Console.WriteLine("Error al obtener artículos: " + respuesta);
                return lista;
            }

            // Divide la respuesta en líneas, donde cada línea es un artículo
            string[] lineas = respuesta.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (string linea in lineas) // Recorre cada línea de artículo
            {
                string[] partes = linea.Split('|'); // Divide la línea en sus partes usando '|'
                if (partes.Length == 6) // Valida que tenga la cantidad de datos correcta
                {
                    // Crea un nuevo objeto Articulo con los datos parseados
                    var art = new Articulo
                    {
                        Id = int.Parse(partes[0]),
                        Nombre = partes[1],
                        TipoArticulo = new TipoArticulo { Nombre = partes[2] },
                        Valor = double.Parse(partes[3]),
                        Inventario = int.Parse(partes[4]),
                        Activo = partes[5] == "1" // Convierte "1" a true, cualquier otra cosa a false
                    };
                    // Agrega el artículo a la lista solo si está activo y tiene inventario
                    if (art.Activo && art.Inventario > 0)
                        lista.Add(art);
                }
            }
            return lista; // Devuelve la lista de artículos
        }

        // ==========================================
        // Consulta un artículo específico por su ID
        // ==========================================
        public static Articulo ConsultarArticuloPorId(int id)
        {
            // Construye el comando con el ID del artículo
            string mensaje = $"CONSULTAR_ARTICULO_ID|{id}";
            // Envía el comando al servidor
            string respuesta = EnviarComando(mensaje);

            // Si la respuesta es nula, vacía o un error, devuelve null
            if (string.IsNullOrWhiteSpace(respuesta) || respuesta.StartsWith("ERROR"))
            {
                Console.WriteLine("Error al consultar artículo por ID: " + respuesta);
                return null;
            }

            // Deserializa la respuesta JSON a un objeto Articulo
            return JsonSerializer.Deserialize<Articulo>(respuesta);
        }

        // ========================================
        // Valida la identificación de un cliente
        // ========================================
        public static Cliente ValidarCliente(string id)
        {
            // Construye el comando de validación con la identificación
            string mensaje = $"VALIDAR_CLIENTE|{id}";
            // Envía el comando al servidor
            string respuesta = EnviarComando(mensaje);

            // Si el servidor devuelve un error, retorna null
            if (respuesta.StartsWith("ERROR"))
            {
                return null;
            }
            // Divide la respuesta en partes
            string[] partes = respuesta.Split('|');
            // Si los datos son correctos y el cliente está activo ("1")
            if (partes.Length == 6 && partes[5] == "1")
            {
                // Crea y devuelve un nuevo objeto Cliente con los datos recibidos
                return new Cliente
                {
                    Identificacion = int.Parse(partes[0]),
                    Nombre = partes[1],
                    PrimerApellido = partes[2],
                    SegundoApellido = partes[3],
                    FechaNacimiento = DateTime.Parse(partes[4]),
                    Activo = true
                };
            }
            return null; // Si no es válido, retorna null
        }

        // ===================================
        // Envía un nuevo pedido al servidor
        // ===================================
        public static string EnviarDetalleTexto(int idCliente, string direccion, int idArticulo, DateTime fecha, int cantidad)
        {
            // Construye el comando del pedido con todos los datos necesarios
            string mensaje = $"PEDIDO|{idCliente}|{direccion}|{fecha.ToString("yyyy-MM-dd")}|{idArticulo}|{cantidad}";
            // Envía el comando y devuelve la respuesta del servidor (ej. "OK" o un error)
            return EnviarComando(mensaje);
        }

        // =========================================
        // Consulta todos los pedidos de un cliente
        // =========================================
        public static List<DetallePedido> ConsultarPedidos(string clienteID)
        {
            var lista = new List<DetallePedido>(); // Prepara la lista para los resultados
            // Construye el comando de consulta
            string mensaje = $"CONSULTAR_PEDIDOS|{clienteID}";
            // Envía el comando al servidor
            string respuesta = EnviarComando(mensaje);

            // Si hay un error, devuelve la lista vacía
            if (respuesta.StartsWith("ERROR"))
            {
                return lista;
            }

            // Divide la respuesta en líneas (cada línea es un pedido/detalle)
            string[] lineas = respuesta.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (string linea in lineas) // Recorre cada línea
            {
                string[] partes = linea.Split('|'); // Divide la línea en sus datos
                if (partes.Length == 4) // Valida que la cantidad de datos sea correcta
                {
                    // Agrega un nuevo DetallePedido a la lista con los datos parseados
                    lista.Add(new DetallePedido
                    {
                        NumeroPedido = int.Parse(partes[0]),
                        Articulo = new Articulo { Nombre = partes[1] },
                        Cantidad = int.Parse(partes[2]),
                        FechaPedido = DateTime.Parse(partes[3])
                    });
                }
            }
            return lista; // Devuelve la lista de pedidos
        }

        // ========================================
        // Consulta un pedido específico por su ID
        // ========================================
        public static List<DetallePedido> ConsultarPedidoPorId(int idPedido)
        {
            var lista = new List<DetallePedido>(); // Prepara la lista para los resultados
            // Construye el comando de consulta
            string mensaje = $"CONSULTAR_PEDIDO_ID|{idPedido}";
            // Envía el comando al servidor
            string respuesta = EnviarComando(mensaje);

            // Si hay un error, devuelve la lista vacía
            if (respuesta.StartsWith("ERROR"))
            {
                return lista;
            }

            // Procesa la respuesta del servidor, similar a la consulta de todos los pedidos
            string[] lineas = respuesta.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (string linea in lineas)
            {
                string[] partes = linea.Split('|');
                if (partes.Length == 4)
                {
                    lista.Add(new DetallePedido
                    {
                        NumeroPedido = idPedido,
                        Articulo = new Articulo { Nombre = partes[0] },
                        Cantidad = int.Parse(partes[1]),
                        Monto = double.Parse(partes[2]),
                        FechaPedido = DateTime.Parse(partes[3])
                    });
                }
            }
            return lista; // Devuelve la lista con los detalles del pedido encontrado
        }
    }
}