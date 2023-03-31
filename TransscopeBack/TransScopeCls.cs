using System.Data;
using System.Data.Common;
using R_BackEnd;
using R_Common;
using TranscopeCommon;

namespace TransscopeBack
{
    public class TransScopeCls
    {
        public TransScopeDataDTO ProcessWithoutTransactionDB(int poProcessRecordCount)
        {
            R_Exception loException = new();
            TransScopeDataDTO loRtn = new();
            List<CustomerDbDTO> Customers;
            try
            {
                Customers = GetAllCustomer(poProcessRecordCount);
                RemoveAllCustomer(Customers);
                AddAllCopyCustomer(Customers);
            }
            catch (Exception e)
            {
                loException.Add(e);
            }
            EndBlock:
            loException.ThrowExceptionIfErrors();

            return loRtn;
        }

        private List<CustomerDbDTO> GetAllCustomer(int pnCount)
        {
            R_Exception loException = new();
            R_Db loDb;
            DbConnection loConn = null;
            List<CustomerDbDTO> loRtn = null;
            string lcCmd;
            string lcCust;

            try
            {
                lcCust = String.Format("Cust{0}", pnCount.ToString("0000"));
                lcCmd = $"select * from TestCustomer(nolock) where CustomerID <= 'Cust{lcCust}';";
                loDb = new R_Db();
                loRtn = loDb.SqlExecObjectQuery<CustomerDbDTO>(lcCmd, loDb.GetConnection(), true);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            EndBlock:
            loException.ThrowExceptionIfErrors();

            return loRtn;
        }

        private void RemoveAllCustomer(List<CustomerDbDTO> poCustomers)
        {
            R_Exception loException = new R_Exception();
            R_Db loDb = null;
            DbConnection loConn = null;
            DbCommand loCommand;
            DbParameter loDbParameter;
            string lcCmd;

            try
            {
                loDb = new R_Db();
                loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();
                loDb.R_AddCommandParameter(loCommand, "StrPar1", DbType.String, 50, "");
                loDbParameter = loCommand.Parameters[0];

                foreach (CustomerDbDTO item in poCustomers)
                {
                    lcCmd = "DELETE FROM TestCustomer WHERE CustomerID = @StrPar1";
                    loCommand.CommandText = lcCmd;
                    loDbParameter.Value = item.CustomerID;
                    loDb.SqlExecNonQuery(loConn, loCommand, false);

                    lcCmd = "INSERT INTO TestCustomerLog(Log) VALUES (@StrPar1)";
                    loCommand.CommandText = lcCmd;
                    loDbParameter.Value = $"Remove Customer {item.CustomerID}";
                    loDb.SqlExecNonQuery(loConn, loCommand, false);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed)
                    {
                        loConn.Close();
                    }

                    loConn.Dispose();
                }
            }

            EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        private void AddAllCopyCustomer(List<CustomerDbDTO> poCustomers)
        {
            R_Exception loException = new R_Exception();
            DbConnection loConn = null;
            DbCommand loCommand;
            R_Db loDb = null;

            DbParameter loDbParameter1;
            DbParameter loDbParameter2;
            DbParameter loDbParameter3;

            string lcCmd;

            try
            {
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();

                loDb.R_AddCommandParameter(loCommand, "CustomerID", DbType.String, 50, "");
                loDb.R_AddCommandParameter(loCommand, "CustomerName", DbType.String, 50, "");
                loDb.R_AddCommandParameter(loCommand, "ContactName", DbType.String, 50, "");

                loDbParameter1 = loCommand.Parameters["CustomerID"];
                loDbParameter2 = loCommand.Parameters["CustomerName"];
                loDbParameter3 = loCommand.Parameters["ContactName"];

                foreach (CustomerDbDTO item in poCustomers)
                {
                    lcCmd = "INSERT INTO TestCopyCustomer(CustomerID, CustomerName, ContactName) VALUES (@CustomerID, @CustomerName, @ContactName)";
                    loCommand.CommandText = lcCmd;

                    loDbParameter1.Value = item.CustomerID;
                    loDbParameter2.Value = item.CustomerName;
                    loDbParameter3.Value = item.ContactName;

                    loDb.SqlExecNonQuery(loConn, loCommand, false);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed)
                    {
                        loConn.Close();
                    }

                    loConn.Dispose();
                }
            }

        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

    }
}