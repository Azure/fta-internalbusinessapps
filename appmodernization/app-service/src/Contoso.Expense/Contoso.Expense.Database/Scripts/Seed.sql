if not exists (select 1 from dbo.Expense)

BEGIN

INSERT INTO dbo.Expense (Purpose, ExpenseDate, Amount, EmployeeName, EmployeeEmail, ApproverName, ApproverEmail, Receipt)  values ('FastTrack team visit-1', '2017-7-10', 1500, 'Umar Mohamed', 'umarm@microsoft.com', 'Dennis Karlinsky', 'denniska@microsoft.com', 'http://contosocandies-cdnep.azureedge.net/cdn/candie-3.jpg')
INSERT INTO dbo.Expense (Purpose, ExpenseDate, Amount, EmployeeName, EmployeeEmail, ApproverName, ApproverEmail, Receipt)  values ('FastTrack team visit-2', '2017-6-11', 990, 'Faisal Mustafa', 'faisalm@microsoft.com', 'Dennis Karlinsky', 'denniska@microsoft.com', 'http://contosocandies-cdnep.azureedge.net/cdn/candie-3.jpg')
INSERT INTO dbo.Expense (Purpose, ExpenseDate, Amount, EmployeeName, EmployeeEmail, ApproverName, ApproverEmail, Receipt)  values ('FastTrack team visit-3', '2017-5-12', 1200, 'Randy Pagels', 'umarm@microsoft.com', 'Anusha Rangaswamy', 'anushar@microsoft.com', 'http://contosocandies-cdnep.azureedge.net/cdn/candie-3.jpg')

END