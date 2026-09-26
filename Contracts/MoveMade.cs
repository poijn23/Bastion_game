using System.Runtime.Serialization;

[DataContract]
public sealed record MoveMade([property: DataMember] int Row, [property: DataMember] int column);