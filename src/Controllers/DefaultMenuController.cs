using Atomus.Console.Menu.Models;
using Atomus.Database;
using Atomus.Service;
using System.Threading.Tasks;

namespace Atomus.Console.Menu.Controllers
{
    internal static class DefaultMenuController
    {
        internal static IResponse SearchOpenControl(this ICore core, DefaultMenuSearchModel search)
        {
            IServiceDataSet serviceDataSet;

            serviceDataSet = new ServiceDataSet
            {
                ServiceName = core.GetAttribute("ServiceName"),
                TransactionScope = false
            };
            serviceDataSet["OpenControl"].ConnectionName = search.DatabaseName;
            serviceDataSet["OpenControl"].CommandText = search.ProcedureID;
            //serviceDataSet["OpenControl"].SetAttribute("DatabaseName", search.DatabaseName);
            //serviceDataSet["OpenControl"].SetAttribute("ProcedureID", search.ProcedureID);
            serviceDataSet["OpenControl"].AddParameter("@MENU_ID", DbType.Decimal, 18);
            serviceDataSet["OpenControl"].AddParameter("@ASSEMBLY_ID", DbType.Decimal, 18);
            serviceDataSet["OpenControl"].AddParameter("@USER_ID", DbType.Decimal, 18);

            serviceDataSet["OpenControl"].NewRow();
            serviceDataSet["OpenControl"].SetValue("@MENU_ID", search.MENU_ID);
            serviceDataSet["OpenControl"].SetValue("@ASSEMBLY_ID", search.ASSEMBLY_ID);
            serviceDataSet["OpenControl"].SetValue("@USER_ID", Config.Client.GetAttribute("Account.USER_ID"));

            return core.ServiceRequest(serviceDataSet);
        }
        internal static async Task<IResponse> SearchOpenControlAsync(this ICore core, DefaultMenuSearchModel search)
        {
            IServiceDataSet serviceDataSet;

            serviceDataSet = new ServiceDataSet
            {
                ServiceName = core.GetAttribute("ServiceName"),
                TransactionScope = false
            };
            serviceDataSet["OpenControl"].ConnectionName = search.DatabaseName;
            serviceDataSet["OpenControl"].CommandText = search.ProcedureID;
            //serviceDataSet["OpenControl"].SetAttribute("DatabaseName", search.DatabaseName);
            //serviceDataSet["OpenControl"].SetAttribute("ProcedureID", search.ProcedureID);
            serviceDataSet["OpenControl"].AddParameter("@MENU_ID", DbType.Decimal, 18);
            serviceDataSet["OpenControl"].AddParameter("@ASSEMBLY_ID", DbType.Decimal, 18);
            serviceDataSet["OpenControl"].AddParameter("@USER_ID", DbType.Decimal, 18);

            serviceDataSet["OpenControl"].NewRow();
            serviceDataSet["OpenControl"].SetValue("@MENU_ID", search.MENU_ID);
            serviceDataSet["OpenControl"].SetValue("@ASSEMBLY_ID", search.ASSEMBLY_ID);
            serviceDataSet["OpenControl"].SetValue("@USER_ID", Config.Client.GetAttribute("Account.USER_ID"));

            return await core.ServiceRequestAsync(serviceDataSet);
        }
    }
}