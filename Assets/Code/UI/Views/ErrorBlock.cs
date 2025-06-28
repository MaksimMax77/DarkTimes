using TMPro;
using UnityEngine;

namespace Code.UI.Views
{
    public class ErrorBlock : MonoBehaviour //todo add a button to restart the operation
    {
        [SerializeField] private TMP_Text _tmpText;

        public void SetMessage(string msg)
        {
            _tmpText.text = msg;
        }
    }
}