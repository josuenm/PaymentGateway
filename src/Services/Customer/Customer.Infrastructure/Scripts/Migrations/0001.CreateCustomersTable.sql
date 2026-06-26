USE PaymentGateway_Customer;
GO

CREATE TABLE Customers (
    Id VARCHAR(50) NOT NULl,
    Name VARCHAR(150) NULL, 
    TaxId VARCHAR(14) NULL, 
    Email VARCHAR(150) NOT NULL,
    LiveMode BIT NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    
    CONSTRAINT PK_Customers PRIMARY KEY (Id)
);
GO