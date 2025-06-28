using UnityEngine;

namespace Code.AssetsLoad.Info
{
    public class LoadInfo: ScriptableObject
    {
        [SerializeField] private string _description;
        
        public string Description => _description;
    }
}