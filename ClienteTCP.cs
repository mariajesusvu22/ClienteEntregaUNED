// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Clase: ClienteTCP.cs (Versión Modificada)
// Estudiante: María Jesús Venegas Ugalde
// Descripción: Clase modificada para usar una única conexión TCP
// persistente con el servidor.
// ========================================================

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using Servidor.Entidad; // Para usar Articulo, DetallePedido y Cliente

namespace ClienteEntrega
{
    public static class ClienteTCP
    {
        private static string ipServidor = "127.0.0.1";
        private static int puerto = 14100;
        private static TcpClient conexionCliente;
        private static NetworkStream stream;

        public static bool EstaConectado()
        {
            return conexionCliente != null && conexionCliente.Connected;
        }

        public static bool Conectar()
        {
            if (EstaConectado())
            {
                return true;
            }
            try
            {
                conexionCliente = new TcpClient(ipServidor, puerto);
                stream = conexionCliente.GetStream();
                return true;
            }
            catch
            {
                conexionCliente = null;
                stream = null;
                return false;
            }
        }

        public static void Desconectar()
        {
            if (conexionCliente != null)
            {
                stream?.Close();
                conexionCliente.Close();
                stream = null;
                conexionCliente = null;
            }
        }

        /// <summary>
        /// Método centralizado para enviar comandos y recibir respuestas del servidor.
        /// </summary>
        /// <summary>
        /// Método centralizado, ahora modificado para leer hasta encontrar un delimitador.
        /// </summary>
        private static string EnviarComando(string comando)
        {
            if (!EstaConectado())
            {
                return "ERROR: No hay conexión con el servidor.";
            }

            try
            {
                byte[] datos = Encoding.UTF8.GetBytes(comando);
                stream.Write(datos, 0, datos.Length);

                // --- Lógica de lectura mejorada con delimitador ---
                byte[] buffer = new byte[1024];
                StringBuilder sb = new StringBuilder();
                string respuestaCompleta = "";

                // Leemos del stream en un bucle hasta que encontremos el delimitador "<EOF>"
                while (!respuestaCompleta.Contains("<EOF>"))
                {
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                    if (bytesLeidos == 0) // El servidor cerró la conexión
                    {
                        Desconectar();
                        return "ERROR: El servidor cerró la conexión.";
                    }
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesLeidos));
                    respuestaCompleta = sb.ToString();
                }

                // Una vez encontrado, removemos el delimitador para obtener el JSON limpio.
                return respuestaCompleta.Replace("<EOF>", "");
            }
            catch (Exception ex)
            {
                Desconectar(); // La conexión se perdió.
                return "ERROR: " + ex.Message;
            }
        }
        public static List<Articulo> ObtenerArticulosDisponibles()
        {
            var lista = new List<Articulo>();
            string respuesta = EnviarComando("OBTENER_ARTICULOS");

            if (respuesta.StartsWith("ERROR"))
            {
                Console.WriteLine("Error al obtener artículos: " + respuesta);
                return lista;
            }

            string[] lineas = respuesta.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (string linea in lineas)
            {
                string[] partes = linea.Split('|');
                if (partes.Length == 6)
                {
                    var art = new Articulo
                    {
                        Id = int.Parse(partes[0]),
                        Nombre = partes[1],
                        TipoArticulo = new TipoArticulo { Nombre = partes[2] },
                        Valor = double.Parse(partes[3]),
                        Inventario = int.Parse(partes[4]),
                        Activo = partes[5] == "1"
                    };
                    if (art.Activo && art.Inventario > 0)
                        lista.Add(art);
                }
            }
            return lista;
        }

        public static Articulo ConsultarArticuloPorId(int id)
        {
            string mensaje = $"CONSULTAR_ARTICULO_ID|{id}";
            string respuesta = EnviarComando(mensaje);

            if (string.IsNullOrWhiteSpace(respuesta) || respuesta.StartsWith("ERROR"))
            {
                Console.WriteLine("Error al consultar artículo por ID: " + respuesta);
                return null;
            }

            return JsonSerializer.Deserialize<Articulo>(respuesta);
        }

        public static Cliente ValidarCliente(string id)
        {
            string mensaje = $"VALIDAR_CLIENTE|{id}";
            string respuesta = EnviarComando(mensaje);

            if (respuesta.StartsWith("ERROR"))
            {
                return null;
            }
            string[] partes = respuesta.Split('|');
            if (partes.Length == 6 && partes[5] == "1")
            {
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
            return null;
        }

        public static string EnviarDetalleTexto(int idCliente, string direccion, int idArticulo, DateTime fecha, int cantidad)
        {
            string mensaje = $"PEDIDO|{idCliente}|{direccion}|{fecha.ToString("yyyy-MM-dd")}|{idArticulo}|{cantidad}";
            return EnviarComando(mensaje);
        }

        public static List<DetallePedido> ConsultarPedidos(string clienteID)
        {
            var lista = new List<DetallePedido>();
            string mensaje = $"CONSULTAR_PEDIDOS|{clienteID}";
            string respuesta = EnviarComando(mensaje);

            if (respuesta.StartsWith("ERROR"))
            {
                return lista;
            }

            string[] lineas = respuesta.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (string linea in lineas)
            {
                string[] partes = linea.Split('|');
                if (partes.Length == 4)
                {
                    lista.Add(new DetallePedido
                    {
                        NumeroPedido = int.Parse(partes[0]),
                        Articulo = new Articulo { Nombre = partes[1] },
                        Cantidad = int.Parse(partes[2]),
                        FechaPedido = DateTime.Parse(partes[3])
                    });
                }
            }
            return lista;
        }

        public static List<DetallePedido> ConsultarPedidoPorId(int idPedido)
        {
            var lista = new List<DetallePedido>();
            string mensaje = $"CONSULTAR_PEDIDO_ID|{idPedido}";
            string respuesta = EnviarComando(mensaje);

            if (respuesta.StartsWith("ERROR"))
            {
                return lista;
            }

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
            return lista;
        }
    }
}