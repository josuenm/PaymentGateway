USE PaymentGateway_Payment;
GO

CREATE TABLE PaymentTransactions(
    Id VARCHAR(50) NOT NULL, 
    CustomerId VARCHAR(50) NOT NULl, 
    UserId VARCHAR(50) NOT NULL, 
    ChargeId VARCHAR(38) NOT NULL,
    Method TINYINT NOT NULL, 
    Status TINYINT NOT NULL, 
    Amount BIGINT NOT NULL, 
    Currency VARCHAR(3) NOT NULL,
    ChargeResponse NVARCHAR(max) NOT NULl, 
    LiveMode BIT NOT NULL, 
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT PK_PaymentTransactions PRIMARY KEY (Id),
);
GO