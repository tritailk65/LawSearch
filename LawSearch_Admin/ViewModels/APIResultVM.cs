using System.Runtime.Serialization;

namespace LawSearch_Admin.ViewModels
{
    public class APIResultVM<T>
    {
        public int Status { set; get; }
        public string Message { set; get; } 
        public string Exception { set; get; }
        public List<T> Data { set; get; }         
        
        public APIResultVM() { }

        
    }
}
