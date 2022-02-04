using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerListPopup : MonoBehaviour
{
    private void OnEnable()
    {
        Managers.Instance.GetScene<GameScene>().SendPlayerList();
    }


}
