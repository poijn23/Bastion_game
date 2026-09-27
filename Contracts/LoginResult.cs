using System.Runtime.Serialization;

namespace Bastion.Contracts;

[DataContract]
public sealed record LoginResult([property: DataMember] bool IsSuccessful, [property: DataMember] Guid? AccountId);
