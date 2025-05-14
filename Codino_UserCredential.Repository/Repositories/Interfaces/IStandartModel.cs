using System;
using Codino_UserCredential.Core.Enums;

namespace Codino_UserCredential.Repository.Repositories.Interfaces
{
    public interface IStandartModel<K> : IBaseModel where K : struct
    {
        K id { get; set; }
        Status StatusId { get; set; }
        int CreaUserId { get; set; }
        DateTime creaDateTime { get; set; }
        DateTime ModifDate { get; set; }
    }
}