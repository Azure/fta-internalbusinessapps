CREATE TABLE [dbo].[Candies]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(50) NULL, 
    [ImageUrl] NVARCHAR(256) NULL, 
    [Price] FLOAT NULL
)
