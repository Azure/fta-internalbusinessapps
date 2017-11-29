CREATE TABLE [dbo].[Expense]
(
	[ExpenseId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Purpose] NVARCHAR(256) NULL, 
    [ExpenseDate] DATETIME NULL, 
    [Amount] FLOAT NULL, 
    [EmployeeName] NVARCHAR(60) NULL, 
    [EmployeeEmail] NVARCHAR(120) NULL, 
    [ApproverName] NVARCHAR(60) NULL, 
    [ApproverEmail] NVARCHAR(120) NULL, 
    [Receipt] NVARCHAR(256) NULL
)
