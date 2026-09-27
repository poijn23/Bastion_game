using System.Runtime.Serialization;

namespace Bastion.Contracts;


[DataContract]
public sealed record MoveMade([property: DataMember] int Row, [property: DataMember] int Column);