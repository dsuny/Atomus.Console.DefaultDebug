using Atomus.Console.Login.Controllers;
using Atomus.Console.Login.Models;
using Atomus.Console.Menu.Controllers;
using Atomus.Console.Menu.Models;
using Atomus.Control;
using System;
using System.Data;

namespace Atomus.Console
{
    public partial class DefaultDebug : IAction
    {
        private IAction userControl;
        private AtomusControlEventHandler beforeActionEventHandler;
        private AtomusControlEventHandler afterActionEventHandler;

        private string menuDatabaseName;
        private string menuProcedureID;

        #region Init
        public DefaultDebug()
        {
            this.Translator().TargetCultureName = "ko-KR";
        }
        #endregion

        #region IO
        object IAction.ControlAction(ICore sender, AtomusControlArgs e)
        {
            IAction userControl;
            object[] objects;

            try
            {
                if (e.Action != "start" && e.Action != "AddUserControl" && e.Action != "Login" && e.Action != "Close")
                    this.beforeActionEventHandler?.Invoke(this, e);

                switch (e.Action)
                {
                    case "start":
                        try
                        {
                            this.beforeActionEventHandler.Invoke(this, new AtomusControlEventArgs() { Action = "Login" });
                            this.beforeActionEventHandler.Invoke(this, new AtomusControlEventArgs() { Action = "AddUserControl" });
                        }
                        catch (Exception exception)
                        {
                            System.Console.WriteLine(exception.Message);
                            System.Console.ReadLine();
                            System.Windows.Forms.Application.Exit();
                        }

                        return true;
                    case "Login":
                        objects = (object[])e.Value;

                        this.Login(new DefaultLoginSearchModel()
                        {
                            DatabaseName = (string)objects[0],
                            ProcedureID = (string)objects[1],
                            EMAIL = (string)objects[2],
                            ACCESS_NUMBER = (string)objects[3]
                        });
                        return true;

                    case "AddUserControl":
                        objects = (object[])e.Value;

                        this.menuDatabaseName = (string)objects[4];
                        this.menuProcedureID = (string)objects[5];

                        userControl = (IAction)objects[9];

                       this.OpenControl(
                             (string)objects[4],
                            (string)objects[5],
                            //MENU_NAME = (string)_object[6],
                            (decimal)objects[7],
                            (decimal)objects[8]
                        , userControl, null, null, true);

                        return true;

                    default:
                        if (this.userControl != null)
                            this.userControl.ControlAction(this, e);

                        return true;
                }
            }
            finally
            {
                if (e.Action != "start" && e.Action != "AddUserControl" && e.Action != "Login" && e.Action != "Close")
                    this.afterActionEventHandler?.Invoke(this, e);
            }
        }

        private void UserControl_BeforeActionEventHandler(ICore sender, AtomusControlEventArgs e) { }
        private void UserControl_AfterActionEventHandler(ICore sender, AtomusControlEventArgs e)
        {
            object[] objects;

            try
            {
                switch (e.Action)
                {
                    case "UserControl.OpenControl" :
                        objects = (object[])e.Value;

                        //                             _MENU_ID, _ASSEMBLY_ID, sender, AtomusControlArgs
                        //_DatabaseName, _ProcedureID, _MENU_ID, _ASSEMBLY_ID, _Core, sender, AtomusControlArgs
                        this.OpenControl(this.menuDatabaseName, this.menuProcedureID,  (decimal)objects[0], (decimal)objects[1], null, sender, (objects[2] == null) ? null : (AtomusControlEventArgs)objects[2], true);

                        break;

                    case "UserControl.GetControl" :
                        objects = (object[])e.Value;//_MENU_ID, _ASSEMBLY_ID, sender, AtomusControlArgs

                        e.Value = this.OpenControl(this.menuDatabaseName, this.menuProcedureID, (decimal)objects[0], (decimal)objects[1], null, sender, (objects[2] == null) ? null : (AtomusControlEventArgs)objects[2], false);
                        break;

                        //default:
                        //    throw new AtomusException("'{0}'은 처리할 수 없는 Action 입니다.".Translate(e.Action));
                }
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.Message);
                //this.MessageBoxShow(this, exception);
            }
        }

        private bool Login(DefaultLoginSearchModel defaultLoginSearch)
        {
            Service.IResponse result;

            try
            {
                result = this.Search(defaultLoginSearch);

                if (result.Status == Service.Status.OK)
                {
                    if (result.DataSet != null && result.DataSet.Tables.Count >= 1)
                        foreach (DataTable _DataTable in result.DataSet.Tables)
                            for (int i = 1; i < _DataTable.Columns.Count; i++)
                                foreach (DataRow _DataRow in _DataTable.Rows)
                                    Config.Client.SetAttribute(string.Format("{0}.{1}", _DataRow[0].ToString(), _DataTable.Columns[i].ColumnName), _DataRow[i]);


                    return true;
                }
                else
                {
                    System.Console.WriteLine(result.Message);
                    return false;
                }
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.Message);
            }
            finally
            {
            }

            return false;
        }

        private ICore OpenControl(string databaseName, string procedureID, decimal MENU_ID, decimal ASSEMBLY_ID, IAction core, ICore sender, AtomusControlEventArgs atomusControlEventArgs, bool addTabControl)
        {
            Service.IResponse result;

            try
            {
                result = this.SearchOpenControl(new DefaultMenuSearchModel()
                {
                    DatabaseName = databaseName,
                    ProcedureID = procedureID,
                    MENU_ID = MENU_ID,
                    ASSEMBLY_ID = ASSEMBLY_ID
                });

                if (result.Status == Service.Status.OK)
                {
                    if (result.DataSet.Tables.Count == 2)
                        if (result.DataSet.Tables[0].Rows.Count == 1)
                        {
                            if (core == null)
                            {
                                if (result.DataSet.Tables[0].Columns.Contains("FILE_TEXT") && result.DataSet.Tables[0].Rows[0]["FILE_TEXT"] != DBNull.Value)
                                    core = (IAction)Factory.CreateInstance(Convert.FromBase64String((string)result.DataSet.Tables[0].Rows[0]["FILE_TEXT"]), result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);
                                else
                                    core = (IAction)Factory.CreateInstance((byte[])result.DataSet.Tables[0].Rows[0]["FILE"], result.DataSet.Tables[0].Rows[0]["NAMESPACE"].ToString(), false, false);

                            }

                            core.BeforeActionEventHandler += UserControl_BeforeActionEventHandler;
                            core.AfterActionEventHandler += UserControl_AfterActionEventHandler;

                            core.SetAttribute("MENU_ID", MENU_ID.ToString());
                            core.SetAttribute("ASSEMBLY_ID", ASSEMBLY_ID.ToString());

                            foreach (DataRow _DataRow in result.DataSet.Tables[1].Rows)
                            {
                                core.SetAttribute(_DataRow["ATTRIBUTE_NAME"].ToString(), _DataRow["ATTRIBUTE_VALUE"].ToString());
                            }

                            if (addTabControl)
                            {
                                this.userControl = core;
                                this.userControl.ControlAction(this, "init", null);
                            }

                            if (atomusControlEventArgs != null)
                                core.ControlAction(sender, atomusControlEventArgs.Action, atomusControlEventArgs.Value);
                        }

                    return core;
                }
                else
                {
                    System.Console.WriteLine(result.Message);
                    return null;
                }
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.Message);
                return null;
            }
            finally
            {
            }
        }
        #endregion

        #region Event
        event AtomusControlEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusControlEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        #endregion

        #region ETC
        private bool ApplicationExit()
        {
            string tmp;

            System.Console.WriteLine("종료하시겠습니까? (y or n)");
            tmp = System.Console.ReadLine();

            if (tmp == "y")
            {
                System.Windows.Forms.Application.Exit();
                return true;
            }
            else
                return false;
        }
        #endregion
    }
}