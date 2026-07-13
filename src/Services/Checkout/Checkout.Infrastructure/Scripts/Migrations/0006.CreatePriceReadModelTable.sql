USE PaymentGateway_Checkout;
GO

CREATE TABLE PriceReadModels(
    Id VARCHAR(50) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Frequency TINYINT NOT NULL, 
    Cycle TINYINT NULL,
    ProductId VARCHAR(50) NOT NULL,
    Amount BIGINT NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    LiveMode BIT NOT NULL,
    
    CONSTRAINT PK_PriceReadModels PRIMARY KEY (Id),
    CONSTRAINT FK_PriceReadModels_ProductReadModels FOREIGN KEY (ProductId) REFERENCES ProductReadModels(Id) ON DELETE CASCADE,
);

GO