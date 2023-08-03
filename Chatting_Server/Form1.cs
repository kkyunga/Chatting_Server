using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chatting_Server
{
    /*
     * ������ ���� ���� ���� �븮��
     * ���� ���� ClientHandler���� ���δ�.
     * ClientHandler���� SetTextDelegate ȣ��
     * ���� UI�����尡 �ƴ� �ٸ� �����忡�� textbox�� ���� ���ٸ� ���� �߻�
     */
    delegate void SetTextDelegate(string s);

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpListener chatServer = new TcpListener(IPAddress.Parse("127.0.0.1"), 2023);
        public static ArrayList clientSocketArray = new ArrayList();

        private void btn_server_Click(object sender, EventArgs e)
        {
            try
            {
                if (lb_server.Tag.ToString() == "stop")
                {
                    lb_server.Tag = "start";
                    lb_server.Text = "Server ���� ��";
                    btn_server.Text = "���� ����";

                    chatServer.Start();
                    /**
                     * ���α׷��� �ٸ� �۾��� ó���ϸ鼭 AcceptClient�� ������ Thread���� �����Ͽ� ���ÿ� ó�� ����
                     * ��Ʈ��ũ �������� Ŭ���̾�Ʈ ������ ó���ϴ� ���
                     **/
                    Thread waitThread = new Thread(new ThreadStart(AcceptClient));  // ThreadStart �븮�� �̿��Ͽ� AcceptClient�� waitThread���� ����ǵ��� ����
                    waitThread.Start();  // AcceptClient ȣ���Ͽ� ����
                }
                else
                {
                    chatServer.Stop();

                    foreach (Socket socket in Form1.clientSocketArray)
                    {
                        socket.Close();
                    }
                    clientSocketArray.Clear();

                    lb_server.Tag = "stop";
                    lb_server.Text = "Server ���� ��";
                    btn_server.Text = "���� ����";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("���� ���� ����" + ex.Message);
            }
        }

        private void AcceptClient()
        {
            Socket socketClient = null;
            try
            {
                while (true)
                {
                    socketClient = chatServer.AcceptSocket();  // Ŭ���̾�Ʈ�� ���� ��û�� �޾Ƶ��̴� �Լ�

                    ClientHandler clientHandler = new ClientHandler();
                    clientHandler.ClientHandler_Setup(this, socketClient, this.txtChatMsg);

                    Thread thd_ChatProcess = new Thread(new ThreadStart(clientHandler.Chat_Process));
                    thd_ChatProcess.Start();
                }
            }
            catch (Exception ex)
            {
                Form1.clientSocketArray.Remove(socketClient);
            }
        }

        public void SetText(string text)  // textbox�� ��ȭ������ ���� �޼���
        {
            if (this.txtChatMsg.InvokeRequired)  // invoke�� true���
            {
                SetTextDelegate d = new SetTextDelegate(SetText);  // �븮�� ����
                this.Invoke(d, new object[] { text });  // �븮�ڸ� ���� �� �ۼ�
            }
            else
            {
                this.txtChatMsg.AppendText(text);  // textbox�� ���� �ִ´�.
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            chatServer.Stop();
            Application.Exit();
        }
    }

    public class ClientHandler
    {
        private Form1 form1;
        private Socket socketClient;
        private NetworkStream netStream;
        private StreamReader strReader;
        private TextBox txtChatMsg;

        public void ClientHandler_Setup(Form1 form1, Socket socketClient, TextBox txtChatMsg)
        {
            this.txtChatMsg = txtChatMsg;  // textbox�� �޼��� ���� ���
            this.socketClient = socketClient;  // Ŭ���̾�Ʈ ���� ����
            this.netStream = new NetworkStream(socketClient);  // stream�� ����� ä��
            Form1.clientSocketArray.Add(socketClient);  // list�� Ŭ���̾�Ʈ ���� ���� �߰�
            this.strReader = new StreamReader(netStream);  // stream���Ͽ��� text�� �о���� �Լ�
            this.form1 = form1;
        }

        public void Chat_Process()
        {
            while (true)
            {
                try
                {
                    string msg = strReader.ReadLine();  // ��ȭ ���� �޾ƿ���
                    if (msg != null && msg != "")
                    {
                        form1.SetText(msg + "\r\n");
                        byte[] bytSand_Data = Encoding.Default.GetBytes(msg + "\r\n");  // ������ ���� �迭�� ��� ���ڸ� ����Ʈ�� ���ڵ�

                        lock (Form1.clientSocketArray)
                        {
                            foreach (Socket socket in Form1.clientSocketArray)
                            {
                                NetworkStream stream = new NetworkStream(socket);
                                stream.Write(bytSand_Data, 0, bytSand_Data.Length);  // ������ ���� bytSand_Data �����͸� ����
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("ä�� ����" + e.Message);
                    Form1.clientSocketArray.Remove(socketClient);
                    break;
                }
            }
        }
    }
}