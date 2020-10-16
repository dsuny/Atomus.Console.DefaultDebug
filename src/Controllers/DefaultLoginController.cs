using Atomus.Console.Login.Models;
using Atomus.Database;
using Atomus.Service;

namespace Atomus.Console.Login.Controllers
{
    internal static class DefaultLoginController
    {
        internal static IResponse Search(this ICore core, DefaultLoginSearchModel search)
        {
            IServiceDataSet serviceDataSet;

            serviceDataSet = new ServiceDataSet { ServiceName = core.GetAttribute("ServiceName") };
            serviceDataSet["LOGIN"].ConnectionName = search.DatabaseName;
            serviceDataSet["LOGIN"].CommandText = search.ProcedureID;
            //serviceDataSet["LOGIN"].SetAttribute("DatabaseName", search.DatabaseName);
            //serviceDataSet["LOGIN"].SetAttribute("ProcedureID", search.ProcedureID);
            serviceDataSet["LOGIN"].AddParameter("@EMAIL", DbType.NVarChar, 100);
            serviceDataSet["LOGIN"].AddParameter("@ACCESS_NUMBER", DbType.NVarChar, 4000);

            serviceDataSet["LOGIN"].NewRow();
            serviceDataSet["LOGIN"].SetValue("@EMAIL", search.EMAIL);
            serviceDataSet["LOGIN"].SetValue("@ACCESS_NUMBER", search.ACCESS_NUMBER);

            return core.ServiceRequest(serviceDataSet);
        }
    }
}