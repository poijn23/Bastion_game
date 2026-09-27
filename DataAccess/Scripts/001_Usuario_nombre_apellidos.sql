-- Nombre y apellidos llegaron despues del modelo original, asi que la tabla
-- Usuario no los tenia y el registro los pedia sin poder guardarlos.
--
-- Van NULL porque las filas que ya existen no los traen y porque una cuenta
-- de invitado no tiene ninguno: D-06 solo los exige al registrarse.
-- Idempotente: se puede volver a correr sin romper nada.

IF COL_LENGTH('dbo.Usuario', 'nombre') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario ADD nombre nvarchar(50) NULL;
END;
GO

-- Dos apellidos caben en cien caracteres; el modelo guarda los dos juntos
-- porque no se consultan por separado.
IF COL_LENGTH('dbo.Usuario', 'apellidos') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario ADD apellidos nvarchar(100) NULL;
END;
GO

-- Un invitado no da nombre, igual que no da correo ni contrasena
-- (misma idea que CK_Usuario_credenciales).
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Usuario_nombre_invitado')
BEGIN
    ALTER TABLE dbo.Usuario ADD CONSTRAINT CK_Usuario_nombre_invitado CHECK
    (
        tipo_cuenta <> 'INVITADO'
        OR (nombre IS NULL AND apellidos IS NULL)
    );
END;
GO
