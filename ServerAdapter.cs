using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Adapter to connect to jNetworkInterface server.
/// </summary>
public class ServerAdapter
{
    private TcpClient socket;
    private string hostname;
    private int port;
    private bool isConnected;

    /// <summary>
    /// Constructor that takes hostname and port.
    /// </summary>
    /// <param name="hostname">Hostname or IP Address</param>
    /// <param name="port">Port number</param>
    public ServerAdapter(string hostname, int port)
    {
        this.hostname = hostname;
        this.port = port;
    }

    /// <summary>
    /// Check connection.
    /// </summary>
    /// <returns>Connection status</returns>
    public bool IsConnected()
    {
        connect();
        return isConnected;
    }

    /// <summary>
    /// Send a command to the server.
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="data">Data to send as ArrayList</param>
    /// <returns></returns>
    public String SendCommand(string command, ArrayList data)
    {
        connect();
        if (isConnected)
        {
            try
            {
                StreamWriter writer = new StreamWriter(socket.GetStream(), Encoding.ASCII);
                // Output the command
                writer.WriteLine(command);
                // Add data
                if (data != null && data.Count != 0)
                    // Push data to sockets
                    foreach (var itm in data)
                        writer.WriteLine(itm);
                // Output end command sequence
                writer.WriteLine("END COMMAND");
                // Flush me baby
                writer.Flush();
                // Read it
                StreamReader reader = new StreamReader(socket.GetStream(), Encoding.ASCII);
                // Storage vars
                string line;
                string response = "";
                while ((line = reader.ReadLine()) != null)
                    response += line;
                // Clean stuff up
                writer.Close();
                reader.Close();
                closeConnection();

                return response;
            }
            catch
            {
                return "ERROR";
            }
        }
        else
            return "INVALID CONNECTION";
    }

    /// <summary>
    /// Connect to the server.
    /// </summary>
    private void connect()
    {
        // Make the connection.
        if (!isConnected)
        {
            try
            {
                socket = new TcpClient(hostname, port);
                isConnected = socket.Connected;
            }
            catch
            {
                isConnected = false;
            }
        }
    }

    /// <summary>
    /// Close server connection.
    /// </summary>
    private void closeConnection()
    {
        try
        {
            socket.Close();
            isConnected = false;
        }
        catch { }
    }
}