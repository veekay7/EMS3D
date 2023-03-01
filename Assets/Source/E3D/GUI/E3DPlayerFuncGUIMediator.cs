using UnityEngine;

namespace E3D
{
    public class E3DPlayerFuncGUIMediator : MonoBehaviour
    {
        public void ReturnToGame()
        {
            if (E3DPlayer.Local != null)
                E3DPlayer.Local.UnPause();
        }

        public void DisconnectFromGame()
        {
            if (E3DPlayer.Local != null)
                E3DPlayer.Local.Disconnect();
        }
    }
}
