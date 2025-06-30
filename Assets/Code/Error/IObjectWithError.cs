using System;

namespace Code.Error
{
    public interface IObjectWithError 
    {
        public event Action<string> OnError;
    }
}
