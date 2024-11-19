using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;


namespace DS_ripoff
{
    public class TitleScreenManager : MonoBehaviour
    {
        public void StartNetworkAsHost()
        {
            //NetworkManager.Singleton.StartHost();

        }
        public void StartPlayerSelect()
        {
            StartCoroutine(WorldSaveGameManager.instance.LoadPlayerSelectScene());
        }

    }
}