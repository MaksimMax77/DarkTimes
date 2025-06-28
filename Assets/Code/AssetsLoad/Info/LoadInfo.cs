using UnityEngine;

namespace Code.AssetsLoad.Info
{
    public class LoadInfo: ScriptableObject
    {
        [SerializeField] private string _description;
        [SerializeField] private string _errorMessage;
        
        public string Description => _description;
        public string ErrorMessage => _errorMessage;
    }
}