CREATE TABLE [dbo].[Orders]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Date] DATETIMEOFFSET NULL, 
    [Price] FLOAT NULL, 
    [Status] NCHAR(50) NULL, 
    [StoreId] INT NULL, 
    CONSTRAINT [FK_Orders_Stores] FOREIGN KEY (StoreId) REFERENCES [dbo].[Stores](Id)
)
