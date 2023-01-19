using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

// Photon.PunBehaviour ������ (������)
public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] 
    private string gameVersion = "0.0.1";   // ���� ����
    [SerializeField] 
    private byte maxPlayerPerRoom = 4;  // �� �ο�

    [SerializeField] 
    // ������ �����Ҷ� ����� �г���
    private string nickName = string.Empty;

    [SerializeField] 
    private Button connectButton = null;


    private void Awake()
    {
        // Application.version

        // �����Ͱ� PhotonNetwork.LoadLevel()�� ȣ���ϸ�,
        // ��� �÷��̾ ������ ������ �ڵ����� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
        // �� ���� ����� �����Ͱ� �Ǵµ� ������ ������� ���� �����¶� �ٰ��� ȭ��?�� �������� ���°ǵ�
        // ���� ���� �������� �ʿ����. ����ȭ�� ���� �ʿ���. 
    }

    private void Start()
    {
        // �����ϴ� ��ư Ȱ��ȭ/��Ȱ��. ���ӽ����ϸ� ��Ȱ��ȭ.
        connectButton.interactable = true;
    }

    // ConnectButton�� �������� ȣ��
    public void Connect()
    {
        if (string.IsNullOrEmpty(nickName))
        {
            Debug.Log("NickName is empty");
            return;
        }
        // ������ �������ִ�(�����ִ�) �����̸�
        if (PhotonNetwork.IsConnected)
        {
            // ���� �� �� �ִ� ���� ã�´�~
            PhotonNetwork.JoinRandomRoom();
            //PhotonNetwork.JoinRoom();
        }
        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);

            // ���� ������ ���ƾ� ���� ���ӿ� ���Ӱ���
            PhotonNetwork.GameVersion = gameVersion;
            // ���� Ŭ���忡 ������ �����ϴ� ����
            // ���ӿ� �����ϸ� OnConnectedToMaster �޼��� ȣ��
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // InputField_NickName�� ������ �г����� ������
    public void OnValueChangedNickName(string _nickName)
    {
        nickName = _nickName;

        // ���� �̸� ����
        PhotonNetwork.NickName = nickName;
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogFormat("Connected to Master: {0}", nickName); // �ڵ����� ȣ��

        connectButton.interactable = false;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Disconnected: {0}", cause);

        connectButton.interactable = true;

        // ���� �����ϸ� OnJoinedRoom ȣ��
        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        // loadscene�� �ҷ��� buildsetting�� scenes in build ������ ���� �� �־���Ѵ�.
        // SceneManager�� ������ using UnityEngine.SceneManager�� �����
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

