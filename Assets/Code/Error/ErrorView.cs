using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Error
{
    public class ErrorView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _restartButton;
        
        public Button RestartButton => _restartButton;

        public void SetDescription(string description)
        {
            _description.text = description;
        }
    }
}