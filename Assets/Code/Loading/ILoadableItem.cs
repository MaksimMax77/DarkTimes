using System;

namespace Code.Loading
{
    public interface ILoadableItem
    {
        public event Action<float> OnProgressChanged;
        public string Description { get; set; }
    }
}