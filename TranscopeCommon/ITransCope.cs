using System;
using System.Collections.Generic;
using System.Text;

namespace TranscopeCommon
{
    public interface ITransCope
    {
        TranScopeDemoResultDTO ProcessWthoutTrnasaction(int ProcessRecordCount);
    }
}
