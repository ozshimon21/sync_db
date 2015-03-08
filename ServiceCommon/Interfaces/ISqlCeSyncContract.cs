using System.ServiceModel;
using Microsoft.Synchronization.Data;

namespace DbService.Interfaces
{
    [ServiceContract(SessionMode=SessionMode.Required)]
    public interface ISqlCeSyncContract : IRelationalSyncContract
    {
        [OperationContract]
        [FaultContract(typeof(WebSyncFaultException))]
        void CreateScopeDescription(DbSyncScopeDescription scopeDescription);

        [OperationContract]
        [FaultContract(typeof(WebSyncFaultException))]
        void DeleteScopeDescription(string scopeName);

        [OperationContract]
        [FaultContract(typeof(WebSyncFaultException))]
        DbSyncScopeDescription GetScopeDescription(string scopeName);

        [OperationContract]
        [FaultContract(typeof(WebSyncFaultException))]
        bool NeedsScope();
    }
}
