﻿using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;
using DbService.Client;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;

namespace DbService.Interfaces

{
    [ServiceContract(SessionMode=SessionMode.Required)]
    [ServiceKnownType(typeof(SyncIdFormatGroup))]
    [ServiceKnownType(typeof(DbSyncContext))]
    [ServiceKnownType(typeof(SyncSchema))]
    [ServiceKnownType(typeof(WebSyncFaultException))]
    [ServiceKnownType(typeof(SyncBatchParameters))]
    [ServiceKnownType(typeof(GetChangesParameters))]
    [ServiceKnownType(typeof(RelationalSyncProvider))]
    //[ServiceKnownType(typeof(ClientData))]
    public interface IRelationalSyncContract
    {
        
        [OperationContract(IsInitiating=true)]
        void Initialize(string scopeName, string hostName);

        [OperationContract]
        void BeginSession(SyncProviderPosition position);

        [OperationContract]
        SyncBatchParameters GetKnowledge();

        [OperationContract]
        GetChangesParameters GetChanges(uint batchSize, SyncKnowledge destinationKnowledge);

        [OperationContract]
        SyncSessionStatistics ApplyChanges(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges, object changeData);

        [OperationContract]
        bool HasUploadedBatchFile(string batchFileid, string remotePeerId);

        [OperationContract]
        void UploadBatchFile(string batchFileid, byte[] batchFile, string remotePeerId);

        [OperationContract]
        byte[] DownloadBatchFile(string batchFileId);

        [OperationContract]
        void EndSession();

        [OperationContract(IsTerminating= true)]
        void Cleanup();
    }

//    [KnownType(typeof(TempData))]
//    [DataContract]
//    public class ClientData
//    {
//        [DataMember]
//        public TempData data;
//    }
//
//    [DataContract]
//    [KnownType(typeof(KnowledgeSyncProvider))]
//    public class ClientData
//    {
//        [DataMember]
//        public KnowledgeSyncProvider ClientProvider;
//    }

    [DataContract]
    public class SyncBatchParameters
    {
        [DataMember]
        public SyncKnowledge DestinationKnowledge;

        [DataMember]
        public uint BatchSize;
    }

    [DataContract]
    [KnownType(typeof(DataSet))]
    public class GetChangesParameters
    {
        [DataMember]
        public object DataRetriever;

        [DataMember]
        public ChangeBatch ChangeBatch;
    }
}
