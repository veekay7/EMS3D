using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public RectTransform m_PopupboxParentTransform;

    public void Prompt()
    {
        var box = PopupboxFactory.Instance.Create<GUIPopupConfirm>(m_PopupboxParentTransform);

        if (m_PopupboxParentTransform != null)
            box.m_RectTransform.SetAsLastSibling();

        box.Open("Are you sure you want to quit?", null, null, (response) =>
        {
            if (response == 0)
                GameSystem.QuitGame();
        });
    }
}
