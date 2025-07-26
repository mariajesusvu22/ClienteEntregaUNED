// ========================================================
// Universidad Estatal a Distancia (UNED)
// II Cuatrimestre 2025 – Programación Avanzada con C#
// Proyecto 2: ClienteEntrega
// Clase: ClienteTCP.cs
// Estudiante: María Jesús Venegas Ugalde
// Descripción: Clase que permite al cliente comunicarse
// por TCP con el servidor para consultar artículos,
// registrar pedidos y ver historial.
// ========================================================

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Servidor.Entidad; // Para usar Articulo, DetallePedido y Cliente

namespace ClienteEntrega
{
    public static class ClienteTCP
    {
        // ===============================================
        // Configuración de conexión al servidor
        // ===============================================
        private static string ipServidor = "127.0.0.1"; // IP del servidor (localhost)
        private static int puerto = 14100;               // Puerto usado por el servidor

        // ===============================================
        // Variable de control de conexión persistente
        // ===============================================
        private static TcpClient conexionCliente;

        // ===============================================
        // Método para verificar si hay conexión activa
        // ===============================================
        public static bool EstaConectado()
        {
            return conexionCliente != null && conexionCliente.Connected;
        }

        // ============================================================
        // Método para obtener la lista de artículos desde el servidor
        // Solo se devuelven los artículos activos
        // ============================================================
        public static List<Articulo> ObtenerArticulosDisponibles()
        {
            List<Articulo> lista = new List<Articulo>();

            try
            {
                using (TcpClient cliente = new TcpClient(ipServidor, puerto))
                {
                    NetworkStream stream = cliente.GetStream();

                    byte[] datos = Encoding.UTF8.GetBytes("OBTENER_ARTICULOS");
                    stream.Write(datos, 0, datos.Length);

                    byte[] buffer = new byte[4096];
                    int bytesLeidos;
                    StringBuilder sb = new StringBuilder();

                    while ((bytesLeidos = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesLeidos));
                    }

                    string respuesta = sb.ToString();
                    string[] lineas = respuesta.Split(';', StringSplitOptions.RemoveEmptyEntries);

                    foreach (string linea in lineas)
                    {
                        string[] partes = linea.Split('|');

                        if (partes.Length == 6)
                        {
                            Articulo art = new Articulo
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener artículos: " + ex.Message);
            }

            return lista;
        }

        // ===================================================================
        // Método para consultar un artículo específico por su ID
        // ===================================================================
        public static Articulo ConsultarArticuloPorId(int id)
        {
            try
            {
                using (TcpClient cliente = new TcpClient(ipServidor, puerto))
                {
                    NetworkStream stream = cliente.GetStream();

                    string mensaje = $"CONSULTAR_ARTICULO_ID|{id}";
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                    stream.Write(datos, 0, datos.Length);

                    byte[] buffer = new byte[2048];
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                    string respuesta = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                    if (string.IsNullOrWhiteSpace(respuesta) || respuesta.StartsWith("ERROR"))
                        return null;

                    Articulo articulo = System.Text.Json.JsonSerializer.Deserialize<Articulo>(respuesta);
                    return articulo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consultar artículo por ID: " + ex.Message);
                return null;
            }
        }

        // ===================================================
        // Método para conectar al servidor (manual)
        // ===================================================
        public static bool Conectar()
        {
            try
            {
                conexionCliente = new TcpClient(ipServidor, puerto);
                return true;
            }
            catch
            {
                conexionCliente = null;
                return false;
            }
        }

        // ===================================================
        // Método para desconectar del servidor
        // ===================================================
        public static void Desconectar()
        {
            if (conexionCliente != null)
            {
                conexionCliente.Close();
                conexionCliente = null;
            }
        }

        // ========================================================================
        // Método para enviar un pedido al servidor. Retorna true si fue exitoso.
        // ========================================================================
        public static bool EnviarDetalle(string identificacion, string direccion, int articuloId, DateTime fecha, int cantidad)
        {
            try
            {
                using (TcpClient cliente = new TcpClient(ipServidor, puerto))
                {
                    NetworkStream stream = cliente.GetStream();
                    string mensaje = $"PEDIDO|{ClienteLogueado.Identificacion}|{direccion}|{fecha:yyyy-MM-dd}|{articuloId}|{cantidad}";
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                    stream.Write(datos, 0, datos.Length);
                    byte[] buffer = new byte[256];
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                    string respuesta = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                    return respuesta.Trim() == "OK";
                }
            }
            catch
            {
                return false;
            }
        }

        // ========================================================================
        // Método para consultar los pedidos de un cliente
        // ========================================================================
        public static List<DetallePedido> ConsultarPedidos(string clienteID)
        {
            List<DetallePedido> lista = new List<DetallePedido>();

            try
            {
                using (TcpClient cliente = new TcpClient(ipServidor, puerto))
                {
                    NetworkStream stream = cliente.GetStream();
                    string mensaje = $"CONSULTAR_PEDIDOS|{clienteID}";
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                    stream.Write(datos, 0, datos.Length);

                    byte[] buffer = new byte[4096];
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                    string respuesta = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                    string[] lineas = respuesta.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    foreach (string linea in lineas)
                    {
                        string[] partes = linea.Split('|');
                        if (partes.Length == 4)
                        {
                            DetallePedido dp = new DetallePedido
                            {
                                NumeroPedido = int.Parse(partes[0]),
                                Articulo = new Articulo { Nombre = partes[1] },
                                Cantidad = int.Parse(partes[2]),
                                FechaPedido = DateTime.Parse(partes[3])
                            };
                            lista.Add(dp);
                        }
                    }
                }
            }
            catch
            {
                // Se devuelve lista vacía en caso de error
            }

            return lista;
        }

        // ========================================================================
        // Método para validar un cliente por su identificación
        // ========================================================================
        public static Cliente ValidarCliente(string id)
        {
            try
            {
                using (TcpClient cliente = new TcpClient(ipServidor, puerto))
                {
                    NetworkStream stream = cliente.GetStream();
                    string mensaje = $"VALIDAR_CLIENTE|{id}";
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                    stream.Write(datos, 0, datos.Length);

                    byte[] buffer = new byte[1024];
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                    string respuesta = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                    if (respuesta.StartsWith("ERROR"))
                        return null;

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
                }
            }
            catch
            {
                // Se devuelve null en caso de error
            }

            return null;
        }

        // ========================================================================
        // Alternativa para enviar el pedido y obtener respuesta como texto
        // ========================================================================
        public static string EnviarDetalleTexto(int idCliente, string direccion, int idArticulo, DateTime fecha, int cantidad)
        {
            try
            {
                using (TcpClient cliente = new TcpClient(ipServidor, puerto))
                {
                    NetworkStream stream = cliente.GetStream();
                    string mensaje = $"PEDIDO|{idCliente}|{direccion}|{fecha.ToShortDateString()}|{idArticulo}|{cantidad}";
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                    stream.Write(datos, 0, datos.Length);

                    byte[] buffer = new byte[1024];
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);

                    return Encoding.UTF8.GetString(buffer, 0, bytesLeidos);
                }
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        // ===================================================================================
        // Método para consultar un pedido específico por su número de pedido
        // ===================================================================================
        public static List<DetallePedido> ConsultarPedidoPorId(int idPedido)
        {
            List<DetallePedido> lista = new List<DetallePedido>();

            try
            {
                using (TcpClient cliente = new TcpClient(ipServidor, puerto))
                {
                    NetworkStream stream = cliente.GetStream();
                    string mensaje = $"CONSULTAR_PEDIDO_ID|{idPedido}";
                    byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                    stream.Write(datos, 0, datos.Length);

                    byte[] buffer = new byte[4096];
                    int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                    string respuesta = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                    string[] lineas = respuesta.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    foreach (string linea in lineas)
                    {
                        string[] partes = linea.Split('|');
                        if (partes.Length == 4) 
                        {
                            DetallePedido dp = new DetallePedido
                            {
                                NumeroPedido = idPedido,
                                Articulo = new Articulo { Nombre = partes[0] },
                                Cantidad = int.Parse(partes[1]),
                                Monto = double.Parse(partes[2]),
                                FechaPedido = DateTime.Parse(partes[3])
                            };
                            lista.Add(dp);
                        }
                    }
                }
            }
            catch
            {
                // En caso de error se retorna lista vacía
            }

            return lista;
        }

    }
}
