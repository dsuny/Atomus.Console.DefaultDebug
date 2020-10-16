namespace Atomus.Console.Login.Models
{
    public class DefaultLoginSearchModel : Mvc.Models
    {
        public string DatabaseName { get; set; }
        public string ProcedureID { get; set; }
        public string EMAIL { get; set; }
        public string ACCESS_NUMBER { get; set; }
    }
}