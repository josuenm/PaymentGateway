USE PaymentGateway_Checkout;
GO

CREATE TABLE ProductReadModels(
    Id VARCHAR(50) NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    LiveMode BIT NOT NULL,
    
    CONSTRAINT PK_ProductReadModels PRIMARY KEY (Id)
);

GO