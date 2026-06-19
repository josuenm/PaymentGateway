USE PaymentGatewayAuth;
GO

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 'rol_3xM9vL2kZ8pW4tRb')
BEGIN
    INSERT INTO Roles (Id, Name) VALUES ('rol_3xM9vL2kZ8pW4tRb', 'admin')
END;

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 'rol_G7nF2wX9mK1pL6zQ')
BEGIN
    INSERT INTO Roles (Id, Name) VALUES ('rol_G7nF2wX9mK1pL6zQ', 'client')
END;

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Id = 'rol_vP5mK8xQ3qL2bW9s')
BEGIN
    INSERT INTO Roles (Id, Name) VALUES ('rol_vP5mK8xQ3qL2bW9s', 'merchant')
END;
GO