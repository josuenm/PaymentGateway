USE PaymentGatewayBilling;
GO

CREATE TABLE PriceReplicas(
    Id VARCHAR(50) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    Frequency TINYINT NOT NULL,
    Cycle TINYINT NULL,
    AmountInCents BIGINT NOT NULL,
    ProductReplicaId VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL,
    LiveMode BIT NOT NULL,
    
    CONSTRAINT PK_PriceReplicas PRIMARY KEY (Id),
    
    CONSTRAINT FK_ProductReplicas_Id FOREIGN KEY (ProductReplicaId) REFERENCES ProductReplicas(Id) ON DELETE CASCADE,
);
GO