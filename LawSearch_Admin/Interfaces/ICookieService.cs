namespace LawSearch_Admin.Interfaces
{
    public interface ICookieService
    {
        public Task SetValue(string key, string value, int? days = null);
        public Task<string> GetValue(string key, string def = "");
        public Task DeleteAllValue();
    }
}
