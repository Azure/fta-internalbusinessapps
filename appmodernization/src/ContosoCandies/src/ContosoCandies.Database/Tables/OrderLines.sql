CREATE TABLE [dbo].[OrderLines]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Quantity] INT NULL, 
    [CandieId] INT NULL, 
    [OrderId] INT NULL, 
    CONSTRAINT [FK_OrderLines_Candies] FOREIGN KEY (CandieId) REFERENCES [dbo].[Candies](Id), 
    CONSTRAINT [FK_OrderLines_Orders] FOREIGN KEY (OrderId) REFERENCES [dbo].[Orders](Id) ON DELETE CASCADE
)
