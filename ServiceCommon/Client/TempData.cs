using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data.SqlServerCe;

namespace DbService.Client
{
    [KnownType(typeof(KnowledgeSyncProvider))]
    [Serializable]   
    public class TempData : SqlCeSyncProvider
    {    
     
    }
}
