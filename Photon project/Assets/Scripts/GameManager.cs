using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab = null;

    // �� Ŭ���̾�Ʈ���� ������ �÷��̾� ���� ������Ʈ�� �迭�� �����Ѵ�.
    private GameObject[] playerGoList = new GameObject[4];


    private void Start()
    {
        if (playerPrefab != null)
        {
            // photonNetwork�ȿ� instantiate�� ����°�. ������ �Ȱ��� ���������.
            // ���ο����� ���͵� ���濡�� �̸� �����س��ٰ� �����ϴ�?
            GameObject go = PhotonNetwork.Instantiate(
                // �������� �����°� �ƴ϶�, �̸��� ��������Ѵ�
                playerPrefab.name, //"P_Player"  �� �̷��� �����൵ �ȴ�
                new Vector3(
                    Random.Range(-10.0f, 10.0f),
                    0.0f,
                    Random.Range(-10.0f, 10.0f)),
                Quaternion.identity,
                0);
            // �÷��̾�� ���� �ٲٴ� �ڵ�. 
            // ���° ������ ������. (�����Ʈ��ũ��-�������-�÷��̾� ��) ������� ���ͼ� ���� ������ / �迭�� 
            go.GetComponent<PlayerCtrl>().SetMaterial(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    // PhotonNetwork.LeaveRoom �Լ��� ȣ��Ǹ� ȣ��
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");

        // ���� �������� ��ó������ ���ư��ڴ�
        SceneManager.LoadScene("Launcher");
    }

    // �÷��̾ ������ �� ȣ��Ǵ� �Լ�
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        // ��� �÷��̾ �� �̰Ÿ� �޴´�. ���� ���Դ��� �˼��ִ� 
        Debug.LogFormat("Player Entered Room: {0}",
                        otherPlayer.NickName);

        // ������ �����ϸ� ��ü Ŭ���̾�Ʈ���� �������� �Լ� ȣ��
        // ���ο� �ְ� �������ϱ� ����� ���� ����,ȣ��
        // �ν�����â���� photon view�� �־�� �̰Ÿ� ���������ִ�.
        // RPC : Remote Procedure Call(�������� ������ ȣ���ϴ�)
        photonView.RPC("ApplayPlayerList", RpcTarget.All);

    }

    [PunRPC]
    public void ApplyPlayerList()
    {
        // ���� �濡 ������ �ִ� �÷��̾��� ��
        Debug.LogError("CurrentRoom PlayerCount : " + PhotonNetwork.CurrentRoom.PlayerCount);

        // ���� �����Ǿ� �ִ� ��� ����� ��������
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        // �Ź� �������� �ϴ°� �����Ƿ� �÷��̾� ���ӿ�����Ʈ ����Ʈ�� �ʱ�ȭ
        // ������ ������ �ٸ��⶧���� 
        System.Array.Clear(playerGoList, 0, playerGoList.Length);

        // ���� �����Ǿ� �ִ� ����� ��ü��
        // �������� �÷��̾���� ���ͳѹ��� ����,
        // �÷��̾� ���ӿ�����Ʈ ����Ʈ�� �߰�
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; ++i)
        {
            // Ű�� 0�� �ƴ� 1���� ����
            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {
                // ���� PhotonNetwork.Instantiate�� ���ؼ� ������ ����䰡 �ƴ϶�� �ѱ�
                if (photonViews[j].isRuntimeInstantiated == false) continue;
                // ���� ���� Ű ���� ��ųʸ� ���� �������� �ʴ´ٸ� �ѱ�
                // ContainsKey - �ش�Ű�� �����ϰ� �ִ��� �˻�. = �÷��̾� ����߿��� Ű���� �ִ����� ���°�. ������ �ǳʶپ�
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;

                // ������� ���ͳѹ�
                int viewNum = photonViews[j].Owner.ActorNumber;
                // �������� �÷��̾��� ���ͳѹ� 
                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;

                // ���ͳѹ��� ���� ������Ʈ�� �ִٸ�,
                if (viewNum == playerNum)
                {
                    // ���� ���ӿ�����Ʈ�� �迭�� �߰�
                    playerGoList[playerNum - 1] = photonViews[j].gameObject;
                    // ���ӿ�����Ʈ �̸��� �˾ƺ��� ���� ����
                    playerGoList[playerNum - 1].name = "Player_" + photonViews[j].gameObject;
                }

            }
        }

        // ����׿�
        PrintPlayerList();
    }

    private void PrintPlayerList()
    {
        foreach (GameObject go in playerGoList)
        {
            if (go != null)
            {
                Debug.LogError(go.name);
            }
        }
    }


    // �÷��̾ ���� �� ȣ��Ǵ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Left Room: {0}", otherPlayer.NickName);
    }

    public void LeaveRoom()
    {
        Debug.Log("Leave Room");

        PhotonNetwork.LeaveRoom();
    }
}
