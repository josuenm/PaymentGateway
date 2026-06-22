USE PaymentGateway_Catalog;
GO

CREATE TABLE Products (
    Id VARCHAR(50) NOT NULL, 
    Name VARCHAR(255) NOT NULL, 
    Description VARCHAR(max) NULL, 
    Metadata NVARCHAR(max) NULL,
    UserId VARCHAR(50) NOT NULL, 
    IsActive BIT NOT NULL, 
    LiveMode BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
                      
    CONSTRAINT PK_Products PRIMARY KEY (Id)
);
GO