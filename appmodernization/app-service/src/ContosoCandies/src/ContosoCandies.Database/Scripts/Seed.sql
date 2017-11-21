
if not exists(select 1 from dbo.Stores) 
BEGIN

INSERT INTO dbo.Candies (ImageUrl, [Name], Price) values ('http://contosocandies-cdnep.azureedge.net/cdn/candie-1.jpg', 'Caramel Candie', 1.5)
INSERT INTO dbo.Candies (ImageUrl, [Name], Price) values ('http://contosocandies-cdnep.azureedge.net/cdn/candie-2.jpg', 'Milk Chocolate', 1.2)
INSERT INTO dbo.Candies (ImageUrl, [Name], Price) values ('http://contosocandies-cdnep.azureedge.net/cdn/candie-3.jpg', 'Mint Candie', 0.5)

INSERT INTO dbo.Stores (Country, [Name]) values ('United States', 'Awesome Candies')
INSERT INTO dbo.Stores (Country, [Name]) values ('India', 'Yummy Candies')
INSERT INTO dbo.Stores (Country, [Name]) values ('United Kingdom', 'Lovely Candies')

INSERT INTO dbo.Orders (StoreId, [Date], Price) values (1, '2017-10-11', 50)
INSERT INTO dbo.OrderLines (OrderId, CandieId, Quantity) values (1, 1, 25)

END
