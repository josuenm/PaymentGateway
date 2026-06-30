USE PaymentGateway_Checkout;
GO

CREATE TABLE CheckoutSessions(
    Id VARCHAR(50) NOT NULL,
    CustomerId VARCHAR(50) NOT NULL,
    PaymentId VARCHAR(50) NOT NULL,
    PaymentLinkId VARCHAR(50) NOT NULL,
    Amount BIGINT NOT NULL, 
    Currency VARCHAR(3) NOT NULL, 
    UserId VARCHAR(50) NOT NULL, 
    Method TINYINT NOT NULL, 
    CreatedAt DATETIME2 NOT NULL,

    CONSTRAINT PK_CheckoutSessions PRIMARY KEY (Id),
);

GO