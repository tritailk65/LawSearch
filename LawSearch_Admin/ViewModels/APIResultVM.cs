namespace LawSearch_Admin.ViewModels
{
    public class APIResultVM
    {
        public int Status { set; get; }
        public string? Message { set; get; }
        public string? Data { set; get; }
    }

    public class APIResultVM<T>
    {
        public int Status { set; get; }
        public string Message { set; get; } 
        public string Exception { set; get; }
        public List<T> Data { set; get; }         
        
        public APIResultVM() { }

        
    }

    public class APIResultSingleVM<T>
    {
        public int Status { set; get; }
        public string Message { set; get; }
        public string Exception { set; get; }
        public T Data { set; get; }

        public APIResultSingleVM()
        {
        }
    }
}
