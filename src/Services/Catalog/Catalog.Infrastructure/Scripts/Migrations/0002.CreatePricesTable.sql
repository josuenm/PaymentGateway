USE PaymentGateway_Catalog;
GO

CREATE TABLE Prices (
    Id VARCHAR(50) NOT NULL, 
    Name VARCHAR(255) NOT NULL, 
    Currency VARCHAR(3) NOT NULL,
    Frequency TINYINT NOT NULL,
    Cycle TINYINT NULL,
    Amount BIGINT NOT NULL, 
    ProductId VARCHAR(50) NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL,
    LiveMode BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL, 
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT PK_Prices PRIMARY KEY (Id), 
                    
    CONSTRAINT FK_Product_Id FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
)
GO