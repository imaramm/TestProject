using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

// Photon.PunBehaviour 사용안함 (옛날꺼)
public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] 
    private string gameVersion = "0.0.1";   // 포톤 버전
    [SerializeField] 
    private byte maxPlayerPerRoom = 4;  // 방 인원

    [SerializeField] 
    // 서버에 접속할때 사용할 닉네임
    private string nickName = string.Empty;

    [SerializeField] 
    private Button connectButton = null;


    private void Awake()
    {
        // Application.version

        // 마스터가 PhotonNetwork.LoadLevel()을 호출하면,
        // 모든 플레이어가 동일한 레벨을 자동으로 로드
        PhotonNetwork.AutomaticallySyncScene = true;
        // 방 만든 사람이 마스터가 되는데 입장한 사람들이 씬을 열었는때 다같은 화면?을 보기위에 쓰는건데
        // 현재 만든 씬에서는 필요없음. 동기화가 많이 필요함. 
    }

    private void Start()
    {
        // 접속하는 버튼 활성화/비활성. 접속실패하면 비활성화.
        connectButton.interactable = true;
    }

    // ConnectButton이 눌러지면 호출
    public void Connect()
    {
        if (string.IsNullOrEmpty(nickName))
        {
            Debug.Log("NickName is empty");
            return;
        }
        // 서버에 접속해있는(들어와있는) 상태이면
        if (PhotonNetwork.IsConnected)
        {
            // 내가 들어갈 수 있는 방을 찾는다~
            PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork.JoinRoom();
        }
        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);

            // 포톤 버전이 같아야 같은 게임에 접속가능
            PhotonNetwork.GameVersion = gameVersion;
            // 포톤 클라우드에 접속을 시작하는 지점
            // 접속에 성공하면 OnConnectedToMaster 메서드 호출
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // InputField_NickName과 연결해 닉네임을 가져옴
    public void OnValueChangedNickName(string _nickName)
    {
        nickName = _nickName;

        // 유저 이름 지정
        PhotonNetwork.NickName = nickName;
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogFormat("Connected to Master: {0}", nickName); // 자동으로 호출

        connectButton.interactable = false;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        connectButton.interactable = true;

        // 방을 생성하면 OnJoinedRoom 호출
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        // loadscene을 할려면 buildsetting에 scenes in build 하위에 씬이 꼭 있어야한다.
        // SceneManager을 쓸려면 using UnityEngine.SceneManager을 써야함
        SceneManager.LoadScene("Room");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);

        connectButton.interactable = true;

        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
}

