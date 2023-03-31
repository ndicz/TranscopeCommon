using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using R_Common;
using TranScopeBack;
using TranscopeCommon;
using TransscopeBack;

namespace TransScopeService
{
  
        [Route("api/[controller]/[action]")]
        [ApiController]
        public class TranScopeController : ControllerBase, ITransCope
        {
            [HttpPost]
            public TranScopeDemoResultDTO ProcessWthoutTrnasaction(int ProcessRecordCount)
            {
            R_Exception loException = new R_Exception();
            TranScopeDemoResultDTO loRtn = null;
            TranScopeCls loCls;

            try
            {
                loCls = new TranScopeCls();
                loRtn = new TranScopeDemoResultDTO();
                loRtn.data = loCls.ProcessWithoutTransactionDB(ProcessRecordCount);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            EndBlock:
            loException.ThrowExceptionIfErrors();

            return loRtn;
        }
        }
    }
