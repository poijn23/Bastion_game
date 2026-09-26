using System.Runtime.Serialization;

namespace Bastion.Contracts;

[DataContract]
public sealed record LoginResult([property: DataMember] bool IsSucceful, [property: DataMember] Guid? AccountId);
