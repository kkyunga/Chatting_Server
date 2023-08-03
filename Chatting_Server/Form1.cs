using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chatting_Server
{
    /*
     * 서버에 글을 쓰기 위한 대리자
     * 실제 글은 ClientHandler에서 쓰인다.
     * ClientHandler에서 SetTextDelegate 호출
     * 만약 UI쓰레드가 아닌 다른 쓰레드에서 textbox에 글을 쓴다면 에러 발생
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
                    lb_server.Text = "Server 시작 됨";
                    btn_server.Text = "서버 종료";

                    chatServer.Start();
                    /**
                     * 프로그램이 다른 작업을 처리하면서 AcceptClient를 별도의 Thread에서 실행하여 동시에 처리 가능
                     * 네트워크 서버에서 클라이언트 접속을 처리하는 방법
                     **/
                    Thread waitThread = new Thread(new ThreadStart(AcceptClient));  // ThreadStart 대리자 이용하여 AcceptClient가 waitThread에서 실행되도록 정의
                    waitThread.Start();  // AcceptClient 호출하여 실행
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
                    lb_server.Text = "Server 중지 됨";
                    btn_server.Text = "서버 시작";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("서버 시작 오류" + ex.Message);
            }
        }

        private void AcceptClient()
        {
            Socket socketClient = null;
            try
            {
                while (true)
                {
                    socketClient = chatServer.AcceptSocket();  // 클라이언트의 접속 요청을 받아들이는 함수

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

        public void SetText(string text)  // textbox에 대화내용을 넣을 메서드
        {
            if (this.txtChatMsg.InvokeRequired)  // invoke가 true라면
            {
                SetTextDelegate d = new SetTextDelegate(SetText);  // 대리자 선언
                this.Invoke(d, new object[] { text });  // 대리자를 통해 글 작성
            }
            else
            {
                this.txtChatMsg.AppendText(text);  // textbox에 글을 넣는다.
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
            this.txtChatMsg = txtChatMsg;  // textbox에 메세지 내용 출력
            this.socketClient = socketClient;  // 클라이언트 접속 소켓
            this.netStream = new NetworkStream(socketClient);  // stream을 만들어 채팅
            Form1.clientSocketArray.Add(socketClient);  // list에 클라이언트 접속 소켓 추가
            this.strReader = new StreamReader(netStream);  // stream파일에서 text를 읽어오는 함수
            this.form1 = form1;
        }

        public void Chat_Process()
        {
            while (true)
            {
                try
                {
                    string msg = strReader.ReadLine();  // 대화 내용 받아오기
                    if (msg != null && msg != "")
                    {
                        form1.SetText(msg + "\r\n");
                        byte[] bytSand_Data = Encoding.Default.GetBytes(msg + "\r\n");  // 지정된 문자 배열의 모든 문자를 바이트로 인코딩

                        lock (Form1.clientSocketArray)
                        {
                            foreach (Socket socket in Form1.clientSocketArray)
                            {
                                NetworkStream stream = new NetworkStream(socket);
                                stream.Write(bytSand_Data, 0, bytSand_Data.Length);  // 소켓을 통해 bytSand_Data 데이터를 전송
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("채팅 오류" + e.Message);
                    Form1.clientSocketArray.Remove(socketClient);
                    break;
                }
            }
        }
    }
}