USE PaymentGateway_PaymentLink;
GO

CREATE TABLE PaymentLinks(
    Id VARCHAR(50) NOT NULL, 
    UserId VARCHAR(50) NOT NULL,
    Methods NVARCHAR(max) NOT NULL, 
    LiveMode BIT NOT NULL, 
    IsActive BIT NOT NULL, 
    CreatedAt DATETIME2 NOT NULL, 
    UpdatedAt DATETIME2 NULL,
                         
    CONSTRAINT PK_PaymentLinks PRIMARY KEY (Id)
)
GO