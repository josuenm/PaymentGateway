USE PaymentGateway_Checkout
GO

CREATE TABLE PaymentLinkReadModels(
    Id VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    LiveMode BIT NOT NULL,
    
    CONSTRAINT PK_PaymentLinkReadModels PRIMARY KEY (Id)
);

GO