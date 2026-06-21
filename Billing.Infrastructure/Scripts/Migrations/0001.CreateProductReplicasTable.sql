USE PaymentGatewayBilling;
GO

CREATE TABLE ProductReplicas(
    Id VARCHAR(50) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Description VARCHAR(max) NULL, 
    Metadata NVARCHAR(max) NULL,
    UserId VARCHAR(50) NOT NULL, 
    IsActive BIT NOT NULL, 
    LiveMode BIT NOT NULL,
                      
    CONSTRAINT PK_ProductReplicas PRIMARY KEY (Id)
);
GO