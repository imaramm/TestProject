using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab = null;

    // 각 클라이언트마다 생성된 플레이어 게임 오브젝트를 배열로 관리한다.
    private GameObject[] playerGoList = new GameObject[4];


    private void Start()
    {
        if (playerPrefab != null)
        {
            // photonNetwork안에 instantiate를 만드는거. 서버에 똑같이 만들어진다.
            // 새로운사람이 들어와도 포톤에서 미리 복제해놨다가 제공하는?
            GameObject go = PhotonNetwork.Instantiate(
                // 프리팹을 던지는게 아니라, 이름을 던져줘야한다
                playerPrefab.name, //"P_Player"  ← 이렇게 던져줘도 된다
                new Vector3(
                    Random.Range(-10.0f, 10.0f),
                    0.0f,
                    Random.Range(-10.0f, 10.0f)),
                Quaternion.identity,
                0);
            // 플레이어마다 색깔 바꾸는 코드. 
            // 몇번째 색깔을 쓸건지. (포톤네트워크의-현재방의-플레이어 수) 순서대로 들어와서 색깔 가져감 / 배열로 
            go.GetComponent<PlayerCtrl>().SetMaterial(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    // PhotonNetwork.LeaveRoom 함수가 호출되면 호출
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");

        // 방을 떠났을때 런처씬으로 돌아가겠다
        SceneManager.LoadScene("Launcher");
    }

    // 플레이어가 입장할 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        // 모든 플레이어가 ↓ 이거를 받는다. 누가 들어왔는지 알수있는 
        Debug.LogFormat("Player Entered Room: {0}",
                        otherPlayer.NickName);

        // 누군가 접속하면 전체 클라이언트에서 원격으로 함수 호출
        // 새로운 애가 들어왔으니깐 목록을 새로 갱신,호출
        // 인스펙터창에서 photon view가 있어야 이거를 던질수가있다.
        // RPC : Remote Procedure Call(원격으로 뭔가를 호출하는)
        photonView.RPC("ApplayPlayerList", RpcTarget.All);

    }

    [PunRPC]
    public void ApplyPlayerList()
    {
        // 현재 방에 접속해 있는 플레이어의 수
        Debug.LogError("CurrentRoom PlayerCount : " + PhotonNetwork.CurrentRoom.PlayerCount);

        // 현재 생성되어 있는 모든 포톤뷰 가져오기
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        // 매번 재정렬을 하는게 좋으므로 플레이어 게임오브젝트 리스트를 초기화
        // 들어오는 순서가 다르기때문에 
        System.Array.Clear(playerGoList, 0, playerGoList.Length);

        // 현재 생성되어 있는 포톤뷰 전체와
        // 접속중인 플레이어들의 액터넘버를 비교해,
        // 플레이어 게임오브젝트 리스트에 추가
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; ++i)
        {
            // 키는 0이 아닌 1부터 시작
            int key = i + 1;
            for (int j = 0; j < photonViews.Length; ++j)
            {
                // 만약 PhotonNetwork.Instantiate를 통해서 생성된 포톤뷰가 아니라면 넘김
                if (photonViews[j].isRuntimeInstantiated == false) continue;
                // 만약 현재 키 값이 딕셔너리 내에 존재하지 않는다면 넘김
                // ContainsKey - 해당키를 보유하고 있는지 검사. = 플레이어 목록중에서 키값이 있는지를 보는거. 없으면 건너뛰어
                if (PhotonNetwork.CurrentRoom.Players.ContainsKey(key) == false) continue;

                // 포톤뷰의 액터넘버
                int viewNum = photonViews[j].Owner.ActorNumber;
                // 접속중인 플레이어의 액터넘버 
                int playerNum = PhotonNetwork.CurrentRoom.Players[key].ActorNumber;

                // 액터넘버가 같은 오브젝트가 있다면,
                if (viewNum == playerNum)
                {
                    // 실제 게임오브젝트를 배열에 추가
                    playerGoList[playerNum - 1] = photonViews[j].gameObject;
                    // 게임오브젝트 이름도 알아보기 쉽게 변경
                    playerGoList[playerNum - 1].name = "Player_" + photonViews[j].gameObject;
                }

            }
        }

        // 디버그용
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


    // 플레이어가 나갈 때 호출되는 함수
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
