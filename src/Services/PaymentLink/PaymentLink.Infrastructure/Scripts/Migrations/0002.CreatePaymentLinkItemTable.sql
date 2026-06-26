USE PaymentGateway_PaymentLink;
GO

CREATE TABLE PaymentLinkItems (
    Id VARCHAR(50) NOT NULL, 
    UserId VARCHAR(50) NOT NULL,
    PaymentLinkId VARCHAR(50) NOT NULL,
    PriceId VARCHAR(50) NOT NULL,
    Quantity INTEGER NOT NULL, 
    LiveMode BIT NOT NULL, 
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    
    CONSTRAINT PK_PaymentLinkItems PRIMARY KEY (Id),
    
    CONSTRAINT FK_PaymentLinkItems_PaymentLinks
        FOREIGN KEY (PaymentLinkId) REFERENCES PaymentLinks(Id)
        ON DELETE CASCADE
);
GO