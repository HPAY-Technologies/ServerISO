using Iso8583;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class Server : Form
    {
        private TcpListener server;
        private X509Certificate2 certificate;

        private const int SERVER_PORT = 5000;
        private readonly IPAddress SERVER_IP = IPAddress.Any;

        public Server()
        {
            InitializeComponent();
            Load += Server_Load;
            FormClosing += Server_FormClosing;
        }

        // ▶️ AUTO START
        private void Server_Load(object sender, EventArgs e)
        {
            StartServer();
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            server?.Stop();
        }

        private void StartServer()
        {
            try
            {
                certificate = LoadCertificate();
                if (certificate == null)
                {
                    Log("❌ Certificat SSL introuvable");
                    return;
                }

                server = new TcpListener(SERVER_IP, SERVER_PORT);
                server.Start();

                Log("🔐 SERVEUR ISO8583 TLS 1.2 DÉMARRÉ");
                Log($"📡 Écoute sur 0.0.0.0:{SERVER_PORT}");
                Log($"📜 Certificat : {certificate.Subject}");
                Log("⏳ En attente de clients...\n");

                server.BeginAcceptTcpClient(OnClientConnect, null);
            }
            catch (Exception ex)
            {
                Log("❌ Erreur démarrage : " + ex.Message);
            }
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            TcpClient client = null;

            try
            {
                client = server.EndAcceptTcpClient(ar);
                server.BeginAcceptTcpClient(OnClientConnect, null);

                Log("🔌 Client connecté");

                Thread t = new Thread(() => HandleClient(client));
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            {
                Log("❌ Erreur accept client : " + ex.Message);
                client?.Close();
            }
        }

        private void HandleClient(TcpClient client)
        {
            SslStream sslStream = null;

            try
            {
                sslStream = new SslStream(client.GetStream(), false);

                sslStream.AuthenticateAsServer(
                    certificate,
                    false,
                    SslProtocols.Tls12,
                    false
                );

                Log($"🔐 SSL établi | {sslStream.SslProtocol}");

                byte[] buffer = new byte[4096];

                while (client.Connected)
                {
                    int bytesRead = sslStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead <= 0) break;

                    string rawIso = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Log("📩 ISO Reçu → " + rawIso);

                    IsoMessage request = IsoParser.Parse(rawIso);

       
                    string isoResponse = IsoResponseGenerator.GenerateResponse(request);

                    byte[] respBytes = Encoding.ASCII.GetBytes(isoResponse);
                    sslStream.Write(respBytes);
                    sslStream.Flush();

                    Log("📤 ISO Réponse → " + isoResponse);
                }
            }
            catch (AuthenticationException)
            {
                Log("❌ CLIENT NON SSL REJETÉ");
            }
            catch (Exception ex)
            {
                Log("❌ Erreur client : " + ex.Message);
            }
            finally
            {
                sslStream?.Close();
                client?.Close();
                Log("🔌 Client déconnecté\n");
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

        // 📝 LOG SAFE UI
        private void Log(string msg)
        {
            Invoke(new Action(() =>
            {
                txtLogs.AppendText(msg + Environment.NewLine);
            }));
        }
    }
}
