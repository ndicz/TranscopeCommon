using System.Data;
using System.Data.Common;

using R_BackEnd;
using R_Common;

using TranscopeCommon;
using TransscopeBack;

namespace TranScopeBack
{
    public class TranScopeCls
    {
        public TransScopeDataDTO ProcessWithoutTransactionDB(int poProcessRecordCount)
        {
            R_Exception loException = new R_Exception();
            TransScopeDataDTO loRtn = new TransScopeDataDTO();
            List<CustomerDbDTO> Customers = null;

            try
            {
                Customers = GetAllCustomer(poProcessRecordCount);
                RemoveAllCustomer(Customers);
                AddAllCopyCustomer(Customers);

                loRtn.IsSuccess = true;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();

            return loRtn;
        }

        private List<CustomerDbDTO> GetAllCustomer(int pnCount)
        {
            R_Exception loException = new R_Exception();
            R_Db loDb = null;
            List<CustomerDbDTO> LoRtn = null;
            string lcCust;
            string lcCmd;

            try
            {
                lcCust = String.Format("Cust{0}", pnCount.ToString("0000"));
                lcCmd = "SELECT * FROM TestCustomer(NoLock) " +
                    "WHERE CustomerId <= {0};";
                loDb = new R_Db();
                LoRtn = loDb.SqlExecObjectQuery<CustomerDbDTO>(lcCmd, lcCust);

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();

            return LoRtn;

        }

        private void RemoveAllCustomer(List<CustomerDbDTO> poCustomer)
        {
            R_Exception loException = new R_Exception();
            R_Db loDb = null;
            DbConnection loConn = null;
            DbCommand loCommand;
            string lcCmd;
            DbParameter loDbParameter;

            try
            {
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();
                loDb.R_AddCommandParameter(loCommand, "StrPar1", DbType.String, 50, "");
                loDbParameter = loCommand.Parameters[0];

                foreach (CustomerDbDTO item in poCustomer)
                {
                    lcCmd = "DELETE FROM TestCustomer WHERE CustomerID = @StrPar1;";
                    loCommand.CommandText = lcCmd;
                    loDbParameter.Value = item.CustomerID;
                    loDb.SqlExecNonQuery(loConn, loCommand, false);

                    lcCmd = "INSERT INTO TestCustomerLog(Log) VALUES (@StrPar1);";
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
                    if (loConn.State != ConnectionState.Closed) loConn.Close();
                    loConn.Dispose();
                }
            }

        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        private void AddAllCopyCustomer(List<CustomerDbDTO> poCustomer)
        {
            R_Exception loException = new R_Exception();
            DbConnection loConn = null;
            DbCommand loCommand;
            R_Db loDb = null;
            string lcCmd;

            DbParameter loDbParCustomerID;
            DbParameter loDbParCustomerName;
            DbParameter loDbParContactName;

            try
            {
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();

                loDb.R_AddCommandParameter(loCommand, "CustomerID", DbType.String, 50, "");
                loDb.R_AddCommandParameter(loCommand, "CustomerName", DbType.String, 50, "");
                loDb.R_AddCommandParameter(loCommand, "ContactName", DbType.String, 50, "");

                loDbParCustomerID = loCommand.Parameters["CustomerID"];
                loDbParCustomerName = loCommand.Parameters["CustomerName"];
                loDbParContactName = loCommand.Parameters["ContactName"];

                lcCmd = "INSERT INTO TestCopyCustomer(CustomerID, CustomerName, ContactName) VALUES (@CustomerID, @CustomerName, @ContactName)";
                loCommand.CommandText = lcCmd;

                foreach (var item in poCustomer)
                {
                    loDbParCustomerID.Value = item.CustomerID;
                    loDbParCustomerName.Value = item.CustomerName;
                    loDbParContactName.Value = item.ContactName;
                    loDb.SqlExecNonQuery(loConn, loCommand, false);

                    //lcCmd = "insert into TestCustomerLog(Log) Values(@CustomerID)";
                    //loCommand.CommandText = lcCmd;
                    //loDbParCustomerID.Value = $"Remove Customer {item.CustomerID}";
                    //loDb.SqlExecNonQuery(loConn, loCommand, false);
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
                    if (loConn.State != ConnectionState.Closed) loConn.Close();
                    loConn.Dispose();
                }
            }

        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

    }
}