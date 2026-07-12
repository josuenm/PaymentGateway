USE PaymentGateway_Checkout
GO

CREATE TABLE CustomerReadModels(
    Id VARCHAR(50) NOT NULL,
    Name VARCHAR(150) NULL,
    TaxId VARCHAR(14) NULL,
    Email VARCHAR(150) NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    LiveMode BIT NOT NULL,
    
    CONSTRAINT PK_CustomerReadModels PRIMARY KEY (Id)
);

GO