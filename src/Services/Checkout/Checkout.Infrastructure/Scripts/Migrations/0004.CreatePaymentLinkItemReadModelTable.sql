USE PaymentGateway_Checkout
GO

CREATE TABLE PaymentLinkItemReadModels (
    Id VARCHAR(50) NOT NULL,
    PaymentLinkId VARCHAR(50) NOT NULL,
    PriceId VARCHAR(50) NOT NULL,
    Quantity INTEGER NOT NULL,
    LiveMode BIT NOT NULL,
    
    CONSTRAINT PK_PaymentLinkItems PRIMARY KEY (Id),
    
    CONSTRAINT FK_PaymentLinkItemReadModels_PaymentLinkReadModels
      FOREIGN KEY (PaymentLinkId) REFERENCES PaymentLinkReadModels(Id)
          ON DELETE CASCADE
);

GO