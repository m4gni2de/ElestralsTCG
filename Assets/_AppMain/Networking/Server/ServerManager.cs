using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Gameplay;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;


public enum ServerMode
{
    HostOnly = 0,
    Both = 1,
}
public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }
    //public static ServerManager Instance
    //{
    //    get => _Instance;
    //    set
    //    {
    //        if (_Instance == null)
    //            _Instance = value;
    //        else if (_Instance != value)
    //        {
    //            Debug.Log($"{nameof(ServerManager)} instance already exists, destroying duplicate!");
    //            Destroy(value);
    //        }
    //    }
    //}
    public string myAddressLocal;
    public string myAddressGlobal;

    public Server Server { get; private set; }

    [SerializeField] private ushort port = 7777;
    [SerializeField] private ushort maxClientCount = 2;
    private ServerMode serverMode = ServerMode.HostOnly;





    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }
    void GetServerAddresses()
    {
        //Get the local IP
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                myAddressLocal = ip.ToString();
                break;
            } //if
        } //foreach
        //Get the global IP
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
        request.Method = "GET";
        request.Timeout = 1000; //time in ms
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                myAddressGlobal = reader.ReadToEnd();
            } //if
            else
            {
                Debug.LogError("Timed out? " + response.StatusDescription);
                myAddressGlobal = "127.0.0.1";
            } //else
        } //try
        catch (WebException ex)
        {
            Debug.Log("Likely no internet connection: " + ex.Message);
            myAddressGlobal = "127.0.0.1";
        } //catch
          //myAddressGlobal=new System.Net.WebClient().DownloadString("https://api.ipify.org"); //single-line solution for the global IP, but long time-out when there is no internet connection, so I prefer to do the method above where I can set a short time-out time
    } //Start

    public void Create()
    {
        if (Server == null)
        {
            
            GetServerAddresses();
            Application.targetFrameRate = 60;

            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

            Server = new Server();
            Server.Start(port, maxClientCount);
            Server.ClientDisconnected += PlayerLeft;
        }
       
    }

    private void FixedUpdate()
    {
        if (Server != null) { Server.Tick(); }
        
    }

    private void OnApplicationQuit()
    {
        if (Server != null) { Server.Stop(); }
        
    }
    private void OnDestroy()
    {
        if (Instance != null) { Instance = null; }
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //Destroy(OnlinePlayer.list[e.Id].gameObject);
    }

    
}
