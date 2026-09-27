namespace Bastion.Managers;

// Lo que la base contesto al intentar escribir la fila. Quien decide si un
// nickname esta tomado es el indice unico, no una pregunta hecha antes.
public enum AccountInsert
{
    Written,
    NicknameTaken,
    EmailTaken,
    FriendCodeTaken
}
