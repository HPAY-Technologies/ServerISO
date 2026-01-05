using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class Server : Form
    {
        private TcpListener server;
        private X509Certificate2 certificate;

        private readonly IPAddress SERVER_IP = IPAddress.Parse("192.168.1.9");
        private const int SERVER_PORT = 5000;

        public Server()
        {
            InitializeComponent();
            Load += Server_Load;
        }

        private void Server_Load(object sender, EventArgs e)
        {
            StartServer();
        }

        private void StartServer()
        {
            try
            {
                certificate = LoadCertificate();
                if (certificate == null)
                {
                    txtLogs.AppendText("❌ Certificat SSL introuvable\r\n");
                    return;
                }

                server = new TcpListener(SERVER_IP, SERVER_PORT);
                server.Start();

                txtLogs.AppendText("🔐 SERVEUR SSL TLS 1.2 DÉMARRÉ\r\n");
                txtLogs.AppendText($"📡 IP : {SERVER_IP} | PORT : {SERVER_PORT}\r\n");
                txtLogs.AppendText("📜 Certificat : " + certificate.Subject + "\r\n");

                server.BeginAcceptTcpClient(OnClientConnect, null);
            }
            catch (Exception ex)
            {
                txtLogs.AppendText("❌ Erreur démarrage : " + ex.Message + "\r\n");
            }
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            TcpClient client = null;
            SslStream sslStream = null;

            try
            {
                client = server.EndAcceptTcpClient(ar);
                server.BeginAcceptTcpClient(OnClientConnect, null);

                Invoke(() => txtLogs.AppendText("🔌 Client connecté\r\n"));

                sslStream = new SslStream(client.GetStream(), false);

                sslStream.AuthenticateAsServer(
                    certificate,
                    false,
                    SslProtocols.Tls12,
                    false
                );

                Invoke(() =>
                {
                    txtLogs.AppendText("🔐 SSL ÉTABLI\r\n");
                    txtLogs.AppendText("➡️ Protocole : " + sslStream.SslProtocol + "\r\n");
                });

                byte[] buffer = new byte[2048];

                while (client.Connected)
                {
                    int bytes = sslStream.Read(buffer, 0, buffer.Length);
                    if (bytes <= 0) break;

                    string request = Encoding.ASCII.GetString(buffer, 0, bytes).Trim();
                    Invoke(() => txtLogs.AppendText("📩 Reçu → " + request + "\r\n"));

                    string responseCode = "00";
                    string message = "APPROVED";

                    string[] parts = request.Split('|');

                    if (!request.StartsWith("0200|") || parts.Length != 5)
                    {
                        responseCode = "30";
                        message = "FORMAT ERROR";
                    }
                    else if (!parts[1].All(char.IsDigit) || parts[1].Length < 13)
                    {
                        responseCode = "14";
                        message = "INVALID PAN";
                    }
                    else if (!parts[3].All(char.IsDigit))
                    {
                        responseCode = "13";
                        message = "INVALID AMOUNT";
                    }

                    string response = $"0210|{responseCode}|{message}";
                    byte[] respBytes = Encoding.ASCII.GetBytes(response);

                    sslStream.Write(respBytes);
                    sslStream.Flush();

                    Invoke(() => txtLogs.AppendText("📤 Réponse → " + response + "\r\n"));
                }
            }
            catch (AuthenticationException)
            {
                Invoke(() => txtLogs.AppendText("❌ CLIENT NON SSL REJETÉ\r\n"));
            }
            catch (Exception ex)
            {
                Invoke(() => txtLogs.AppendText("❌ Erreur serveur : " + ex.Message + "\r\n"));
            }
            finally
            {
                sslStream?.Close();
                client?.Close();
                Invoke(() => txtLogs.AppendText("🔌 Client déconnecté\r\n"));
            }
        }

        private X509Certificate2 LoadCertificate()
        {
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            var certs = store.Certificates.Find(
                X509FindType.FindBySubjectName,
                "localhost",
                false
            );

            return certs.Count > 0 ? certs[0] : null;
        }
    }
}
