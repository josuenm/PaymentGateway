USE PaymentGateway_PaymentLink;
GO

CREATE TABLE PriceReplicas(
    Id VARCHAR(50) NOT NULL,
    UserId VARCHAR(50) NOT NULL,
    
    CONSTRAINT PK_PriceReplicas PRIMARY KEY (Id)
)
GO